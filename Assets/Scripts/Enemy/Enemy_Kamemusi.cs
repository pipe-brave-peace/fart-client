using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Life))]
[RequireComponent(typeof(Enemy_State))]
[RequireComponent(typeof(Enemy_Score))]

public class Enemy_Kamemusi : MonoBehaviour {

    [SerializeField]
    TextMesh Debug_State_Text;
    [SerializeField]
    Renderer m_Color;           // 自分の色
    [SerializeField]
    GameObject m_FadePoint;
    [SerializeField]
    GameObject m_TargetObj;
    [SerializeField]
    GameObject m_AttackEffect;           // インク
    [SerializeField]
    GameObject m_DamageEffect;          // ダメージエフェクト
    [SerializeField]
    GameObject m_BuffEffect;          // バフエフェクト

    private Enemy_State m_State;        // 状態
    private NavMeshAgent m_Nav;         // ナビメッシュ
    private Vector3 m_PosOld;           // 満腹後向かう座標
    private Life m_Life;                // 体力
    private Color m_FadeColor;
    private bool m_isAttack;            // 攻撃チェック
    private bool m_isBuff;

    // Use this for initialization
    void Start()
    {
        m_Life = GetComponent<Life>();
        m_State = GetComponent<Enemy_State>();
        m_Nav = GetComponent<NavMeshAgent>();               // ナビメッシュの取得
        m_PosOld = transform.position;                      // 満腹後向かう座標のセット
        m_FadeColor = m_Color.material.color;
        m_isAttack = false;
        m_isBuff = false;

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
            case Enemy_State.STATE.MOVE:     // 移動
                Debug_State_Text.text = "STATE:Move";
                StateMove();
                break;

            case Enemy_State.STATE.ATTACK:      // 攻撃
                Debug_State_Text.text = "STATE:攻撃している";

                // エフェクトの生成
                GameObject attack_effect = Instantiate(m_AttackEffect, transform.position, Quaternion.identity) as GameObject;
                attack_effect.GetComponent<Effect_Kamemusi>().SetTargetObj(m_TargetObj);

                // 攻撃フラグを立つ
                m_isAttack = true;

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

            case Enemy_State.STATE.SPRAY:      // スプレー状態
                Debug_State_Text.text = "STATE:見えねぇ！！";
                // スプレーを受けた
                m_isBuff = true;

                // エフェクトの生成
                m_BuffEffect.SetActive(true);

                m_State.SetState(Enemy_State.STATE.MOVE);     // 移動状態へ
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
                break;

            case Enemy_State.STATE.DAMAGE:      // ダメージ状態
                // バフがない&&虫ではない？
                if (!m_isBuff)
                {
                    m_State.SetState(Enemy_State.STATE.MOVE);     // 移動状態へ
                    StateMove();
                    break;
                }
                Debug_State_Text.text = "STATE:痛えぇ！";
                // 体力を減らす
                m_Life.SubLife(1.0f);

                // エフェクトの生成
                GameObject damage_effect = Instantiate(m_DamageEffect, transform.position, Quaternion.identity) as GameObject;

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
                m_State.SetState(Enemy_State.STATE.MOVE);     // 移動状態へ
                break;

            case Enemy_State.STATE.ESCAPE:   // 逃げる
                Debug_State_Text.text = "STATE:FadeOut";

                // 離脱の位置の方向に移動
                m_Nav.SetDestination(m_PosOld);

                // アルファ値を減らす
                m_FadeColor.a -= 0.02f;
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
    bool DistanceNoneY(GameObject Target, float var)
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
    // 移動モード
    void StateMove()
    {
        // 攻撃したら離脱する
        if (m_isAttack)
        {
            m_State.SetState(Enemy_State.STATE.SATIETY);
            return;
        }

        // 近い？
        if (DistanceNoneY(m_TargetObj, 5.0f))
        {
            // 攻撃状態に変更
            m_State.SetState(Enemy_State.STATE.ATTACK);
            m_Nav.SetDestination(transform.position);       // 移動を止める
            return;
        }
        //対象の位置の方向に移動
        MoveHoming(m_TargetObj);
    }
}
