using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {

    // 対象カメラ
    private Camera targetCamera;

    private void Start()
    {
        targetCamera = Camera.main;
    }

    void Update()
    {
        // 対象カメラに向く
        transform.LookAt(transform.position + targetCamera.transform.rotation * Vector3.forward,
                         targetCamera.transform.rotation * Vector3.up);
    }
}
