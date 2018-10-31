using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Trigger : MonoBehaviour {

    [SerializeField]
    float m_TimerMax;

    private ParticleSystem m_Effect;
    private float m_Timer;

    // Use this for initialization
    void Start()
    {
        m_Timer = 0.0f;
        m_Effect = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        // テスト
        //if( Input.GetKeyDown(KeyCode.A))
        //{
        //    Play();
        //}
        m_Timer -= Time.deltaTime;
        if (m_Timer > 0.0f) return;
        m_Effect.Stop();
    }
    public void Play()
    {
        m_Effect.Play();
        m_Timer = m_TimerMax;
    }
}
