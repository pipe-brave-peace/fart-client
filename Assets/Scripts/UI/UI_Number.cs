using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Number : MonoBehaviour {
    public Sprite[] m_NumberImage;
    public List<int> m_Number;
    public void Set(int Number)
    {
        //いままで表示されてたスコアオブジェクト削除
        var objs = GameObject.FindGameObjectsWithTag("Number");
        foreach (var obj in objs)
        {
            if (0 <= obj.name.LastIndexOf("Clone"))
            {
                Destroy(obj);
            }
        }
        View(Number);
    }
    //スコアを表示するメソッド
    void View(int score)
    {
        var digit = score;
        //要素数0には１桁目の値が格納
        m_Number = new List<int>();
        while (digit != 0)
        {
            score = digit % 10;
            digit = digit / 10;
            m_Number.Add(score);
        }

        GameObject.Find("Number").GetComponent<Image>().sprite = m_NumberImage[m_Number[0]];
        for (int i = 1; i < m_Number.Count; i++)
        {
            //複製
            RectTransform scoreimage = (RectTransform)Instantiate(GameObject.Find("Number")).transform;
            scoreimage.SetParent(this.transform, false);
            scoreimage.localPosition = new Vector2(
                scoreimage.localPosition.x - scoreimage.sizeDelta.x * i,
                scoreimage.localPosition.y);
            scoreimage.GetComponent<Image>().sprite = m_NumberImage[m_Number[i]];
        }
    }
}