using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_KarasuLifeRot : MonoBehaviour {    

	// Use this for initialization
	void Start ()
    {
        transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        transform.localRotation = Quaternion.Euler(0.0f, -60.0f, 0.0f);
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }
}
