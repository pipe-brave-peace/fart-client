using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class StageManager : MonoBehaviour {
    
    public AudioSource m_Bgm;           // BGM

    [SerializeField]
    GameObject m_Stage_Object;

    [SerializeField]
    GameObject m_Stage_UI;

    [SerializeField]
    GameObject m_Display2Stage_UI;

    [SerializeField]
    GameObject m_ResultCamera;

    [SerializeField]
    CinemachineBrain m_MainChine;

    [SerializeField]
    GameObject m_NarrationObject;

    [SerializeField]
    UI_IventAll m_IventUI;

    // ゲーム終了表示
    [SerializeField]
    GameObject m_GameClear;
    [SerializeField]
    GameObject m_GameOver;
    [SerializeField]
    PhaseManager m_PhaseManager;

    [SerializeField]
    Enemy_State m_Boss;

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
        AllManager.Instance.SetStateScene(AllManager.STATE_SCENE.STATE_STAGE);

        //m_Stage_Object.SetActive(true);
        m_Stage_UI.SetActive(true);

        Mode = STAGE_MODE.READY;
        m_Bgm.Play ();

        m_GameClear.SetActive(false);
        m_GameOver.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        // テスト
        if (m_Boss.GetState() == Enemy_State.STATE.FAINT)
        {
            m_ResultCamera.SetActive(true);
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

        if (m_MainChine.IsBlending)
        {
            if (m_MainChine.ActiveBlend.BlendWeight >= 0.9f)
            {
                m_NarrationObject.SetActive(true);
            }
        }

        if (m_NarrationObject.GetComponent<UI_Narration>().GetTurn())
        {
            m_IventUI.SetIventFlg(false);
        }

        if (m_Stage_UI.GetComponent<CanvasGroup>().alpha < 1)
        {
            m_Display2Stage_UI.GetComponent<CanvasGroup>().alpha += 0.05f;

            m_Stage_UI.GetComponent<CanvasGroup>().alpha += 0.05f;
        }
        else if (m_Stage_UI.GetComponent<CanvasGroup>().alpha >= 1)
        {
            m_Display2Stage_UI.GetComponent<CanvasGroup>().alpha = 1f;
            m_Stage_UI.GetComponent<CanvasGroup>().alpha = 1;
        }

        if (m_NarrationObject.GetComponent<UI_Narration>().GetTurn())
        {
            // 次のモードに移行
            Mode = STAGE_MODE.GAME;
        }

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
        SoundManager.Instance.PlaySE(SoundManager.SE_TYPE.PUSH_BUTTON);
        m_Bgm.Stop();

        if (m_Stage_UI.GetComponent<CanvasGroup>().alpha > 0)
        {
            m_Stage_UI.GetComponent<CanvasGroup>().alpha -= 0.05f;
        }
        else if (m_Stage_UI.GetComponent<CanvasGroup>().alpha <= 0)
        {
            // シーン遷移処理
            AllManager.Instance.SetStateScene(AllManager.STATE_SCENE.STATE_RESULT);

            m_Stage_Object.SetActive(false);
            m_Stage_UI.SetActive(false);

            // 次のモードに移行
            Mode = STAGE_MODE.MAX;
        }
    }
}
