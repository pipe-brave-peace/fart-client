using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubImgAlpha : MonoBehaviour {

    [SerializeField]
    Image[] m_ChildImage;
    [SerializeField]
    float m_SubVal;
    [SerializeField]
    float m_StartTimer;

    private Color m_Color;

    // Use this for initialization
    void Start () {
        m_Color = GetComponent<Image>().color;
	}
	
	// Update is called once per frame
	void Update () {
        if (m_StartTimer > 0.0f)
        {
            m_StartTimer -= Time.deltaTime;
            return;
        }

        m_Color.a -= m_SubVal;
        if( m_Color.a <= 0.0f)
        {
            Destroy(gameObject);
            return;
        }

        GetComponent<Image>().color = m_Color;
        for (int i = 0; i < m_ChildImage.Length; ++i)
        {
            m_ChildImage[i].color = m_Color;
        }
    }
}
