using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Life))]
[RequireComponent(typeof(Enemy_State))]
[RequireComponent(typeof(Enemy_Score))]

public class Enemy_Inosisi : MonoBehaviour {

    //[SerializeField]
    //TextMesh     Debug_State_Text;      // テスト
    [SerializeField]
    GameObject   m_FadePoint;           // 退却ポイント
    [SerializeField]
    GameObject   m_TargetObj;           // 目標オブジェクト
    [SerializeField]
    float        m_AttackTimer;         // 突進までのカウント
    [SerializeField]
    GameObject[] m_NavCrops;            // 農作物リスト
    [SerializeField]
    float        m_Satiety;             // 満腹度
    [SerializeField]
    float        m_EatSpeed = 1.0f;     // 食べるスピード
    [SerializeField]
    SkinnedMeshRenderer m_Color;        // 自分の色
    [SerializeField]
    GameObject   m_AttackEffect;        // 攻撃エフェクト
    [SerializeField]
    GameObject   m_DamageEffect;        // 弾の爆発エフェクト
    [SerializeField]
    GameObject   m_EscapeEffect;        // 退却時汗のエフェクト
    [SerializeField]
    GameObject   m_BuffEffect;          // バフ時オナラのエフェクト

    private Enemy_State     m_State;        // 状態
    private NavMeshAgent    m_Nav;          // ナビメッシュ
    private float           m_MoveSpeed;    // 移動スピード
    private Vector3         m_FadePos;      // 退却座標
    private Life            m_Life;         // 体力
    private Color           m_FadeColor;    // 退却時の色の変化用
    private bool            m_isAttack;     // 攻撃したかどうか
    private bool            m_AttackMode;   // 攻撃モードの突進準備と突進の判別
    private bool            m_isBuff;       // オナラスプレー受けたかどうか
    private Animator        m_Animator;     // アニメション

    // 初期化
    void Start()
    {
        // ターゲットの代入
        if (!m_TargetObj) { m_TargetObj = m_NavCrops[0]; }

        // コンポーネントの取得
        m_Life      = GetComponent<Life>();
        m_State     = GetComponent<Enemy_State>();
        m_Nav       = GetComponent<NavMeshAgent>();
        m_Animator  = GetComponent<Animator>();

        // 変数初期化
        m_MoveSpeed  = m_Nav.speed;                          // 移動スピードの代入
        m_FadeColor  = new Color( 1.0f,1.0f,1.0f,1.0f);      // 現在の色をセット
        m_isAttack   = false;                                // 攻撃していない
        m_isBuff     = false;                                // オナラスプレーに攻撃されていない
        m_AttackMode = false;                                // 攻撃モードに入ったら突進準備
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
        score.SetScore(Score_List.Enemy.Sika);
    }

