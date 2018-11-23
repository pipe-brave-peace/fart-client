using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Life))]
[RequireComponent(typeof(Enemy_State))]
[RequireComponent(typeof(Enemy_Score))]

public class Enemy_Eat_Inago : MonoBehaviour
{
    [SerializeField]
    float GRAVITY = 0.1f;

    private const float BACK_TIME = 1.0f;

    //[SerializeField]
    //TextMesh     Debug_State_Text;
    [SerializeField]
    GameObject   m_FadePoint;           // 退却ポイント
    [SerializeField]
    GameObject[] m_NavCrops;                // 農作物リスト
    [SerializeField]
    float        m_Satiety;                 // 満腹度
    [SerializeField]
    float        m_EatSpeed = 1.0f;         // 食べるスピード
    [SerializeField]
    float        m_Jump = 1.5f;           // ジャンプ力
    [SerializeField]
    float        m_MoveSpeed = 0.2f;           // ジャンプ力
    [SerializeField]
    float        m_CntJump = 2.0f;          // ジャンプ間隔
    [Header("以下編集しないこと！")]
    [SerializeField]
    SkinnedMeshRenderer m_Color;            // 自分の色
    [SerializeField]
    GameObject          m_DamageEffect;     // ダメージエフェクト
    [SerializeField]
    GameObject          m_EscapeEffect;     // 退却時汗のエフェクト

    private Enemy_State m_State;            // 状態
    private Life        m_Life;             // 体力
    private Animator    m_Animator;         // アニメション
    private NavMeshAgent m_Nav;              // ナビメッシュ

    private float m_NavMoveSpeed;     // 移動スピード
    private float m_BackTimer;        // 後退するカウント

    private GameObject  m_TargetObj;        // ターゲットオブジェクト
    private Vector3     m_TargetPos;        // ターゲット座標
    private Vector3     m_Move;             // 移動量
    private float       m_JumpTiming;       // ジャンプ間隔
    private Vector3     m_FadePos;          // 退却座標
    private Color       m_FadeColor;        // 退却時の色の変化用
    private bool        m_isTerrain;        // 着地判断
    public bool        m_isStop;        // ストップしてるか
    // 初期化
    void Start()
    {
        // ターゲットの代入
        m_TargetObj = m_NavCrops[0];
        m_TargetPos = m_TargetObj.transform.position;

        // コンポーネントの取得
        m_Life      = GetComponent<Life>();
        m_State     = GetComponent<Enemy_State>();
        m_Animator  = GetComponent<Animator>();
        m_Nav = GetComponent<NavMeshAgent>();

        // 変数初期化
        m_NavMoveSpeed = m_Nav.speed;
        m_BackTimer = BACK_TIME;
        m_Animator.speed = 0.0f;
        m_FadeColor  = new Color(1.0f, 1.0f, 1.0f, 1.0f);    // 現在の色をセット
        m_Move       = new Vector3(0.0f, 0.0f, 0.0f);
        m_JumpTiming = m_CntJump;                           // ジャンプ間隔
        m_isTerrain  = false;
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
        score.SetScore(Score_List.Enemy.Inago);
    }

