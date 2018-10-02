using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Furz : MonoBehaviour {

    [SerializeField]
    Slider FurzUI;

    [SerializeField]
    float power = 0.0f;

    private List<Joycon> m_joycons;
    private Joycon m_joyconL;

    // Use this for initialization
    void Start () {
        m_joycons = JoyconManager.Instance.j;

        m_joyconL = m_joycons.Find(c => c.isLeft);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            FurzUI.value = 0;
        }

        if (/*m_joyconL.GetGyro().y < power ||*/ Input.GetKey(KeyCode.B))
        {
            FurzUI.value += 0.1f;
        }
    }
}
