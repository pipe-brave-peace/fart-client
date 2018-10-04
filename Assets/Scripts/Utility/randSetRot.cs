using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randSetRot : MonoBehaviour {

	// Use this for initialization
	void Start () {
        transform.rotation = Random.rotation;
	}
}
