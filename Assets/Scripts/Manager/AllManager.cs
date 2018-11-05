using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllManager : MonoBehaviour {

    public enum STATE_SCENE
    {
        STATE_TITLE,
        STATE_STAGE,
        STATE_RESULT
    };

    [SerializeField]
    STATE_SCENE m_StateScene;


    public STATE_SCENE GetStateScene()
    {
        return m_StateScene;
    }

    public void SetStateScene(STATE_SCENE StateScene)
    {
        m_StateScene = StateScene;
    }
}
