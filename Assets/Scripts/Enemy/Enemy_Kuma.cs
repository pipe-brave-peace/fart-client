using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

[RequireComponent(typeof(Life))]
[RequireComponent(typeof(Enemy_State))]
[RequireComponent(typeof(Enemy_Score))]

public class Enemy_Kuma : MonoBehaviour {
    
    [SerializeField]
    TextMesh Debug_State_Text;
    [Header("農作物を荒らすスピード")]
    [SerializeField]
    float m_EatSpeed = 1.0f;
    [Header("NavCropsがない,TargetObjがあり：攻撃モード")]
    [Header("NavCropsがあり,TargetObjがない：荒らしモード")]
    [Header("NavCropsがあり,TargetObjがあり：ミックスモード")]
    [SerializeField]
    List<GameObject> m_NavPoints;     // 移動ポイントリスト
    [SerializeField]
    List<GameObject> m_NavCrops;      // 農作物リスト
    [SerializeField]
    GameObject m_TargetObj;     // 移動目標
    [Header("怯むまでダメージを受ける回数")]
    [SerializeField]
    int m_FearCnt;
    [Header("ウロウロする時間")]
    [SerializeField]
    float m_UrouroTimer;
    [Header("逃げるまでの時間")]
    [SerializeField]
    float m_EscapeTimer;
    [Header("逃げる時の目的地")]
    [SerializeField]
    GameObject m_FadePoint;     // 退却ポイント
    [SerializeField]
    GameObject m_EffectAttack;     // 攻撃エフェクト

    // フェーズタイプ
    private enum PHASE
    {
        EAT = 0,    // 農作物を荒らす
        ATTACK,     // 攻撃
        MIX,        // 両方
        MAX
    }

    private Enemy_State     m_State;            // 状態
    private NavMeshAgent    m_Nav;              // ナビメッシュ
    private Life            m_Life;             // 体力
    private PHASE           m_Phase;            // フェーズ判断用
    private int             m_FearCntMax;       // 怯むまでダメージを受ける回数
    private GameObject      m_AttackObj;        // 攻撃目標
    private float           m_UrouroTimerMax;   // ウロウロする時間
    private int             m_CropIndex;        // 農作物リストのインデックス
    private Animator        m_Animator;        // アニメション

    // 初期化
    void Start()
    {
        m_Life           = GetComponent<Life>();
        m_State          = GetComponent<Enemy_State>();
        m_Nav            = GetComponent<NavMeshAgent>();               // ナビメッシュの取得
        m_Animator       = GetComponent<Animator>();
        m_FearCntMax     = m_FearCnt;
        m_AttackObj      = m_TargetObj;
        m_UrouroTimerMax = m_UrouroTimer;
        m_CropIndex      = 0;
        // スコアセット
        Enemy_Score score = GetComponent<Enemy_Score>();
        score.SetScore(Score_List.Enemy.Kuma);

        // nullリストの除外
        for (int i = 0; i < m_NavCrops.Count; ++i)
        {
            if (m_NavCrops[i] == null)
            {
                m_NavCrops.Remove(m_NavCrops[i]);
                continue;
            }
        }

        // フェーズチェック ////////////////
        // 農作物リストがなければ攻撃フェーズへ
        if (m_NavCrops.Count <= 0) { m_Phase = PHASE.ATTACK; return; }
        
        // 農作物の先頭をターゲットに
        m_TargetObj = SerchCrops();

        // 攻撃ターゲットがいなければ荒らすフェーズへ
        if (m_AttackObj == null) { m_Phase = PHASE.EAT; return; }

        // 両方ある場合は両方フェーズへ
        m_Phase = PHASE.MIX;
    }

    // Update is called once per frame
    void Update()
    {
        // テスト
        if( Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_Animator.Play("Walk");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            m_Animator.Play("Eat");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            m_Animator.Play("Bark");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            m_Animator.Play("Claw");
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            m_Animator.Play("Flinch");
        }
        // フェーズ判定
        switch (m_Phase)
        {
            case PHASE.EAT      : PhaseEat();       break;
            case PHASE.ATTACK   : PhaseAttack();    break;
            case PHASE.MIX      : PhaseMix();       break;
        }
    }

