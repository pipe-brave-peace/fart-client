using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGame : MonoBehaviour {

    [SerializeField]
    GameObject m_Player;

    [SerializeField]
    PhaseManager m_PhaseManager;

    [SerializeField]
    GameObject[] m_PhaseObject;

    [SerializeField]
    GameObject[] m_PhasePoint;

    [SerializeField]
    int[] m_nWarpPoint;

    [SerializeField]
    PlayerAll m_PlayerAll;

    [SerializeField]
    GameObject m_Stage;

    [SerializeField]
    int m_nPhaseWarp;

    [SerializeField]
    StageManager m_StageManager;

    [SerializeField]
    UI_IventAll m_UI_Ivent;

    [SerializeField]
    bool m_bUse;

    // Use this for initialization
    void Start () {
        m_Stage.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {


        switch (AllManager.Instance.GetStateScene())
        {
            case AllManager.STATE_SCENE.STATE_STAGE:

                m_StageManager.SetMode(StageManager.STAGE_MODE.GAME);

                if (!m_bUse)
                {
                    m_UI_Ivent.SetIventFlg(false);

                    m_PlayerAll.m_Navigate.enabled = false;
                    m_PhaseManager.SetNowPhaseIndex(m_nPhaseWarp);
                    m_PhaseObject[m_nPhaseWarp].SetActive(true);
                    for (int i = 0; i < m_PhaseObject.Length; i++)
                    {
                        if (i < m_nPhaseWarp)
                        {
                            Destroy(m_PhaseObject[i]);
                        }
                    }
                    m_PlayerAll.nNumber = m_nWarpPoint[m_nPhaseWarp];
                    m_Player.transform.position = m_PhasePoint[m_nPhaseWarp].transform.position;
                    m_bUse = true;
                    
                    m_PlayerAll.m_Navigate.enabled = true;
                }
                else
                {
                    m_PlayerAll.m_Navigate.enabled = true;
                }

            break;
        }

     }
}
