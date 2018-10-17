using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseManager : MonoBehaviour {
    
    [SerializeField]
    GameObject[] m_Phase;     // フェーズ情報の代入

    GameObject m_NowPhase;
    int m_PhaseIndex;

	void Start () {
        foreach(GameObject phase in m_Phase)
        {
            phase.SetActive(false);
        }
        m_PhaseIndex = 0;
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
        m_NowPhase = m_Phase[m_PhaseIndex];
        m_NowPhase.SetActive(true);
    }
}
