using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Life))]
[RequireComponent(typeof(Enemy_State))]
[RequireComponent(typeof(Enemy_Score))]

public class Enemy_Sika : MonoBehaviour {

    [SerializeField]
    TextMesh Debug_State_Text;
    [SerializeField]
    Renderer m_Color;           // 自分の色
    [SerializeField]
    GameObject[] m_NavCrops;    // 農作物リスト
    [SerializeField]
    float m_Satiety;            // 満腹度
    [SerializeField]
    float m_EatSpeed = 1.0f;    // 食べるスピード
    

    private GameObject m_TargetObj;     // ターゲットオブジェクト
    private Enemy_State m_State;        // 状態
    private NavMeshAgent m_Nav;         // ナビメッシュ
    private Vector3 m_PosOld;           // 満腹後向かう座標
    private Life m_Life;                // 体力

    // Use this for initialization
    void Start()
    {
        m_Life = GetComponent<Life>();
        m_State = GetComponent<Enemy_State>();
        m_TargetObj = SerchCrops();                         // 農作物をサーチ
        m_Nav = GetComponent<NavMeshAgent>();               // ナビメッシュの取得
        m_PosOld = transform.position;                      // 満腹後向かう座標のセット
        // スコアセット
        Enemy_Score score = GetComponent<Enemy_Score>();
        score.SetScore(Score_List.Enemy.Sika);
    }

    // Update is called once per frame
    void Update()
    {
        // 死亡した？
        if (m_Life.GetLife() <= 0)
        {
            m_State.SetState(Enemy_State.STATE.ESCAPE);     // 逃げるモード
        }

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
                //対象の位置の方向に移動
                m_Nav.SetDestination(m_TargetObj.transform.position);

                // Y軸無視の距離算出
                Vector2 this_pos = new Vector2(transform.position.x, transform.position.z);
                Vector2 target_pos = new Vector2(m_TargetObj.transform.position.x, m_TargetObj.transform.position.z);
                // 近い？
                if (Vector2.Distance(this_pos, target_pos) <= 1.0f)
                {
                    // 食べる状態に変更
                    m_State.SetState(Enemy_State.STATE.EAT);
                    m_Nav.SetDestination(transform.position);       // 移動を止める
                }
                break;

            case Enemy_State.STATE.EAT:      // 食べる
                Debug_State_Text.text = "STATE:食べているよ";
                // 食べ終わった？
                if (m_TargetObj == null)
                {
                    // だいたい食べた？
                    if( m_Satiety <= 0.5f)
                    {
                        // 満腹になる
                        m_State.SetState(Enemy_State.STATE.SATIETY);
                        // 移動速度が減る
                        m_Nav.speed = m_Nav.speed * 0.5f;
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
                    // 移動速度が減る
                    m_Nav.speed = m_Nav.speed * 0.5f;
                }
                break;

            case Enemy_State.STATE.SATIETY:  // 満腹
                Debug_State_Text.text = "STATE:満腹";
                // 離脱の位置の方向に移動
                m_Nav.SetDestination(m_PosOld);
                // 目標に着いた？
                if (Vector3.Distance(m_PosOld, transform.position) <= 1.0f)
                {
                    // 自分を消す
                    Destroy(gameObject);
                }
                break;

            case Enemy_State.STATE.ESCAPE:   // 逃げる
                Debug_State_Text.text = "STATE:FadeOut";

                // 離脱の位置の方向に移動
                m_Nav.SetDestination(m_PosOld);

                // アルファ値を減らす
                Color color = m_Color.material.color;
                color.a -= 0.01f;
                m_Color.material.color = color;

                // 透明になった？
                if (color.a > 0.0f) { break; }

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
            if( obs != null) { return obs; }
        }
        m_State.SetState(Enemy_State.STATE.SATIETY);
        // リストがなくなったらnull
        return null;
    }
}
