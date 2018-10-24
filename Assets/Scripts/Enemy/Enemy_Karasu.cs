using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Life))]
[RequireComponent(typeof(Enemy_State))]
[RequireComponent(typeof(Enemy_Score))]

public class Enemy_Karasu : MonoBehaviour {

    [SerializeField]
    TextMesh Debug_State_Text;
    [SerializeField]
    GameObject m_FadePoint;     // 退却ポイント
    [SerializeField]
    Renderer m_Color;               // 自分の色
    [SerializeField]
    GameObject[] m_NavObj;          // 農作物リスト
    [SerializeField]
    float m_Speed = 0.05f;          // スピード
    [SerializeField]
    float m_Satiety;            // 満腹度
    [SerializeField]
    float m_EatSpeed = 1.0f;        // 食べるスピード
    [SerializeField]
    GameObject m_TargetObj; // ターゲット

    private Enemy_State m_State;    // 状態
    private Life m_Life;            // 体力
    private Color m_FadeColor;

    // Use this for initialization
    void Start()
    {
        // ターゲットがなければ農作物をターゲットに
        if( !m_TargetObj) { m_TargetObj = m_NavObj[0]; }
        // モードの取得
        m_State = GetComponent<Enemy_State>();
        // 体力の取得
        m_Life = GetComponent<Life>();
        m_FadeColor = m_Color.material.color;
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
            case Enemy_State.STATE.NORMAL:   // 通常
                Debug_State_Text.text = "STATE:Normal";
                m_State.SetState(Enemy_State.STATE.MOVE);
                break;

            case Enemy_State.STATE.MOVE:     // 移動
                Debug_State_Text.text = "STATE:Move";
                // 目標がなくなった？
                if (m_TargetObj == null)
                {
                    // 再検索
                    m_TargetObj = SerchCrops();          // 農作物をサーチ
                    break;
                }
                //対象の位置の方向を向く
                LookAtNoneY(m_TargetObj);
                // 目標へ移動
                MoveHoming(m_TargetObj, m_Speed);

                // 目標が農作物？
                if( m_TargetObj.tag != "Crops") { break; }
                // 近い？
                if (Vector3.Distance(transform.position, m_TargetObj.transform.position) <= 1.0f)
                {
                    // 食べる状態に変更
                    m_State.SetState(Enemy_State.STATE.EAT);
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
                    m_State.SetState(Enemy_State.STATE.NORMAL);
                }
                
                // 農作物体力を減らす
                Life target_life = m_TargetObj.GetComponent<Life>();
                target_life.SubLife(Time.deltaTime * m_EatSpeed);
                // 食べ終わった？
                if (target_life.GetLife() <= 0) { Destroy(m_TargetObj.gameObject); }

                // 満腹までのカウント
                m_Satiety -= Time.deltaTime;
                // カウント到達した？
                if (m_Satiety <= 0.0f)
                {
                    // 満腹になる
                    m_State.SetState(Enemy_State.STATE.SATIETY);
                }
                break;

            case Enemy_State.STATE.SATIETY:  // 満腹
                Debug_State_Text.text = "STATE:満腹";
                // 目標に着いた？
                if (m_TargetObj == null)
                {
                    // カラスを消す
                    Destroy(gameObject.transform.parent.gameObject);
                    return;
                }

                // 対象の位置の方向を向く
                LookAtNoneY(m_TargetObj);
                // 目標へ移動
                MoveHoming(m_TargetObj, m_Speed * 0.5f);
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
                m_State.SetState(Enemy_State.STATE.NORMAL);     // 通常状態へ
                break;

            case Enemy_State.STATE.ESCAPE:   // 逃げる
                Debug_State_Text.text = "STATE:FadeOut";

                // 離脱の位置の方向に移動
                LookAtNoneY(m_FadePoint);
                // 目標へ移動
                MoveHoming(m_FadePoint, m_Speed);

                // アルファ値を減らす
                m_FadeColor.a -= 0.01f;
                m_Color.material.color = m_FadeColor;

                // 透明になった？
                if (m_FadeColor.a > 0.0f) { break; }

                // カラスを消す
                Destroy(gameObject.transform.parent.gameObject);
                return;
        }
    }

    // リストから目標を取得
    GameObject SerchCrops()
    {
        //目標を配列で取得する
        foreach (GameObject obs in m_NavObj)
        {
            if (obs == null) continue;
            if( obs.tag == "Crops") { return obs; }
        }
        // 満腹になる
        m_State.SetState(Enemy_State.STATE.SATIETY);
        // リストがなくなったら退却座標
        return m_FadePoint;
    }

    // Y軸無視でターゲットに向く
    void LookAtNoneY(GameObject Target)
    {
        Vector3 target = Target.transform.position;
        target.y = transform.position.y;       // y軸無視
        transform.LookAt(target);
    }
    void LookAtNoneY(Vector3 TargetPos)
    {
        TargetPos.y = transform.position.y;       // y軸無視
        transform.LookAt(TargetPos);
    }

    // 目標に移動
    void MoveHoming(GameObject Target, float Speed)
    {
        Vector3 move = Target.transform.position - transform.position;   // 目的へのベクトル
        move = move.normalized;                                             // 正規化
        transform.position += move * m_Speed;
    }
    void MoveHoming(Vector3 TargetPos, float Speed)
    {
        Vector3 move = TargetPos - transform.position;   // 目的へのベクトル
        move = move.normalized;                                             // 正規化
        transform.position += move * m_Speed;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Point")
        {
            Destroy(other.gameObject);
        }
    }
}
