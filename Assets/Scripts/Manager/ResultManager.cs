using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour {
    
	public AudioSource m_Bgm;           // BGM

    [SerializeField]
    GameObject m_ResultUI;

    private List<Joycon> m_joycons;

    private Joycon m_joyconR1;
    private Joycon m_joyconR2;

    bool m_bUse;

    // 初期化
    void Start () {

        m_joycons = NintendoManager.Instance.j;

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

        // BGMプレイー
        m_Bgm.Play ();
        m_ResultUI.SetActive(true);

    }

	// Update is called once per frame
	void Update () {
		if(!FadeManager.Instance.IsFadeInEnd())
		{
			return;
		}

        if (m_ResultUI.GetComponent<CanvasGroup>().alpha < 1)
        {
            m_ResultUI.GetComponent<CanvasGroup>().alpha += 0.05f;
        }
        else if (m_ResultUI.GetComponent<CanvasGroup>().alpha >= 1)
        {
            m_ResultUI.GetComponent<CanvasGroup>().alpha = 1;
        }


        if (!m_bUse)
        {
            if (m_joyconR1 != null)
            {
                if (m_joyconR1.GetButtonDown(Joycon.Button.SHOULDER_1))
                {
                    m_ResultUI.SetActive(false);
                    m_Bgm.Stop();                                                           // BGMストップ
                    SoundManager.Instance.PlaySE(SoundManager.SE_TYPE.PUSH_BUTTON);        // 効果音プレイー
                    ModeManager.Instance.SetChangeScene(ModeManager.SCENE_TYPE.GAME);
                    m_bUse = true;
                }
            }

            if (m_joyconR2 != null)
            {
                if (m_joyconR2.GetButtonDown(Joycon.Button.SHOULDER_1))
                {
                    m_ResultUI.SetActive(false);
                    m_Bgm.Stop();                                                           // BGMストップ
                    SoundManager.Instance.PlaySE(SoundManager.SE_TYPE.PUSH_BUTTON);        // 効果音プレイー
                    ModeManager.Instance.SetChangeScene(ModeManager.SCENE_TYPE.GAME);
                    m_bUse = true;
                }
            }

            // キー押したら
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                m_ResultUI.SetActive(false);
                m_Bgm.Stop();                                                           // BGMストップ
                SoundManager.Instance.PlaySE(SoundManager.SE_TYPE.PUSH_BUTTON);        // 効果音プレイー
                ModeManager.Instance.SetChangeScene(ModeManager.SCENE_TYPE.GAME);
                m_bUse = true;
            }
        }
	}
}
