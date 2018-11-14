using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IventOn : MonoBehaviour {

    [SerializeField]
    GameObject NarrationObject;

    bool m_bUse = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!m_bUse)
            {
                NarrationObject.SetActive(true);
                m_bUse = true;
            }
        }
    }
}
