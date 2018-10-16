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

	public void Awake()
	{
		if(this != Instance)
		{
			Destroy(this);
			return;
		}
		Application.targetFrameRate = 60;
		m_IsChangeing = false;
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
}
