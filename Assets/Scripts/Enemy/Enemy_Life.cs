using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Life : MonoBehaviour {

    // 対象カメラ
    [SerializeField]
    Camera targetCamera;
    [SerializeField]
    GameObject m_DamageEffect;        // 弾の爆発エフェクト

    void Update()
    {
        // 対象カメラに向く
        transform.LookAt(transform.position + targetCamera.transform.rotation * Vector3.forward,
                         targetCamera.transform.rotation * Vector3.up);
    }
    private void OnDisable()
    {
        // エフェクトの生成
        GameObject damage_effect = Instantiate(m_DamageEffect, transform.position, Quaternion.identity) as GameObject;
        damage_effect.transform.parent = transform.parent;
        damage_effect.transform.localScale *= 0.075f;
    }
}
