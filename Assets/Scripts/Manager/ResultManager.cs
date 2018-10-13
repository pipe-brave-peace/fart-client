using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour {

	//public Text m_Text;                 // 点滅テキスト
	//public float m_FlashSpd = 1.0f;     // 点滅速さ
	public AudioSource m_Bgm;           // BGM

	//private float m_FlashTimer;         // 点滅カウンター

	// 初期化
	void Start () {
        // 点滅カウンター
		//m_FlashTimer = 0.0f;
        // 色の初期化
		//float alpha = Mathf.Sin (m_FlashTimer);
		//Color color = new Color (m_Text.color.r, m_Text.color.g, m_Text.color.b, alpha);    // アルファ値代入
		//m_Text.color = color;
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

        // テキスト点滅処理
		//FlashText ();

	}

    //// テキストの点滅
    //void FlashText()
	//{
    //    // フレームカウント
	//	m_FlashTimer += Time.deltaTime * m_FlashSpd;
	//	if (m_FlashTimer >= Mathf.PI) {
	//		m_FlashTimer = 0.0f;
	//	}
    //
    //    // 色の代入
	//	float alpha = Mathf.Sin (m_FlashTimer);
	//	Color color = new Color (m_Text.color.r, m_Text.color.g, m_Text.color.b, alpha);
	//	m_Text.color = color;
	//}
}
