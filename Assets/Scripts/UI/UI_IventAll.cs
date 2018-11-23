using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_IventAll : MonoBehaviour {

    [SerializeField]
    UI_Player Ivent1;

    [SerializeField]
    UI_Player Ivent2;

    [SerializeField]
    bool m_bIventFlg;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (m_bIventFlg)
        {
            Ivent1.IventOnMove();
            Ivent2.IventOnMove();
        }
        else
        {
            Ivent1.IventOffMove();
            Ivent2.IventOffMove();
        }

	}

    public void SetIventFlg(bool bIvent){ m_bIventFlg = bIvent; }

    public bool GetIventFlg() { return m_bIventFlg;  }
}
