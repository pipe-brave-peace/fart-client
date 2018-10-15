﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour {

    [SerializeField]
    NavMeshAgent m_Navigate = null;

    [SerializeField]
    GameObject[] m_NavPoint;

    int nNumber = 0;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (m_NavPoint.Length - 1 < nNumber)
        {
            //対象の位置の方向に移動
            m_Navigate.SetDestination(transform.position);
        }
        else
        {
            //対象の位置の方向に移動
            m_Navigate.SetDestination(m_NavPoint[nNumber].transform.position);

            if (transform.position.x == m_NavPoint[nNumber].transform.position.x &&
                transform.position.z == m_NavPoint[nNumber].transform.position.z)
            {
                nNumber++;
            }
        }

    }
}
