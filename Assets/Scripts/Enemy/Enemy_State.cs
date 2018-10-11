using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_State : MonoBehaviour {

    public enum STATE
    {
        NORMAL = 0, // 通常
        MOVE,       // 移動
        EAT,        // 食べる
        ATTACK,     // 攻撃
        SATIETY,    // 満腹
        ESCAPE,     // 逃げる
        MAX
    }

    private STATE m_State;
   
    public void SetState(STATE State)
    {
        m_State = State;
    }

    public STATE GetState()
    {
        return m_State;
    }
}
