using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_e : MonoBehaviour {

    [SerializeField]
    GameObject m_Effect;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if( Input.GetKeyDown(KeyCode.E))
        {
            GameObject effet = Instantiate(m_Effect, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
            effet.transform.position = transform.position;
        }
	}
}
