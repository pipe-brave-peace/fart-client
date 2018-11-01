using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseManager : MonoBehaviour {
    
    [SerializeField]
    GameObject[] m_Phase;     // フェーズ情報の代入
    
    GameObject m_NowPhase;
    int m_PhaseIndex;
    bool m_Use;

	void Start () {
        foreach(GameObject phase in m_Phase)
        {
            phase.SetActive(false);
        }
        m_PhaseIndex = 0;
        
        SetPhase(m_PhaseIndex);
        m_Use = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (!m_Use) return;
        if (m_PhaseIndex >= m_Phase.Length) return;
		if( m_NowPhase.transform.childCount <= 0)
        {
            m_PhaseIndex++;
            if (m_PhaseIndex >= m_Phase.Length) return;
            Destroy(m_NowPhase.gameObject);
            SetPhase(m_PhaseIndex);
        }
	}

    void SetPhase(int Index)
    {
        m_NowPhase = m_Phase[m_PhaseIndex];
        m_NowPhase.SetActive(true);
    }

    public int GetNowPhaseIndex() { return m_PhaseIndex; }
    public void Play() { m_Use = true; }
    public void Stop() { m_Use = false; }
}
