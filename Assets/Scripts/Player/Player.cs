using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public enum STATE
    {
        NORMAL = 0, // 通常
        DAMAGE,     // ダメージ
        MAX
    }

    [SerializeField]
    STATE m_State;

    [SerializeField]
    int m_nTime = 0;

    [SerializeField]
    Text ScoreUI;

    [SerializeField]
    int m_nPlayerNumber = 0;

    int m_nOldTime = 0;

    // Use this for initialization
    void Start () {

        m_nOldTime = m_nTime;

    }
	
	// Update is called once per frame
	void Update () {
        ScoreUI.text = InfoManager.Instance.GetPlayerInfo(m_nPlayerNumber).GetScore().ToString();

        if (m_State == STATE.DAMAGE)
        {
            m_nTime--;

            if (m_nTime <= 0)
            {
                m_State = STATE.NORMAL;
                m_nTime = m_nOldTime;
            }
        }
	}

    public STATE GetState() { return m_State; }

    public int GetPlayerNumber() { return m_nPlayerNumber; }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            if (m_State != STATE.DAMAGE)
            {
                m_State = STATE.DAMAGE;
            }
        }
    }
}
