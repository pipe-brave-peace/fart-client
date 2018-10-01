using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : MonoBehaviour {

    public int m_FarmHP = 100;
    public UI_Farm m_Farm;

    // Use this for initialization
    void Start () {
        m_Farm.SetValue(m_FarmHP);
    }
	
	// Update is called once per frame
	void Update () {

        // キー押し判定
        if (Input.GetKeyDown(KeyCode.Q))
        {
            m_FarmHP = Mathf.Max(0, m_FarmHP - 10);
            m_Farm.SetValue(m_FarmHP);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            m_FarmHP = Mathf.Max(0, m_FarmHP - 1);
            m_Farm.SetValue(m_FarmHP);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            m_FarmHP = 100;
            m_Farm.SetValue(m_FarmHP);
        }
    }
}
