using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Result_InfoNum : MonoBehaviour {

    [SerializeField]
    Text m_Player1_Score;
    [SerializeField]
    Text m_Player2_Score;


    [SerializeField]
    Text m_Player1_Enemy;
    [SerializeField]
    Text m_Player2_Enemy;
    [SerializeField]
    Text m_Player1_Combo;
    [SerializeField]
    Text m_Player2_Combo;
    [SerializeField]
    Text m_Player1_TotalScore;
    [SerializeField]
    Text m_Player2_TotalScore;
    [SerializeField]
    Text m_Player1_Rank;
    [SerializeField]
    Text m_Player2_Rank;

    [SerializeField]
    float m_Time;

    enum MODE
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

    // Use this for initialization
    void Start () {
        m_CntFrame = 0.0f;
        m_Mode = MODE.SCORE;

        // 数字の初期化
        m_Player1_Score.text = " ";
        m_Player2_Score.text = " ";
        m_Player1_Enemy.text = " ";
        m_Player2_Enemy.text = " ";
        m_Player1_Combo.text = " ";
        m_Player2_Combo.text = " ";
        m_Player1_TotalScore.text = " ";
        m_Player2_TotalScore.text = " ";
        m_Player1_Rank.text = " ";
        m_Player2_Rank.text = " ";

        // テスト
        InfoManager.Instance.SetPlayerScore(0, 2000);
    }
	
	// Update is called once per frame
	void Update () {
        m_CntFrame += Time.deltaTime;
        int num = 0;

        switch(m_Mode)
        {
            case MODE.SCORE:
                num = Random.Range(10000, 99999);
                m_Player1_Score.text = num.ToString();
                m_Player2_Score.text = num.ToString();
                if ( m_CntFrame >= m_Time)
                {
                    m_Player1_Score.text = InfoManager.Instance.GetPlayerInfo(0).GetScore().ToString();
                    m_Player2_Score.text = InfoManager.Instance.GetPlayerInfo(1).GetScore().ToString();
                    m_CntFrame = 0.0f;
                    m_Mode++;
                }
                break;
            case MODE.ENEMY:
                num = Random.Range(10000, 99999);
                m_Player1_Enemy.text = num.ToString();
                m_Player2_Enemy.text = num.ToString();
                if (m_CntFrame >= m_Time)
                {
                    m_Player1_Enemy.text = InfoManager.Instance.GetPlayerInfo(0).GetEnemy().ToString();
                    m_Player2_Enemy.text = InfoManager.Instance.GetPlayerInfo(1).GetEnemy().ToString();
                    m_CntFrame = 0.0f;
                    m_Mode++;
                }
                break;
            case MODE.COMBO:
                num = Random.Range(10000, 99999);
                m_Player1_Combo.text = num.ToString();
                m_Player2_Combo.text = num.ToString();
                if (m_CntFrame >= m_Time)
                {
                    m_Player1_Combo.text = InfoManager.Instance.GetPlayerInfo(0).GetCombo().ToString();
                    m_Player2_Combo.text = InfoManager.Instance.GetPlayerInfo(1).GetCombo().ToString();
                    m_CntFrame = 0.0f;
                    m_Mode++;
                }
                break;
            case MODE.TOTAL_SCORE:
                num = Random.Range(10000, 99999);
                m_Player1_TotalScore.text = num.ToString();
                m_Player2_TotalScore.text = num.ToString();
                if (m_CntFrame >= m_Time)
                {
                    m_Player1_TotalScore.text = InfoManager.Instance.GetPlayerInfo(0).GetTotalScore().ToString();
                    m_Player2_TotalScore.text = InfoManager.Instance.GetPlayerInfo(1).GetTotalScore().ToString();
                    m_CntFrame = 0.0f;
                    m_Mode++;
                }
                break;
            case MODE.RANK:
                m_Player1_Rank.text = InfoManager.Instance.GetPlayerRank(0);
                m_Player2_Rank.text = InfoManager.Instance.GetPlayerRank(1);
                m_CntFrame = 0.0f;
                m_Mode++;
                break;
        }
    }
}
