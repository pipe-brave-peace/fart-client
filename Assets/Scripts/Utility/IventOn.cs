using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IventOn : MonoBehaviour {

    public enum IVENTSTATE
    {
        IVENT_NARRATION,
        IVENT_BOSS_START,
        IVENT_BOSS_END,
    };

    [SerializeField]
    IVENTSTATE m_IventState;

    [SerializeField]
    GameObject NarrationObject;

    [SerializeField]
    Enemy_State m_EnemyState;

    [SerializeField]
    GameObject[] IventCameraObject;

    [SerializeField]
    UI_IventAll[] m_IventAll;

    [SerializeField]
    CameraBlur m_CameraBlur;

    bool m_bUse = false;

    bool m_bBossIventFlg;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        switch (m_IventState)
        {
            case IVENTSTATE.IVENT_NARRATION:
                break;

            case IVENTSTATE.IVENT_BOSS_START:

                if (m_bBossIventFlg)
                {
                    if (m_EnemyState.GetState() == Enemy_State.STATE.CRY)
                    {
                        IventCameraObject[0].SetActive(false);
                        IventCameraObject[1].SetActive(true);
                        m_bBossIventFlg = false;
                        m_CameraBlur.m_bUse = true;
                        m_bUse = true;
                    }
                    else if (m_EnemyState.GetState() == Enemy_State.STATE.MOVE)
                    {
                        m_IventAll[0].SetIventFlg(true);
                        m_IventAll[1].SetIventFlg(true);
                        IventCameraObject[1].SetActive(false);
                    }
                }
                else
                {
                    if (m_EnemyState.GetState() == Enemy_State.STATE.MOVE)
                    {
                        if (m_bUse)
                        {
                            m_IventAll[0].SetIventFlg(false);
                            m_IventAll[1].SetIventFlg(false);

                            NarrationObject.SetActive(true);
                            m_bUse = false;
                        }
                        IventCameraObject[1].SetActive(false);
                        m_CameraBlur.m_bUse = false;
                    }
                }

                break;

            case IVENTSTATE.IVENT_BOSS_END:
                break;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            switch (m_IventState)
            {
                case IVENTSTATE.IVENT_NARRATION:

                    if (!m_bUse)
                    {
                        NarrationObject.SetActive(true);
                        m_bUse = true;
                    }

                    break;

                case IVENTSTATE.IVENT_BOSS_START:

                    m_bBossIventFlg = true;

                    break;

                case IVENTSTATE.IVENT_BOSS_END:
                    break;
            }
        }
    }
}
