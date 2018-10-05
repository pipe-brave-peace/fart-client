using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Enemy_Boar : MonoBehaviour {
    enum MODE
    {
        NORMAL = 0, // 通常
        MOVE,       // 移動
        EAT,        // 食べる
        ATTACK,     // 攻撃
        SATIETY,    // 満腹
        ESCAPE,     // 逃げる
        MAX
    }

    [SerializeField]
    TextMesh Debug_Mode_Text;
    [SerializeField]
    int m_Life = 1;             // 体力

    GameObject m_NearObj;       // 一番近いオブジェクト
    MODE m_Mode;                // 状態
    NavMeshAgent m_Nav;         // ナビメッシュ
    Vector3 m_PosOld;           // 満腹後向かう座標

    // Use this for initialization
    void Start () {
        m_Mode = MODE.NORMAL;                               // 状態の初期化
        m_NearObj = SerchTag(gameObject, "Crops");          // 一番近い農作物をサーチ
        m_Nav = GetComponent<NavMeshAgent>();               // ナビメッシュの取得
        m_PosOld = transform.position;                      // 満腹後向かう座標のセット
    }
	
	// Update is called once per frame
	void Update () {
        // 状態判定
        switch (m_Mode)
        {
            case MODE.NORMAL:   // 通常
                Debug_Mode_Text.text = "MODE:Normal";
                m_Mode = MODE.MOVE;
                break;

            case MODE.MOVE:     // 移動
                Debug_Mode_Text.text = "MODE:Move";
                // 目標がなくなった？
                if (m_NearObj == null)
                {
                    // 再検索
                    m_NearObj = SerchTag(gameObject, "Crops");
                    break;
                }
                //対象の位置の方向に移動
                m_Nav.SetDestination(m_NearObj.transform.position);
                break;

            case MODE.EAT:      // 食べる
                Debug_Mode_Text.text = "MODE:Eat";
                // 食べ終わった？
                if (m_NearObj == null)
                {
                    // 満腹になる
                    m_Mode = MODE.SATIETY;
                    // 移動速度が減る
                    m_Nav.speed = m_Nav.speed * 0.5f;
                }
                break;

            case MODE.ATTACK:   // 攻撃
                Debug_Mode_Text.text = "MODE:Attack";
                if ( m_NearObj == null)
                {
                    m_Mode = MODE.NORMAL;
                }
                break;

            case MODE.SATIETY:  // 満腹
                Debug_Mode_Text.text = "MODE:満腹";
                // 離脱の位置の方向に移動
                m_Nav.SetDestination(m_PosOld);
                // 目標に着いた？
                if(Vector3.Distance(m_PosOld, transform.position) <= 1.0f)
                {
                    // 自分を消す
                    Destroy(gameObject);
                }
                break;

            case MODE.ESCAPE:   // 逃げる
                Debug_Mode_Text.text = "MODE:FadeOut";
                break;
        }
	}

    //指定されたタグの中で最も近いものを取得
    GameObject SerchTag(GameObject nowObj, string tagName)
    {
        float tmpDis = 0;           //距離用一時変数
        float nearDis = 0;          //最も近いオブジェクトの距離
        GameObject targetObj = null; //オブジェクト

        //タグ指定されたオブジェクトを配列で取得する
        foreach (GameObject obs in GameObject.FindGameObjectsWithTag(tagName))
        {
            //自身と取得したオブジェクトの距離を取得
            tmpDis = Vector3.Distance(obs.transform.position, nowObj.transform.position);

            //オブジェクトの距離が近いか、距離0であればオブジェクト名を取得
            //一時変数に距離を格納
            if (nearDis == 0 || nearDis > tmpDis)
            {
                nearDis = tmpDis;
                targetObj = obs;
            }

        }
        //最も近かったオブジェクトを返す
        return targetObj;
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
        if( m_Life <= 0)
        {
            m_Mode = MODE.ESCAPE;
        }
    }

    // 当たり判定に侵入した時
    void OnTriggerEnter(Collider other)
    {
        // 対象は農作物？
        if (other.gameObject.tag == "Crops")
        {
            // 農作物を食べる
            m_Mode = MODE.EAT;
        }
    }
}
