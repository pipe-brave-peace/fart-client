using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour {
    
	public AudioSource m_Bgm;           // BGM

	// 初期化
	void Start () {
        // BGMプレイー
		m_Bgm.Play ();
	}

	// Update is called once per frame
	void Update () {
		if(!FadeManager.Instance.IsFadeInEnd())
		{
			return;
		}

        // キー押したら
		if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
		{
			m_Bgm.Stop();                                                           // BGMストップ
			SoundManager.Instance.PlaySE (SoundManager.SE_TYPE.PUSH_BUTTON);        // 効果音プレイー
			ModeManager.Instance.SetChangeScene (ModeManager.SCENE_TYPE.TITLE);     // シーンの切り替え
		}
	}
}
