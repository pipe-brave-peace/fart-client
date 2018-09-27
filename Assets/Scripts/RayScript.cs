using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RayScript : MonoBehaviour
{

    private static readonly Joycon.Button[] m_buttons =
    Enum.GetValues(typeof(Joycon.Button)) as Joycon.Button[];

    private List<Joycon> m_joycons;
    private Joycon m_joyconL;
    private Joycon m_joyconR;
    private Joycon.Button? m_pressedButtonL;
    private Joycon.Button? m_pressedButtonR;

    // 照準のImageをインスペクターから設定
    [SerializeField]
    private Image aimPointImage;

    private void Start()
    {
        m_joycons = JoyconManager.Instance.j;

        if (m_joycons == null || m_joycons.Count <= 0) return;

        m_joyconL = m_joycons.Find(c => c.isLeft);
        m_joyconR = m_joycons.Find(c => !c.isLeft);
    }

    void Update()
    {
        var orientation = m_joyconL.GetVector().eulerAngles;
        var angles = transform.localEulerAngles;
        angles.x = -orientation.x;
        angles.y = orientation.z;
        angles.z = orientation.y;
        //angles.z = orientation.z;
        transform.localEulerAngles = angles;

        if (m_joyconL.GetButton(Joycon.Button.DPAD_RIGHT))
        {
            transform.localRotation = new Quaternion(0, 0, 0, 0);
        }

        // Rayを飛ばす（第1引数がRayの発射座標、第2引数がRayの向き）
        Ray ray = new Ray(transform.position, transform.forward);

        // outパラメータ用に、Rayのヒット情報を取得するための変数を用意
        RaycastHit hit;

        // シーンビューにRayを可視化するデバッグ（必要がなければ消してOK）
        Debug.DrawRay(ray.origin, ray.direction * 30.0f, Color.red, 0.0f);

        // Rayのhit情報を取得する
        if (Physics.Raycast(ray, out hit, 30.0f))
        {

            // Rayがhitしたオブジェクトのタグ名を取得
            string hitTag = hit.collider.tag;

            // タグの名前がEnemyだったら、照準の色が変わる
            if ((hitTag.Equals("Enemy")))
            {
                //照準を赤に変える
                aimPointImage.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
            }
            else
            {
                // Enemy以外では水色に
                aimPointImage.color = new Color(0.0f, 1.0f, 1.0f, 1.0f);
            }

        }
        else
        {
            // Rayがヒットしていない場合は水色に
            aimPointImage.color = new Color(0.0f, 1.0f, 1.0f, 1.0f);
        }
    }
}