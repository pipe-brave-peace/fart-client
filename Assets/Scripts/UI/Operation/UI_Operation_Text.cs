using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Operation_Text : MonoBehaviour
{
    [SerializeField]
    Image UI_text;

    public bool m_bOnText;

	
	// Update is called once per frame
	void Update ()
    {
        if (!m_bOnText)
        {
            UI_text.color = new Color(UI_text.color.r, UI_text.color.g, UI_text.color.b, 0);
        }
        else
        {
            UI_text.color = Color.Lerp(UI_text.color, new Color(UI_text.color.r, UI_text.color.g, UI_text.color.b, 1), 0.1f);
        }
	}
}
