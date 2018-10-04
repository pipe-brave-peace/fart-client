using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {

    // 対象カメラ
    [SerializeField]
    Camera targetCamera;

    void Update()
    {
        // 対象カメラに向く
        transform.LookAt(transform.position + targetCamera.transform.rotation * Vector3.forward,
                         targetCamera.transform.rotation * Vector3.up);
    }
}
