using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingletonMonoBehaviour<SoundManager> {

    // 効果音名の定義（効果音追加したらここにも追加）
    public enum SE_TYPE
	{
		PUSH_BUTTON,
		CLEAR,
		MAX
	}
	readonly private static int MAX_SE_AUDIOSOURCE_NUM = 20;
    [Header("効果音追加するとき、効果音の命名が必要")]
    [Header("（SoundManagerに入ってSE_TYPEに項目を追加）")]
    public AudioClip[] m_SE;
	private AudioSource[] m_SEAudioSource = new AudioSource[MAX_SE_AUDIOSOURCE_NUM];

	public void Awake()
	{
		if(this != Instance)
		{
			Destroy(this);
			return;
		}
		for (int i = 0; i < MAX_SE_AUDIOSOURCE_NUM; i++) {
			m_SEAudioSource[i] = this.gameObject.AddComponent<AudioSource> ();
		}
		DontDestroyOnLoad(this.gameObject);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.K)) {
			PlaySE(SE_TYPE.PUSH_BUTTON);
		}
	}

	/// <summary>
	/// SE再生
	/// </summary>
	public void PlaySE(SE_TYPE type)
	{
		for (int i = 0; i < MAX_SE_AUDIOSOURCE_NUM; i++) {
			if (m_SEAudioSource [i].isPlaying) {
				continue;
			}
			m_SEAudioSource[i].PlayOneShot(m_SE[(int)type]);
			return;
		}

	}
}
