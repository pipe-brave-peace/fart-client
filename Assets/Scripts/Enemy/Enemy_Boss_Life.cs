using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Enemy_Boss_Life : MonoBehaviour {

    private const int LIFE_CNT = 3;

    [SerializeField]
    List<GameObject> m_Life;
	// Use this for initialization
	void Start () {
        int life_cnt = m_Life.Count - LIFE_CNT;
        for (int i = 0; i < life_cnt; ++i)
        {
            int index = Random.Range(0, m_Life.Count);            
            Destroy(m_Life[index]);
            m_Life.Remove(m_Life[index]);
        }


	}
}
