using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase : MonoBehaviour {

    // 農作物
    public Crops m_Crops;
    
    // 現在の農作物の数の取得
    public int GetCropsChildCount()
    {
        return m_Crops.GetChildCount();
    }
    // 最初の農作物の数の取得
    public int GetMaxCropsChildCount()
    {
        return m_Crops.GetMaxChildCount();
    }
}
