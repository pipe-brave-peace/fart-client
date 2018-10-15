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
        private int ComboMax;

        // セット
        public void SetScore(int var) { Score = var; }
        public void SetEnemy(int var) { Enemy = var; }
        public void SetCombo(int var) { Combo = var; }

        // 計算
        public void AddScore(int var = 1) { Score += var;}
        public void AddEnemy(int var = 1) { Enemy += var; }
        public void AddCombo(int var = 1)
        {
            Combo += var;
            ComboMax = Mathf.Max(ComboMax, Combo);  //  最大コンボ数のチェック
        }

        // 初期化
        public void Init()
        {
            Score = 10;
            Enemy = 0;
            Combo = 0;
            ComboMax = 0;
        }
        public void InitCombo() { Combo = 0; }

        // 取得
        public int GetScore() { return Score; }
        public int GetEnemy() { return Enemy; }
        public int GetCombo() { return Combo; }
        public int GetTotalScore() { return Score + Enemy + Combo; }
    }

    private INFO[] m_Player = new INFO[2];

    public void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }
        InitInfo();
        DontDestroyOnLoad(this.gameObject);
    }

    // 数値の初期化
    public void InitInfo()
    {
        for(int i = 0; i < 2; ++i)
        {
            m_Player[i].Init();
        }
    }

    // プレイヤーの数値を取得
    public INFO GetPlayerInfo(int Index)
    {
        return m_Player[Index];
    }
    public void SetPlayerScore(int Index,int var)
    {
        m_Player[Index].SetScore(var);
    }
    public void SetPlayerEnemy(int Index, int var)
    {
        m_Player[Index].SetEnemy(var);
    }
    public void SetPlayerCombo(int Index, int var)
    {
        m_Player[Index].SetCombo(var);
    }
}
