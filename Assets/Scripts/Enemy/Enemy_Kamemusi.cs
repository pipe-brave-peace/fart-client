using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Life))]
[RequireComponent(typeof(Enemy_State))]
[RequireComponent(typeof(Enemy_Score))]

public class Enemy_Kamemusi : MonoBehaviour {

    private const float BACK_TIME = 1.0f;

    //[SerializeField]
    //TextMesh Debug_State_Text;      // テスト
    [SerializeField]
    GameObject m_FadePoint;         // 退却ポイント
    [SerializeField]
    GameObject m_TargetObj;         // ターゲット

    [Header("以下編集しないこと！")]
    [SerializeField]
    SkinnedMeshRenderer m_Color;    // 自分の色
    [SerializeField]
    GameObject m_AttackEffect;      // インク
    [SerializeField]
    GameObject m_DamageEffect;      // ダメージエフェクト
    [SerializeField]
    GameObject m_EscapeEffect;      // 退却時汗のエフェクト
    [SerializeField]
    GameObject m_BuffEffect;        // バフエフェクト
    [SerializeField]
    Animator m_Animator;     // アニメション

    private float m_NavMoveSpeed;     // 移動スピード
    private float m_BackTimer;        // 後退するカウント

    private Enemy_State  m_State;       // 状態
    private NavMeshAgent m_Nav;         // ナビメッシュ
    private Life         m_Life;        // 体力

    private Vector3      m_FadePos;     // 満腹後向かう座標
    private Color        m_FadeColor;   // 退却時の色
    private int          m_AttackMode;  // 攻撃するモード：０、準備　１、発射　２、クールタイム
    private float        m_AttackTimer; // 攻撃モード用カウント

    // Use this for initialization
    void Start()
    {
        // コンポーネントの取得
        m_Life     = GetComponent<Life>();
        m_State    = GetComponent<Enemy_State>();
        m_Nav      = GetComponent<NavMeshAgent>();

        // 変数初期化
        m_NavMoveSpeed = m_Nav.speed;
        m_BackTimer = BACK_TIME;
        m_FadeColor     = new Color(1.0f, 1.0f, 1.0f, 1.0f);    // 現在の色をセット
        m_AttackMode    = 0;                                    // 攻撃モード
        m_AttackTimer   = 1.0f;                                 // 攻撃モード用カウント
        // 退却ポイントがない：生成座標を代入
        // 退却ポイントがある：退却ポイント座標を代入
        if (m_FadePoint == null)
        {
            m_FadePos = transform.position;
        }
        else
        {
            m_FadePos = m_FadePoint.transform.position;
        }

        // スコアセット
        Enemy_Score score = GetComponent<Enemy_Score>();
        score.SetScore(Score_List.Enemy.Sika);
    }

