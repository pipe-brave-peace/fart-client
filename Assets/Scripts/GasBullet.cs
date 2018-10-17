using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasBullet : MonoBehaviour {

    [SerializeField]
    GameObject m_ExplosionObject;

    Vector3 m_localGravity;

    Rigidbody m_rb;

    // Use this for initialization
    void Start () {

        m_rb = GetComponent<Rigidbody>();
        m_rb.useGravity = false;

        m_localGravity = new Vector3(0, -1, 0);

    }
	
	// Update is called once per frame
	void Update () {

        m_rb.AddForce(m_localGravity, ForceMode.Acceleration);

    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag != "Gas" ||
            other.gameObject.tag != "Player")
        {
            Instantiate(m_ExplosionObject, gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
