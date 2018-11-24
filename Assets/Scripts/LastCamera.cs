using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CameraVector(Vector3 pos)
    {

        // 次の位置への方向を求める
        var dir = pos - transform.position;


        // 方向と現在の前方との角度を計算（スムーズに回転するように係数を掛ける）
        float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.5f);
        var angle = Mathf.Acos(Vector3.Dot(transform.forward, dir.normalized)) * Mathf.Rad2Deg * smooth;

        // 回転軸を計算
        var axis = Vector3.Cross(transform.forward, dir);

        // 回転の更新
        var rot = Quaternion.AngleAxis(angle, axis);

        if (rot * transform.forward != Vector3.zero)
        {
            transform.forward = rot * transform.forward;
        }

    }
}
