﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_NumEffectScl : MonoBehaviour {

    [SerializeField]
    Vector3 m_MovePos;
    [SerializeField]
    float m_MoveScl;
    [SerializeField]
    float m_MoveAlpha;

    private Text m_Text;
    private Color m_Color;
    private float m_Scl;
    private Vector3 m_Pos;

    // Use this for initialization
    void Start () {
        m_Text = GetComponent<Text>();
        m_Color = m_Text.color;
        m_Scl = m_Text.transform.localScale.x;
        m_Pos = m_Text.transform.localPosition;

    }
	
	// Update is called once per frame
	void Update () {
        m_Pos += m_MovePos;
        m_Scl += m_MoveScl;
        m_Color.a -= m_MoveAlpha;

        if ( m_Color.a <= 0.0)
        {
            Destroy(gameObject);
        }

        m_Text.color = m_Color;
        m_Text.transform.localScale = new Vector3(m_Scl, m_Scl, 1.0f);
        m_Text.transform.localPosition = m_Pos;
	}
}
