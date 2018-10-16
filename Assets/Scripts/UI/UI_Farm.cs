using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Farm : MonoBehaviour {
    [SerializeField]
    Text m_HP_Text;      // 残りの農作物
    [SerializeField]
    Text m_HP_MAX_Text;  // 農作物の総数
    [SerializeField]
    Phase[] m_Phase;     // フェーズ情報の代入

    float m_HP = 100;           // 畑HP
    float m_MAX_HP = 100;       // MAX HP
    Phase m_NowPhase;           // 現在のフェーズ情報

    // Use this for initialization
    void Start () {
        // 初期フェーズ情報の代入
        m_NowPhase = m_Phase[0];

        // 農作物をカウントし、代入
        m_HP = m_NowPhase.GetCropsChildCount();
        m_MAX_HP = m_HP;

        // 残数表示
        m_HP_Text.text = Mathf.FloorToInt(m_HP).ToString();
        // 農作物総数の表示
        m_HP_MAX_Text.text = "/"+ Mathf.FloorToInt(m_MAX_HP).ToString();
    }

    // Update is called once per frame
    void Update () {
        // 残り数のカウント
        m_HP = m_NowPhase.GetCropsChildCount();
        m_HP_Text.text = Mathf.FloorToInt(m_HP).ToString();

        // デバッグ用キー押し判定
        if( Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetPhase(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetPhase(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetPhase(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetPhase(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SetPhase(4);
        }
    }

    // 
    public void SetPhase( int Phase)
    {
        // フェーズ情報の代入
        m_NowPhase = m_Phase[Phase];

        // 農作物をカウントし、代入
        m_HP = m_NowPhase.GetCropsChildCount();
        m_MAX_HP = m_NowPhase.GetMaxCropsChildCount();

        // 残数表示
        m_HP_Text.text = Mathf.FloorToInt(m_HP).ToString();
        // 農作物総数の表示
        m_HP_MAX_Text.text = "/" + Mathf.FloorToInt(m_MAX_HP).ToString();
    }
}
