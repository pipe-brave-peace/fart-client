using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Operation0 : MonoBehaviour {

    [SerializeField]
    UI_Operation_Text[] Player;

    [SerializeField]
    UI_Operation_Text Text;

    [SerializeField]
    UI_Operation1_Arrow[] Arrow;

    [SerializeField]
    int m_Time;

    int m_OldTime;

    bool m_bUse;

    // Use this for initialization
    void Start () {
        m_OldTime = m_Time;
	}
	
	// Update is called once per frame
	void Update () {
        if (m_Time > 0)
        {
            if (!m_bUse)
            {
                Player[0].m_bOnText = true;
            }
            else
            {
                Arrow[0].m_bOnArrow = true;
                Arrow[1].m_bOnArrow = true;
                Text.m_bOnText = true;
                Player[1].m_bOnText = true;
            }

            m_Time--;
        }
        else if (m_Time <= 0)
        {
            if (!m_bUse)
            {
                Player[0].m_bOnText = false;
                Player[0].m_bFade = true;
            }
            else
            {
                Arrow[0].m_bOnArrow = false;
                Arrow[1].m_bOnArrow = false;
                Text.m_bOnText = false;
                Player[1].m_bOnText = false;
                Player[1].m_bFade = true;
            }

            m_Time = m_OldTime;
            m_bUse = !m_bUse;
        }
	}
}
