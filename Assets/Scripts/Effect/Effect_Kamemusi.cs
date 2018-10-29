using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Kamemusi : MonoBehaviour {

    [SerializeField]
    GameObject m_Canvas;
    [SerializeField]
    GameObject m_Inc;           // インク
    [SerializeField]
    Vector2 m_IncNum;           // インク数の範囲
    [SerializeField]
    Vector2 m_IncScl;           // インクの大きさ
    [SerializeField]
    GameObject m_MoveObj;       // 弾
    [SerializeField]
    private float m_MoveSpeed;  // 移動スピード

    private int         m_IncCnt;       // インクの数
    private float       m_Timer;        // カウンター
    private int         m_Mode;         // モード
    private Vector3     m_MoveVector;   // 移動ベクトル
    private GameObject  m_TargetObj;    // 目的

    // 初期化
    void Start ()
    {
        m_Timer = 0.0f;
        m_IncCnt = Mathf.CeilToInt(Random.Range(m_IncNum.x, m_IncNum.y));   // インク数の代入
        m_Mode = 0;
    }
    public void SetTargetObj(GameObject obj)
    {
        m_TargetObj = obj;
        // 移動量の代入
        m_MoveVector = m_TargetObj.transform.position - m_MoveObj.transform.position;
        m_MoveVector.Normalize();
        m_MoveVector *= m_MoveSpeed;
    }
	
	// Update is called once per frame
	void Update () {
        switch(m_Mode)
        {
            case 0:
                // 弾の移動
                m_MoveVector = m_TargetObj.transform.position - m_MoveObj.transform.position;
                m_MoveVector.Normalize();
                m_MoveVector *= m_MoveSpeed;
                m_MoveObj.transform.position += m_MoveVector;
                // 近い？
                if(Vector3.Distance(m_MoveObj.transform.position, m_TargetObj.transform.position) <= 0.1f)
                {
                    // 弾を消す
                    Destroy(m_MoveObj.gameObject);
                    m_Mode++;   // 次へ
                }
                break;
            case 1:
                if (m_IncCnt <= 0) return;
                m_Timer -= Time.deltaTime;      // カウント
                if (m_Timer > 0.0f) return;
                m_Timer = Random.Range(0.1f, 0.5f); // 次のインクの出るタイミング
                m_IncCnt--;                         // インク数のカウント

                // インクの生成
                GameObject inc = (GameObject)Instantiate(m_Inc);
                inc.transform.SetParent(m_Canvas.transform, false);

                // 生成したインクの配置
                inc.transform.position = new Vector3(Random.Range(0.0f, Screen.width), Random.Range(0.0f, Screen.height), 0.0f);
                float size = Random.Range(m_IncScl.x, m_IncScl.y);
                inc.transform.localScale = new Vector3(size, size, 0.0f);
                break;
        }
    }
}
