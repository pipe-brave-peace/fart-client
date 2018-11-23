using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalk : MonoBehaviour {

    [SerializeField]
    float m_fMoveY;

    [SerializeField]
    bool m_bUse = false;

    [SerializeField]
    float m_fTime = 0.0f;

    public float m_fValue;

    public bool m_bStop;

    bool m_bFlg;

    Vector3 m_positon;

    // Use this for initialization
    void Start () {
        m_positon = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);
    }
	
	// Update is called once per frame
	void Update () {

        if (!m_bStop)
        {
            gameObject.transform.localPosition = Vector3.Lerp(new Vector3(gameObject.transform.localPosition.x, m_positon.y - m_fMoveY, gameObject.transform.localPosition.z)
                , new Vector3(gameObject.transform.localPosition.x, m_positon.y + m_fMoveY, gameObject.transform.localPosition.z), m_fTime);

            if (!m_bFlg)
            {
                m_fTime += m_fValue;
            }
            else if (m_bFlg)
            {
                m_fTime -= m_fValue;
            }

            if (m_fTime >= 1)
            {
                m_bFlg = true;
            }
            else if (m_fTime <= 0)
            {
                m_bFlg = false;
            }
        }
        else
        {
            m_fTime = 0.0f;

            m_bFlg = false;

            gameObject.transform.localPosition = Vector3.Lerp(gameObject.transform.localPosition
                , new Vector3(gameObject.transform.localPosition.x, m_positon.y, gameObject.transform.localPosition.z), 0.8f);
        }

    }
}
