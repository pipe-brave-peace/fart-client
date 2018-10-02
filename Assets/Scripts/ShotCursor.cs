using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShotCursor : MonoBehaviour
{

    //　カーソルに使用するテクスチャ
    [SerializeField]
    private Texture2D cursor;

    [SerializeField]
    GameObject BulletObject;

    [SerializeField]
    float VecPow = 0.0f;

    [SerializeField]
    Slider FurzUI;

    [SerializeField]
    Image image;

    private List<Joycon> m_joycons;
    private Joycon m_joyconR;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        m_joycons = JoyconManager.Instance.j;

        m_joyconR = m_joycons.Find(c => !c.isLeft);

        //　カーソルを自前のカーソルに変更
        Cursor.SetCursor(cursor, new Vector2(cursor.width / 2, cursor.height / 2), CursorMode.ForceSoftware);
    }

    void Update()
    {
        Vector3 rayPos = new Vector3(image.rectTransform.position.x * 0.999f, image.rectTransform.position.y, image.rectTransform.position.z);
  
        Ray ray = Camera.main.ScreenPointToRay(rayPos);
        transform.rotation = Quaternion.LookRotation(ray.direction);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1f, LayerMask.GetMask("Gun")))
        {
            Cursor.visible = false;
        }
        else
        {
            Cursor.visible = true;
        }

        //　マウスの左クリックで撃つ
        if (/*m_joyconR.GetButtonDown( Joycon.Button.SR ) ||*/ Input.GetKeyDown(KeyCode.P))
        {
            if (FurzUI.value <= 0) { return; }

            if (m_joyconR != null)
            {
                m_joyconR.SetRumble(1000, 1000, 1.0f, 200);
            }

            FurzUI.value -= 0.1f;

            GameObject newBullet =  Instantiate(BulletObject, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            //Shot();
            Vector3 force = transform.forward * VecPow;

            newBullet.GetComponent<Rigidbody>().AddForce(force);
        }
    }

    //　敵を撃つ
    void Shot()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Enemy")))
        {
            Destroy(hit.collider.gameObject);
        }
    }
}