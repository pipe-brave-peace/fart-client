using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Boar : MonoBehaviour {

    public int m_Life = 3;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if( m_Life <= 0)
        {
            Destroy(this);
        }
	}

    public void Damage(int Damage = 1)
    {
        m_Life -= Damage;
    }
}
