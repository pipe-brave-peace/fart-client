using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_FarmGauge : MonoBehaviour {
    
    [SerializeField]
    Text m_FarmNum;   // パーセントのテキスト

    Slider m_Farm_UI;        // 畑ゲージ
    int m_FarmVerMax;
    int m_FarmVer;

    void Start () {
        m_Farm_UI = GetComponent<Slider>();
        m_FarmVer = 0;
        m_FarmNum.text = m_FarmVer.ToString()+"%";
        m_FarmVerMax = InfoManager.Instance.GetFarmGauge();
    }
	
	// Update is called once per frame
	void Update () {
        m_Farm_UI.value = m_FarmVer;
        m_FarmVer = Mathf.Min(m_FarmVer+1, m_FarmVerMax);
        m_FarmNum.text = m_FarmVer.ToString() + "%";
    }
}
