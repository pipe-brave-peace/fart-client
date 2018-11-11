using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {


    [SerializeField]
    bool m_bStan;

    [SerializeField]
    Transform m_PlayerPoint;

    [SerializeField]
    GameObject m_PlayerStanUI;

    [SerializeField]
    int m_nTime = 0;

    [SerializeField]
    Text ScoreUI;

    [SerializeField]
    int m_nPlayerNumber = 0;

    [SerializeField]
    GameObject m_BazookaObject;

    int m_nOldTime = 0;

    bool m_bUse;

    // Use this for initialization
    void Start () {

        m_bUse = true;

        m_nOldTime = m_nTime;

    }
	
	// Update is called once per frame
	void Update () {
        ScoreUI.text = InfoManager.Instance.GetPlayerInfo(m_nPlayerNumber).GetScore().ToString();

        if (m_bStan)
        {
            m_PlayerStanUI.SetActive(true);

            m_bUse = !m_bUse;

            m_nTime--;

            if (m_nTime <= 0)
            {
                m_bStan = false;
                m_nTime = m_nOldTime;
            }
        }
        else
        {
            m_bUse = true;
            m_nTime = m_nOldTime;
            m_PlayerStanUI.SetActive(false);
        }

        m_BazookaObject.SetActive(m_bUse);

    }

    public bool GetStan() { return m_bStan; }

    public int GetPlayerNumber() { return m_nPlayerNumber; }

    public Vector3 GetPlayerPosition() { return m_PlayerPoint.position; }
}
