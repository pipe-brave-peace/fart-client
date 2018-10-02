using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crops_Ninjin : MonoBehaviour {

    public int m_Life = 5;
    float m_Rot_Y = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        m_Rot_Y += 1.0f;
        transform.rotation = Quaternion.Euler(45.0f, m_Rot_Y, 45.0f);
        if( m_Life <= 0)
        {
            Destroy(this);
        }
	}

    // ダメージ処理
    public void Damage(int Damage)
    {
        m_Life -= Damage;
    }
}
