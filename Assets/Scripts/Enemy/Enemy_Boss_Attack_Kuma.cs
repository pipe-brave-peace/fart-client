using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

[RequireComponent(typeof(Life))]
[RequireComponent(typeof(Enemy_State))]
[RequireComponent(typeof(Enemy_Score))]

public class Enemy_Boss_Attack_Kuma : MonoBehaviour {

    private const float FEAR_TIME   = 5.0f;
    private const float BACK_TIME   = 1.0f;
    private const float CRY_TIME    = 1.0f;
    private const float CAN_DAMAGE_LEN = 20.0f;
    
    //[SerializeField]
    //TextMesh Debug_State_Text;
    [Header("吼える場所")]
    [SerializeField]
    GameObject CryPoint;
    [SerializeField]
    GameObject m_TargetObj;             // 移動目標
    [SerializeField]
    List<GameObject> m_NavPoints;       // 移動ポイントリスト
    [Header("ウロウロする時間")]
    [SerializeField]
    float m_UrouroTimer;
    [Header("逃げるまでの時間")]
    [SerializeField]
    float m_EscapeTimer;
    [Header("逃げる時の目的地")]
    [SerializeField]
    GameObject m_FadePoint;         // 退却ポイント

    [Header("以下編集しないこと！")]
    [SerializeField]
    GameObject m_AttackEffect;      // クマのジャマのエフェクト
    [SerializeField]
    GameObject m_EscapeEffect;      // 退却時汗のエフェクト

    private Enemy_State     m_State;            // 状態
    private NavMeshAgent    m_Nav;              // ナビメッシュ
    private Rigidbody       m_Rigidbody;        // リジッドボディ
    private GameObject      m_AttackObj;        // 攻撃目標
    private float           m_NavMoveSpeed;     // 移動スピード
    private float           m_UrouroTimerMax;   // ウロウロする時間
    private Vector3         m_FadePos;          // 退却時向かうの座標
    private Animator        m_Animator;         // アニメション
    private bool            m_isCry;            // 吼えたかどうか
    private float           m_CryTimer;         // 吼えるまでのカウント
    private float           m_FearTimer;        // 怯む時間
    private float           m_BackTimer;        // 後退するカウント
    private GameObject      m_LifeList;         // ライフ照準のリスト
    private GameObject      m_CryEffect;        // 吼えるのエフェクト

    // 初期化
    void Start()
    {
        // コンポーネントの取得
        m_State      = GetComponent<Enemy_State>();
        m_Nav        = GetComponent<NavMeshAgent>();
        m_Rigidbody  = GetComponent<Rigidbody>();
        m_Animator   = GetComponent<Animator>();

        // 変数の初期化
        m_State.CanSet(false);
        m_AttackObj      = m_TargetObj;
        m_NavMoveSpeed   = m_Nav.speed;
        m_UrouroTimerMax = m_UrouroTimer;
        m_isCry          = false;
        m_CryTimer       = CRY_TIME;
        m_FearTimer      = FEAR_TIME;
        m_BackTimer      = BACK_TIME;
        m_LifeList       = null;
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
        score.SetScore(Score_List.Enemy.Kuma);
    }

