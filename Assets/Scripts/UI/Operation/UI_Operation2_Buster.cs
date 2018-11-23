using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Operation2_Buster : MonoBehaviour {

    [SerializeField]
    RectTransform rect;

    [SerializeField]
    RectTransform ExplosionRect;

    Vector3 OldSize;
    Vector3 OldExplosionSize;

    public bool m_bPos;

    // Use this for initialization
    void Start () {
        OldSize = rect.localScale;
        OldExplosionSize = ExplosionRect.localScale;

        rect.localScale = new Vector3(0, 0, 0);
        ExplosionRect.localScale = new Vector3(0,0,0);
    }

    public void IventOnMove() { 
        rect.localScale = Vector3.Lerp(rect.localScale, OldSize, 0.25f);
        ExplosionRect.localScale = Vector3.Lerp(ExplosionRect.localScale, OldExplosionSize, 0.25f);

        if (!m_bPos)
        {
            if (ExplosionRect.localScale.x >= OldExplosionSize.x - 0.1f)
            {
                m_bPos = true;
            }
        }
    }

    public void IventOffMove()
    {
        rect.localScale = new Vector3(0, 0, 0);
        ExplosionRect.localScale = new Vector3(0, 0, 0);

        m_bPos = false;
    }
}
