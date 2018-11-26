using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Cinemachine;

public class PlayerAll : MonoBehaviour {

    public NavMeshAgent m_Navigate = null;

    [SerializeField]
    GameObject[] m_NavPoint;

    [SerializeField]
    GameObject[] m_rotPoint;

    [SerializeField]
    PhaseManager m_PhaseManager;

    [SerializeField]
    StageManager m_StageManager;

    [SerializeField]
    GameObject m_StageObject;

    [SerializeField]
    GameObject m_NarrationObject3;

    [SerializeField]
    GameObject m_NarrationObject4;

    [SerializeField]
    GameObject m_NarrationObject5;

    [SerializeField]
    GameObject m_IventCamera1;

    [SerializeField]
    GameObject m_IventCamera2;

    [SerializeField]
    UI_IventAll[] m_IventAll;

    [SerializeField]
    Buster[] m_Buster;

    [SerializeField]
    GameObject[] m_Operation;

    [SerializeField]
    int[] m_fPointMove;

    [SerializeField]
    GameObject[] m_PlayerModelObject;

    PlayerModel[] m_PlayerModel;

    [SerializeField]
    GameObject[] m_BusterObject;

    [SerializeField]
    GameObject[] m_Reticle;

    [SerializeField]
    GameObject[] m_BossObject;

    [SerializeField]
    UI_Operation_Text[] m_LastUI;

    [SerializeField]
    PlayerWalk m_PlayerWalk;

    [SerializeField]
    Phase1 m_Phase;

    [SerializeField]
    CinemachineBrain m_Cinema;

    public bool m_bTankMax;

    public bool m_bLastBuster;

    public int nNumber = 0;

    int index = 0;

    public bool m_bIvent3;

    bool m_bIvent4;

    public bool m_bIvent5;

    int m_nTime = 150;

    int m_nOldTime;

    [SerializeField]
    bool[] m_bBossForcus;

    private void Awake()
    {
        m_Navigate.updateRotation = false;

        m_nOldTime = m_nTime;
    }

    // Use this for initialization
    void Start () {

        //m_PlayerModel[0] = m_PlayerModelObject[0].GetComponent<PlayerModel>();
        //m_PlayerModel[1] = m_PlayerModelObject[1].GetComponent<PlayerModel>();
    }
	
