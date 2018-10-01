using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Farm : MonoBehaviour {

    public Slider m_Farm_UI;
    public int m_Farm_HP = 100;
    public Text m_Farm_Text;

    void Start()
    {
        m_Farm_Text.text = m_Farm_HP.ToString();
    }

    void Update()
    {
    }
    
    public void SetValue( int Value)
    {
        m_Farm_HP = Value;
        m_Farm_UI.value = m_Farm_HP;
        m_Farm_Text.text = m_Farm_HP.ToString();
    }

    public void Damage(int Value)
    {
        m_Farm_HP -= Value;
        if (m_Farm_HP < 0)
        {
            m_Farm_HP = 0;
            return;
        }
        m_Farm_UI.value = m_Farm_HP;
        m_Farm_Text.text = m_Farm_HP.ToString();
    }

}
