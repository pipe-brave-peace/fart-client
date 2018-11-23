using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Operation1 : MonoBehaviour {

    [SerializeField]
    UI_Operation1_Smoke Smoke;

    [SerializeField]
    UI_Operation1_Enemy Enemy;

    [SerializeField]
    UI_Operation_Text Text;

    [SerializeField]
    UI_Operation1_Arrow Arrow;

    [SerializeField]
    bool m_bUse;

    [SerializeField]
    bool m_bTurn = false;

    [SerializeField]
    bool m_bDisplay = false;

    [SerializeField]
    int m_nTime;

    int m_nOldTime;

    // Use this for initialization
    void Start () {
        m_nOldTime = m_nTime;
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

            if (Enemy.m_bEnemyStop)
            {
                Text.m_bOnText = true;
                Arrow.m_bOnArrow = true;
                Smoke.IventOnMove();

                if (Smoke.m_bPos)
                {
                    Enemy.m_bSmokeOn = true;
                }
            }
            else
            {
                Text.m_bOnText = false;
                Arrow.m_bOnArrow = false;
                Smoke.IventOffMove();
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
