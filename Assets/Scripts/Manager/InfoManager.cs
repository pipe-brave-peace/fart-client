using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoManager : SingletonMonoBehaviour<InfoManager>
{
    public struct INFO
    {
        // メンバー変数
        private int Score;
        private int Enemy;
        private int Combo;

        // セット
        public void SetScore(int var) { Score = var; }
        public void SetEnemy(int var) { Enemy = var; }
        public void SetCombo(int var) { Combo = var; }

        // 取得
        public int GetScore() { return Score; }
        public int GetEnemy() { return Enemy; }
        public int GetCombo() { return Combo; }
        public int GetTotalScore() { return Score + Enemy + Combo; }
    }

    private INFO[] m_Player;
    // Use this for initialization
    void Start () {
        m_Player = new INFO[2];
        InitInfo();
    }

    // 数値の初期化
    public void InitInfo()
    {
        for(int i = 0; i < 2; ++i)
        {
            m_Player[i].SetScore(0);
            m_Player[i].SetEnemy(0);
            m_Player[i].SetCombo(0);
        }
    }

    // プレイヤーの数値を取得
    public INFO GetPlayerInfo(int Index)
    {
        return m_Player[Index];
    }
}
