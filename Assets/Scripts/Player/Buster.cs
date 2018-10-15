using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Buster : MonoBehaviour
{
    [SerializeField]
    Tank m_Tank = null;

    [SerializeField]
    GameObject m_BulletObject;

    [SerializeField]
    float m_VecPow = 0.0f;

    [SerializeField]
    float m_FartsValue = 0.0f;

    [SerializeField]
    Slider m_FartsUI = null;

    [SerializeField]
    Image m_ReticleUI = null;

    [SerializeField]
    WiimoteSharing m_WiimoteSharing = null;

    private List<Joycon> m_joycons;
    private Joycon m_joyconR;

    void Start()
    {
        m_joycons = JoyconManager.Instance.j;

        m_joyconR = m_joycons.Find(c => !c.isLeft);
    }

    void Update()
    {

        if (m_WiimoteSharing.GetWiimote() != null)
        {
            float[] pointer = m_WiimoteSharing.GetWiimote().Ir.GetPointingPosition();
            var point = new Vector2(pointer[0], pointer[1]);
            m_ReticleUI.rectTransform.anchorMax = Vector2.Lerp(m_ReticleUI.rectTransform.anchorMax, point, 0.5f);
            m_ReticleUI.rectTransform.anchorMin = Vector2.Lerp(m_ReticleUI.rectTransform.anchorMin, point, 0.5f);

            if (m_WiimoteSharing.GetWiimote().Button.a)
            {
                GasShot();
            }
        }
        else
        {
            Cursor.visible = false;
            var position = Input.mousePosition;
            m_ReticleUI.rectTransform.position = position;
        }

        Vector3 rayPos = new Vector3(m_ReticleUI.rectTransform.position.x, m_ReticleUI.rectTransform.position.y, m_ReticleUI.rectTransform.position.z);

        Ray ray = Camera.main.ScreenPointToRay(rayPos);
        transform.rotation = Quaternion.LookRotation(ray.direction);

        if (m_joyconR != null)
        {
            if (m_joyconR.GetButtonDown(Joycon.Button.SR))
            {
                m_joyconR.SetRumble(1000, 1000, 1.0f, 200);

                BulletShot();
            }
        }

        if (Input.GetKey(KeyCode.B))
        {
            GasShot();
        }
    }

    //　敵を撃つ
    void BulletShot()
    {
        //if (m_FartsUI.value <= 0) { return; }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Enemy")))
        {
            Destroy(hit.collider.gameObject);
        }

        m_Tank.FartingFarts(-0.5f);

        //m_FartsUI.value -= m_FartsValue;

        //GameObject newBullet = Instantiate(m_BulletObject, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        //
        //Vector3 force = transform.forward * m_VecPow;
        //
        //newBullet.GetComponent<Rigidbody>().AddForce(force);
    }

    void GasShot()
    {
        m_Tank.FartingFarts(-0.01f);
    }
}