using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Life))]
[RequireComponent(typeof(Enemy_State))]
[RequireComponent(typeof(Enemy_Score))]

public class Enemy_Eat_Karasu : MonoBehaviour {

    [SerializeField]
    TextMesh     Debug_State_Text;      // テスト
    [SerializeField]
    GameObject[] m_NavObj;              // 農作物リスト
    [SerializeField]
    GameObject   m_FadePoint;           // 退却ポイント
    [SerializeField]
    float        m_MoveSpeed = 0.05f;   // スピード
    [SerializeField]
    float        m_Satiety = 5;         // 満腹度
    [SerializeField]
    float        m_EatSpeed = 1.0f;     // 食べるスピード

    [Header("以下編集しないこと！")]
    [SerializeField]
    SkinnedMeshRenderer m_Color;        // 自分の色
    [SerializeField]
    GameObject   m_DamageEffect;        // ダメージエフェクト
    [SerializeField]
    GameObject   m_EscapeEffect;        // 退却時汗のエフェクト
    [SerializeField]
    GameObject   m_BuffEffect;          // バフ時オナラのエフェクト

    private GameObject  m_TargetObj;    // ターゲット
    private Enemy_State m_State;        // 状態
    private Life        m_Life;         // 体力
    private Vector3     m_FadePos;      // 退却座標
    private Color       m_FadeColor;    // 退却時の色
    private bool        m_isBuff;       // オナラスプレー受けたかどうか
    private Animator    m_Animator;     // アニメション
    private int         m_MoveMode;     // 移動モード：0、上に飛ぶ　1、目標に移動　2、着地
    private float       m_MoveUpTimer;  // 上に飛ぶ時間

    // Use this for initialization
    void Start()
    {
        // コンポーネント取得
        m_State     = GetComponent<Enemy_State>();
        m_Life      = GetComponent<Life>();
        m_Animator  = GetComponent<Animator>();
        // 変数初期化
        m_TargetObj   = m_NavObj[0];                          // 最優先の農作物を代入
        m_FadeColor   = new Color(1.0f, 1.0f, 1.0f, 1.0f);    // 現在の色をセット
        m_isBuff      = false;                                // オナラスプレーに攻撃されていない
        m_MoveMode    = 1;                                    // 目標に移動
        m_MoveUpTimer = 1.0f;                                 // 上に飛ぶ時間の初期化
        // 退却ポイントがない：生成座標を代入
        // 退却ポイントがある：退却ポイント座標を代入
        if ( m_FadePoint == null)
        {
            m_FadePos = transform.position;
        }
        else
        {
            m_FadePos = m_FadePoint.transform.position;
        }
        // スコアセット
        Enemy_Score score = GetComponent<Enemy_Score>();
        score.SetScore(Score_List.Enemy.Karasu);
    }

