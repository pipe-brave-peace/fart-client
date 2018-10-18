﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Life))]
[RequireComponent(typeof(Enemy_State))]
[RequireComponent(typeof(Enemy_Score))]

public class Enemy_Kuma : MonoBehaviour {

    [SerializeField]
    TextMesh Debug_State_Text;
    [SerializeField]
    Renderer m_Color;           // 自分の色
    [SerializeField]
    GameObject m_Point;     // 移動ポイント

    private GameObject m_TargetObj;     // ターゲットオブジェクト
    private Enemy_State m_State;        // 状態
    private NavMeshAgent m_Nav;         // ナビメッシュ
    private Vector3 m_PosOld;           // 生成座標
    private Life m_Life;                // 体力

    // 初期化
    void Start()
    {
        m_Life = GetComponent<Life>();
        m_State = GetComponent<Enemy_State>();
        m_TargetObj = GameObject.FindGameObjectWithTag("MainCamera");                         // プレイヤーを取得
        m_Nav = GetComponent<NavMeshAgent>();               // ナビメッシュの取得
        m_PosOld = transform.position;                      // 満腹後向かう座標のセット
        // スコアセット
        Enemy_Score score = GetComponent<Enemy_Score>();
        score.SetScore(Score_List.Enemy.Kuma);
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
                if (DistanceNoneY(m_TargetObj, 5.0f))
                {
                    // 攻撃状態に変更
                    m_State.SetState(Enemy_State.STATE.ATTACK);
                    m_Nav.SetDestination(transform.position);       // 移動を止める
                }
                break;

            case Enemy_State.STATE.ATTACK:      // 攻撃
                Debug_State_Text.text = "STATE:攻撃している";
                m_State.SetState(Enemy_State.STATE.SATIETY);
                break;

            case Enemy_State.STATE.DAMAGE:      // ダメージ状態
                // 体力を減らす
                m_Life.SubLife(1.0f);

                // 体力がなくなった？
                if (m_Life.GetLife() <= 0)
                {
                    m_State.SetState(Enemy_State.STATE.ESCAPE);     // 離脱状態へ
                }
                break;

            case Enemy_State.STATE.ESCAPE:   // 逃げる
                Debug_State_Text.text = "STATE:FadeOut";

                // 離脱の位置の方向に移動
                m_Nav.SetDestination(m_PosOld);

                // アルファ値を減らす
                Color color = m_Color.material.color;
                color.a -= 0.01f;
                m_Color.material.color = color;

                // 透明になった？
                if (color.a > 0.0f) { break; }

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
    bool DistanceNoneY(GameObject Target, float var)
    {
        // Y軸無視の距離算出
        Vector2 this_pos = new Vector2(transform.position.x, transform.position.z);
        Vector2 target_pos = new Vector2(Target.transform.position.x, Target.transform.position.z);
        // 近い？
        Debug.Log(Vector2.Distance(this_pos, target_pos));
        if (Vector2.Distance(this_pos, target_pos) <= var)
        {
            return true;
        }
        return false;
    }
}