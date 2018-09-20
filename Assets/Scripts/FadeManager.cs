using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : SingletonMonoBehaviour<FadeManager> {

	public FadeSprite m_FadeSprite;

	public void Awake()
	{
		if(this != Instance)
		{
			Destroy(this);
			return;
		}

		m_FadeSprite = Instantiate (m_FadeSprite);
		DontDestroyOnLoad(this.gameObject);
	}    

	// Use this for initialization
	void Start () {
		m_FadeSprite.StartFadeIn ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/// <summary>
	/// FadeIn開始
	/// </summary>
	public void FadeIn()
	{
		m_FadeSprite.StartFadeIn ();
	}

	/// <summary>
	/// FadeOut開始
	/// </summary>
	public void FadeOut()
	{
		m_FadeSprite.StartFadeOut ();
	}

	/// <summary>
	/// FadeIn終わった？
	/// </summary>
	/// <returns><c>true</c> if this instance is fade in end; otherwise, <c>false</c>.</returns>
	public bool IsFadeInEnd()
	{
		bool ret = m_FadeSprite.GetState () == FadeSprite.STATE.FADE_IN_END;
        return ret;
	}

	/// <summary>
	/// FadeIn終わった？
	/// </summary>
	/// <returns><c>true</c> if this instance is fade in end; otherwise, <c>false</c>.</returns>
	public bool IsFadeOutEnd()
	{
		bool ret = m_FadeSprite.GetState () == FadeSprite.STATE.FADE_OUT_END;
		return ret;
	}
}
