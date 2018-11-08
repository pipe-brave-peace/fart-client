using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CinemaCamera : MonoBehaviour {

    [SerializeField]
    PlayableDirector m_PlayableDirector;

	// Use this for initialization
	void Start () {
        m_PlayableDirector.Play();
	}
	
	// Update is called once per frame
	void Update () {

        Debug.Log(m_PlayableDirector.time);

        if (m_PlayableDirector.time > 150)
        {
            m_PlayableDirector.Pause();
        }

	}
}
