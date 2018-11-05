using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gas : MonoBehaviour {

    Vector3 m_localGravity;

    Rigidbody m_rb;

    Material m_material;

    [SerializeField]
    int m_nTime = 0;

    // Use this for initialization
    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        m_rb.useGravity = false;

        m_material = GetComponent<Renderer>().material;

        m_localGravity = new Vector3(0, -1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        m_rb.AddForce(m_localGravity, ForceMode.Acceleration);

        gameObject.transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);

        m_material.color -= new Color(0, 0, 0, 0.001f);

        if (m_material.color.a <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        //if (col.gameObject.tag == "Explosion")
        //{
        //   col.gameObject.transform.localScale = new Vector3(5,5,5);
        //}

        if (col.gameObject.tag == "Enemy")
        {
            col.GetComponent<Enemy_State>().SetState(Enemy_State.STATE.SPRAY);
        }

    }
}
