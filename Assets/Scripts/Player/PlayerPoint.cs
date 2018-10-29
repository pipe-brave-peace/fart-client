using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPoint : MonoBehaviour {

    public enum STATE
    {
        MOVE,       // 移動
        STOP,       //停止
        MAX
    }


    [SerializeField]
    STATE m_state;

    public STATE GetState() { return m_state; }
    public void SetState(STATE state) { m_state = state; }
}
