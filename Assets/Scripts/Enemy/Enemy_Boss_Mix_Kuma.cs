using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

[RequireComponent(typeof(Life))]
[RequireComponent(typeof(Enemy_State))]
[RequireComponent(typeof(Enemy_Score))]

public class Enemy_Boss_Mix_Kuma : MonoBehaviour {

    private const float FEAR_TIME   = 5.0f;
    private const float BACK_TIME = 1.0f;
    private const float CRY_TIME    = 1.0f;
    private const float CAN_DAMAGE_LEN = 20.0f;
    [SerializeField]
    bool Test_MaxTank = false;
    [SerializeField]
    bool Test_LastAttack = false;

    [SerializeField]
    PlayerAll m_PlayerAll;

    //[SerializeField]
    //TextMesh    Debug_State_Text;
    [Header("吼える場所")]
    [SerializeField]
    GameObject  CryPoint;
    [Header("農作物を荒らすスピード")]
    [SerializeField]
    float       m_EatSpeed = 1.0f;
    [Header("攻撃対象の座標")]
    [SerializeField]
    GameObject  m_AttackObj;
    [Header("農作物の優先順位")]
    [SerializeField]
    List<GameObject> m_NavCrops;
    [Header("ラスト一撃までのタイマー")]
    [SerializeField]
    float       m_LastTimer;

    [Header("以下編集しないこと！")]
    [SerializeField]
    GameObject m_DamageEffect;        // 弾の爆発エフェクト
    [SerializeField]
    GameObject m_AttackEffect;      // クマのジャマのエフェクト
    [SerializeField]
    GameObject m_BuffEffect;        // オナラ匂いのエフェクト
    [SerializeField]
    GameObject m_BlowEffect;
    [SerializeField]
    GameObject m_EscapeEffect;        // 退却時汗のエフェクト

    private Enemy_State     m_State;            // 状態
    private NavMeshAgent    m_Nav;              // ナビメッシュ
    private GameObject      m_TargetObj;        // 移動目標
    private float           m_NavMoveSpeed;     // 移動スピード
    private int             m_CropIndex;        // 農作物リストのインデックス
    private Animator        m_Animator;         // アニメション
    private bool            m_isCry;            // 吼えたかどうか
    private float           m_CryTimer;         // 吼えるまでのカウント
    private bool            m_isBuff;           // オナラスプレー受けたかどうか
    private float           m_FearTimer;        // 怯む時間
    private float           m_BackTimer;        // 後退するカウント
    private Life            m_Life;             // ライフ
    private GameObject      m_LifeList;         // ライフ照準のリスト
    private GameObject      m_LastLife;         // ラスト一撃のライフ照準
    private GameObject      m_CryEffect;        // 吼えるのエフェクト
    private bool            m_isEatDamage;      // 農作物を荒らす時に攻撃された？

    public bool            m_isLastAttack;     // ラストアタックチャンス？

    public bool isLastAttack() { return m_isLastAttack; }

