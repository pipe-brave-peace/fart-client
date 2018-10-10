using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crops : MonoBehaviour {

    // 最大数
    private int m_MaxChildCount = 0;

    void Start()
    {
        // 最大数の代入
        m_MaxChildCount = transform.childCount;
    }
    
    // 農作物の数を取得
    public int GetChildCount()
    {
        return transform.childCount;
    }
    // 農作物の最大数を取得
    public int GetMaxChildCount()
    {
        return m_MaxChildCount;
    }
}
