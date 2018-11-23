using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Tank : MonoBehaviour
{

    [SerializeField]
    float m_ChargeValue = 0.0f;

    [SerializeField]
    float m_FurzValue = 0.0f;

    [SerializeField]
    float m_GyroValue = 0.0f;

    [SerializeField]
    float m_fWeak = 0.0f;

    [SerializeField]
    float m_fNormal = 0.0f;

    [SerializeField]
    float m_fStrength = 0.0f;

    [SerializeField]
    RawImage m_FurzUI = null;

    [SerializeField]
    int StartPos;

    [SerializeField]
    int EndPos;

    [SerializeField]
    float m_fMove = 0;

    [SerializeField]
    GameObject m_Boss;

    [SerializeField]
    PlayerAll m_PlayerAll;

    [SerializeField]
    Player m_Player;

    [SerializeField]
    LED m_LED;


    public float m_fTank = 1;

    float m_RectX = 0.0f;

    private float m_fOldMove = 0.0f;

    private float m_fUvRectX = 0;

    private List<Joycon> m_joycons;

    private Joycon m_joyconL;

    private bool m_bFurzFlg = false;

    private bool m_bFiring = false;

    private int m_nTime = 15;
    private int m_nOldTime = 0;

    private float OldPos = 0.0f;

    // Use this for initialization
    void Start () {

        m_nOldTime = m_nTime;

        m_fOldMove = m_fMove;

        m_fUvRectX = 0.01f;

        m_joycons = JoyconManager.Instance.j;

        m_joyconL = m_joycons.Find(c => c.isLeft);
    }

    // Update is called once per frame
    void Update()
    {

        switch (AllManager.Instance.GetStateScene())
        {
            case AllManager.STATE_SCENE.STATE_TITLE:
                break;

            case AllManager.STATE_SCENE.STATE_STAGE:
                Debug.Log(m_RectX);

                if (m_joyconL != null)
                {
                    if (OldPos < -0.2f)
                    {
                        if (m_joyconL.GetGyro().y < -0.2f)
                        {
                            if (m_nTime > 0)
                            {
                                m_nTime--;
                            }

                            if (m_nTime <= 0)
                            {
                                Farmer(0.5f);
                                m_nTime = m_nOldTime;
                            }
                        }
                        else if (m_joyconL.GetGyro().y < -0.1f)
                        {
                            Farmer(0.001f);
                            m_nTime = m_nOldTime;
                        }
                    }
                    else
                    {
                        m_nTime = m_nOldTime;
                    }

                    OldPos = m_joyconL.GetGyro().y;
                }

                GaugeAdjustment();

                WidthAdjustment();

                m_LED.Gage(m_Player.GetPlayerNumber(), m_FurzUI.uvRect.x);

                if (m_Boss.GetComponent<Enemy_Boss_Mix_Kuma>().m_isLastAttack)
                {
                    if (m_FurzUI.uvRect.x == 0)
                    {
                        m_PlayerAll.m_bTankMax = true;
                    }
                }

                break;

            case AllManager.STATE_SCENE.STATE_RESULT:
                break;
        }
    }

    //オナラ貯める
    public void Farmer(float fChargeValue)
    {
        if (m_bFurzFlg){ return; }
        m_fTank -= fChargeValue;
        m_bFurzFlg = true;
        m_bFiring = false;
        m_FurzValue = 0.0f;
        m_ChargeValue = fChargeValue;
        m_fUvRectX = 0.01f;
        m_fMove = m_fOldMove;
    }

    //オナラ発射
    public void FartingFarts(float fFartingValue)
    {
        m_fTank -= fFartingValue;
        m_bFurzFlg = true;
        m_bFiring = true;
        m_FurzValue = 0.0f;
        m_ChargeValue = fFartingValue;
        m_fUvRectX = -0.01f;
        m_fMove = -m_fOldMove;
    }

    //UI幅調整
    private void GaugeAdjustment()
    {
        if (m_bFurzFlg)
        {
            var uvRect = m_FurzUI.uvRect;
            m_FurzValue += m_fUvRectX;
            uvRect.x -= m_fUvRectX;
            m_RectX += m_fUvRectX;
            m_FurzUI.uvRect = uvRect;
            m_FurzUI.rectTransform.localPosition -= new Vector3(m_fMove, 0, 0);
            if (!m_bFiring)
            {
                if (m_FurzValue > m_ChargeValue)
                {
                    m_bFurzFlg = false;
                }
            }
            else
            {
                if (m_FurzValue < m_ChargeValue)
                {
                    m_bFurzFlg = false;
                }
            }
        }
    }

    //UI幅調整
    private void WidthAdjustment()
    {
        if (m_FurzUI.uvRect.x > 1)
        {
            var uvRect = m_FurzUI.uvRect;
            uvRect.x = 1;
            m_RectX = 0;
            m_fTank = 1;
            m_FurzUI.uvRect = uvRect;
            m_FurzUI.rectTransform.localPosition = new Vector3(EndPos,
                                               m_FurzUI.rectTransform.localPosition.y,
                                               m_FurzUI.rectTransform.localPosition.z);
        }

        if (m_FurzUI.uvRect.x < 0)
        {
            var uvRect = m_FurzUI.uvRect;
            uvRect.x = 0;
            m_RectX = 1;
            m_fTank = 0;
            m_FurzUI.uvRect = uvRect;
            m_FurzUI.rectTransform.localPosition = new Vector3(StartPos,
                                                           m_FurzUI.rectTransform.localPosition.y,
                                                           m_FurzUI.rectTransform.localPosition.z);
        }
    }

    public bool GetFurzFlg() { return m_bFurzFlg; }
}
