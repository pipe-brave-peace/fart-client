using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_IventFlame : MonoBehaviour {

    [SerializeField]
    RectTransform rect;

    [SerializeField]
    int MovePosY;

    Vector3 OnIventPos;

    Vector3 OffIventPos;

    // Use this for initialization
    void Start()
    {
        OnIventPos = new Vector3(rect.localPosition.x, rect.localPosition.y + MovePosY, rect.localPosition.z);
        OffIventPos = new Vector3(rect.localPosition.x, rect.localPosition.y, rect.localPosition.z);
    }

    public void IventOnMove()
    {
        rect.localPosition = Vector3.Lerp(rect.localPosition, OnIventPos, 0.25f);
    }

    public void IventOffMove()
    {
        rect.localPosition = Vector3.Lerp(OffIventPos, rect.localPosition, 0.25f);
    }
}
