using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Damage : MonoBehaviour {
    
    [SerializeField]
    GameObject p_ShockImg;           // 衝撃イメージ画像
    [SerializeField]
    GameObject p_PlayerImg;           // 衝撃受けたイメージ画像

    public void Set(int TargetIndex = 0)
    {
        // 画像の生成
        GameObject shock_img = Instantiate(p_ShockImg, Vector3.zero, Quaternion.Euler(0, 0, Random.Range(-180, 180))) as GameObject;
        shock_img.transform.SetParent(transform, false);
        GameObject player_img = Instantiate(p_PlayerImg, Vector3.zero, Quaternion.Euler(0, 180*TargetIndex, 0)) as GameObject;
        player_img.transform.SetParent(transform, false);
        // ザイズの初期化
        shock_img.transform.localScale  = new Vector3(1.0f, 1.0f, 0.0f);
        player_img.transform.localScale = new Vector3(1.0f, 1.0f, 0.0f);

        // 生成した画像の配置
        float width_offset = Screen.width * 0.5f * TargetIndex;     // ターゲットによる表示する範囲
        float size_offset  = Screen.width * 0.2f;                   // 画像が見切れないように大まかの範囲
        // 座標配置
        shock_img.transform.position = new Vector3(Random.Range(width_offset + size_offset, Screen.width * 0.5f + width_offset - size_offset),
                                                   Random.Range(size_offset, Screen.height - size_offset), 0.0f);
    }
}
