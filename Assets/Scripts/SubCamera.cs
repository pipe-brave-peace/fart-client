using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubCamera : MonoBehaviour {

    [SerializeField]
    GameObject[] m_Root;

    [SerializeField]
    PhaseManager m_PhaseManager;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (m_PhaseManager.GetNowPhaseIndex() >= 2)
        {
            transform.position = Vector3.Lerp(transform.position, m_Root[m_PhaseManager.GetNowPhaseIndex()].transform.position, 0.1f);
        }
	}
}
