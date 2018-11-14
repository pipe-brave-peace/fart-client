using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Life : MonoBehaviour {

    [SerializeField]
    float m_Life;

    private float m_LifeMax;

    private void Start()
    {
        m_LifeMax = m_Life;
    }
    public bool isDamage()
    {
        return (m_LifeMax <= m_Life) ? false : true;
    }

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
