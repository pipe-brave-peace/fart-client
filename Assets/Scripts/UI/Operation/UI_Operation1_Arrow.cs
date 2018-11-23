using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Operation1_Arrow : MonoBehaviour {

    [SerializeField]
    RectTransform rect;

    [SerializeField]
    Image m_image;

    [SerializeField]
    Vector3 MovePos;

    [SerializeField]
    bool m_bColor;

    public bool m_bOnArrow;

    Vector3 OnIventPos;

    Vector3 OffIventPos;

    // Use this for initialization
    void Start()
    {
        OnIventPos = new Vector3(rect.localPosition.x + MovePos.x, rect.localPosition.y + MovePos.y, rect.localPosition.z + MovePos.z);
        OffIventPos = new Vector3(rect.localPosition.x, rect.localPosition.y, rect.localPosition.z);
    }

    private void Update()
    {
        if (m_bOnArrow)
        {
            IventOnMove();
        }
        else
        {
            IventOffMove();
        }
    }

    public void IventOnMove()
    {
        rect.localPosition = Vector3.Lerp(rect.localPosition, OnIventPos, 0.25f);

        if (!m_bColor)
        {
            if (m_image.color.r < 0.9f)
            {
                m_image.color = Color.Lerp(m_image.color, new Color(1, 1, m_image.color.b, m_image.color.a), 0.25f);
            }
            else if (m_image.color.r >= 0.9f)
            {
                m_bColor = true;
            }
        }
        else
        {
            if (m_image.color.r > 0.1f)
            {
                m_image.color = Color.Lerp(m_image.color, new Color(0, 0, m_image.color.b, m_image.color.a), 0.25f);
            }
            else if (m_image.color.r <= 0.1f)
            {
                m_bColor = false;
            }
        }

        m_image.color = Color.Lerp(m_image.color, new Color(m_image.color.r, m_image.color.g, m_image.color.b, 1), 0.25f);
    }

    public void IventOffMove()
    {
        rect.localPosition = OffIventPos;
        m_image.color = new Color(m_image.color.r, m_image.color.g, m_image.color.b, 0);
    }
}
