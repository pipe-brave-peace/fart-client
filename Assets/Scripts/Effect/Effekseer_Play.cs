using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effekseer_Play : MonoBehaviour {

    private EffekseerEmitter m_Effekseer;

	// Use this for initialization
	void Start () {
        m_Effekseer = GetComponent<EffekseerEmitter>();
	}
	
	// Update is called once per frame
	void Update () {
		if( Input.GetKeyDown(KeyCode.P))
        {
            m_Effekseer.Play();
        }
	}
}
