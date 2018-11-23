using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour {

    [SerializeField]
    Animator m_Animator;

    [SerializeField]
    Transform m_Rotation;

    public bool m_bStart;

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {

        if (m_bStart)
        {
            m_Rotation.localEulerAngles = Vector3.Lerp(m_Rotation.localEulerAngles, new Vector3(0, 0, 0), 0.1f);
            m_Animator.SetBool("StartBool", true);
        }
    }
}
