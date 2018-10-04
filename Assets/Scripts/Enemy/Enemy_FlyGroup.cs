using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_FlyGroup : MonoBehaviour {

    public GameObject[] m_NavObj;
    GameObject m_NextObj;
    // Use this for initialization
    void Start () {
        m_NextObj = m_NavObj[0];
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Crops")
        {
            //m_Mode = MODE.ATTACK;
        }
    }
}
