using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Life : MonoBehaviour {

    public float m_Life;

    public void SetLife(float Life)
    {
        m_Life = Life;
    }

    public void SubLife( float var)
    {
        m_Life -= var;
    }

    public int GetLife()
    {
        return Mathf.CeilToInt(m_Life);
    }
}
