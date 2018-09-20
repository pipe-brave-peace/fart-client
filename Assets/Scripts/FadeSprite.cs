using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeSprite : SingletonMonoBehaviour<FadeSprite> {

	public float FADE_END_TIME = 1.5f;

	// ステート
	public enum STATE
	{
		NONE,
		FADE_IN,
		FADE_IN_END,
		FADE_OUT,
		FADE_OUT_END,
		MAX
	}
		
	private Image m_Image;
	private Color m_Color;
	private STATE m_State;
	private float m_FadeTimer;

	public void Awake()
	{
		if(this != Instance)
		{
			Destroy(this);
			return;
		}

		m_Image = GetComponentInChildren<Image> ();
		m_Color = m_Image.color;
		StartFadeIn ();
		m_FadeTimer = 0.0f;
		DontDestroyOnLoad(this.gameObject);
	}

	// Update is called once per frame
	void Update ()
    {
        switch (m_State) {
		case STATE.FADE_IN:
			FadeInUpdate ();
			break;
		case STATE.FADE_OUT:
			FadeOutUpdate ();
			break;
		default:
			break;
		}
	}
		
	/// <summary>
	/// フェードイン
	/// 明るくなっていく
	/// </summary>
	private void FadeInUpdate()
    {
        m_FadeTimer += Time.deltaTime / FADE_END_TIME;
		if(m_FadeTimer >= 1.0f)
		{
			m_FadeTimer = 1.0f;
			m_State = STATE.FADE_IN_END;
		}
		m_Color.a = Mathf.Lerp (1.0f, 0.0f, m_FadeTimer);
		m_Image.color = m_Color;
	}
		
	/// <summary>
	/// フェードアウト
	/// 暗くなっていく
	/// </summary>
	private void FadeOutUpdate()
    {
        m_FadeTimer += Time.deltaTime / FADE_END_TIME;
		if(m_FadeTimer >= 1.0f)
		{
			m_FadeTimer = 1.0f;
			m_State = STATE.FADE_OUT_END;
		}
		m_Color.a = Mathf.Lerp (0.0f, 1.0f, m_FadeTimer);
		m_Image.color = m_Color;
	}

	/// <summary>
	/// FadeIn開始
	/// </summary>
	public void StartFadeIn()
    {
        if (m_State == STATE.FADE_IN) {
			return;
		}
		m_State = STATE.FADE_IN;
		m_Color.a = 1.0f;
		m_FadeTimer = 0.0f;
	}

	/// <summary>
	/// FadeOut開始
	/// </summary>
	public void StartFadeOut()
    {
        if (m_State == STATE.FADE_OUT) {
			return;
		}
		m_State = STATE.FADE_OUT;
		m_Color.a = 0.0f;
		m_FadeTimer = 0.0f;
	}

	/// <summary>
	/// STATE取得
	/// </summary>
	/// <returns>The state.</returns>
	public STATE GetState()
    {
        return m_State;
	}
}
