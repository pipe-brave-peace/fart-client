using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour {

    public Text m_Text;                 // 点滅テキスト
    public float m_FlashSpd = 1.0f;     // 点滅速さ
    public AudioSource m_Bgm;           // BGM

    // ステージモード定義
    enum STAGE_MODE
    {
        READY = 0,  // 準備段階
        GAME,       // ゲームプレイ
        TO_RESULT,  // ゲーム終了
        MAX
    }
    float m_FlashTimer;         // 点滅カウンター
    STAGE_MODE Mode;            // 現在のモード

    // ゲームモードの取得
    public bool isGameMode()
    {
        return (Mode == STAGE_MODE.GAME) ? true : false;
    }

	// 初期化
	void Start () {
        Mode = STAGE_MODE.READY;
        m_FlashTimer = 0.0f;
        float alpha = Mathf.Sin(m_FlashTimer);
        Color color = new Color(m_Text.color.r, m_Text.color.g, m_Text.color.b, alpha);
        m_Text.color = color;
        m_Bgm.Play ();
	}
	
	// Update is called once per frame
	void Update () {
        // 状態別処理
        switch(Mode)
        {
            case STAGE_MODE.READY:
                ModeStart();
                break;
            case STAGE_MODE.GAME:
                ModeGame();
                break;
            case STAGE_MODE.TO_RESULT:
                ModeToResult();
                break;
            default:
                break;
        }
	}

    // スタート時処理
    void ModeStart()
    {
        // フェード時実行しない
        if( !FadeManager.Instance.IsFadeInEnd())
        {
            return;
        }

        // 次のモードに移行
        Mode = STAGE_MODE.GAME;
    }

    // ゲームメイン処理
    void ModeGame()
    {
        // キー押し判定
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            // 次のモードに移行
            Mode = STAGE_MODE.TO_RESULT;
        }
        // 点滅処理
        FlashText();
    }

    // リザルトへの処理
    void ModeToResult()
    {
        // シーン遷移処理
        ModeManager.Instance.SetChangeScene(ModeManager.SCENE_TYPE.RESULT);
        SoundManager.Instance.PlaySE(SoundManager.SE_TYPE.PUSH_BUTTON);
        m_Bgm.Stop();

        // 次のモードに移行
        Mode = STAGE_MODE.MAX;
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
