﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Tank : MonoBehaviour
{
    [SerializeField]
    Effect_Trigger Effect;

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

        if (Input.GetKey(KeyCode.R))
        {
            //m_FurzUI.value = 0;
        }

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

        DebugController();

        GaugeAdjustment();

        WidthAdjustment();
    }

    //オナラ貯める
    public void Farmer(float fChargeValue)
    {
        if (m_bFurzFlg){ return; }
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
        //Effect.Play();
        m_bFurzFlg = true;
        m_bFiring = true;
        m_FurzValue = 0.0f;
        m_ChargeValue = fFartingValue;
        m_fUvRectX = -0.01f;
        m_fMove = -m_fOldMove;
    }

    //デバッグ用キーボード入力
    private void DebugController()
    {
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    Farmer(m_fWeak);
        //}
        //
        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    Farmer(m_fNormal);
        //}
        //
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    Farmer(m_fStrength);
        //}
    }

    //UI幅調整
    private void GaugeAdjustment()
    {
        if (m_bFurzFlg)
        {
            var uvRect = m_FurzUI.uvRect;
            m_FurzValue += m_fUvRectX;
            uvRect.x -= m_fUvRectX;
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
            m_FurzUI.uvRect = uvRect;
            m_FurzUI.rectTransform.localPosition = new Vector3(EndPos,
                                               m_FurzUI.rectTransform.localPosition.y,
                                               m_FurzUI.rectTransform.localPosition.z);
        }

        if (m_FurzUI.uvRect.x < 0)
        {
            var uvRect = m_FurzUI.uvRect;
            uvRect.x = 0;
            m_FurzUI.uvRect = uvRect;
            m_FurzUI.rectTransform.localPosition = new Vector3(StartPos,
                                                           m_FurzUI.rectTransform.localPosition.y,
                                                           m_FurzUI.rectTransform.localPosition.z);
        }
    }

    public bool GetFurzFlg() { return m_bFurzFlg; }
}
