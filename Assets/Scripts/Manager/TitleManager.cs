using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour {

    public Text m_Text;                 // 点滅テキスト
    public float m_FlashSpd = 1.0f;     // 点滅速さ
    public AudioSource m_Bgm;           // BGM

    private float m_FlashTimer;         // 点滅カウンター

    // Use this for initialization
    void Start () {
		m_FlashTimer = 0.0f;
		float alpha = Mathf.Sin (m_FlashTimer);
		Color color = new Color (m_Text.color.r, m_Text.color.g, m_Text.color.b, alpha);
		m_Text.color = color;
		m_Bgm.Play ();

        // テスト 
        InfoManager.Instance.SetPlayerScore(0, 11);
        InfoManager.Instance.SetPlayerScore(1, 12);

        InfoManager.Instance.SetPlayerEnemy(0, 21);
        InfoManager.Instance.SetPlayerEnemy(1, 22);

        InfoManager.Instance.SetPlayerCombo(0, 123);
        InfoManager.Instance.SetPlayerCombo(1, 456);
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
		}

        // テキスト点滅処理
		FlashText ();

	}

    // テキストの点滅
    void FlashText()
    {
        // フレームカウント
        m_FlashTimer += Time.deltaTime * m_FlashSpd;
        if (m_FlashTimer >= Mathf.PI)
        {
            m_FlashTimer = 0.0f;
        }

        // 色の代入
        float alpha = Mathf.Sin(m_FlashTimer);
        Color color = new Color(m_Text.color.r, m_Text.color.g, m_Text.color.b, alpha);
        m_Text.color = color;
    }
}
