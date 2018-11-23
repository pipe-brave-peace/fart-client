using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllManager : SingletonMonoBehaviour<AllManager>
{

    public enum STATE_SCENE
    {
        STATE_TITLE,
        STATE_STAGE,
        STATE_RESULT
    };

    [SerializeField]
    STATE_SCENE m_StateScene;

    [SerializeField]
    GameObject m_TitleManager;

    [SerializeField]
    GameObject m_StageManager;

    [SerializeField]
    GameObject m_ResultManager;

    private void Start()
    {
        int maxDisplayCount = 2;
        for (int i = 0; i < maxDisplayCount && i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
    }

    private void Update()
    {
        switch (m_StateScene)
        {
            case STATE_SCENE.STATE_TITLE:
                m_TitleManager.SetActive(true);
                m_StageManager.SetActive(false);
                m_ResultManager.SetActive(false);
                break;
            case STATE_SCENE.STATE_STAGE:
                m_TitleManager.SetActive(false);
                m_StageManager.SetActive(true);
                m_ResultManager.SetActive(false);
                break;
            case STATE_SCENE.STATE_RESULT:
                m_TitleManager.SetActive(false);
                m_StageManager.SetActive(false);
                m_ResultManager.SetActive(true);
                break;
        }
    }

    public STATE_SCENE GetStateScene()
    {
        return m_StateScene;
    }

    public void SetStateScene(STATE_SCENE StateScene)
    {
        m_StateScene = StateScene;
    }
}
