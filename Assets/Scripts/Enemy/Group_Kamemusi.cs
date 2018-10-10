using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group_Kamemusi : MonoBehaviour {

    [SerializeField]
    GameObject[] m_NavObj;
    [SerializeField]
    float m_Speed = 0.05f;

    private GameObject m_NextObj;
    private int m_PointID;

    // Use this for initialization
    void Start()
    {
        m_PointID = 0;
        m_NextObj = m_NavObj[m_PointID];
    }

    // Update is called once per frame
    void Update()
    {
        if (m_NextObj == null)
        {
            return;
        }

        //対象の位置の方向を向く
        Vector3 target = m_NextObj.transform.position;
        target.y = this.transform.position.y;       // y軸無視
        transform.LookAt(target);

        //自分自身の位置から相対的に移動する
        Vector3 move = m_NextObj.transform.position - transform.position;   // 目的へのベクトル
        move = move.normalized;                                             // 正規化
        transform.position += move* m_Speed;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Point")
        {
            m_PointID++;
            if( m_PointID >= m_NavObj.Length)
            {
                m_NextObj = null;
                return;
            }
            m_NextObj = m_NavObj[m_PointID];
            Destroy(other.gameObject);
        }
    }
}
