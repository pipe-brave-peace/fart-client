using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoneChild : MonoBehaviour {
    
	// Update is called once per frame
	void Update () {

        // 子がなくなった？
        if (transform.childCount <= 0)
        {
            // 自分を消す
            Destroy(gameObject);
        }
    }
}
