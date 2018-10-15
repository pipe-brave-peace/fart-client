using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameOver : MonoBehaviour {

    [SerializeField]
    GameObject m_Background;
    [SerializeField]
    Vector2 m_BG_SizeMove;
    [SerializeField]
    Text m_Text;

    Vector3 m_BG_Size;
    float m_FontSize;
    Color m_FontColor;
    int m_Mode;

    // Use this for initialization
    void Start()
    {
        // 背景の初期化
        m_BG_Size.x = 0.0f;
        m_BG_Size.y = 0.5f;
        m_BG_Size.z = 1.0f;
        m_Background.transform.localScale = m_BG_Size;

        // フォントの初期化
        m_FontColor = m_Text.color;
        m_FontColor.a = 0.0f;
        m_Text.color = m_FontColor;
        m_Text.GetComponent<TypefaceAnimator>().enabled = false;

        // アニメション順番の初期化
        m_Mode = 0;

        // 他のUIを消す
        GameObject[] game_UI = GameObject.FindGameObjectsWithTag("GameUI");
        foreach (GameObject UI in game_UI)
        {
            Destroy(UI);
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_Mode)
        {
            case 0:
                m_BG_Size.x = Mathf.Min(m_BG_Size.x + m_BG_SizeMove.x, 1.0f);
                m_Background.transform.localScale = m_BG_Size;
                if (m_BG_Size.x >= 0.5f)
                {
                    m_Mode++;
                }
                break;
            case 1:
                m_BG_Size.x = Mathf.Min(m_BG_Size.x + m_BG_SizeMove.x, 1.0f);
                m_BG_Size.y = Mathf.Min(m_BG_Size.y + m_BG_SizeMove.y, 1.0f);
                m_Background.transform.localScale = m_BG_Size;

                m_FontColor.a = Mathf.Min(m_FontColor.a + 0.01f, 1.0f);
                m_Text.color = m_FontColor;

                if (m_BG_Size.y >= 1.0f)
                {
                    m_Mode++;
                }
                break;
            case 2:
                m_FontColor.a = Mathf.Min(m_FontColor.a + 0.01f, 1.0f);
                m_Text.color = m_FontColor;
                if (m_FontColor.a >= 1.0f)
                {
                    m_Mode++;
                    m_Text.GetComponent<TypefaceAnimator>().enabled = true;
                }
                break;
        }
    }
}