    // 更新処理
    void Update()
    {
        // 状態判定
        switch (m_State.GetState())
        {
            case Enemy_State.STATE.MOVE:     // 移動
                //Debug_State_Text.text = "STATE:Move";   // テスト
                // 移動処理
                StateMove();
                break;

            case Enemy_State.STATE.EAT:      // 食べる
                //Debug_State_Text.text = "STATE:食べているよ";   // テスト
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
                    m_Animator.SetBool("MoveToEat", false);
                }
                break;

            case Enemy_State.STATE.ATTACK:      // 攻撃
                // モードの判別：false-突進準備、true-突進
                if( !m_AttackMode)
                {
                    //Debug_State_Text.text = "STATE:突進準備";   // テスト
                    // アニメション終わった？
                    if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
                    {
                        // 突進へ
                        m_AttackMode = true;
                        // 移動アニメーションへ
                        m_Animator.Play("Move");
                        // 対象の位置の方向に移動
                        MoveHoming(m_TargetObj.transform.position);
                        // 移動スピードアップ
                        m_Nav.speed = m_MoveSpeed * 3.0f;
                    }
                }
                else
                {
                    //Debug_State_Text.text = "STATE:進め！！！";   // テスト
                    // 対象の位置の方向に移動
                    MoveHoming(m_TargetObj.transform.position);
                    // 対象と近いなら
                    if (DistanceNoneY(m_TargetObj.transform.position, 3.0f))
                    {
                        // 攻撃のエフェクトを生成
                        GameObject attack_effect = Instantiate(m_AttackEffect, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
                        // 攻撃するプレイヤーを判別
                        //attack_effect.GetComponent<Effect_Damage>().Set(m_TargetObj.GetComponent<Player>().GetPlayerNumber());
                        attack_effect.GetComponent<Effect_Damage>().Set(0);
                        // フラグを攻撃したに変更
                        m_isAttack = true;
                        // 満足状態へ
                        m_State.SetState(Enemy_State.STATE.SATIETY);
                        // 通常スピード
                        m_Nav.speed = m_MoveSpeed;
                        return;
                    }
                }
                break;

            case Enemy_State.STATE.SATIETY:     // 満足（攻撃したら）
                //Debug_State_Text.text = "STATE:満足した";   // テスト
                
                // 退却座標に向かう
                MoveHoming(m_FadePos);

                // 退却座標に近いなら自分を消す
                if (DistanceNoneY(m_FadePos, 1.0f)) { Destroy(gameObject); }
                break;

            case Enemy_State.STATE.SPRAY:      // スプレー状態
                //Debug_State_Text.text = "STATE:見えねぇ！！";   // テスト
                // フラグをスプレーを受けたに変更
                m_isBuff = true;
                // 匂いのエフェクトの再生
                m_BuffEffect.SetActive(true);

                // 直前が攻撃状態なら
                if( m_State.GetStateOld() == Enemy_State.STATE.ATTACK)
                {
                    // 攻撃を続く
                    m_State.SetState(Enemy_State.STATE.ATTACK);
                    break;
                }

                // 移動状態へ
                m_State.SetState(Enemy_State.STATE.MOVE);
                // 移動処理
                StateMove();
                break;

            case Enemy_State.STATE.DAMAGE:      // ダメージ状態
                // オナラスプレーに攻撃されていないならダメージ処理しない
                if (!m_isBuff )
                {
                    m_State.SetState(Enemy_State.STATE.MOVE);     // 移動状態へ
                    StateMove();                                  // 移動処理
                    break;
                }
                // ダメージ処理
                //Debug_State_Text.text = "STATE:痛えぇ！";   // テスト
                // 通常スピードに戻る
                m_Nav.speed = m_MoveSpeed;
                // 体力を減らす
                m_Life.SubLife(1.0f);
                // フラグをスプレーを受けてないに変更
                m_isBuff = false;
                // 匂いのエフェクトの停止
                m_BuffEffect.SetActive(false);

                // エフェクトの生成
                GameObject damage_effect = Instantiate(m_DamageEffect, transform.position, Quaternion.identity) as GameObject;

                // 体力がなくなったら
                if (m_Life.GetLife() <= 0)
                {
                    // 透明できる描画モードに変更
                    BlendModeUtils.SetBlendMode(m_Color.material, BlendModeUtils.Mode.Fade);
                    m_Color.material.color = m_FadeColor;           // 色の代入
                    m_State.SetState(Enemy_State.STATE.ESCAPE);     // 離脱状態へ
                    m_Animator.SetBool("MoveToEat", false);
                    m_Animator.SetBool("MoveToAttack", false);
                    break;
                }
                m_State.SetState(Enemy_State.STATE.MOVE);     // 移動状態へ
                break;

            case Enemy_State.STATE.ESCAPE:   // 逃げる
               // Debug_State_Text.text = "STATE:FadeOut";   // テスト
                // 状態遷移はもうできない
                m_State.CanSet(false);
                // 汗のエフェクトを出す
                m_EscapeEffect.SetActive(true);
                // 離脱の位置の方向に移動
                m_Nav.SetDestination(m_FadePos);

                // 消えていく
                m_FadeColor.a -= 0.02f;
                m_Color.material.color = m_FadeColor;

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
        m_Nav.SetDestination(Target);          // ナビメッシュ上での移動処理
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

    // 農作物リストから目標を取得
    GameObject SerchCrops()
    {
        //目標を配列で取得する
        foreach (GameObject obs in m_NavCrops)
        {
            // nullじゃなかったら返す
            if (obs != null) { return obs; }
        }
        // リストがなくなったら満足状態に変更
        m_State.SetState(Enemy_State.STATE.SATIETY);
        return null;
    }
    // 移動モード
    void StateMove()
    {
        // 目標が農作物の場合
        if (m_TargetObj.tag == "Crops")
        {
            // だいたい食べた
            if (m_Satiety <= 0.5f)
            {
                // 離脱する
                m_State.SetState(Enemy_State.STATE.SATIETY);
                return;
            }
            // 目標がなくなった？
            if (m_TargetObj == null)
            {
                // 再検索
                m_TargetObj = SerchCrops();          // 農作物をサーチ
                return;
            }
            // 対象と近いなら
            if (DistanceNoneY(m_TargetObj.transform.position, 3.0f))
            {
                // 食べる状態に変更
                m_State.SetState(Enemy_State.STATE.EAT);
                m_Animator.SetBool("MoveToEat", true);
                m_Nav.SetDestination(transform.position);       // 移動を止める
                return;
            }
        }
        // 目標がプレイヤーの場合
        else
        {
            // 攻撃した？
            if (m_isAttack)
            {
                // 離脱する
                m_State.SetState(Enemy_State.STATE.SATIETY);
                return;
            }
            // 突進までのカウント
            m_AttackTimer -= Time.deltaTime;
            if( m_AttackTimer <= 0.0f)
            {
                // 攻撃準備状態に変更
                m_State.SetState(Enemy_State.STATE.ATTACK);
                m_Animator.Play("Attack");
                m_Nav.SetDestination(transform.position);       // 移動を止める
                return;
            }
        }
        // 対象の位置の方向に移動
        MoveHoming(m_TargetObj.transform.position);
    }
}
