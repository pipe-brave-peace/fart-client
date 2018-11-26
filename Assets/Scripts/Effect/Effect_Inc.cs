using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Effect_Inc : MonoBehaviour {

    [SerializeField]
    Sprite[] m_ImageList;       // テクスチャ素材
    [SerializeField]
    private float m_Timer;          // カウンター
    [SerializeField]
    private float m_MoveAlpha;

    private RectTransform m_Image;  // 位置、サイズ更新用
    private Image m_ImageColor;     // 色、テクスチャ更新

    private Vector2 m_Size;         // サイズ
    private Vector3 m_Pos;          // 座標
    private Color m_Color;          // 色
    private int m_Mode;             // モード

    // Use this for initialization
    void Start () {
        int ImageID = Mathf.RoundToInt( Random.Range(0, m_ImageList.Length));
        m_Image = GetComponent<RectTransform>();
        m_ImageColor = GetComponent<Image>();

        m_ImageColor.sprite = m_ImageList[ImageID];       // テクスチャの挿入
        m_Image.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        m_Size = m_Image.sizeDelta;                       // サイズの記憶
        m_Pos = m_Image.transform.position;               // 座標の記憶
        m_Image.sizeDelta = m_Size * 0.8f;                // 初期サイズの代入
        m_Color = m_ImageColor.color;                     // 色の記憶

        SoundManager.Instance.PlaySE(SoundManager.SE_TYPE.BUG_ATTACK);
    }
	
	// Update is called once per frame
	void Update () {
        switch(m_Mode)
        {
            case 0:
                // だんだん大きく
                m_Image.sizeDelta *= 1.1f;
                // 元のサイズになった？
                if(m_Image.sizeDelta.x >= m_Size.x)
                {
                    m_Image.sizeDelta = m_Size;
                    m_Mode++;       // 次へ
                }
                break;
            case 1:
                // サイズの更新
                m_Size.y += 1.0f;
                m_Size.x -= 0.1f;
                // 座標の更新
                m_Pos.y -= 0.5f;
                // 数値の代入
                m_Image.sizeDelta = m_Size*0.99f;
                m_Image.transform.position = m_Pos;

                // カウント
                m_Timer -= Time.deltaTime;
                if(m_Timer <= 0.0f)
                {
                    m_Mode++;   // 次へ
                }
                break;
            case 2:
                // サイズの更新
                m_Size.y += 1.0f;
                m_Size.x -= 0.1f;
                // 座標の更新
                m_Pos.y -= 0.5f;
                // 数値の代入
                m_Image.sizeDelta = m_Size * 0.99f;
                m_Image.transform.position = m_Pos;

                // アルファ値の更新
                m_Color.a = Mathf.Max(0.0f, m_Color.a - m_MoveAlpha);
                m_ImageColor.color = m_Color;
                // 透明になった？
                if (m_Color.a <= 0.0f)
                {
                    Destroy(gameObject);    // 終了
                    return;
                }
                break;
        }
    }
}
