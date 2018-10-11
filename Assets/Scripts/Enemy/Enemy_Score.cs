using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Score : MonoBehaviour {

    [SerializeField]
    int m_Score;

    public void SetScore(int Score)
    {
        m_Score = Score;
    }

    public int GetScore()
    {
        int score = m_Score;
        m_Score = 0;
        return score;
    }
}
