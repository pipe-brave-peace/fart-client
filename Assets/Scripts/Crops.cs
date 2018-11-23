using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crops : MonoBehaviour {
    
    [SerializeField]
    GameObject UI_HP;      // 残りの農作物
    [SerializeField]
    GameObject m_Soil;

    private Life m_Life;

    // Use this for initialization
    void Start () {
        m_Life = GetComponent<Life>();
        UI_HP.SetActive(false);
        UI_HP.GetComponent<TextMesh>().text = "　"+m_Life.GetLife().ToString();
    }
	
	// Update is called once per frame
	void Update ()
    {
        UI_HP.GetComponent<TextMesh>().text = "　" + m_Life.GetLife().ToString();
        if(m_Life.isDamage())
        {
            UI_HP.SetActive(true);
        }
    }
    private void OnDestroy()
    {
        m_Soil.transform.parent = transform.parent;
        m_Soil.GetComponent<Soil>().enabled = true;
        m_Soil.GetComponent<Soil>().m_bDead = true;
    }
}
