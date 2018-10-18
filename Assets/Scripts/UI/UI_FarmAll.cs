using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_FarmAll : MonoBehaviour {

    [SerializeField]
    Slider m_Farm_UI;        // 畑ゲージ
    [SerializeField]
    Text m_PercentHP_Text;   // パーセントのテキスト
    [SerializeField]
    Text m_DebugText_HPMAX;
    [SerializeField]
    Text m_DebugText_HP;

    private float m_HP = 100;        // 畑HP
    private float m_MAX_HP = 100;     // MAX HP

    void Start()
    {
        // 農作物をカウントし、代入
        m_HP = GameObject.FindGameObjectsWithTag("Crops").Length;
        m_MAX_HP = m_HP;
        // 残数代入
        m_Farm_UI.value = m_HP;
        
        // 畑ゲージのMAXゲージの代入
        m_Farm_UI.maxValue = m_MAX_HP;

        // パーセントの初期化
        m_PercentHP_Text.text = "100%";

        // テスト
        m_DebugText_HPMAX.text = Mathf.CeilToInt( m_MAX_HP).ToString()+"/";
        m_DebugText_HP.text = Mathf.CeilToInt(m_HP).ToString();
    }

    void Update()
    {
        // 
        m_HP = GameObject.FindGameObjectsWithTag("Crops").Length;
        m_Farm_UI.value = m_HP;

        // テスト
        m_DebugText_HP.text = Mathf.CeilToInt(m_HP).ToString();

        // パーセントの算出
        int percentHP = Mathf.FloorToInt(m_HP / m_MAX_HP * 100.0f);
        m_PercentHP_Text.text = percentHP.ToString() + "%";
        InfoManager.Instance.SetFarmGauge(percentHP);
    }
}
