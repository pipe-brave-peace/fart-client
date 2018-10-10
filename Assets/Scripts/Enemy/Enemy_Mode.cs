using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Mode : MonoBehaviour {

    public enum MODE
    {
        NORMAL = 0, // 通常
        MOVE,       // 移動
        EAT,        // 食べる
        ATTACK,     // 攻撃
        SATIETY,    // 満腹
        ESCAPE,     // 逃げる
        MAX
    }

    private MODE m_Mode;
   
    public void SetMode( MODE Mode)
    {
        m_Mode = Mode;
    }

    public MODE GetMode()
    {
        return m_Mode;
    }
}
