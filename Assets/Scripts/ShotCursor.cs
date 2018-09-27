using UnityEngine;
using System.Collections;

public class ShotCursor : MonoBehaviour
{

    //　カーソルに使用するテクスチャ
    [SerializeField]
    private Texture2D cursor;

    [SerializeField]
    GameObject BulletObject;

    [SerializeField]
    float VecPow = 0.0f;

    void Start()
    {
        //　カーソルを自前のカーソルに変更
        Cursor.SetCursor(cursor, new Vector2(cursor.width / 2, cursor.height / 2), CursorMode.ForceSoftware);
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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
        if (Input.GetButtonDown("Fire1"))
        {
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