    // Update is called once per frame
    void Update()
    {
        m_Animator.speed = 3.0f;
        // 状態判定
        switch (m_State.GetState())
        {
            case Enemy_State.STATE.MOVE:     // 移動
                //Debug_State_Text.text = "STATE:Move";
                // 近い？
                if (DistanceNoneY(m_TargetObj.transform.position, 5.0f))
                {
                    // 攻撃状態に変更
                    m_State.SetState(Enemy_State.STATE.ATTACK);
                    m_Nav.SetDestination(transform.position);       // 移動を止める
                    return;
                }
                //対象の位置の方向に移動
                MoveHoming(m_TargetObj.transform.position);
                break;

            case Enemy_State.STATE.ATTACK:      // 攻撃
                //Debug_State_Text.text = "STATE:攻撃している";
                m_Animator.speed = 0.0f;

                // 攻撃モード処理
                switch (m_AttackMode)
                {
                    case 0:     // 準備
                        m_AttackTimer -= Time.deltaTime;
                        if( m_AttackTimer <= 0.0f)
                        {
                            m_AttackMode++;
                            m_AttackTimer = 1.0f;   // クールタイムのセット
                        }
                        break;

                    case 1:     // 攻撃
                        // エフェクトの生成
                        GameObject attack_effect = Instantiate(m_AttackEffect, transform.position, Quaternion.identity) as GameObject;
                        // エフェクト飛ぶベクトルのターゲットセット
                        attack_effect.GetComponent<Effect_Kamemusi>().SetTargetObj(m_TargetObj);
                        m_AttackMode++;
                        break;

                    case 2:     // クールタイム
                        m_AttackTimer -= Time.deltaTime;
                        if( m_AttackTimer <= 0.0f)
                        {
                            // 満足状態へ
                            m_State.SetState(Enemy_State.STATE.SATIETY);
                        }
                        break;
                }
                break;

            case Enemy_State.STATE.SATIETY:     // 攻撃した
                //Debug_State_Text.text = "STATE:満足した";

                //対象の位置の方向に移動
                MoveHoming(m_FadePos);

                // 近い？
                if (DistanceNoneY(m_FadePos, 1.0f))
                {
                    Destroy(transform.parent.gameObject);    // 消去
                    return;
                }
                break;

            case Enemy_State.STATE.SPRAY:      // スプレー状態
                //Debug_State_Text.text = "STATE:見えねぇ！！";

                // エフェクトの生成
                m_BuffEffect.SetActive(true);

                // 体力を減らす
                m_Life.SubLife(1.0f);

                // 体力がなくなった？
                if (m_Life.GetLife() <= 0)
                {
                    // 透明できる描画モードに変更
                    BlendModeUtils.SetBlendMode(m_Color.material, BlendModeUtils.Mode.Fade);
                    m_FadeColor.a = 1.0f;
                   m_Color.material.SetColor("_MainColor", m_FadeColor);
                    m_State.SetState(Enemy_State.STATE.BACK);     // 離脱状態へ
                    break;
                }
                m_State.SetState(Enemy_State.STATE.MOVE);     // 移動状態へ
                break;

            case Enemy_State.STATE.DAMAGE:      // ダメージ状態
                //Debug_State_Text.text = "STATE:痛えぇ！";
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
                   m_Color.material.SetColor("_MainColor", m_FadeColor);
                    m_State.SetState(Enemy_State.STATE.ESCAPE);         // 離脱状態へ
                    break;
                }
                // 満足だったら
                if( m_State.GetStateOld() == Enemy_State.STATE.SATIETY)
                {
                    m_State.SetState(Enemy_State.STATE.SATIETY);        // 満足状態へ
                    break;
                }
                m_State.SetState(Enemy_State.STATE.MOVE);     // 移動状態へ
                break;

            case Enemy_State.STATE.BACK:      // 後退
                //Debug_State_Text.text = "STATE:あ！！！！";

                // 汗のエフェクトを出す
                m_EscapeEffect.SetActive(true);

                // 後退処理
                m_Nav.updateRotation = false;
                Vector3 pos = transform.position;
                pos = m_TargetObj.transform.position - pos;
                pos = -10.0f * Vector3.Normalize(pos);
                pos += transform.position;
                MoveHoming(pos);
                m_Nav.speed = m_NavMoveSpeed * m_BackTimer * 30.0f;
                m_Nav.acceleration = m_Nav.speed;

                gameObject.transform.Rotate(0, 25, 0);

                // カウント処理
                m_BackTimer -= Time.deltaTime;
                if (m_BackTimer <= 0.0f)
                {
                    m_BackTimer = BACK_TIME;
                    m_State.EnemySetState(Enemy_State.STATE.ESCAPE);
                    m_Nav.enabled = true;
                }
                break;

            case Enemy_State.STATE.ESCAPE:   // 逃げる
                //Debug_State_Text.text = "STATE:FadeOut";   // テスト
                // 状態遷移はもうできない
                m_State.CanSet(false);
                // 汗のエフェクトを出す
                m_EscapeEffect.SetActive(true);
                // 離脱の位置の方向に移動
                m_Nav.SetDestination(m_FadePos);

                // 消えていく
                m_FadeColor.a -= 0.02f;
               m_Color.material.SetColor("_MainColor", m_FadeColor);

                // 汗を止める
                if (m_FadeColor.a <= 0.3f) { m_EscapeEffect.SetActive(false); }

                // 透明になった親を消す
                if (m_FadeColor.a <= 0.0f) { Destroy(transform.parent.gameObject); }
                return;
        }
    }

    // Y軸無視でターゲットに向く
    void MoveHoming(Vector3 Target)
    {
        Target.y = transform.position.y;       // y軸無視
        m_Nav.SetDestination(Target);
    }

    // Y軸無視でターゲットと近い？
    bool DistanceNoneY(Vector3 Target, float var)
    {
        // Y軸無視の距離算出
        Vector2 this_pos = new Vector2(transform.position.x, transform.position.z);
        Vector2 target_pos = new Vector2(Target.x, Target.z);
        // 近い？
        return (Vector2.Distance(this_pos, target_pos) <= var) ? true : false;
    }
}
