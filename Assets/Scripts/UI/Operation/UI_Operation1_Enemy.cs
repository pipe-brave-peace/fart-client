using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Operation1_Enemy : MonoBehaviour {

    [SerializeField]
    RectTransform rect;

    [SerializeField]
    GameObject Damege;

    [SerializeField]
    float StartRot;

    [SerializeField]
    float EndRot;

    [SerializeField]
    bool m_bUse;

    public bool m_bEnemyStop;

    public bool m_bSmokeOn;

    Vector3 OldPos;

    // Use this for initialization
    void Start () {
        OldPos = rect.localPosition;
	}
	
	// Update is called once per frame
	void Update () {

        if (!m_bUse)
        {
            if (StartRot < rect.rotation.z)
            {
                rect.rotation = new Quaternion(rect.rotation.x, rect.rotation.y, rect.rotation.z - 0.01f, rect.rotation.w);
            }
            else if (StartRot >= rect.rotation.z)
            {
                m_bUse = true;
            }
        }
        else
        {
           if (EndRot > rect.rotation.z)
           {
               rect.rotation = new Quaternion(rect.rotation.x, rect.rotation.y, rect.rotation.z + 0.01f, rect.rotation.w);
           }
           else if (EndRot <= rect.rotation.z)
           {
               m_bUse = false;
           }
        }

        if (!m_bEnemyStop)
        {
            if (rect.localPosition.x < 0)
            {
                rect.localPosition += new Vector3(6, 0, 0);
            }
            else if (rect.localPosition.x >= 0)
            {
                m_bEnemyStop = true;
            }
        }

        if (m_bSmokeOn)
        {
            Damege.SetActive(true);

            if (rect.rotation.y <= 1)
            {
                rect.rotation = new Quaternion(rect.rotation.x, -180, 0, rect.rotation.w);
            }

            if (rect.localPosition.x > OldPos.x)
            {
                rect.localPosition -= new Vector3(6, 0, 0);
            }
            else if (rect.localPosition.x <= OldPos.x)
            {
                Damege.SetActive(false);
                m_bSmokeOn = false;
                m_bEnemyStop= false;
                rect.rotation = new Quaternion(rect.rotation.x, 0, 0, rect.rotation.w);
            }
        }

	}
}
