using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crops_Ninjin : MonoBehaviour {
    
    [SerializeField]
    float m_DamageTime;
    [SerializeField]
    TextMesh UI_HP;      // 残りの農作物

    private Life m_Life;

    // Use this for initialization
    void Start () {
        m_Life = GetComponent<Life>();
        UI_HP.text = m_Life.GetLife().ToString();
    }
	
	// Update is called once per frame
	void Update ()
    {
        UI_HP.text = m_Life.GetLife().ToString();
    }
}
