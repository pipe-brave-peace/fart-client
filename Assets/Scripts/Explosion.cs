using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{

    [SerializeField]
    int m_nTime = 0;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (m_nTime <= 0)
        {
            Destroy(gameObject);
        }

        m_nTime--;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            col.GetComponent<Enemy_State>().SetState(Enemy_State.STATE.ESCAPE);
        }
    }
}
