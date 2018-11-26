using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soil : MonoBehaviour {

    public bool m_bDead;

    [SerializeField]
    Renderer SoilRenderer;
    [SerializeField]
    Transform DeadZone;
    [SerializeField]
    ParticleSystem Dead;

    bool m_bUse;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (m_bDead)
        {
            SoilRenderer.material.color = Color.Lerp(SoilRenderer.material.color, DeadZone.GetComponent<Renderer>().material.color, 0.1f);

            if (!m_bUse)
            {
                SoundManager.Instance.PlaySE(SoundManager.SE_TYPE.VEGETABLES_EAT);
                m_bUse = true;
                //Dead.Play();
            }

            if (DeadZone.localScale.x < 0.5f)
            {
                DeadZone.localScale += new Vector3(0.05f, 0.05f, 0.05f);
            }
        }
    }
}
