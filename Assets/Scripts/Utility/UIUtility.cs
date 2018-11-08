using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIUtility : MonoBehaviour
{
    [SerializeField]
    bool bFlashing = false; //点滅させるかどうか

    [SerializeField]
    bool bStretch = false;　//伸縮させるかどうか

    [SerializeField]
    float m_fNumValue = 0.0f; //点滅させる値

    [SerializeField]
    float m_fUp = 0.0f; //Upさせる値

    [SerializeField]
    float m_fDown = 0.0f;　//Downさせる値

    bool bFade = false; //どのフェードか

    bool bUp = false;　//伸びか縮みか

    [SerializeField]
    Text text = null; //テキスト

    [SerializeField]
    Outline outline = null;　//テキスト

    //---------------------------------
    //初期化処理(要はInit)
    //---------------------------------
    private void Awake()
    {
        //テキスト取得
        if (text == null)
        {
            text = GetComponent<Text>();
        }

        if (outline == null)
        {
            outline = GetComponent<Outline>();
        }
    }

    //---------------------------------
    //更新処理
    //---------------------------------
    private void Update()
    {
        //点滅するか
        if (bFlashing)
        {
            text.color = new Color(text.color.r,
                                    text.color.g,
                                    text.color.b,
                                    Flashing(text.color.a, m_fNumValue));//α値代入

            outline.effectColor = new Color(outline.effectColor.r,
                        outline.effectColor.g,
                        outline.effectColor.b,
                        Flashing(outline.effectColor.a, m_fNumValue));//α値代入
        }

        //伸縮するか
        if (bStretch)
        {
            transform.localScale = new Vector3(Scale(transform.localScale.x, m_fNumValue),
                                               Scale(transform.localScale.x, m_fNumValue),
                                               transform.localScale.z);
        }
    }

    //---------------------------------
    //UI点滅処理
    //---------------------------------
    public float Flashing(float fValue, float fNumValue)
    {
        fValue = FadeOut(fValue, fNumValue);
        fValue = FadeIn(fValue, fNumValue);

        return fValue;
    }

    //---------------------------------
    //UIフェードアウト
    //---------------------------------
    public float FadeOut(float fValue, float fNumValue)
    {
        if (!bFade)
        {
            if (fValue > 0)
            {
                fValue -= fNumValue;
            }

            if (fValue <= 0)
            {
                bFade = true;
            }
        }

        return fValue;
    }

    //---------------------------------
    //UIフェードイン
    //---------------------------------
    public float FadeIn(float fValue, float fNumValue)
    {
        if (bFade)
        {
            if (fValue < 1)
            {
                fValue += fNumValue;
            }

            if (fValue >= 1)
            {
                bFade = false;
            }
        }

        return fValue;
    }

    //---------------------------------
    //UI伸び縮み処理
    //---------------------------------
    public float Scale(float fValue, float fNumValue)
    {
        fValue = Up(fValue, fNumValue);
        fValue = Down(fValue, fNumValue);

        return fValue;
    }

    //---------------------------------
    //UI拡大
    //---------------------------------
    public float Up(float fValue, float fNumValue)
    {
        if (bUp)
        {
            if (fValue < m_fUp)
            {
                fValue += fNumValue;
            }

            if (fValue >= m_fUp)
            {
                bUp = false;
            }
        }

        return fValue;
    }

    //---------------------------------
    //UI縮小
    //---------------------------------
    public float Down(float fValue, float fNumValue)
    {
        if (!bUp)
        {
            if (fValue > m_fDown)
            {
                fValue -= fNumValue;
            }

            if (fValue <= m_fDown)
            {
                bUp = true;
            }
        }

        return fValue;
    }

}