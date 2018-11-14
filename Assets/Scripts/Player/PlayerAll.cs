using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerAll : MonoBehaviour {

    [SerializeField]
    NavMeshAgent m_Navigate = null;

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
    GameObject m_IventCamera1;

    int nNumber = 0;

    int index = 0;

    bool m_bIvent3;

    int m_nTime = 100;

    private void Awake()
    {
        m_Navigate.updateRotation = false;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        switch (AllManager.Instance.GetStateScene())
        {
            case AllManager.STATE_SCENE.STATE_TITLE:
                break;
        
            case AllManager.STATE_SCENE.STATE_STAGE:
                if (!m_StageManager.isGameMode()) { return; }
                if (m_NavPoint.Length - 1 < nNumber)
                {
                }
                else
                { 
                    //対象の位置の方向に移動
                    m_Navigate.SetDestination(m_NavPoint[nNumber].transform.position);

                    if (transform.position.x == m_NavPoint[nNumber].transform.position.x &&
                        transform.position.z == m_NavPoint[nNumber].transform.position.z)
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
                            if (m_PhaseManager.GetNowPhaseIndex() == 0)
                            {
                                if (!m_bIvent3)
                                {
                                    m_NarrationObject3.SetActive(true);

                                    if (m_nTime > 0)
                                    {
                                        m_nTime--;
                                    }
                                    else if (m_nTime <= 0)
                                    {
                                        m_IventCamera1.SetActive(false);
                                        m_bIvent3 = true;

                                    }
                                }
                                m_StageObject.SetActive(true);
                            }

                            Vector();
                            if (index != m_PhaseManager.GetNowPhaseIndex())
                            {
                                nNumber++;
                            }
                        }
                        else if (m_NavPoint[nNumber].GetComponent<PlayerPoint>().GetState() == PlayerPoint.STATE.MOVE)
                        {
                            nNumber++;
                        }
                    }

                    index = m_PhaseManager.GetNowPhaseIndex();
                }
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
        transform.forward = rot * transform.forward;

        //transform.position = m_Navigate.nextPosition;
    }
}
