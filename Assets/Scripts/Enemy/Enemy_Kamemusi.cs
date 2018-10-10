using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Kamemusi : MonoBehaviour {

    [SerializeField]
    int m_FlightSpeed_Y_Max = 100;
    [SerializeField]
    int m_FlightSpeed_Y_Min = 50;
    [SerializeField]
    int m_FlightSpeed_X_Max = 100;
    [SerializeField]
    int m_FlightSpeed_X_Min = 50;
    [SerializeField]
    int m_FlightTime_Max = 100;
    [SerializeField]
    int m_FlightTime_Min = 50;

    private float m_TimerCnt;         // カウンター
    private float m_FlightTimer;         // 飛行カウンター
    private float m_FlightSpeedY = 1.0f;     // 飛行速さ
    private float m_FlightSpeedX = 2.0f;     // 飛行速さ

    // Use this for initialization
    void Start () {
        m_FlightSpeedY = Random.Range(m_FlightSpeed_Y_Min, m_FlightSpeed_Y_Max) * 0.001f;
        m_FlightSpeedX = Random.Range(m_FlightSpeed_X_Min, m_FlightSpeed_X_Max) * 0.004f;
        m_FlightTimer = Random.Range(m_FlightTime_Min, m_FlightTime_Max) * 0.1f;
    }
	
	// Update is called once per frame
	void Update () {
        Flight();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Crops")
        {
            //m_Mode = MODE.ATTACK;
        }
    }

    void Flight()
    {
        // フレームカウント
        m_TimerCnt += Time.deltaTime* m_FlightTimer;

        // Y座標の代入
        float moveY = Mathf.Sin(m_TimerCnt) * m_FlightSpeedY;
        float moveX = Mathf.Sin(m_TimerCnt*4) * m_FlightSpeedX;
        transform.position = new Vector3(transform.position.x+ moveX, transform.position.y + moveY, transform.position.z);
    }
}
