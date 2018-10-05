using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour {

    [SerializeField]
    Vector3 m_MoveRot;
    Quaternion m_Quaternion;
    Vector3 m_Rot;

    // Use this for initialization
    void Start () {

        m_Quaternion = transform.rotation;
        m_Rot.x = m_Quaternion.eulerAngles.x;
        m_Rot.y = m_Quaternion.eulerAngles.y;
        m_Rot.z = m_Quaternion.eulerAngles.z;
    }
	
	// Update is called once per frame
	void Update () {
        m_Rot += m_MoveRot;
        transform.rotation = Quaternion.Euler(m_Rot);
    }
}
