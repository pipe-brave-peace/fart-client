using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Narration : MonoBehaviour {

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
            if (transform.localScale.x < 0.5f)
            {
                transform.localScale += new Vector3(0.25f, 0, 0);
            }

            if (transform.localScale.y < 0.5f)
            {
                transform.localScale += new Vector3(0, 0.1f, 0);
            }

            if (transform.localScale.y >= 0.5f && transform.localScale.x >= 0.5f)
            {
                m_bDisplay = true;
            }

            if (m_bDisplay)
            {
                if (m_nTime <= 0)
                {
                    m_bDisplay = false;
                    m_bTurn = true;
                    m_nTime = m_nOldTime;
                }
                else
                {
                    m_nTime--;
                }
            }
        }
        else
        {
            if (transform.localScale.x > 0)
            {
                transform.localScale -= new Vector3(0.1f, 0, 0);
            }

            if (transform.localScale.y > 0)
            {
                transform.localScale -= new Vector3(0, 0.05f, 0);
            }

            if (transform.localScale.x <= 0)
            {
                //transform.localScale = new Vector3(0, transform.localScale.y, transform.localScale.z);
                if (transform.localScale.y <= 0)
                {
                    Destroy(gameObject);
                }
            }

        }
    }

    public bool GetTurn() { return m_bTurn;  }
}
