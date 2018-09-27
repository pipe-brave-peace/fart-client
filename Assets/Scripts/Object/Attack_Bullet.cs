using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Bullet : MonoBehaviour {

    EffekseerEmitter m_Effect;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        m_Effect.Play();
    }
}
