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
        SPRAY,      // スプレー
        BACK,       // 後退
        DAMAGE,     // ダメージ
        FEAR,       // 怯む
        SATIETY,    // 満腹
        ESCAPE,     // 逃げる
        FAINT,      // 気絶
        MAX
    }

    private STATE m_State    = STATE.MOVE;
    private STATE m_StateOld = STATE.MOVE;
    private bool  m_canSet   = true;
   
    public void SetState(STATE State)
    {
        if(!m_canSet) { return; }
        if( m_State != m_StateOld) { m_StateOld = m_State; }
        m_State = State;
    }
    public void EnemySetState(STATE State)
    {
        if (m_State != m_StateOld) { m_StateOld = m_State; }
        m_State = State;
    }

    public STATE GetState()     { return m_State; }
    public STATE GetStateOld()  { return m_StateOld; }

    public void CanSet(bool CanSet) { m_canSet = CanSet; }
}
