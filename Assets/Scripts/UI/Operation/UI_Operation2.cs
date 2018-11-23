using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Operation2 : MonoBehaviour {

    [SerializeField]
    UI_Operation1_Smoke Smoke;

    [SerializeField]
    UI_Operation2_Enemy Enemy;

    [SerializeField]
    UI_Operation_Text[] Text;

    [SerializeField]
    UI_Operation1_Arrow[] Arrow;

    [SerializeField]
    UI_Operation2_Buster Buster;

    [SerializeField]
    int m_Time = 0;

    [SerializeField]
    bool m_bTurn = false;

    [SerializeField]
    bool m_bDisplay = false;

    int m_OldTime = 0;

    // Use this for initialization
    void Start () {
        m_OldTime = m_Time;

    }
	
	// Update is called once per frame
	void Update () {

        if (!m_bTurn)
        {
            if (transform.localScale.x < 0.2f)
            {
                transform.localScale += new Vector3(0.05f, 0, 0);
            }

            if (transform.localScale.y < 0.2f)
            {
                transform.localScale += new Vector3(0, 0.05f, 0);
            }

            if (transform.localScale.y >= 0.2f && transform.localScale.x >= 0.2f)
            {
                m_bDisplay = true;
            }

            if (!Enemy.m_bBusterOn)
            {
                if (!Enemy.m_bSmokeOn)
                {
                    Buster.IventOffMove();
                    Text[1].m_bOnText = false;
                    Arrow[1].m_bOnArrow = false;
                    Text[0].m_bOnText = true;
                    Arrow[0].m_bOnArrow = true;
                    Smoke.IventOnMove();
                    if (Smoke.m_bPos)
                    {
                        m_Time = m_OldTime;
                        Enemy.m_bSmokeOn = true;
                        Smoke.IventOffMove();
                    }
                }
                else
                {
                    if (m_Time <= 0)
                    {
                        Text[0].m_bOnText = false;
                        Arrow[0].m_bOnArrow = false;
                        Text[1].m_bOnText = true;
                        Arrow[1].m_bOnArrow = true;
                        Buster.IventOnMove();
                        if (Buster.m_bPos)
                        {
                            Enemy.m_bBusterOn = true;
                        }
                    }
                    m_Time--;
                }
            }
        }
        else
        {
            if (transform.localScale.x > 0)
            {
                transform.localScale -= new Vector3(0.05f, 0, 0);
            }

            if (transform.localScale.y > 0)
            {
                transform.localScale -= new Vector3(0, 0.05f, 0);
            }

            if (transform.localScale.x <= 0)
            {
                if (transform.localScale.y <= 0)
                {
                    Destroy(gameObject);
                }
            }

        }
    }

    public void SetTurn(bool bTurn) { m_bTurn = bTurn; } 
}
