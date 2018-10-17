using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Flash : MonoBehaviour {

    [SerializeField]
    float m_Speed;

    float m_Timer;
    Image m_Image;

	// Use this for initialization
	void Start () {
        m_Image = GetComponent<Image>();
    }
	
	// Update is called once per frame
	void Update () {

        // フレームカウント
        m_Timer += Time.deltaTime * m_Speed;
        if (m_Timer >= Mathf.PI)
        {
            m_Timer = 0.0f;
        }

        // 色の代入
        float alpha = Mathf.Sin(m_Timer);
        Color color = m_Image.color;
        color.a = alpha;
        m_Image.color = color;
    }
}
