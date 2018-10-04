using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeManager : SingletonMonoBehaviour<ModeManager>{

	public enum SCENE_TYPE
	{
		TITLE = 0,
		GAME,
		RESULT,
		MAX
	}

	/// <summary>
	/// シーンの名前
	/// MAXは使わないが来たらtitleへ
	/// </summary>
	readonly private string[] m_SceneName = {"title", "stage", "result", "title"};

	private SCENE_TYPE m_NextScene;
	private bool m_IsChangeing;
    private int m_Player1Score;
    private int m_Player2Score;

	public void Awake()
	{
		if(this != Instance)
		{
			Destroy(this);
			return;
		}
		Application.targetFrameRate = 60;
		m_IsChangeing = false;
        m_Player1Score = 0;
        m_Player2Score = 0;
        DontDestroyOnLoad(this.gameObject);
	} 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
		// フェード終わってたらシーン遷移
		if (m_IsChangeing && FadeManager.Instance.IsFadeOutEnd ()) {
			MoveScene ();
		}
	}

	/// <summary>
	/// シーンチェンジして
	/// </summary>
	public void SetChangeScene(SCENE_TYPE scene)
	{
		if (m_IsChangeing) {
			return;
		}
		m_NextScene = scene;
		m_IsChangeing = true;
		FadeManager.Instance.FadeOut ();
	}

	/// <summary>
	/// シーンチェンジ
	/// </summary>
	private void MoveScene()
	{
		m_IsChangeing = false;
        SceneManager.LoadScene(m_SceneName[(int)m_NextScene]);
        FadeManager.Instance.FadeIn();
	}

    // プレイヤー１のスコアセット・ゲット
    public void SetPlayer1Score(int score)
    {
        m_Player1Score = score;
    }
    public int GetPlayer1Score()
    {
        return m_Player1Score;
    }
    // プレイヤー２のスコアセット・ゲット
    public void SetPlayer2Score(int score)
    {
        m_Player2Score = score;
    }
    public int GetPlayer2Score()
    {
        return m_Player2Score;
    }
}
