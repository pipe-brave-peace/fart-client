using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_State : MonoBehaviour {

    public enum STATE
    {
        MOVE = 0,   // 移動
        EAT,        // 食べる
        ATTACK,     // 攻撃
        CRY,        // 叫ぶ
        DAMAGE,     // ダメージ
        FEAR,       // 怯む
        SATIETY,    // 満腹
        ESCAPE,     // 逃げる
        FAINT,      // 気絶
        MAX
    }
    [SerializeField]
    bool m_canBuff;

    private STATE m_State = STATE.MOVE;
    private bool m_Buff = false;
    private bool m_canSet = true;
   
    public void SetState(STATE State)
    {
        if(!m_canSet) { return; }
        m_State = State;
    }

    public STATE GetState()
    {
        return m_State;
    }

    public void CanSet(bool CanSet)
    {
        m_canSet = CanSet;
    }

    public void SetBuff(bool Buff = true)
    {
        if (!m_canBuff) { return; }
        m_Buff = Buff;
    }

    public bool isBuff()
    {
        return m_Buff;
    }
    public bool canBuff()
    {
        return m_canBuff;
    }
}