    // Update is called once per frame
    void Update()
    {
        if ( !m_isCry)
        {
            // 近い？
            if (DistanceNoneY(CryPoint.transform.position, 1.0f))
            {
                m_Nav.updatePosition = false;
                MoveHoming(Camera.main.transform.position);
                m_CryTimer -= Time.deltaTime;
                if( m_CryTimer <= 0.0f)
                {
                    m_CryTimer = CRY_TIME;
                    m_isCry = true;
                    m_State.EnemySetState(Enemy_State.STATE.CRY);
                    m_Animator.SetBool("ToCry", true);
                }
            }
            else
            {
                // 目標へ移動
                MoveHoming(CryPoint.transform.position);
            }
            return;
        }
        if( m_LifeList != null)
        {
            // ライフリストのライフがなくなったら離脱する
            if (m_LifeList.transform.childCount <= 0)
            {
                m_State.EnemySetState(Enemy_State.STATE.BACK);     // 後退状態へ
                m_Animator.SetBool("ToDamage", true);
                Destroy(m_LifeList.gameObject);
                m_Nav.updatePosition = true;
            }
        }
        // 状態判定
        switch (m_State.GetState())
        {
            case Enemy_State.STATE.MOVE:     // 移動
                //Debug_State_Text.text = "STATE:Move";
                
                m_Animator.SetBool("ToMove", false);

                // 目標へ移動
                m_Nav.updatePosition = true;
                MoveHoming(m_TargetObj.transform.position);

                // 目標タグチェック
                if (m_TargetObj.tag == "Point") // ポイント？
                {
                    if( m_LifeList != null) { Destroy(m_LifeList.gameObject); }
                    // ウロウロする時間のカウント
                    m_UrouroTimer -= Time.deltaTime;
                    // 来たら攻撃目標を狙う
                    if (m_UrouroTimer <= 0.0f)
                    {
                        m_UrouroTimer = m_UrouroTimerMax;
                        m_TargetObj = m_AttackObj;
                    }
                }
                else                            // プレイヤー？
                {
                    // 近い？
                    if (DistanceNoneY(m_TargetObj.transform.position, 5.0f))
                    {
                        // 攻撃状態に変更
                        m_State.EnemySetState(Enemy_State.STATE.ATTACK);
                        m_Animator.SetBool("ToAttack", true);
                        m_Nav.updatePosition = false;
                        break;
                    }

                    // 攻撃出来条件
                    if (m_LifeList == null && DistanceNoneY(m_TargetObj.transform.position, CAN_DAMAGE_LEN))
                    {
                        CreateLifeList();
                    }
                }
                break;
                
            case Enemy_State.STATE.CRY:
                {
                    //Debug_State_Text.text = "STATE:がおぉぉ！！！";

                    if (m_LifeList != null) { Destroy(m_LifeList.gameObject); }
                    // 吼えるエフェクトの再生
                    if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f && m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
                    {
                        CreateCryEffect();
                    }

                    // アニメション終わった？
                    if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f && m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
                    {
                        m_State.EnemySetState(Enemy_State.STATE.MOVE);
                        m_Animator.SetBool("ToCry", false);
                        m_Nav.updatePosition = true;
                    }
                    break;
                }

            case Enemy_State.STATE.ATTACK:      // 攻撃
                //Debug_State_Text.text = "STATE:喰らえ！！";

                // アニメション終わった？
                if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f && m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
                {
                    GameObject effet = Instantiate(m_AttackEffect, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;

                    m_TargetObj = SerchPoint();
                    m_State.EnemySetState(Enemy_State.STATE.MOVE);
                    m_Animator.SetBool("ToAttack", false);
                }

                break;

            case Enemy_State.STATE.BACK:      // 後退
                //Debug_State_Text.text = "STATE:あ！！！！";

                // 後退処理
                m_Nav.updateRotation = false;
                Vector3 pos = transform.position;
                pos = m_TargetObj.transform.position - pos;
                pos = -10.0f * Vector3.Normalize(pos);
                pos += transform.position;
                MoveHoming(pos);
                m_Nav.speed = m_NavMoveSpeed * m_BackTimer*30.0f;
                m_Nav.acceleration = m_Nav.speed;

                // カウント処理
                m_BackTimer -= Time.deltaTime;
                if(m_BackTimer <= 0.0f)
                {
                    m_BackTimer = BACK_TIME;
                    m_State.EnemySetState(Enemy_State.STATE.FEAR);
                    m_Animator.SetBool("ToFear", true);
                    m_Nav.enabled = false;
                }
                break;
                
            case Enemy_State.STATE.FEAR:        // 怯む
                //Debug_State_Text.text = "STATE:怖いよ、怖いよぉ～";
                
                m_FearTimer -= Time.deltaTime;
                if( m_FearTimer <= 0.0f)
                {
                    m_FearTimer = FEAR_TIME;
                    m_State.EnemySetState(Enemy_State.STATE.CRY);
                    AnimatorFuraguInit();
                    m_Animator.SetBool("ToMove", true);
                    m_Animator.SetBool("ToCry" , true);
                    m_Nav.enabled        = true;
                    m_Nav.updateRotation = true;
                    m_Nav.speed          = m_NavMoveSpeed;
                    m_Nav.acceleration   = 8;
                }

                break;

            case Enemy_State.STATE.ESCAPE:   // 逃げる
                //Debug_State_Text.text = "STATE:FadeOut";

                // 汗のエフェクトを出す
                m_EscapeEffect.SetActive(true);

                // 離脱の位置の方向に移動
                MoveHoming(m_FadePos);

                // クマオブジェクトを消す
                if (DistanceNoneY(m_FadePos, 1.0f)) { Destroy(transform.parent.gameObject); }
                return;
        }

        // 時間来たら逃げる～
        m_EscapeTimer -= Time.deltaTime;
        if (m_EscapeTimer > 0.0f) return;
        if (m_State.GetState() == Enemy_State.STATE.ESCAPE) return;
        if (m_State.GetState() == Enemy_State.STATE.ATTACK) return;
        if (m_State.GetState() == Enemy_State.STATE.CRY) return;
        if (m_State.GetState() == Enemy_State.STATE.BACK) return;
        if (m_State.GetState() == Enemy_State.STATE.FEAR) return;
        m_State.EnemySetState(Enemy_State.STATE.ESCAPE);
        if (m_LifeList != null) { Destroy(m_LifeList.gameObject); }
        m_Animator.Play("Move");
    }

    // Y軸無視でターゲットに向く
    void MoveHoming(Vector3 Target)
    {
        Target.y = transform.position.y;        // y軸無視
        m_Nav.SetDestination(Target);           // ナビメッシュ上の移動
    }

    // Y軸無視でターゲットと近い？
    bool DistanceNoneY(Vector3 Target, float var)
    {
        // Y軸無視の距離算出
        Vector2 this_pos    = new Vector2(transform.position.x, transform.position.z);
        Vector2 target_pos  = new Vector2(Target.x, Target.z);

        // 近い？
        return (Vector2.Distance(this_pos, target_pos) <= var) ? true : false;
    }

    // ポイントリストから目標を取得
    GameObject SerchPoint()
    {
        // リストがなくなったらnull
        if (m_NavPoints.Count <= 0) { return null; }

        // ランダムでポイントを返す
        int max = m_NavPoints.Count;
        int pointID = Random.Range(0, max);
        return m_NavPoints[pointID];
    }
    void CreateCryEffect()
    {
        if (m_CryEffect != null) return;
        // プレハブを取得
        GameObject prefab = (GameObject)Resources.Load("Prefabs/Effect/E_BossCry");

        // プレハブからインスタンスを生成
        m_CryEffect = (GameObject)Instantiate(prefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        // 作成したオブジェクトを子として登録
        m_CryEffect.transform.parent = transform;
        m_CryEffect.transform.localPosition = new Vector3(0.0f, 1.14f, 0.3f);
        m_CryEffect.transform.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        m_CryEffect.transform.localScale    = new Vector3(1.0f, 1.0f, 1.0f);
    }
    void CreateLifeList()
    {
        if (m_LifeList != null) return;
        // プレハブを取得
        GameObject prefab = (GameObject)Resources.Load("Prefabs/Enemy/P_Boss_LifeList");

        // プレハブからインスタンスを生成
        m_LifeList = (GameObject)Instantiate(prefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        // 作成したオブジェクトを子として登録
        m_LifeList.transform.parent = transform;
        m_LifeList.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        m_LifeList.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        m_LifeList.transform.localScale    = new Vector3(1.0f, 1.0f, 1.0f);
    }
    void AnimatorFuraguInit()
    {
        m_Animator.SetBool("ToEat", false);
        m_Animator.SetBool("ToCry", false);
        m_Animator.SetBool("ToAttack", false);
        m_Animator.SetBool("ToFear", false);
        m_Animator.SetBool("ToMove", false);
        m_Animator.SetBool("ToDamage", false);
        m_Animator.SetBool("ToFaint", false);
    }
}
