using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameClear : MonoBehaviour {

    [SerializeField]
    StageManager m_StageManager;    // シーン遷移用
    [SerializeField]
    GameObject m_Background;        // 背景
    [SerializeField]
    Vector2 m_BG_SizeMove;          // 背景を現すスピード
    [SerializeField]
    Text m_Text;                    // 文字

    Vector3 m_BG_Size;      // 背景サイズ
    float m_FontSize;       // 文字サイズ
    int m_Mode;             // モード

    // Use this for initialization
    void Start ()
    {
        SoundManager.Instance.PlaySE(SoundManager.SE_TYPE.GAME_CLEAR);

        // 背景の初期化
        m_BG_Size.x = 0.0f;
        m_BG_Size.y = 0.5f;
        m_BG_Size.z = 1.0f;
        m_Background.transform.localScale = m_BG_Size;

        // フォントの初期化
        Color color = m_Text.color;
        color.a = 0.0f;
        m_Text.color = color;
        m_Text.GetComponent<TypefaceAnimator>().enabled = false;

        // アニメション順番の初期化
        m_Mode = 0;

        // 他のUIを消す
        GameObject[] game_UI = GameObject.FindGameObjectsWithTag("GameUI");
        foreach (GameObject UI in game_UI)
        {
            Destroy(UI);
        }
    }
	
	// Update is called once per frame
	void Update () {
        switch (m_Mode)
        {
            case 0:     // 背景幅を徐々に伸ばす
                // キー押した？
                InputAnyKey();

                // 背景処理
                m_BG_Size.x = Mathf.Min(m_BG_Size.x + m_BG_SizeMove.x, 1.0f);
                m_Background.transform.localScale = m_BG_Size;

                // 一定の幅が来た？
                if (m_BG_Size.x >= 0.5f)
                {
                    m_Mode++;
                }
                break;

            case 1:     // 背景を徐々に現す
                // キー押した？
                InputAnyKey();

                // 背景処理
                m_BG_Size.x = Mathf.Min(m_BG_Size.x + m_BG_SizeMove.x, 1.0f);
                m_BG_Size.y = Mathf.Min(m_BG_Size.y + m_BG_SizeMove.y, 1.0f);
                m_Background.transform.localScale = m_BG_Size;

                // 背景が全部現した？
                if (m_BG_Size.y >= 1.0f)
                {
                    m_Mode++;

                    // 文字処理
                    Color color = m_Text.color;
                    color.a = 1.0f;
                    m_Text.color = color;

                    // アニメションを有効
                    m_Text.GetComponent<TypefaceAnimator>().enabled = true;
                }
                break;
            case 2:
                // キー押した？
                if (!Input.anyKeyDown) { return; }

                // アニメション完結した？
                if (m_Text.GetComponent<TypefaceAnimator>().progress >= 1.0f)
                {
                    // シーン遷移
                    m_StageManager.ModeToResult();
                }

                // アニメションを完結
                m_Text.GetComponent<TypefaceAnimator>().progress = 1.0f;
                break;
        }

        // テスト（アニメションの再スタート）
        TypefaceAnimator restart = m_Text.GetComponent<TypefaceAnimator>();
        if( Input.GetKeyDown(KeyCode.R))
        {
            restart.Play();
        }
	}

    // キー押したら全てを表示
    void InputAnyKey()
    {
        // キー押した？
        if (!Input.anyKeyDown) { return; }

        // モード変更
        m_Mode = 2;

        // 背景を全部現す
        m_Background.transform.localScale = new Vector3(1.0f,1.0f,1.0f);

        // 文字を全部現す
        Color color = m_Text.color;
        color.a = 1.0f;
        m_Text.color = color;

        // アニメションを有効し完結
        m_Text.GetComponent<TypefaceAnimator>().enabled = true;
        m_Text.GetComponent<TypefaceAnimator>().progress = 1.0f;
    }
}
