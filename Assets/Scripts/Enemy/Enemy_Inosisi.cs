﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Life))]
[RequireComponent(typeof(Enemy_State))]
[RequireComponent(typeof(Enemy_Score))]

public class Enemy_Inosisi : MonoBehaviour {

    [SerializeField]
    TextMesh Debug_State_Text;
    [SerializeField]
    GameObject m_FadePoint;     // 退却ポイント
    [SerializeField]
    GameObject m_TargetObj;
    [SerializeField]
    GameObject[] m_NavObj;          // 農作物リスト
    [SerializeField]
    float m_Satiety;            // 満腹度
    [SerializeField]
    float m_EatSpeed = 1.0f;        // 食べるスピード

    private Enemy_State m_State;        // 状態
    private NavMeshAgent m_Nav;         // ナビメッシュ
    private Vector3 m_PosOld;           // 満腹後向かう座標
    private Life m_Life;                // 体力
    private Renderer m_Color;           // 自分の色
    private Color m_FadeColor;

    // 初期化
    void Start()
    {
        // ターゲットの代入
        if (!m_TargetObj) { m_TargetObj = m_NavObj[0]; }
        m_Life = GetComponent<Life>();
        m_State = GetComponent<Enemy_State>();
        m_Nav = GetComponent<NavMeshAgent>();               // ナビメッシュの取得
        m_Color = GetComponent<Renderer>();
        m_PosOld = transform.position;                      // 満腹後向かう座標のセット
        m_FadeColor = m_Color.material.color;

        // スコアセット
        Enemy_Score score = GetComponent<Enemy_Score>();
        score.SetScore(Score_List.Enemy.Sika);
    }

    // Update is called once per frame
    void Update()
    {
        // 状態判定
        switch (m_State.GetState())
        {
            case Enemy_State.STATE.NORMAL:   // 通常
                Debug_State_Text.text = "STATE:Normal";
                m_State.SetState(Enemy_State.STATE.MOVE);
                break;

            case Enemy_State.STATE.MOVE:     // 移動
                Debug_State_Text.text = "STATE:Move";
                //対象の位置の方向に移動
                MoveHoming(m_TargetObj);
                // 近い？
                if (!DistanceNoneY(m_TargetObj, 5.0f)) { break; }

                // 目標が農作物？
                if (m_TargetObj.tag == "Crops")
                {
                    // 食べる状態に変更
                    m_State.SetState(Enemy_State.STATE.EAT);
                    break;
                }
                // 攻撃状態に変更
                m_State.SetState(Enemy_State.STATE.ATTACK);
                m_Nav.SetDestination(transform.position);       // 移動を止める
                break;

            case Enemy_State.STATE.EAT:      // 食べる
                Debug_State_Text.text = "STATE:食べているよ";
                // 食べ終わった？
                if (m_TargetObj == null)
                {
                    // だいたい食べた？
                    if (m_Satiety <= 0.5f)
                    {
                        // 満腹になる
                        m_State.SetState(Enemy_State.STATE.SATIETY);
                        // 移動速度が減る
                        m_Nav.speed = m_Nav.speed * 0.5f;
                        break;
                    }
                    // 次を探す
                    m_State.SetState(Enemy_State.STATE.NORMAL);
                }

                // 農作物体力を減らす
                Life target_life = m_TargetObj.GetComponent<Life>();
                target_life.SubLife(Time.deltaTime * m_EatSpeed);
                // 食べ終わった？
                if (target_life.GetLife() <= 0) { Destroy(m_TargetObj.gameObject); }

                // 満腹までのカウント
                m_Satiety -= Time.deltaTime;
                // カウント到達した？
                if (m_Satiety <= 0.0f)
                {
                    // 満腹になる
                    m_State.SetState(Enemy_State.STATE.SATIETY);
                    // 移動速度が減る
                    m_Nav.speed = m_Nav.speed * 0.5f;
                }
                break;

            case Enemy_State.STATE.ATTACK:      // 攻撃
                Debug_State_Text.text = "STATE:攻撃している";
                m_State.SetState(Enemy_State.STATE.SATIETY);
                break;

            case Enemy_State.STATE.SATIETY:     // 攻撃した
                Debug_State_Text.text = "STATE:満足した";

                //対象の位置の方向に移動
                MoveHoming(m_FadePoint);

                // 近い？
                if (DistanceNoneY(m_FadePoint, 1.0f))
                {
                    Destroy(gameObject);    // 消去
                }
                break;

            case Enemy_State.STATE.DAMAGE:      // ダメージ状態
                Debug_State_Text.text = "STATE:痛えぇ！";
                // 体力を減らす
                m_Life.SubLife(1.0f);

                // 体力がなくなった？
                if (m_Life.GetLife() <= 0)
                {
                    // 透明できる描画モードに変更
                    BlendModeUtils.SetBlendMode(m_Color.material, BlendModeUtils.Mode.Fade);
                    m_FadeColor.a = 1.0f;
                    m_Color.material.color = m_FadeColor;
                    m_State.SetState(Enemy_State.STATE.ESCAPE);     // 離脱状態へ
                    break;
                }
                m_State.SetState(Enemy_State.STATE.NORMAL);     // 通常状態へ
                break;

            case Enemy_State.STATE.ESCAPE:   // 逃げる
                Debug_State_Text.text = "STATE:FadeOut";

                // 離脱の位置の方向に移動
                m_Nav.SetDestination(m_PosOld);

                // アルファ値を減らす
                m_FadeColor.a -= 0.01f;
                m_Color.material.color = m_FadeColor;

                // 透明になった？
                if (m_FadeColor.a > 0.0f) { break; }

                // 自分を消す
                Destroy(gameObject);
                return;
        }
    }

    // Y軸無視でターゲットに向く
    void MoveHoming(GameObject Target)
    {
        Vector3 target = Target.transform.position;
        target.y = transform.position.y;       // y軸無視
        m_Nav.SetDestination(target);
    }

    // Y軸無視でターゲットと近い？
    bool DistanceNoneY( GameObject Target, float var)
    {
        // Y軸無視の距離算出
        Vector2 this_pos = new Vector2(transform.position.x, transform.position.z);
        Vector2 target_pos = new Vector2(Target.transform.position.x, Target.transform.position.z);
        // 近い？
        if (Vector2.Distance(this_pos, target_pos) <= var)
        {
            return true;
        }
        return false;
    }
}
