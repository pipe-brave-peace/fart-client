using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Farm : MonoBehaviour {

    // デバッグ用変数
    public Text Debug_HP_Text;      // 残りの農作物
    public Text Debug_HP_MAX_Text;  // 農作物の総数

    public Slider m_Farm_UI;        // 畑ゲージ
    public Text m_PercentHP_Text;   // パーセントのテキスト

    float m_HP = 100;        // 畑HP
    float m_MAX_HP = 100;     // MAX HP

    void Start()
    {
        // 農作物をカウントし、代入
        m_HP = GameObject.FindGameObjectsWithTag("Crops").Length;
        m_MAX_HP = m_HP;
        // 残数表示
        Debug_HP_Text.text = Mathf.FloorToInt(m_HP).ToString();
        // 残数代入
        m_Farm_UI.value = m_HP;

        // 農作物総数の表示
        Debug_HP_MAX_Text.text = Mathf.FloorToInt(m_MAX_HP).ToString() + "/";
        // 畑ゲージのMAXゲージの代入
        m_Farm_UI.maxValue = m_MAX_HP;

        // パーセントの初期化
        m_PercentHP_Text.text = "100%";
    }

    void Update()
    {
        // 
        m_HP = GameObject.FindGameObjectsWithTag("Crops").Length;
        Debug_HP_Text.text = Mathf.FloorToInt(m_HP).ToString();
        m_Farm_UI.value = m_HP;

        // パーセントの算出
        int percentHP = Mathf.FloorToInt(m_HP / m_MAX_HP * 100.0f);
        m_PercentHP_Text.text = percentHP.ToString() + "%";
    }
}