    // Update is called once per frame
    void Update()
    {
        // 移動処理
        if (m_State.GetState() != Enemy_State.STATE.BACK)
        {
            if (!m_isStop)
            {
                if (m_isTerrain)    // 着地
                {
                    m_Animator.Play("Jump", 0, 0.0f);
                    m_CntJump -= Time.deltaTime;
                    if (m_CntJump <= 0.0f)
                    {
                        m_CntJump = m_JumpTiming;
                        //対象の位置の方向を計算
                        m_Move = m_TargetPos - transform.position;     // 目的へのベクトル
                        m_Move.y = 0.0f;
                        m_Move = m_Move.normalized * m_MoveSpeed;     // 目的へのベクトル
                        m_Move.y = m_Jump;
                        m_isTerrain = false;
                    }
                }
                else                // ジャンプ中
                {
                    m_Animator.Play("Jump");
                    m_Animator.speed = 0.7f;
                    LookAtNoneY(m_TargetPos);
                    m_CntJump = m_JumpTiming;
                    m_Move.y = Mathf.Max(m_Move.y - GRAVITY, -1.0f);
                    transform.position += m_Move;
                    m_Nav.enabled = false;
                }
            }
        }
        // 状態判定
        switch (m_State.GetState())
        {
            case Enemy_State.STATE.MOVE:     // 移動
                //Debug_State_Text.text = "STATE:Jump(Move)";
                StateMove();
                break;

            case Enemy_State.STATE.EAT:      // 食べる
                //Debug_State_Text.text = "STATE:食べているよ";
                m_CntJump = m_JumpTiming;
                // 食べ終わった？
                if (m_TargetObj == null)
                {
                    // だいたい食べた？
                    if (m_Satiety <= 0.5f)
                    {
                        // 満腹になる
                        m_State.SetState(Enemy_State.STATE.SATIETY);
                        m_TargetPos = m_FadePos;
                        break;
                    }
                    // 次を探す
                    m_State.SetState(Enemy_State.STATE.MOVE);
                    break;
                }

                // 食べる処理
                // 食べている農作物体力を減らす
                Life target_life = m_TargetObj.GetComponent<Life>();
                target_life.SubLife(Time.deltaTime * m_EatSpeed);
                // 食べ終わったら農作物を消す
                if (target_life.GetLife() <= 0) { Destroy(m_TargetObj.gameObject); }

                // 満腹値をカウントダウン
                m_Satiety -= Time.deltaTime;
                // 0になったら満腹状態へ
                if (m_Satiety <= 0.0f)
                {
                    m_State.SetState(Enemy_State.STATE.SATIETY);
                    m_TargetPos = m_FadePos;
                }
                break;

            case Enemy_State.STATE.SATIETY:  // 満腹
                //Debug_State_Text.text = "STATE:満腹";

                // 目標に着いた？
                if (Vector3.Distance(m_TargetPos, transform.position) <= 10.0f)
                {
                    // 自分を消す
                    Destroy(transform.parent.gameObject);
                }
                break;

            case Enemy_State.STATE.SPRAY:      // スプレー状態
                //Debug_State_Text.text = "STATE:見えねぇ！！";
                
                // 体力を減らす
                m_Life.SubLife(1.0f);

                // 体力がなくなった？
                if (m_Life.GetLife() <= 0)
                {
                    // 透明できる描画モードに変更
                    //BlendModeUtils.SetBlendMode(m_Color.material, BlendModeUtils.Mode.Fade);
                    m_FadeColor.a = 1.0f;
                    m_Color.material.SetColor("_MainColor", m_FadeColor);
                    m_State.SetState(Enemy_State.STATE.BACK);     // 離脱状態へ
                    m_TargetPos = m_FadePos;
                    break;
                }
                // 移動状態へ
                m_State.SetState(Enemy_State.STATE.MOVE);
                // 移動処理
                StateMove();
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
                   // BlendModeUtils.SetBlendMode(m_Color.material, BlendModeUtils.Mode.Fade);
                    m_FadeColor.a = 1.0f;
                    m_Color.material.SetColor("_MainColor", m_FadeColor);
                    m_State.SetState(Enemy_State.STATE.ESCAPE);     // 離脱状態へ
                    m_TargetPos = m_FadePos;
                    break;
                }
                m_State.SetState(Enemy_State.STATE.MOVE);     // 移動状態へ
                // 移動処理
                StateMove();
                break;

            case Enemy_State.STATE.BACK:      // 後退
                //Debug_State_Text.text = "STATE:あ！！！！";

                // 汗のエフェクトを出す
                m_EscapeEffect.SetActive(true);

                // 後退処理
                m_Nav.updateRotation = false;
                Vector3 pos = transform.position;
                pos = Camera.main.transform.position - pos;
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
                    m_Nav.enabled = false;
                }
                break;

            case Enemy_State.STATE.ESCAPE:   // 逃げる
                //Debug_State_Text.text = "STATE:FadeOut";

                // 状態遷移はもうできない
                m_State.CanSet(false);

                // アルファ値を減らす
                m_FadeColor.a -= 0.02f;
                m_Color.material.SetColor("_MainColor", m_FadeColor);

                // 汗を止める
                if (m_FadeColor.a <= 0.3f) { m_EscapeEffect.SetActive(false); }

                // 透明になった親を消す
                if (m_FadeColor.a <= 0.0f) { Destroy(transform.parent.gameObject); }
                return;
        }
    }

    // 農作物リストから目標を取得
    GameObject SerchCrops()
    {
        //目標を配列で取得する
        foreach (GameObject obs in m_NavCrops)
        {
            // nullじゃなかったら返す
            if (obs != null) {
                m_TargetPos = obs.transform.position;
                return obs;
            }
        }
        // リストがなくなったらnull
        m_State.SetState(Enemy_State.STATE.SATIETY);
        m_TargetPos = m_FadePos;
        return null;
    }

    // Y軸無視でターゲットに向く
    void LookAtNoneY(Vector3 Target)
    {
        Target.y = transform.position.y;       // y軸無視
        transform.LookAt(Target);
    }

    // 移動モード
    void StateMove()
    {
        // だいたい食べた？
        if (m_Satiety <= 0.5f)
        {
            // 満腹になる
            m_State.SetState(Enemy_State.STATE.SATIETY);
            m_TargetPos = m_FadePos;
            return;
        }
        // 目標がなくなった？
        if (m_TargetObj == null)
        {
            // 再検索
            m_TargetObj = SerchCrops();          // 農作物をサーチ
            return;
        }
        // 近い？
        if (Vector3.Distance(transform.position, m_TargetPos) <= 2.0f)
        {
            // 食べる状態に変更
            m_State.SetState(Enemy_State.STATE.EAT);
            return;
        }
    }

    // Y軸無視でターゲットに向く
    void MoveHoming(Vector3 Target)
    {
        m_Nav.enabled = true;
        Target.y = transform.position.y;       // y軸無視
        m_Nav.SetDestination(Target);          // ナビメッシュ上での移動処理
    }

    private void OnTriggerEnter(Collider col)
    {
        string layerName = LayerMask.LayerToName(col.gameObject.layer);

        if (col.gameObject.tag == "Terrain")
        {
            m_isTerrain = true;
            m_Nav.enabled = true;
            m_Move = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }
}
