using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour {
    
	public AudioSource m_Bgm;           // BGM

    [SerializeField]
    GameObject m_ResultUI;

	// 初期化
	void Start () {
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


        // キー押したら
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
		{
            m_ResultUI.SetActive(false);
            m_Bgm.Stop();                                                           // BGMストップ
			//SoundManager.Instance.PlaySE (SoundManager.SE_TYPE.PUSH_BUTTON);        // 効果音プレイー
            ModeManager.Instance.SetChangeScene(ModeManager.SCENE_TYPE.GAME);
            AllManager.Instance.SetStateScene(AllManager.STATE_SCENE.STATE_TITLE);     // シーンの切り替え
        }
	}
}
