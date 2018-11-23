using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Operation1_Smoke : MonoBehaviour {

    [SerializeField]
    RectTransform rect;

    [SerializeField]
    Image[] m_ImageSmoke;

    [SerializeField]
    Vector3 MovePos;

    Vector3 OnIventPos;
    Vector3 OffIventPos;

    [SerializeField]
    Color[] alpha;

    public bool m_bPos;

    // Use this for initialization
    void Start () {
        OnIventPos = new Vector3(rect.localPosition.x + MovePos.x, rect.localPosition.y + MovePos.y, rect.localPosition.z + MovePos.z);
        OffIventPos = new Vector3(rect.localPosition.x, rect.localPosition.y, rect.localPosition.z);

        for (int i = 0; i < alpha.Length; i++)
        {
            alpha[i] = m_ImageSmoke[i].color;
            m_ImageSmoke[i].color = new Color(m_ImageSmoke[i].color.r, m_ImageSmoke[i].color.g, m_ImageSmoke[i].color.b, 0);
        }
    }

    public void IventOnMove()
    {
        rect.localPosition = Vector3.Lerp(rect.localPosition, OnIventPos, 0.25f);

        for (int i = 0; i < alpha.Length; i++)
        {
            m_ImageSmoke[i].color = Color.Lerp(m_ImageSmoke[i].color, alpha[i], 0.25f);
        }

        if (rect.localPosition.y >= OnIventPos.y - 2 )
        {
            m_bPos = true;
        }
    }

    public void IventOffMove()
    {
        rect.localPosition = OffIventPos;

        for (int i = 0; i < alpha.Length; i++)
        {
            m_bPos = false;
            m_ImageSmoke[i].color = new Color(m_ImageSmoke[i].color.r, m_ImageSmoke[i].color.g, m_ImageSmoke[i].color.b, 0);
        }
    }
}
