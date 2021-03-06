﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_FarmGauge : MonoBehaviour {
    
    [SerializeField]
    Text m_FarmNum;   // パーセントのテキスト
    [SerializeField]
    GameObject m_ResultFarm;
    [SerializeField]
    float m_MoveX;
    [SerializeField]
    Result_InfoNum m_ResultInfo;

    Slider m_Farm_UI;        // 畑ゲージ
    int m_FarmVerMax;
    int m_FarmVer;
    float m_Timer;
    int m_Mode;

    void Start () {
        m_Farm_UI = GetComponent<Slider>();
        m_FarmVer = 0;
        m_FarmNum.text = m_FarmVer.ToString()+"%";
        m_FarmVerMax = InfoManager.Instance.GetFarmGauge();
        m_Timer = 0.0f;
        m_Mode = 0;
        m_ResultInfo.SetMode(Result_InfoNum.MODE.MAX);
    }
	
	// Update is called once per frame
	void Update () {
        switch(m_Mode)
        {
            case 0:
                m_Farm_UI.value = m_FarmVer;
                m_FarmVer = Mathf.Min(m_FarmVer + 1, m_FarmVerMax);
                m_FarmNum.text = m_FarmVer.ToString() + "%";
                if( m_FarmVer >= m_FarmVerMax)
                {
                    m_FarmVer = m_FarmVerMax;
                    m_FarmNum.text = m_FarmVerMax.ToString() + "%";
                    UI_CreateNum effect = m_FarmNum.GetComponent<UI_CreateNum>();
                    effect.CreateNum();
                    m_Mode++;
                }
                break;
            case 1:
                m_Timer += Time.deltaTime;
                if (m_Timer >= 3.0f || Input.anyKeyDown)
                {
                    m_Mode++;
                }
                break;
            case 2:
                Vector3 pos = m_ResultFarm.transform.localPosition;
                pos.x -= m_MoveX;
                m_ResultFarm.transform.localPosition = pos;
                if(m_ResultFarm.transform.localPosition.x <= -1000.0f)
                {
                    m_Mode++;
                    m_ResultInfo.SetMode(Result_InfoNum.MODE.SCORE);
                }
                break;
        }
    }
}
