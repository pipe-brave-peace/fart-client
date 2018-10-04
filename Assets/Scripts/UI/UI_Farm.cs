using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Farm : MonoBehaviour {

    public Text HP_Text;      // 残りの農作物
    public Text HP_MAX_Text;  // 農作物の総数
    float m_HP = 100;         // 畑HP
    float m_MAX_HP = 100;     // MAX HP

    // Use this for initialization
    void Start () {
        // 農作物をカウントし、代入
        m_HP = GameObject.FindGameObjectsWithTag("Crops").Length;
        m_MAX_HP = m_HP;

        // 残数表示
        HP_Text.text = Mathf.FloorToInt(m_HP).ToString();
        // 農作物総数の表示
        HP_MAX_Text.text = "/"+ Mathf.FloorToInt(m_MAX_HP).ToString();
    }

    // Update is called once per frame
    void Update () {
        // 残り数のカウント
        m_HP = GameObject.FindGameObjectsWithTag("Crops").Length;
        HP_Text.text = Mathf.FloorToInt(m_HP).ToString();
    }
}
