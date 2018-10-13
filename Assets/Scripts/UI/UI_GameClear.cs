using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameClear : MonoBehaviour {

    [SerializeField]
    GameObject m_Background;
    [SerializeField]
    Vector2 m_BG_SizeMove;
    [SerializeField]
    float m_Clear_FontSizeMove;

    Vector3 m_BG_Size;
    Text m_Text;
    float m_FontSize;
    Color m_FontColor;
    int m_Mode;

    // Use this for initialization
    void Start () {
        // 背景の初期化
        m_BG_Size.x = 0.0f;
        m_BG_Size.y = 0.5f;
        m_BG_Size.z = 1.0f;
        m_Background.transform.localScale = m_BG_Size;

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
	void Update () {
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
                if (m_BG_Size.y >= 1.0f)
                {
                    m_Mode++;
                }
                break;
            case 2:
                m_FontSize = Mathf.Max(m_FontSize - m_Clear_FontSizeMove, 1.0f);
                m_FontColor.a = Mathf.Min(m_FontColor.a + 0.02f, 1.0f);
                if (m_FontSize >= 1.0f && m_FontColor.a >= 1.0f)
                {
                    m_Mode++;
                    GameObject obj = (GameObject)Resources.Load("Text_GameClear");
                    Instantiate(obj, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
                    //m_Text = obj.GetComponent<Text>();
                    //// フォントの初期化
                    //m_FontSize = 1.5f;
                    //m_Text.transform.localScale = new Vector3(m_FontSize, m_FontSize, 1.0f);
                    //m_FontColor = m_Text.color;
                    //m_FontColor.a = 0.0f;
                    //m_Text.color = m_FontColor;
                }
                break;
            case 3:
                m_FontSize = Mathf.Min(m_FontSize + m_Clear_FontSizeMove, 1.0f);
                m_FontColor.a = Mathf.Min(m_FontColor.a + 0.1f, 1.0f);
                //m_Text.transform.localScale = new Vector3(m_FontSize, m_FontSize, 1.0f);
                //m_Text.color = m_FontColor;
                if (m_FontSize >= 1.0f && m_FontColor.a >= 1.0f)
                {
                    m_Mode++;
                }
                break;
        }
	}
}
