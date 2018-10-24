using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Life))]
[RequireComponent(typeof(Enemy_State))]
[RequireComponent(typeof(Enemy_Score))]

public class Enemy_Inago : MonoBehaviour
{

    [SerializeField]
    TextMesh Debug_State_Text;
    [SerializeField]
    Renderer m_Color;               // 自分の色
    [SerializeField]
    float m_Satiety;                // 満腹度
    [SerializeField]
    float m_EatSpeed = 1.0f;        // 食べるスピード
    [SerializeField]
    float m_JumpHeight = 10.0f;     // ジャンプ力
    [SerializeField]
    float m_Jump = 150.0f;          // ジャンプ力
    [SerializeField]
    float m_CntJump = 3.0f;         // ジャンプ間隔
    [SerializeField]
    GameObject[] m_NavCrops;        // 農作物リスト


    private GameObject m_TargetObj;     // ターゲットオブジェクト
    private Enemy_State m_State;        // 状態
    private Vector3 m_PosOld;           // 満腹後向かう座標
    private Life m_Life;                // 体力
    private float m_JumpTiming;         // ジャンプ間隔
    private Rigidbody m_Rigidbody;      // 移動用ボディ
    private Color m_FadeColor;

    // 初期化
    void Start()
    {
        m_Life = GetComponent<Life>();
        m_State = GetComponent<Enemy_State>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_TargetObj = SerchCrops();                         // 農作物をサーチ
        m_PosOld = transform.position;                      // 満腹後向かう座標のセット
        m_JumpTiming = m_CntJump;                           // ジャンプ間隔
        m_FadeColor = m_Color.material.color;
        // スコアセット
        Enemy_Score score = GetComponent<Enemy_Score>();
        score.SetScore(Score_List.Enemy.Inago);
    }

    // Update is called once per frame
    void Update()
    {
        // 状態判定
        switch (m_State.GetState())
        {
            case Enemy_State.STATE.MOVE:     // 移動
                Debug_State_Text.text = "STATE:Jump(Move)";
                // 目標がなくなった？
                if (m_TargetObj == null)
                {
                    // 再検索
                    m_TargetObj = SerchCrops();          // 農作物をサーチ
                    break;
                }
                // ジャンプ
                Jump(m_TargetObj.transform.position);
                // 近い？
                if (Vector3.Distance(transform.position, m_TargetObj.transform.position) <= 2.0f)
                {
                    // 食べる状態に変更
                    m_State.SetState(Enemy_State.STATE.EAT);
                    // ジャンプタイミングをリセット
                    m_CntJump = m_JumpTiming;
                }
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
                        break;
                    }
                    // 次を探す
                    m_State.SetState(Enemy_State.STATE.MOVE);
                }
                // 満腹？
                if (m_Satiety <= 0.0f)
                {
                    m_State.SetState(Enemy_State.STATE.SATIETY);
                }
                // 農作物を食べる
                EatCrop(m_TargetObj.GetComponent<Life>());

                break;

            case Enemy_State.STATE.SATIETY:  // 満腹
                Debug_State_Text.text = "STATE:満腹";
                // ジャンプ
                Jump(m_PosOld,0.5f);

                // 目標に着いた？
                if (Vector3.Distance(m_PosOld, transform.position) <= 1.0f)
                {
                    // 自分を消す
                    Destroy(gameObject);
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
                m_State.SetState(Enemy_State.STATE.MOVE);     // 移動状態へ
                break;

            case Enemy_State.STATE.ESCAPE:   // 逃げる
                Debug_State_Text.text = "STATE:FadeOut";

                // 離脱の位置の方向に移動
                Jump(m_PosOld);

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

    // 農作物リストから目標を取得
    GameObject SerchCrops()
    {
        //目標を配列で取得する
        foreach (GameObject obs in m_NavCrops)
        {
            // nullじゃなかったら返す
            if (obs != null) { return obs; }
        }
        // リストがなくなったらnull
        m_State.SetState(Enemy_State.STATE.SATIETY);
        return null;
    }
    
    // ジャンプ
    void Jump(Vector3 Target, float JumpVar = 1.0f)
    {
        m_CntJump -= Time.deltaTime;
        if (m_CntJump <= 0.0f)
        {
            m_CntJump = m_JumpTiming;
            //対象の位置の方向にジャンプ
            LookAtNoneY(Target);
            MoveHoming(Target, m_Jump * JumpVar);
        }
    }

    // Y軸無視でターゲットに向く
    void LookAtNoneY(Vector3 Target)
    {
        Vector3 target = Target;
        target.y = transform.position.y;       // y軸無視
        transform.LookAt(target);
    }

    // 目標に移動
    void MoveHoming(Vector3 Target, float Speed)
    {
        Vector3 move = Target - transform.position;                     // 目的へのベクトル
        move = move.normalized;                                         // 正規化
        move.y += m_JumpHeight;
        m_Rigidbody.AddForce(move * Speed);
    }

    // 農作物を食べる
    void EatCrop(Life TargetLife)
    {
        TargetLife.SubLife(Time.deltaTime * m_EatSpeed);
        if (TargetLife.GetLife() <= 0)
        {
            Destroy(m_TargetObj.gameObject);
        }
        // 満腹までのカウント
        m_Satiety -= Time.deltaTime;
    }
}
