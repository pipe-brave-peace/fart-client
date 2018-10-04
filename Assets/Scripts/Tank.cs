using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Tank : MonoBehaviour {

    [SerializeField]
    Slider m_FurzUI;

    [SerializeField]
    float m_ChargeValue = 0.0f;

    [SerializeField]
    float m_GyroValue = 0.0f;

    private List<Joycon> m_joycons;
    private Joycon m_joyconL;

    // Use this for initialization
    void Start () {
        m_joycons = JoyconManager.Instance.j;

        m_joyconL = m_joycons.Find(c => c.isLeft);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            m_FurzUI.value = 0;
        }

        if (m_joyconL != null)
        {
            if (m_joyconL.GetGyro().y < m_GyroValue)
            {
                m_FurzUI.value += m_ChargeValue;
            }
        }

        if (Input.GetKey(KeyCode.C))
        {
            m_FurzUI.value += m_ChargeValue;
        }
    }
}
