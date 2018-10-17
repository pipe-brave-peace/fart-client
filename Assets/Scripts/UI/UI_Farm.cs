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
    GameObject[] m_AreaCrops;     // エリアごとの農作物情報の代入

    int m_HP = 100;           // 畑HP
    int[] m_MAX_HP;       // MAX HP
    int m_AreaIndex;

    // Use this for initialization
    void Start () {
        // 農作物をカウントし、代入
        m_MAX_HP = new int[m_AreaCrops.Length];
        for (int i = 0; i < m_AreaCrops.Length; ++i)
        {
            m_MAX_HP[i] = m_AreaCrops[i].transform.childCount;
        }
        m_AreaIndex = 0;
        m_HP = m_MAX_HP[m_AreaIndex];
        // 残数表示
        m_HP_Text.text = m_HP.ToString();
        // 農作物総数の表示
        m_HP_MAX_Text.text = "/"+ m_MAX_HP[m_AreaIndex].ToString();
    }

    // Update is called once per frame
    void Update () {
        // 残り数のカウント
        m_HP = m_AreaCrops[m_AreaIndex].transform.childCount;
        m_HP_Text.text = m_HP.ToString();

        // デバッグ用キー押し判定
        if ( Input.GetKeyDown(KeyCode.Alpha1))
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
    public void SetPhase( int Index)
    {
        // 農作物をカウントし、代入
        m_AreaIndex = Index;
        m_HP = m_AreaCrops[m_AreaIndex].transform.childCount;

        // 残数表示
        m_HP_Text.text = m_HP.ToString();
        // 農作物総数の表示
        m_HP_MAX_Text.text = "/" + m_MAX_HP[m_AreaIndex].ToString();
    }
}