    // 初期化
    void Start()
    {
        // コンポーネントの取得
        m_State      = GetComponent<Enemy_State>();
        m_Nav        = GetComponent<NavMeshAgent>();
        m_Life       = GetComponent<Life>();
        m_Animator   = GetComponent<Animator>();

        // 変数の初期化
        m_State.CanSet(false);
        m_NavMoveSpeed   = m_Nav.speed;
        m_isCry          = false;
        m_isEatDamage    = false;
        m_CryTimer       = CRY_TIME;
        m_FearTimer      = FEAR_TIME;
        m_BackTimer      = BACK_TIME;
        m_LifeList       = null;
        m_LastLife       = null;
        m_isLastAttack   = false;

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

        // 時間来たらライフを1に
        m_LastTimer = Mathf.Max(m_LastTimer - Time.deltaTime, -1.0f);
        if(m_LastTimer <= 0.0f)
        {
            if( m_Life.GetLife() > 1)
            {
                m_Life.SetLife(1.0f);
            }
        }

        if ( m_LifeList != null)
        {
            // ライフリストのライフがなくなったら離脱する
            if (m_LifeList.transform.childCount <= 0)
            {
                m_State.EnemySetState(Enemy_State.STATE.BACK);     // 後退状態へ
                m_Animator.SetBool("ToDamage", true);
                Destroy(m_LifeList.gameObject);
                m_Nav.updatePosition = true;
                // 体力を減らす
                SubLife(1.0f);
            }
        }
        // 状態判定
        switch (m_State.GetState())
        {
            case Enemy_State.STATE.MOVE:     // 移動
                //Debug_State_Text.text = "STATE:Move";
                
                m_Animator.SetBool("ToMove", false);

                // 目標がなくなった？
                if (m_TargetObj == null)
                {
                    // 再検索
                    m_TargetObj = SerchCrops(m_CropIndex);          // 農作物をサーチ
                    break;
                }

                // 目標へ移動
                m_Nav.updatePosition = true;
                MoveHoming(m_TargetObj.transform.position);

                // 目標タグチェック
                if (m_TargetObj.tag == "Crops") // 農作物？
                {
                    if( m_LifeList != null) { Destroy(m_LifeList.gameObject); }
                    // 近い？
                    if (DistanceNoneY(m_TargetObj.transform.position, 1.0f))
                    {
                        // 食べる状態に変更
                        m_State.EnemySetState(Enemy_State.STATE.EAT);
                        AnimatorFuraguInit();
                        m_Animator.SetBool("ToEat", true);
                        MoveHoming(transform.position);     // 止まる
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

            case Enemy_State.STATE.EAT:      // 食べる
                //Debug_State_Text.text = "STATE:食べているよ";

                // 攻撃可能
                m_State.CanSet(true);
                Debug.Log("STATE:食べているよ");

                // 食べ終わった？
                if (m_TargetObj == null)
                {
                    Debug.Log("m_TargetObj:null");
                    // 次を探す
                    m_State.EnemySetState(Enemy_State.STATE.MOVE);
                    m_Animator.SetBool("ToEat", false);
                    m_State.CanSet(false);
                    break;
                }

                // 農作物体力を減らす
                Life target_life = m_TargetObj.GetComponent<Life>();
                target_life.SubLife(Time.deltaTime * m_EatSpeed);

                Debug.Log(Time.deltaTime * m_EatSpeed);
                Debug.Log(target_life.GetLife());
                // 食べ終わった？
                if (target_life.GetLife() <= 0) { Destroy(m_TargetObj.gameObject); }
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
                    if (m_LifeList != null) { Destroy(m_LifeList.gameObject); }
                    GameObject effet = Instantiate(m_AttackEffect, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;

                    m_TargetObj = SerchCrops(m_CropIndex);
                    m_State.EnemySetState(Enemy_State.STATE.MOVE);
                    m_Animator.SetBool("ToAttack", false);
                }

                break;

            case Enemy_State.STATE.SPRAY:
                //Debug_State_Text.text = "STATE:見えねぇ！！";   // テスト
                // フラグをスプレーを受けたに変更
                m_isBuff = true;
                // 匂いのエフェクトの再生
                m_BuffEffect.SetActive(true);

                // 直前が食事状態なら
                if (m_State.GetStateOld() == Enemy_State.STATE.EAT)
                {
                    // 食事を続く
                    m_State.EnemySetState(Enemy_State.STATE.EAT);
                    break;
                }

                // 移動状態へ
                m_State.EnemySetState(Enemy_State.STATE.MOVE);
                break;

            case Enemy_State.STATE.DAMAGE:      // ダメージ状態
                //Debug_State_Text.text = "STATE:痛えぇ！";

                if (!m_isBuff)
                {
                    // 直前が食事状態なら
                    if (m_State.GetStateOld() == Enemy_State.STATE.EAT)
                    {
                        // 食事を続く
                        m_State.EnemySetState(Enemy_State.STATE.EAT);
                        break;
                    }
                    m_State.EnemySetState(Enemy_State.STATE.MOVE);     // 移動状態へ
                    break;
                }
                // フラグをスプレーを受けてないに変更
                m_isBuff = false;
                m_isEatDamage = true;
                // 体力を減らす
                SubLife(1.0f);

                // エフェクトの生成
                GameObject damage_effect = Instantiate(m_DamageEffect, transform.position, Quaternion.identity) as GameObject;

                m_TargetObj = m_AttackObj;
                // 怯む
                m_State.EnemySetState(Enemy_State.STATE.FEAR);
                m_Animator.SetBool("ToFear", true);
                m_State.CanSet(false);
                break;

            case Enemy_State.STATE.BACK:      // ダメージ
                //Debug_State_Text.text = "STATE:あぁ！！";

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
                    m_isEatDamage = false;
                    // 次の農作物を狙う
                    m_CropIndex++;
                    m_TargetObj = SerchCrops(m_CropIndex);          // 農作物をサーチ
                }
                break;
                
            case Enemy_State.STATE.FEAR:        // 怯む
                //Debug_State_Text.text = "STATE:回る回る";
                if (m_isLastAttack)
                {
                    // プレイヤーバズーカ発射したら気絶する
                    // テスト
                    // オナラゲージがMAX？
                    if (m_PlayerAll.m_bTankMax)
                    {
                        CreateLastLife();
                    }
                    if (!m_PlayerAll.m_bLastBuster) break;
                    if( m_LastLife != null) { Destroy(m_LastLife.gameObject); }
                    m_Animator.SetBool("ToFaint", true);
                    m_State.EnemySetState(Enemy_State.STATE.FAINT);
                    m_Nav.enabled = false;
                    break;
                }
                
                m_FearTimer -= Time.deltaTime;
                if( m_FearTimer <= 0.0f)
                {
                    m_FearTimer = FEAR_TIME;
                    if (m_isEatDamage)
                    {
                        m_State.EnemySetState(Enemy_State.STATE.MOVE);
                        AnimatorFuraguInit();
                        m_Animator.SetBool("ToMove", true);
                    }
                    else
                    {
                        m_State.EnemySetState(Enemy_State.STATE.CRY);
                        AnimatorFuraguInit();
                        m_Animator.SetBool("ToMove", true);
                        m_Animator.SetBool("ToCry", true);
                        m_Nav.enabled = true;
                        m_Nav.updateRotation = true;
                        m_Nav.speed = m_NavMoveSpeed;
                        m_Nav.acceleration = 8;
                    }
                }
                break;

            case Enemy_State.STATE.FAINT:   // 気絶
                //Debug_State_Text.text = "STATE:おっふ";
                m_State.CanSet(false);
                m_Nav.enabled = false;

                m_BlowEffect.SetActive(true);
                m_EscapeEffect.SetActive(true);

                Vector3 vec = transform.position;
                vec = m_AttackObj.transform.position - vec;
                vec = -Vector3.Normalize(vec);

                gameObject.GetComponent<Rigidbody>().freezeRotation = false;

                gameObject.GetComponent<Rigidbody>().AddTorque(Vector3.right * Mathf.PI * 100);


                transform.position += new Vector3(vec.x, 1, vec.z) * 50f * Time.deltaTime;

                m_BlowEffect.transform.localPosition = transform.localPosition;

                return;
        }
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

    // 農作物リストから目標を取得
    GameObject SerchCrops()
    {
        //目標を配列で取得する
        foreach (GameObject obs in m_NavCrops)
        {
            // nullじゃなかったら返す
            if (obs != null) { return obs; }
        }
        // ゲームオーバー




















        // リストがなくなったらnull
        return null;
    }
    // 農作物リストからインデックスで目標を取得
    GameObject SerchCrops(int Index)
    {
        // nullリストの除外
        for (int i = 0; i < m_NavCrops.Count; ++i)
        {
            if (m_NavCrops[i] == null)
            {
                m_NavCrops.Remove(m_NavCrops[i]);
                continue;
            }
        }
        // リストなくなった？
        if (m_NavCrops.Count <= 0)
        {
            // ゲームオーバー




















            return null;
        }
        // インデックスがリストより大きかったら、最初のオブジェクトを返す
        if (Index >= m_NavCrops.Count) { Index = 0; }
        m_CropIndex = Index;
        return m_NavCrops[Index];
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
    void CreateLastLife()
    {
        if (m_LastLife != null) return;
        // プレハブを取得
        GameObject prefab = (GameObject)Resources.Load("Prefabs/Enemy/P_Boss_LastLife");

        // プレハブからインスタンスを生成
        m_LastLife = (GameObject)Instantiate(prefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        // 作成したオブジェクトを子として登録
        m_LastLife.transform.parent = transform;
        m_LastLife.transform.localPosition = new Vector3(0.0f, 0.7f, 0.0f);
        m_LastLife.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        m_LastLife.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
    }
    void SubLife(float var)
    {
        // 体力を減らす
        m_Life.SubLife(var);
        // 体力がなくなったら
        if (m_Life.GetLife() <= 0)
        {
            m_isLastAttack = true;
        }
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
