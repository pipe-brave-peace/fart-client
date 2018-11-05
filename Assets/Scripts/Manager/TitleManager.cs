using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour {
    
    public AudioSource m_Bgm;           // BGM

    // Use this for initialization
    void Start () {
		m_Bgm.Play ();
    }
	
	// Update is called once per frame
	void Update () {
		if(!FadeManager.Instance.IsFadeInEnd())
		{
			return;
		}

	    // キー押し判定
		if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
		{
			ModeManager.Instance.SetChangeScene (ModeManager.SCENE_TYPE.GAME);
            SoundManager.Instance.PlaySE(SoundManager.SE_TYPE.PUSH_BUTTON);
			m_Bgm.Stop();
            InfoManager.Instance.InitInfo();
		}
	}
}
