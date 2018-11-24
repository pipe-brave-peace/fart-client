using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IventOn : MonoBehaviour {

    public enum IVENTSTATE
    {
        IVENT_NARRATION,
        IVENT_BOSS_CRY,
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

    [SerializeField]
    GameObject m_CryEffect;

    [SerializeField]
    int m_Time;

    [SerializeField]
    LastCamera m_LastCamera;

    [SerializeField]
    GameObject[] m_Reticle;

    bool m_bUse = false;

    bool m_bBossIventFlg;

    bool m_bBossCryFlg;

    public bool m_bBossEndFlg;

    bool m_bEffectOn;

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
                        m_Reticle[0].SetActive(false);
                        m_Reticle[1].SetActive(false);
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
                            m_Reticle[0].SetActive(true);
                            m_Reticle[1].SetActive(true);
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

            case IVENTSTATE.IVENT_BOSS_CRY:

                if (m_bBossCryFlg)
                {
                    if (m_Time > 0)
                    {
                        if (!m_bEffectOn && m_Time == 100) { SoundManager.Instance.PlaySE(SoundManager.SE_TYPE.BEAR_ROAR); m_CryEffect.SetActive(true); m_bEffectOn = true; }

                        IventCameraObject[0].SetActive(true);

                        m_Reticle[0].SetActive(false);
                        m_Reticle[1].SetActive(false);
                        m_IventAll[0].SetIventFlg(true);
                        m_IventAll[1].SetIventFlg(true);
                    }
                    else if (m_Time <= 0)
                    {
                        m_bBossCryFlg = false;
                        m_bUse = true;
                    }

                    m_Time--;
                }
                else
                {
                    if (m_bUse)
                    {
                        m_Reticle[0].SetActive(true);
                        m_Reticle[1].SetActive(true);
                        NarrationObject.SetActive(true);
                        m_IventAll[0].SetIventFlg(false);
                        m_IventAll[1].SetIventFlg(false);
                        IventCameraObject[0].SetActive(false);
                        m_bUse = false;
                    }
                }

                break;

            case IVENTSTATE.IVENT_BOSS_END:

                if (m_bBossEndFlg)
                {
                    m_Reticle[0].SetActive(false);
                    m_Reticle[1].SetActive(false);
                    m_IventAll[0].SetIventFlg(true);
                    m_IventAll[1].SetIventFlg(true);

                    m_LastCamera.gameObject.SetActive(true);

                    m_LastCamera.CameraVector(m_EnemyState.gameObject.transform.position);
                }

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

                case IVENTSTATE.IVENT_BOSS_CRY:

                    m_bBossCryFlg = true;

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
