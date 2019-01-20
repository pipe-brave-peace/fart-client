using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour {

    [SerializeField]
    GameObject m_TitleUI;

    [SerializeField]
    GameObject m_Display2TitleUI;

    [SerializeField]
    GameObject TitleCamera;

    [SerializeField]
    LED m_LED;

    bool m_bTitleLED;

    bool m_bToGameLED;

    private List<Joycon> m_joycons;

    private Joycon m_joyconR1;
    private Joycon m_joyconR2;

    public AudioSource m_Bgm;           // BGM

    bool m_bUse = false;

    // Use this for initialization
    void Start () {

       // if (JoyconManager.Instance != null)
        {
            m_joycons = JoyconManager.Instance.j;

            int count = 0;

            for (int i = 0; i < m_joycons.Count; i++)
            {
                if (!m_joycons[i].isLeft)
                {
                    if (count == 0)
                    {
                        m_joyconR1 = m_joycons[i];
                    }
                    else
                    {
                        m_joyconR2 = m_joycons[i];
                    }
                    count++;
                }
            }
        }
        m_Bgm.Play ();
    }
	
	// Update is called once per frame
	void Update () {
		if(!FadeManager.Instance.IsFadeInEnd())
		{
			return;
		}

        if (!m_bUse)
        {
            if (!m_bTitleLED)
            {
                m_LED.Title();
                m_bTitleLED = true;
            }

            if (m_joyconR1 != null)
            {
                if (m_joyconR1.GetButtonDown(Joycon.Button.SHOULDER_1))
                {
                    SoundManager.Instance.PlaySE(SoundManager.SE_TYPE.PUSH_BUTTON);
                    m_bUse = true;
                }
            }

            if (m_joyconR2 != null)
            {
                if (m_joyconR2.GetButtonDown(Joycon.Button.SHOULDER_1))
                {
                    SoundManager.Instance.PlaySE(SoundManager.SE_TYPE.PUSH_BUTTON);
                    m_bUse = true;
                }
            }

            // キー押し判定
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                SoundManager.Instance.PlaySE(SoundManager.SE_TYPE.PUSH_BUTTON);
                m_bUse = true;
            }
        }

        if (m_bUse)
        {
            if (!m_bToGameLED)
            {
                m_LED.ToGame();
                m_bToGameLED = true;
            }

            if (m_TitleUI.GetComponent<CanvasGroup>().alpha > 0)
            {
                m_Display2TitleUI.GetComponent<CanvasGroup>().alpha -= 0.05f;
                m_TitleUI.GetComponent<CanvasGroup>().alpha -= 0.05f;
            }
            else if (m_TitleUI.GetComponent<CanvasGroup>().alpha <= 0)
            {
                AllManager.Instance.SetStateScene(AllManager.STATE_SCENE.STATE_STAGE);
                m_Bgm.Stop();
                InfoManager.Instance.InitInfo();
                gameObject.SetActive(false);
                TitleCamera.SetActive(false);
            }
        }
    }
}
