using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour {
    
    public AudioSource m_Bgm;           // BGM

    private List<Joycon> m_joycons;
    private Joycon m_joyconR;

    // Use this for initialization
    void Start () {
		m_Bgm.Play ();

        m_joycons = JoyconManager.Instance.j;

        m_joyconR = m_joycons.Find(c => !c.isLeft);
    }
	
	// Update is called once per frame
	void Update () {
		if(!FadeManager.Instance.IsFadeInEnd())
		{
			return;
		}

        if (m_joyconR != null)
        {
            if (m_joyconR.GetButtonDown(Joycon.Button.SHOULDER_1))
            {
                ModeManager.Instance.SetChangeScene(ModeManager.SCENE_TYPE.GAME);
                SoundManager.Instance.PlaySE(SoundManager.SE_TYPE.PUSH_BUTTON);
                m_Bgm.Stop();
                InfoManager.Instance.InitInfo();
            }
        }

        // キー押し判定
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
		{
			ModeManager.Instance.SetChangeScene (ModeManager.SCENE_TYPE.GAME);
            SoundManager.Instance.PlaySE(SoundManager.SE_TYPE.PUSH_BUTTON);
			m_Bgm.Stop();
            InfoManager.Instance.InitInfo();
		}
	}
}