    // 農作物を荒らすフェーズ
    void PhaseEat()
    {
        // 状態判定
        switch (m_State.GetState())
        {
            case Enemy_State.STATE.MOVE:     // 移動
                Debug_State_Text.text = "STATE:Move";

                // 攻撃不可
                m_State.CanSet(false);

                // 目標がなくなった？
                if (m_TargetObj == null)
                {
                    // 再検索
                    m_TargetObj = SerchCrops(m_CropIndex);          // 農作物をサーチ
                    break;
                }
                // 目標へ移動
                MoveHoming(m_TargetObj);

                // 近い？
                if (DistanceNoneY(m_TargetObj, 1.0f))
                {
                    // 食べる状態に変更
                    m_State.CanSet(true);
                    m_State.SetState(Enemy_State.STATE.EAT);
                    m_Animator.SetBool("WalkToEat", true);
                    MoveHoming(gameObject);     // 止まる
                }
                break;

            case Enemy_State.STATE.EAT:      // 食べる
                Debug_State_Text.text = "STATE:食べているよ";

                // 攻撃可能
                m_State.CanSet(true);

                // 食べ終わった？
                if (m_TargetObj == null)
                {
                    // 次を探す
                    m_State.SetState(Enemy_State.STATE.MOVE);
                    m_Animator.SetBool("WalkToEat", false);
                    break;
                }

                // 農作物体力を減らす
                Life target_life = m_TargetObj.GetComponent<Life>();
                target_life.SubLife(Time.deltaTime * m_EatSpeed);
                // 食べ終わった？
                if (target_life.GetLife() <= 0) { Destroy(m_TargetObj.gameObject); }
                break;

            case Enemy_State.STATE.CRY:
                Debug_State_Text.text = "STATE:がおぉぉ！！！";
                m_Animator.SetBool("WalkToCry", true);

                // アニメション終わった？
                if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                {
                    m_State.SetState(Enemy_State.STATE.MOVE);
                    m_Animator.SetBool("WalkToCry", false);
                }
                break;

            case Enemy_State.STATE.SPRAY:
                m_State.SetState(Enemy_State.STATE.MOVE);
                //m_Animator.Play("Walk");
                break;
                
            case Enemy_State.STATE.DAMAGE:      // ダメージ状態
                Debug_State_Text.text = "STATE:痛えぇ！";

                // 怯むまでのカウント
                m_FearCnt--;
                if( m_FearCnt <= 0)
                {
                    m_FearCnt = m_FearCntMax;                     // カウントのクリア
                    m_State.SetState(Enemy_State.STATE.FEAR);     // 怯む状態へ
                    m_Animator.Play("Flinch");
                    break;
                }
                
                m_State.SetState(Enemy_State.STATE.EAT);     // 食べる状態へ
                m_Animator.Play("Eat");
                break;

            case Enemy_State.STATE.FEAR:        // 怯む
                Debug_State_Text.text = "STATE:怖いよ、怖いよぉ～";

                // 攻撃不能
                m_State.CanSet(false);

                // アニメション終わった？
                if( m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                {
                    m_State.SetState(Enemy_State.STATE.MOVE);
                    m_Animator.Play("Walk");
                }

                // 次の農作物を狙う
                m_CropIndex++;

                break;

            case Enemy_State.STATE.ESCAPE:   // 逃げる
                Debug_State_Text.text = "STATE:FadeOut";

                // 攻撃不能
                m_State.CanSet(false);

                // 離脱の位置の方向に移動
                MoveHoming(m_FadePoint);

                // クマオブジェクトを消す
                if (DistanceNoneY(m_FadePoint, 1.0f)) { Destroy(transform.parent.gameObject); }
                return;
        }

        // 時間来たら逃げる～
        m_EscapeTimer -= Time.deltaTime;
        if (m_EscapeTimer > 0.0f) return;
        if (m_State.GetState() == Enemy_State.STATE.ESCAPE) return;
        if (m_State.GetState() == Enemy_State.STATE.CRY)    return;
        if (m_State.GetState() == Enemy_State.STATE.DAMAGE) return;
        if (m_State.GetState() == Enemy_State.STATE.FEAR)   return;
        m_State.CanSet(true);
        m_State.SetState( Enemy_State.STATE.ESCAPE );
        m_Animator.Play("Walk");
    }

    // プレイヤーを攻撃するフェーズ
    void PhaseAttack()
    {
        // 状態判定
        switch (m_State.GetState())
        {
            case Enemy_State.STATE.MOVE:     // 移動
                Debug_State_Text.text = "STATE:Move";

                // 目標がなくなった？
                if (m_TargetObj == null) { break; }
                // 目標へ移動
                MoveHoming(m_TargetObj);

                // 目標タグチェック
                if (m_TargetObj.tag == "Point") // ポイント？
                {
                    // 攻撃不能
                    m_State.CanSet(false);

                    // ウロウロする時間のカウント
                    m_UrouroTimer -= Time.deltaTime;
                    // 来たら攻撃目標を狙う
                    if( m_UrouroTimer<= 0.0f)
                    {
                        m_UrouroTimer = m_UrouroTimerMax;
                        m_TargetObj = m_AttackObj;
                    }
                }
                else                            // プレイヤー？
                {
                    // 攻撃可能
                    m_State.CanSet(true);

                    // 近い？
                    if (DistanceNoneY(m_TargetObj, 5.0f))
                    {
                        // 攻撃状態に変更
                        m_State.SetState(Enemy_State.STATE.ATTACK);
                        m_Animator.Play("Claw");
                        MoveHoming(gameObject);     // 止まる
                    }
                }
                break;

            case Enemy_State.STATE.CRY:
                Debug_State_Text.text = "STATE:がおぉぉ！！！";
                m_Animator.Play("Bark");
                break;

            case Enemy_State.STATE.ATTACK:      // 攻撃
                Debug_State_Text.text = "STATE:喰らえ！！";

                // アニメション終わった？
                if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                {

                    GameObject effet = Instantiate(m_EffectAttack, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;

                    m_TargetObj = SerchPoint();
                    m_State.SetState(Enemy_State.STATE.MOVE);
                    m_Animator.Play("Walk");
                }

                break;

            case Enemy_State.STATE.DAMAGE:      // ダメージ状態
                Debug_State_Text.text = "STATE:痛えぇ！";

                // 体力を減らす
                m_Life.SubLife(1.0f);

                // 怯むまでのカウント
                m_FearCnt--;
                if (m_FearCnt <= 0)
                {
                    m_FearCnt = m_FearCntMax;                     // カウントのクリア
                    m_State.SetState(Enemy_State.STATE.FEAR);     // 怯む状態へ
                    m_Animator.Play("Flinch");
                    break;
                }

                m_State.SetState(Enemy_State.STATE.MOVE);     // 移動状態へ
                break;

            case Enemy_State.STATE.FEAR:        // 怯む
                Debug_State_Text.text = "STATE:怖いよ、怖いよぉ～";

                // 攻撃不能
                m_State.CanSet(false);

                // アニメション終わった？
                if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                {
                    m_State.SetState(Enemy_State.STATE.MOVE);
                    m_Animator.Play("Walk");
                }

                break;

            case Enemy_State.STATE.ESCAPE:   // 逃げる
                Debug_State_Text.text = "STATE:FadeOut";

                // 攻撃不能
                m_State.CanSet(false);

                // 離脱の位置の方向に移動
                MoveHoming(m_FadePoint);

                // クマオブジェクトを消す
                if (DistanceNoneY(m_FadePoint, 1.0f)) { Destroy(transform.parent.gameObject); }
                return;
        }

        // 時間来たら逃げる～
        m_EscapeTimer -= Time.deltaTime;
        if (m_EscapeTimer > 0.0f) return;
        if (m_State.GetState() == Enemy_State.STATE.ESCAPE) return;
        if (m_State.GetState() == Enemy_State.STATE.ATTACK) return;
        if (m_State.GetState() == Enemy_State.STATE.CRY)    return;
        if (m_State.GetState() == Enemy_State.STATE.DAMAGE) return;
        if (m_State.GetState() == Enemy_State.STATE.FEAR)   return;
        m_State.CanSet(true);
        m_State.SetState( Enemy_State.STATE.ESCAPE );
        m_Animator.Play("Walk");
    }

    // 攻撃と荒らすフェーズ
    void PhaseMix()
    {
        // 状態判定
        switch (m_State.GetState())
        {
            case Enemy_State.STATE.MOVE:     // 移動
                Debug_State_Text.text = "STATE:Move";

                // 目標がなくなった？
                if (m_TargetObj == null)
                {
                    // 再検索
                    m_TargetObj = SerchCrops();          // 農作物をサーチ
                    break;
                }
                // 目標へ移動
                MoveHoming(m_TargetObj);
                
                // 目標タグチェック
                if (m_TargetObj.tag == "Crops") // 農作物？
                {
                    // 攻撃不能
                    m_State.CanSet(false);

                    // 近い？
                    if (DistanceNoneY(m_TargetObj, 1.0f))
                    {
                        // 食べる状態に変更
                        m_State.CanSet(true);
                        m_State.SetState(Enemy_State.STATE.EAT);
                        m_Animator.Play("Eat");
                        MoveHoming(gameObject);     // 止まる
                    }
                }
                else                            // 攻撃目標？
                {
                    // 攻撃可能
                    m_State.CanSet(true);

                    // 近い？
                    if (DistanceNoneY(m_TargetObj, 1.0f))
                    {
                        // 攻撃状態に変更
                        m_State.SetState(Enemy_State.STATE.ATTACK);
                        m_Animator.Play("Claw");
                        MoveHoming(gameObject);     // 止まる
                    }
                }

                break;

            case Enemy_State.STATE.EAT:      // 食べる
                Debug_State_Text.text = "STATE:食べているよ";

                // 攻撃可能
                m_State.CanSet(true);

                // 食べ終わった？
                if (m_TargetObj == null)
                {
                    // 次を探す
                    m_State.SetState(Enemy_State.STATE.MOVE);
                    m_Animator.Play("Walk");
                    break;
                }

                // 農作物体力を減らす
                Life target_life = m_TargetObj.GetComponent<Life>();
                target_life.SubLife(Time.deltaTime * m_EatSpeed);
                // 食べ終わった？
                if (target_life.GetLife() <= 0) { Destroy(m_TargetObj.gameObject); }
                break;

            case Enemy_State.STATE.CRY:
                Debug_State_Text.text = "STATE:がおぉぉ！！！";
                m_Animator.Play("Bark");
                break;

            case Enemy_State.STATE.ATTACK:      // 攻撃
                Debug_State_Text.text = "STATE:喰らえ！！";
                // アニメション終わった？
                if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                {
                    GameObject effet = Instantiate(m_EffectAttack, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;

                    m_TargetObj = SerchCrops();
                    m_State.SetState(Enemy_State.STATE.MOVE);
                    m_Animator.Play("Walk");
                }
                break;

            case Enemy_State.STATE.DAMAGE:      // ダメージ状態
                Debug_State_Text.text = "STATE:痛えぇ！";

                // 体力を減らす
                m_Life.SubLife(1.0f);

                if( m_Life.GetLife() <= 0.0f)
                {
                    m_State.SetState(Enemy_State.STATE.FAINT);     // 気絶状態へ
                    transform.parent = transform.parent.parent.parent;
                }

                // 怯むまでのカウント
                m_FearCnt--;
                if (m_FearCnt <= 0)
                {
                    m_FearCnt = m_FearCntMax;                     // カウントのクリア
                    m_State.SetState(Enemy_State.STATE.FEAR);     // 怯む状態へ
                    m_Animator.Play("Flinch");
                    break;
                }

                m_State.SetState(Enemy_State.STATE.MOVE);     // 食べる状態へ
                m_Animator.Play("Walk");
                break;

            case Enemy_State.STATE.FEAR:        // 怯む
                Debug_State_Text.text = "STATE:怖いよ、怖いよぉ～";

                // 攻撃不能
                m_State.CanSet(false);

                // アニメション終わった？
                if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                {
                    m_State.SetState(Enemy_State.STATE.MOVE);
                    m_Animator.Play("Walk");
                }
                break;

            case Enemy_State.STATE.FAINT:   // 気絶
                Debug_State_Text.text = "STATE:ここどこ？私は誰？";

                // 攻撃不能
                m_State.CanSet(false);

                return;
        }
    }

    // Y軸無視でターゲットに向く
    void MoveHoming(GameObject Target)
    {
        if (Target == null) return;
        Vector3 target  = Target.transform.position;
        target.y        = transform.position.y;         // y軸無視
        m_Nav.SetDestination(target);                   // ナビメッシュ上の移動
    }

    // Y軸無視でターゲットと近い？
    bool DistanceNoneY(GameObject Target, float var)
    {
        if (Target == null) return false;
        // Y軸無視の距離算出
        Vector2 this_pos    = new Vector2(transform.position.x, transform.position.z);
        Vector2 target_pos  = new Vector2(Target.transform.position.x, Target.transform.position.z);

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
        m_State.SetState(Enemy_State.STATE.ESCAPE);
        // リストがなくなったらnull
        return null;
    }
    // 農作物リストからインデックスで目標を取得
    GameObject SerchCrops(int Index)
    {
        // nullリストの除外
        for( int i = 0; i < m_NavCrops.Count; ++i)
        {
            if( m_NavCrops[i] == null)
            {
                m_NavCrops.Remove(m_NavCrops[i]);
                continue;
            }
        }
        // リストなくなった？
        if( m_NavCrops.Count <= 0)
        {
            m_State.SetState(Enemy_State.STATE.ESCAPE);
            return null;
        }
        // インデックスがリストより大きかったら、最初のオブジェクトを返す
        if( Index >= m_NavCrops.Count ) { Index = 0; }
        m_CropIndex = Index;
        return m_NavCrops[Index];
    }
    // ポイントリストから目標を取得
    GameObject SerchPoint()
    {
        // リストがなくなったらnull
        if ( m_NavPoints.Count <= 0) { return null; }

        // ランダムでポイントを返す
        int max = m_NavPoints.Count;
        int pointID = Random.Range(0, max);
        return m_NavPoints[pointID];
    }
}
