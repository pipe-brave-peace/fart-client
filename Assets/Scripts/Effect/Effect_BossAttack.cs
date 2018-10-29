using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_BossAttack : MonoBehaviour {

    [SerializeField]
    GameObject p_Img;           // 衝撃イメージ画像

    void Start()
    {
        // 画像の生成
        GameObject shock_img = Instantiate(p_Img, Vector3.zero, Quaternion.identity) as GameObject;
        shock_img.transform.SetParent(transform, false);
        // ザイズの初期化
        shock_img.transform.localScale = new Vector3(1.0f, 1.0f, 0.0f);
    }
}
