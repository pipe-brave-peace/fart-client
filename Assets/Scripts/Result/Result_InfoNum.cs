using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Result_InfoNum : MonoBehaviour {

    [SerializeField]
    GameObject m_Player1_Score;
    [SerializeField]
    GameObject m_Player2_Score;
    [SerializeField]
    GameObject m_Player1_Enemy;
    [SerializeField]
    GameObject m_Player2_Enemy;
    [SerializeField]
    GameObject m_Player1_Combo;
    [SerializeField]
    GameObject m_Player2_Combo;
    [SerializeField]
    GameObject m_Player1_TotalScore;
    [SerializeField]
    GameObject m_Player2_TotalScore;
    [SerializeField]
    GameObject m_Player1_Rank;
    [SerializeField]
    GameObject m_Player2_Rank;

    [SerializeField]
    float m_Time;

    public enum MODE
    {
        SCORE = 0,
        ENEMY,
        COMBO,
        TOTAL_SCORE,
        RANK,
        MAX
    }
    
    private float m_CntFrame;
    private MODE m_Mode;
    Text m_Player1_Text_Score;
    Text m_Player2_Text_Score;
    Text m_Player1_Text_Enemy;
    Text m_Player2_Text_Enemy;
    Text m_Player1_Text_Combo;
    Text m_Player2_Text_Combo;
    Text m_Player1_Text_TotalScore;
    Text m_Player2_Text_TotalScore;
    Text m_Player1_Text_Rank;
    Text m_Player2_Text_Rank;

    // Use this for initialization
    void Start () {
        m_CntFrame = 0.0f;

        // テキストの代入
        m_Player1_Text_Score      = m_Player1_Score.GetComponent<Text>();
        m_Player2_Text_Score      = m_Player2_Score.GetComponent<Text>();
        m_Player1_Text_Enemy      = m_Player1_Enemy.GetComponent<Text>();
        m_Player2_Text_Enemy      = m_Player2_Enemy.GetComponent<Text>();
        m_Player1_Text_Combo      = m_Player1_Combo.GetComponent<Text>();
        m_Player2_Text_Combo      = m_Player2_Combo.GetComponent<Text>();
        m_Player1_Text_TotalScore = m_Player1_TotalScore.GetComponent<Text>();
        m_Player2_Text_TotalScore = m_Player2_TotalScore.GetComponent<Text>();
        m_Player1_Text_Rank       = m_Player1_Rank.GetComponent<Text>();
        m_Player2_Text_Rank       = m_Player2_Rank.GetComponent<Text>();

        // 数字の初期化
        m_Player1_Text_Score.text = " ";
        m_Player2_Text_Score.text = " ";
        m_Player1_Text_Enemy.text = " ";
        m_Player2_Text_Enemy.text = " ";
        m_Player1_Text_Combo.text = " ";
        m_Player2_Text_Combo.text = " ";
        m_Player1_Text_TotalScore.text = " ";
        m_Player2_Text_TotalScore.text = " ";
        m_Player1_Text_Rank.text = " ";
        m_Player2_Text_Rank.text = " ";
    }
	
	// Update is called once per frame
	void Update ()
    {
        int num = 0;
        switch (m_Mode)
        {
            case MODE.SCORE:
                num = Random.Range(10000, 99999);
                m_Player1_Text_Score.text = num.ToString();
                m_Player2_Text_Score.text = num.ToString();
                if (m_CntFrame >= m_Time)
                {
                    m_Player1_Text_Score.text = InfoManager.Instance.GetPlayerInfo(0).GetScore().ToString();
                    m_Player2_Text_Score.text = InfoManager.Instance.GetPlayerInfo(1).GetScore().ToString();
                    m_Player1_Score.GetComponent<UI_CreateNum>().CreateNum();
                    m_Player2_Score.GetComponent<UI_CreateNum>().CreateNum();

                    m_CntFrame = 0.0f;
                    m_Mode++;
                }
                break;
            case MODE.ENEMY:
                num = Random.Range(10000, 99999);
                m_Player1_Text_Enemy.text = num.ToString();
                m_Player2_Text_Enemy.text = num.ToString();
                if (m_CntFrame >= m_Time)
                {
                    m_Player1_Text_Enemy.text = InfoManager.Instance.GetPlayerInfo(0).GetEnemy().ToString();
                    m_Player2_Text_Enemy.text = InfoManager.Instance.GetPlayerInfo(1).GetEnemy().ToString();
                    m_Player1_Enemy.GetComponent<UI_CreateNum>().CreateNum();
                    m_Player2_Enemy.GetComponent<UI_CreateNum>().CreateNum();
                    m_CntFrame = 0.0f;
                    m_Mode++;
                }
                break;
            case MODE.COMBO:
                num = Random.Range(10000, 99999);
                m_Player1_Text_Combo.text = num.ToString();
                m_Player2_Text_Combo.text = num.ToString();
                if (m_CntFrame >= m_Time)
                {
                    m_Player1_Text_Combo.text = InfoManager.Instance.GetPlayerInfo(0).GetCombo().ToString();
                    m_Player2_Text_Combo.text = InfoManager.Instance.GetPlayerInfo(1).GetCombo().ToString();
                    m_Player1_Combo.GetComponent<UI_CreateNum>().CreateNum();
                    m_Player2_Combo.GetComponent<UI_CreateNum>().CreateNum();
                    m_CntFrame = 0.0f;
                    m_Mode++;
                }
                break;
            case MODE.TOTAL_SCORE:
                num = Random.Range(10000, 99999);
                m_Player1_Text_TotalScore.text = num.ToString();
                m_Player2_Text_TotalScore.text = num.ToString();
                if (m_CntFrame >= m_Time)
                {
                    m_Player1_Text_TotalScore.text = InfoManager.Instance.GetPlayerInfo(0).GetTotalScore().ToString();
                    m_Player2_Text_TotalScore.text = InfoManager.Instance.GetPlayerInfo(1).GetTotalScore().ToString();
                    m_Player1_TotalScore.GetComponent<UI_CreateNum>().CreateNum();
                    m_Player2_TotalScore.GetComponent<UI_CreateNum>().CreateNum();
                    m_CntFrame = 0.0f;
                    m_Mode++;
                }
                break;
            case MODE.RANK:
                m_Player1_Text_Rank.text = InfoManager.Instance.GetPlayerRank(0);
                m_Player2_Text_Rank.text = InfoManager.Instance.GetPlayerRank(1);
                m_CntFrame = 0.0f;
                m_Mode++;
                break;
            case MODE.MAX:
                return;
        }
        m_CntFrame += Time.deltaTime;
    }

    public void SetMode(MODE mode)
    {
        m_Mode = mode;
    }
}
