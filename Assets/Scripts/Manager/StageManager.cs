using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour {
    
    public AudioSource m_Bgm;           // BGM
    // 敵のスコア
    [SerializeField] int Score_Enemy_Sika;
    [SerializeField] int Score_Enemy_Inosisi;
    [SerializeField] int Score_Enemy_Karasu;
    [SerializeField] int Score_Enemy_Inago;
    [SerializeField] int Score_Enemy_Kamemusi;
    [SerializeField] int Score_Enemy_Kuma;

    // ゲーム終了表示
    [SerializeField]
    GameObject m_GameClear;
    [SerializeField]
    GameObject m_GameOver;
    
    // ステージモード定義
    enum STAGE_MODE
    {
        READY = 0,  // 準備段階
        GAME,       // ゲームプレイ
        TO_RESULT,  // ゲーム終了
        MAX
    }

    STAGE_MODE Mode;            // 現在のモード

    // ゲームモードの取得
    public bool isGameMode()
    {
        return (Mode == STAGE_MODE.GAME) ? true : false;
    }

	// 初期化
	void Start () {
        Mode = STAGE_MODE.READY;
        m_Bgm.Play ();

        m_GameClear.SetActive(false);
        m_GameOver.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        // テスト
        if( Input.GetKeyDown(KeyCode.C))
        {
            m_GameClear.SetActive(true);
            m_GameOver.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            m_GameOver.SetActive(true);
            m_GameClear.SetActive(false);
        }
        // 状態別処理
        switch (Mode)
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
    }

    // リザルトへの処理
    public void ModeToResult()
    {
        // シーン遷移処理
        ModeManager.Instance.SetChangeScene(ModeManager.SCENE_TYPE.RESULT);
        SoundManager.Instance.PlaySE(SoundManager.SE_TYPE.PUSH_BUTTON);
        m_Bgm.Stop();

        // 次のモードに移行
        Mode = STAGE_MODE.MAX;
    }

    // スコアの取得
    public int GetScoreEnemySika()     { return Score_Enemy_Sika;     }
    public int GetScoreEnemyInosisi()  { return Score_Enemy_Inosisi;  }
    public int GetScoreEnemyKarasu()   { return Score_Enemy_Karasu;   }
    public int GetScoreEnemyInago()    { return Score_Enemy_Inago;    }
    public int GetScoreEnemyKamemusi() { return Score_Enemy_Kamemusi; }
    public int GetScoreEnemyKuma()     { return Score_Enemy_Kuma;     }
}
