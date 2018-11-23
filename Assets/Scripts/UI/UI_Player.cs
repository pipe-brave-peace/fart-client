using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Player : MonoBehaviour
{

    [SerializeField]
    RectTransform rect;

    [SerializeField]
    Vector3 MovePos;

    Vector3 OnIventPos;

    Vector3 OffIventPos;

    // Use this for initialization
    void Start()
    {
        OnIventPos = new Vector3(rect.localPosition.x + MovePos.x, rect.localPosition.y + MovePos.y, rect.localPosition.z + MovePos.z);
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