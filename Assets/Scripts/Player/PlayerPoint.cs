using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPoint : MonoBehaviour {

    public enum STATE
    {
        MOVE,       // 移動
        CORNER,       // 角
        STOP,       //停止
        ROTSTOP,
        MAX
    }


    [SerializeField]
    STATE m_state;

    [SerializeField]
    bool m_ChangePhase;

    public STATE GetState() { return m_state; }
    public void SetState(STATE state) { m_state = state; }

    public bool GetChangePhase() { return m_ChangePhase; }
}
