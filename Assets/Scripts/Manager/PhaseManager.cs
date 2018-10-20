using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseManager : MonoBehaviour {
    
    [SerializeField]
    GameObject[] m_Phase;     // フェーズ情報の代入
    [SerializeField]
    int m_BoosAppPhase;

    GameObject m_Boos;
    GameObject m_NowPhase;
    int m_PhaseIndex;

	void Start () {
        foreach(GameObject phase in m_Phase)
        {
            phase.SetActive(false);
        }
        m_PhaseIndex = 0;

        m_Boos = GameObject.FindGameObjectWithTag("Boos");                         // Boosを取得
        m_Boos.SetActive(false);
        SetPhase(m_PhaseIndex);
    }
	
	// Update is called once per frame
	void Update () {
        if (m_PhaseIndex >= m_Phase.Length) return;
		if( m_NowPhase.transform.childCount <= 0)
        {
            m_PhaseIndex++;
            if (m_PhaseIndex >= m_Phase.Length) return;
            SetPhase(m_PhaseIndex);
        }
	}

    void SetPhase(int Index)
    {
        if( Index == m_BoosAppPhase)
        {
            m_Boos.SetActive(true);
        }
        m_NowPhase = m_Phase[m_PhaseIndex];
        m_NowPhase.SetActive(true);
    }

    public int GetNowPhaseIndex() { return m_PhaseIndex; }
}