	// Update is called once per frame
	void Update () {


        switch (AllManager.Instance.GetStateScene())
        {
            case AllManager.STATE_SCENE.STATE_TITLE:
                break;
        
            case AllManager.STATE_SCENE.STATE_STAGE:

                if (m_BossObject[2].GetComponent<Enemy_Boss_Mix_Kuma>().m_isLastAttack)
                {
                    if (!m_bTankMax)
                    {
                        m_LastUI[1].m_bOnText = false;
                        m_LastUI[0].m_bOnText = true;
                    }
                }

                if (m_bTankMax && m_PhaseManager.GetNowPhaseIndex() == 6)
                {
                    m_LastUI[1].m_bOnText = true;
                    m_LastUI[0].m_bOnText = false;
                }

                if (m_bLastBuster)
                {
                    m_LastUI[1].m_bOnText = false;
                    m_LastUI[0].m_bOnText = false;
                }

                if (!m_PlayerModelObject[0].GetComponent<PlayerModel>().m_bStart || !m_PlayerModelObject[1].GetComponent<PlayerModel>().m_bStart)
                {
                    m_PlayerModelObject[0].GetComponent<PlayerModel>().m_bStart = true;
                    m_PlayerModelObject[1].GetComponent<PlayerModel>().m_bStart = true;
                }

                if (m_Cinema.IsBlending)
                {
                    if (m_Cinema.ActiveBlend.CamA.VirtualCameraGameObject.name == "CM vcam2")
                    {
                        if (m_Cinema.ActiveBlend.BlendWeight >= 0.5f)
                        {
                            m_PlayerModelObject[0].GetComponent<PlayerModel>().FadeOutModel(0.05f);
                            m_PlayerModelObject[1].GetComponent<PlayerModel>().FadeOutModel(0.05f);
                        }
                    }
                }


                if (!m_StageManager.isGameMode()) { return; }

                if (nNumber == 12)
                {
                    if (m_Cinema.IsBlending)
                    {
                        if (m_Cinema.ActiveBlend.CamB.VirtualCameraGameObject.name == "CM vcam1")
                        {
                            if (m_Cinema.ActiveBlend.BlendWeight >= 0.5f)
                            {
                                m_PlayerModelObject[0].GetComponent<PlayerModel>().FadeOutModel(0.05f);
                                m_PlayerModelObject[1].GetComponent<PlayerModel>().FadeOutModel(0.05f);
                            }
                            if (m_Cinema.ActiveBlend.BlendWeight >= 0.9f)
                            {
                                m_BusterObject[0].SetActive(true);
                                m_BusterObject[1].SetActive(true);
                                m_Buster[0].BusterSet(true);
                                m_Buster[1].BusterSet(true);
                            }
                        }
                        else if (m_Cinema.ActiveBlend.CamB.VirtualCameraGameObject.name == "CM vcam7")
                        {
                            m_Buster[0].BusterSet(false);
                            m_Buster[1].BusterSet(false);
                            m_BusterObject[0].SetActive(false);
                            m_BusterObject[1].SetActive(false);
                            m_PlayerModelObject[0].GetComponent<PlayerModel>().FadeInModel(1);
                            m_PlayerModelObject[1].GetComponent<PlayerModel>().FadeInModel(1);
                        }
                    }
                }
                else
                {
                    m_PlayerModelObject[0].GetComponent<PlayerModel>().FadeOutModel(1);
                    m_PlayerModelObject[1].GetComponent<PlayerModel>().FadeOutModel(1);
                }

                if (nNumber == 0)
                {
                    m_BusterObject[0].SetActive(true);
                    m_BusterObject[1].SetActive(true);
                }

                if (m_Buster[0].m_bBuster)
                {
                    if (m_Buster[1].m_bBuster)
                    {
                        m_bLastBuster = true;
                    }
                }

                if (m_Buster[1].m_bBuster)
                {
                    if (m_Buster[0].m_bBuster)
                    {
                        m_bLastBuster = true;
                    }
                }
               //対象の位置の方向に移動
               m_Navigate.SetDestination(m_NavPoint[nNumber].transform.position);
               m_Navigate.speed = m_fPointMove[nNumber];

                if (m_fPointMove[nNumber] == 2)
                {
                    m_PlayerWalk.m_fValue = 0.01f;
                }
                else if (m_fPointMove[nNumber] == 4)
                {
                    m_PlayerWalk.m_fValue = 0.05f;
                }
                else if (m_fPointMove[nNumber] == 10)
                {
                    m_PlayerWalk.m_fValue = 0.1f;
                }

                if (m_NavPoint[nNumber].GetComponent<PlayerPoint>().GetState() == PlayerPoint.STATE.STOP ||
                       m_NavPoint[nNumber].GetComponent<PlayerPoint>().GetState() == PlayerPoint.STATE.ROTSTOP ||
                    m_NavPoint[nNumber].GetComponent<PlayerPoint>().GetState() == PlayerPoint.STATE.CORNER)
                { 
                   // m_Navigate.autoBraking = true;
                }
                else if (m_NavPoint[nNumber].GetComponent<PlayerPoint>().GetState() == PlayerPoint.STATE.MOVE)
                {
                        //m_Navigate.autoBraking = false;
                }

                if (!m_Navigate.pathPending && m_Navigate.remainingDistance <= 0)
               {
                   if (m_NavPoint[nNumber].GetComponent<PlayerPoint>().GetChangePhase())
                   {
                       m_PhaseManager.Play();
                   }
                   else
                   {
                       m_PhaseManager.Stop();
                   }

                   if (m_NavPoint[nNumber].GetComponent<PlayerPoint>().GetState() == PlayerPoint.STATE.STOP ||
                       m_NavPoint[nNumber].GetComponent<PlayerPoint>().GetState() == PlayerPoint.STATE.ROTSTOP)
                   {
                        m_PlayerWalk.m_bStop = true;

                        if (m_PhaseManager.GetNowPhaseIndex() == 0)
                       {
                            m_StageObject.SetActive(true);

                            if (!m_bIvent3)
                            {
                                m_NarrationObject3.SetActive(true);

                                m_Reticle[0].SetActive(false);
                                m_Reticle[1].SetActive(false);

                                m_IventAll[0].SetIventFlg(true);
                                m_IventAll[1].SetIventFlg(true);

                                if (m_nTime > 0)
                                {
                                    m_nTime--;
                                }
                                else if (m_nTime <= 0)
                                {
                                    m_Reticle[0].SetActive(true);
                                    m_Reticle[1].SetActive(true);
                                    m_Operation[2].SetActive(true);
                                    m_nTime = m_nOldTime;
                                    m_IventAll[0].SetIventFlg(false);
                                    m_IventAll[1].SetIventFlg(false);
                                    m_IventCamera1.SetActive(false);
                                    m_bIvent3 = true;

                                }
                            }

                            if (!m_bIvent5)
                            {
                                if (m_bTankMax)
                                {
                                   for (int i = 0; i < m_Phase.GetEnemy().Count; i++)
                                   {
                                       GameObject enemy = m_Phase.GetEnemy()[i].Enemy;
                                       enemy.transform.GetChild(0).GetComponent<Enemy_Eat_Inago>().m_isStop = false;
                                   }

                                    m_Operation[2].SetActive(false);
                                    m_Operation[0].SetActive(true);
                                    m_NarrationObject5.SetActive(true);
                                    m_bTankMax = false;
                                    m_bIvent5 = true;
                                }
                                else
                                {
                                    if (m_Phase != null)
                                    {
                                        for (int i = 0; i < m_Phase.GetEnemy().Count; i++)
                                        {
                                            GameObject enemy = m_Phase.GetEnemy()[i].Enemy;
                                            enemy.transform.GetChild(0).GetComponent<Enemy_Eat_Inago>().m_isStop = true;
                                        }
                                    }
                                }
                            }
                       }

                       if (m_PhaseManager.GetNowPhaseIndex() == 1)
                       {
                            m_bTankMax = false;
                            if (!m_bIvent4)
                            {
                                m_Operation[0].SetActive(false);

                                m_Reticle[0].SetActive(false);
                                m_Reticle[1].SetActive(false);

                                m_IventAll[0].SetIventFlg(true);
                                m_IventAll[1].SetIventFlg(true);

                                m_NarrationObject4.SetActive(true);

                                if (m_nTime > 0)
                                {
                                    m_nTime--;
                                }
                                else if (m_nTime <= 0)
                                {
                                    m_Reticle[0].SetActive(true);
                                    m_Reticle[1].SetActive(true);

                                    m_Operation[1].SetActive(true);

                                    m_IventAll[0].SetIventFlg(false);
                                    m_IventAll[1].SetIventFlg(false);

                                    m_nTime = m_nOldTime;
                                    m_IventCamera2.SetActive(false);
                                    m_bIvent4 = true;

                                }
                            }
                           m_StageObject.SetActive(true);
                       }
                       else
                       {
                           m_Operation[1].SetActive(false);
                       }

                       Vector();
                       if (index != m_PhaseManager.GetNowPhaseIndex())
                       {
                           if (m_PhaseManager.GetNowPhaseIndex() > 1)
                           {
                               nNumber++;
                           }
                       }
                   }
                   else if (m_NavPoint[nNumber].GetComponent<PlayerPoint>().GetState() == PlayerPoint.STATE.MOVE ||
                        m_NavPoint[nNumber].GetComponent<PlayerPoint>().GetState() == PlayerPoint.STATE.CORNER)
                   {

                        if (m_NavPoint[nNumber].GetComponent<PlayerPoint>().GetState() == PlayerPoint.STATE.CORNER)
                        {
                            m_PlayerWalk.m_bStop = true;
                        }

                        Vector();
                        if (m_PhaseManager.GetNowPhaseIndex() > 1)
                       {
                           nNumber++;
                       }
                   }
               }
               else
               {
                    m_PlayerWalk.m_bStop = false;

                    if (m_NavPoint[nNumber].GetComponent<PlayerPoint>().GetState() == PlayerPoint.STATE.MOVE ||
                       m_NavPoint[nNumber].GetComponent<PlayerPoint>().GetState() == PlayerPoint.STATE.ROTSTOP ||
                        m_NavPoint[nNumber].GetComponent<PlayerPoint>().GetState() == PlayerPoint.STATE.CORNER)
                   {
                       Vector();
                   }
               }

               index = m_PhaseManager.GetNowPhaseIndex();
            
                break;
        
            case AllManager.STATE_SCENE.STATE_RESULT:
                break;
        }
    }

    void Vector()
    {

        // 次の位置への方向を求める
        var dir = m_rotPoint[nNumber].transform.position - transform.position;
          

        // 方向と現在の前方との角度を計算（スムーズに回転するように係数を掛ける）
        float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.5f);
        var angle = Mathf.Acos(Vector3.Dot(transform.forward, dir.normalized)) * Mathf.Rad2Deg * smooth;

        // 回転軸を計算
        var axis = Vector3.Cross(transform.forward, dir);

        // 回転の更新
        var rot = Quaternion.AngleAxis(angle, axis);

        if (rot * transform.forward != Vector3.zero)
        {
            transform.forward = rot * transform.forward;
        }

        transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);

        //transform.position = m_Navigate.nextPosition;
    }
}
