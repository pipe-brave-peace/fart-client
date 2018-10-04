using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Enemy_Boar : MonoBehaviour {
    enum MODE
    {
        NORMAL = 0,
        MOVE,
        ATTACK,
        DEATH,
        MAX
    }

    [SerializeField]
    TextMesh Debug_Mode_Text;
    [SerializeField]
    int m_Life = 1;

    GameObject m_NearObj;
    MODE m_Mode;
    NavMeshAgent m_Nav;

    // Use this for initialization
    void Start () {
        m_Mode = MODE.NORMAL;
        m_NearObj = SerchTag(gameObject, "Crops");
        m_Nav = GetComponent<NavMeshAgent>();
    }
	
	// Update is called once per frame
	void Update () {
        switch (m_Mode)
        {
            case MODE.NORMAL:
                Debug_Mode_Text.text = "MODE:Normal";
                m_NearObj = SerchTag(gameObject, "Crops");
                m_Mode = MODE.MOVE;
                break;

            case MODE.MOVE:
                Debug_Mode_Text.text = "MODE:Move";
                if (m_NearObj == null)
                {
                    break;
                }
                //対象の位置の方向を向く
                m_Nav.SetDestination(m_NearObj.transform.position);
                break;

            case MODE.ATTACK:
                Debug_Mode_Text.text = "MODE:Attack";
                if ( m_NearObj == null)
                {
                    m_Mode = MODE.NORMAL;
                }
                break;
            case MODE.DEATH:
                Debug_Mode_Text.text = "MODE:Death";
                Destroy(gameObject);
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
            m_Mode = MODE.DEATH;
        }
    }

    // 当たり判定に侵入した時
    void OnTriggerEnter(Collider other)
    {
        // 対象は農作物？
        if (other.gameObject.tag == "Crops")
        {
            // 農作物を食べる
            m_Mode = MODE.ATTACK;
        }
    }
    // 当たり判定から離れた時
    void OnTriggerExit(Collider other)
    {
        // 通常状態に変更
        m_Mode = MODE.NORMAL;
    }
}
