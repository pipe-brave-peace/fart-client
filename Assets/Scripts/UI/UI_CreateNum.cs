using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CreateNum : MonoBehaviour {

    [SerializeField]
    GameObject m_EffectNum;
    
    Text m_Text;

    // Use this for initialization
    void Start () {
        m_Text = GetComponent<Text>();
	}

    public void CreateNum()
    {
        // 数字の生成
        GameObject num = (GameObject)Instantiate(m_EffectNum);
        num.transform.SetParent(transform.parent.transform, false);

        // 初期化
        num.transform.position = transform.position;
        num.transform.localScale = transform.localScale;
        num.GetComponent<Text>().text = m_Text.text;
    }
}
