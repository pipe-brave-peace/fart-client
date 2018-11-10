using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_KarasuLifeRot : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        transform.Rotate(new Vector3(0, -60, 0));  // 向きの微調整
    }
}
