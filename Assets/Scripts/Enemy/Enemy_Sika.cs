using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Sika : MonoBehaviour {

    [SerializeField]
    TextMesh Debug_Mode_Text;
    [SerializeField]
    int m_Life = 1;             // 体力
    [SerializeField]
    GameObject[] m_NavCrops;    // 農作物リスト
    [SerializeField]
    float m_Satiety;            // 満腹度

    private GameObject m_TargetObj;     // ターゲットオブジェクト
    private Enemy_Mode m_Mode;          // 状態
    private NavMeshAgent m_Nav;         // ナビメッシュ
    private Vector3 m_PosOld;           // 満腹後向かう座標

    // Use this for initialization
    void Start()
    {
        m_Mode = GetComponent<Enemy_Mode>();
        m_TargetObj = SerchCrops();                         // 農作物をサーチ
        m_Nav = GetComponent<NavMeshAgent>();               // ナビメッシュの取得
        m_PosOld = transform.position;                      // 満腹後向かう座標のセット
    }

    // Update is called once per frame
    void Update()
    {
        // 状態判定
        switch (m_Mode.GetMode())
        {
            case Enemy_Mode.MODE.NORMAL:   // 通常
                Debug_Mode_Text.text = "MODE:Normal";
                m_Mode.SetMode(Enemy_Mode.MODE.MOVE);
                break;

            case Enemy_Mode.MODE.MOVE:     // 移動
                Debug_Mode_Text.text = "MODE:Move";
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
                    m_Mode.SetMode(Enemy_Mode.MODE.EAT);
                    m_Nav.SetDestination(transform.position);       // 移動を止める
                }
                break;

            case Enemy_Mode.MODE.EAT:      // 食べる
                Debug_Mode_Text.text = "MODE:Eat";
                // 食べ終わった？
                if (m_TargetObj == null)
                {
                    // だいたい食べた？
                    if( m_Satiety <= 0.5f)
                    {
                        // 満腹になる
                        m_Mode.SetMode(Enemy_Mode.MODE.SATIETY);
                        // 移動速度が減る
                        m_Nav.speed = m_Nav.speed * 0.5f;
                        break;
                    }
                    // 次を探す
                    m_Mode.SetMode(Enemy_Mode.MODE.NORMAL);
                }
                // 満腹までのカウント
                m_Satiety -= Time.deltaTime;
                // カウント到達した？
                if (m_Satiety <= 0.0f)
                {
                    // 満腹になる
                    m_Mode.SetMode(Enemy_Mode.MODE.SATIETY);
                    // 移動速度が減る
                    m_Nav.speed = m_Nav.speed * 0.5f;
                }
                break;

            case Enemy_Mode.MODE.SATIETY:  // 満腹
                Debug_Mode_Text.text = "MODE:満腹";
                // 離脱の位置の方向に移動
                m_Nav.SetDestination(m_PosOld);
                // 目標に着いた？
                if (Vector3.Distance(m_PosOld, transform.position) <= 1.0f)
                {
                    // 自分を消す
                    Destroy(gameObject);
                }
                break;

            case Enemy_Mode.MODE.ESCAPE:   // 逃げる
                Debug_Mode_Text.text = "MODE:FadeOut";
                break;
        }
    }

    // 農作物リストから目標を取得
    GameObject SerchCrops()
    {
        //目標を配列で取得する
        foreach (GameObject obs in m_NavCrops)
        {
            // nullじゃなかったら返す
            if( obs != null)
            {
                return obs;
            }

        }
        // リストがなくなったらnull
        return null;
    }

    // ライフの取得
    public int GetLife()
    {
        return m_Life;
    }

    // ダメージを受ける
    public void SubLife(int Damage = 1)
    {
        m_Life -= Damage;
        if (m_Life <= 0)
        {
            m_Mode.SetMode(Enemy_Mode.MODE.ESCAPE);
        }
    }
}