    // Update is called once per frame
    void Update()
    {
        // 状態判定
        switch (m_State.GetState())
        {
            case Enemy_State.STATE.MOVE:     // 移動
                Debug_State_Text.text = "STATE:Move";  // テスト
                // 目標がなくなったら再検索
                if (m_TargetObj == null)
                {
                    m_TargetObj = SerchCrops();
                    break;
                }

                switch (m_MoveMode)
                {
                    case 0:
                        // 飛行
                        transform.Translate(0, m_MoveSpeed, 0);
                        m_MoveUpTimer -= Time.deltaTime;
                        if( m_MoveUpTimer <= 0.0f)
                        {
                            m_MoveMode++;
                            m_MoveUpTimer = 1.0f;
                        }
                        break;
                    case 1:
                        // 対象の位置の方向を向く
                        LookAtNoneY(m_TargetObj.transform.position);
                        // 目標へ移動
                        MoveHoming(m_TargetObj.transform.position, m_MoveSpeed);
                        // 近くなったら着地準備
                        if (Vector3.Distance(transform.position, m_TargetObj.transform.position) <= 1.0f)
                        {
                            m_MoveMode++;
                        }
                        break;
                    case 2:
                        // 着地移動
                        transform.Translate(0, -m_MoveSpeed, 0);
                        break;
                }
                break;

            case Enemy_State.STATE.EAT:      // 食べる
                Debug_State_Text.text = "STATE:食べているよ";  // テスト
                // 食べ終わったら
                if (m_TargetObj == null)
                {
                    // だいたい食べた？
                    if (m_Satiety <= 0.5f)
                    {
                        // 満腹になる
                        m_State.SetState(Enemy_State.STATE.SATIETY);
                        m_Animator.SetBool("MoveToEat", false);
                        break;
                    }
                    // まだ足りないなら次を探す
                    m_State.SetState(Enemy_State.STATE.MOVE);
                    m_Animator.SetBool("MoveToEat", false);
                    m_Animator.Play("Move");
                    m_MoveMode = 0;
                    break;
                }

                // 食べる処理
                // 食べている農作物体力を減らす
                Life target_life = m_TargetObj.GetComponent<Life>();
                target_life.SubLife(Time.deltaTime * m_EatSpeed);
                // 食べ終わったら農作物を消す
                if (target_life.GetLife() <= 0) { Destroy(m_TargetObj.gameObject); }

                // 満腹までのカウント
                m_Satiety -= Time.deltaTime;
                // 0になったら満腹状態へ
                if (m_Satiety <= 0.0f)
                {
                    // 満腹になる
                    m_State.SetState(Enemy_State.STATE.SATIETY);
                }
                break;

            case Enemy_State.STATE.SPRAY:      // スプレー状態
                Debug_State_Text.text = "STATE:見えねぇ！！";   // テスト
                // フラグをスプレーを受けたに変更
                m_isBuff = true;
                // 匂いのエフェクトの再生
                m_BuffEffect.SetActive(true);

                // 直前が食事状態なら
                if (m_State.GetStateOld() == Enemy_State.STATE.EAT)
                {
                    // 食事状態へ
                    m_State.SetState(Enemy_State.STATE.EAT);
                    m_Animator.SetBool("MoveToEat", true);
                    break;
                }
                // 移動状態へ
                m_State.SetState(Enemy_State.STATE.MOVE);
                m_Animator.SetBool("MoveToEat", false);
                // 対象の位置の方向を向く
                LookAtNoneY(m_TargetObj.transform.position);
                // 目標へ移動
                MoveHoming(m_TargetObj.transform.position, m_MoveSpeed);
                break;

            case Enemy_State.STATE.DAMAGE:      // ダメージ状態
                Debug_State_Text.text = "STATE:痛えぇ！";  // テスト
                // オナラスプレーに攻撃されていないならダメージ処理しない
                if (!m_isBuff)
                {
                    m_State.SetState(Enemy_State.STATE.MOVE);     // 移動状態へ
                    // 対象の位置の方向を向く
                    LookAtNoneY(m_TargetObj.transform.position);
                    // 目標へ移動
                    MoveHoming(m_TargetObj.transform.position, m_MoveSpeed);
                    break;
                }
                // 体力を減らす
                m_Life.SubLife(1.0f);
                // フラグをスプレーを受けてないに変更
                m_isBuff = false;
                // 匂いのエフェクトの停止
                m_BuffEffect.SetActive(false);

                // エフェクトの生成
                GameObject damage_effect = Instantiate(m_DamageEffect, transform.position, Quaternion.identity) as GameObject;

                // 体力がなくなった？
                if (m_Life.GetLife() <= 0)
                {
                    // 透明できる描画モードに変更
                    BlendModeUtils.SetBlendMode(m_Color.material, BlendModeUtils.Mode.Fade);
                    m_Color.material.color = m_FadeColor;
                    m_State.SetState(Enemy_State.STATE.ESCAPE);     // 離脱状態へ
                    m_Animator.SetBool("MoveToEat", false);
                    m_Animator.Play("Move");
                    break;
                }
                m_State.SetState(Enemy_State.STATE.MOVE);     // 移動状態へ
                break;

            case Enemy_State.STATE.SATIETY:  // 満足
                Debug_State_Text.text = "STATE:満足";  // テスト

                // 離脱の位置の方向に移動
                LookAtNoneY(m_FadePos);
                // 目標へ移動
                MoveHoming(m_FadePos, m_MoveSpeed);

                // 近い？
                if (Vector3.Distance(m_FadePos, transform.position) <= 1.0f)
                {
                    // カラスを消す
                    Destroy(gameObject.transform.parent.gameObject);
                    return;
                }
                break;

            case Enemy_State.STATE.ESCAPE:   // 逃げる
                Debug_State_Text.text = "STATE:FadeOut";  // テスト

                // 状態遷移はもうできない
                m_State.CanSet(false);
                // 汗のエフェクトを出す
                m_EscapeEffect.SetActive(true);
                // 離脱の位置の方向に移動
                LookAtNoneY(m_FadePos);
                // 目標へ移動
                MoveHoming(m_FadePos, m_MoveSpeed);

                // アルファ値を減らす
                m_FadeColor.a -= 0.02f;
                m_Color.material.color = m_FadeColor;

                // 汗を止める
                if (m_FadeColor.a <= 0.3f) { m_EscapeEffect.SetActive(false); }

                // 透明になった親を消す
                if (m_FadeColor.a <= 0.0f) { Destroy(transform.parent.gameObject); }
                return;
        }
    }

    // リストから目標を取得
    GameObject SerchCrops()
    {
        //目標を配列で取得する
        foreach (GameObject obs in m_NavObj)
        {
            if( obs != null) { return obs; }
        }
        // リストがなくなったら満腹になる
        m_State.SetState(Enemy_State.STATE.SATIETY);
        m_Animator.SetBool("MoveToEat", false);
        return null;
    }

    // Y軸無視でターゲットに向く
    void LookAtNoneY(Vector3 TargetPos)
    {
        TargetPos.y = transform.position.y;       // y軸無視
        transform.LookAt(TargetPos);              // ターゲットに向く
        transform.Rotate(new Vector3(0, 60, 0));  // 向きの微調整
    }

    // 目標に移動
    void MoveHoming(Vector3 TargetPos, float Speed)
    {
        Vector3 move = TargetPos - transform.position;   // 目的へのベクトル
        move = move.normalized;                          // 正規化
        transform.position += move * Speed;              // 移動処理
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Terrain")
        {
            // 食べる状態に変更
            m_State.SetState(Enemy_State.STATE.EAT);
            m_Animator.SetBool("MoveToEat", true);
        }
    }
}
