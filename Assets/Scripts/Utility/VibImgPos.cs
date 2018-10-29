using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VibImgPos : MonoBehaviour {

    [SerializeField]
    Image[] m_ChildImage;
    [SerializeField]
    float m_MoveSpeed;
    [SerializeField]
    bool Loop;
    [SerializeField]
    int Cnt;

    int m_Mode;

    // Use this for initialization
    void Start()
    {
        m_Mode = 0;
        Vector3 pos = transform.position;
        pos.x -= m_MoveSpeed * 0.5f;
        pos.y -= m_MoveSpeed * 0.5f;
        transform.position = pos;
    }

    // Update is called once per frame
    void Update()
    {
        if( !Loop)
        {
            if (Cnt <= 0) return;
        }
        Vector3 pos = transform.position;
        switch(m_Mode)
        {
            case 0:
                pos.x += m_MoveSpeed;
                m_Mode++;
                break;
            case 1:
                pos.x -= m_MoveSpeed;
                m_Mode++;
                break;
            case 2:
                pos.y += m_MoveSpeed;
                m_Mode++;
                break;
            case 3:
                pos.y -= m_MoveSpeed;
                m_Mode = 0;
                Cnt--;
                break;
        }
        transform.position = pos;
        
        for (int i = 0; i < m_ChildImage.Length; ++i)
        {
            m_ChildImage[i].transform.position = pos;
        }
    }
}
