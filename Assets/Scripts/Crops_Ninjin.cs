using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crops_Ninjin : MonoBehaviour {

    [SerializeField]
    float m_Life;
    [SerializeField]
    float m_DamageTime;
    [SerializeField]
    TextMesh Debug_HP_Text;      // 残りの農作物

    // Use this for initialization
    void Start () {
        Debug_HP_Text.text = Mathf.CeilToInt(m_Life).ToString();
    }
	
	// Update is called once per frame
	void Update () {
	}

    // 当たり続けるの時
    void OnTriggerStay(Collider other)
    {
        // 敵ではない？
        if (other.gameObject.tag != "Enemy") { return; }
        // 敵が食べる状態ではない？
        if( other.GetComponent<Enemy_Mode>().GetMode() != Enemy_Mode.MODE.EAT) { return; }

        // ダメージ時間のカウント
        m_Life -= Time.deltaTime*m_DamageTime;
        Debug_HP_Text.text = Mathf.CeilToInt(m_Life).ToString();     // HP表示

        // 死亡した？
        if (m_Life <= 0.0f)
        {
            Destroy(this.gameObject);
        }
    }
}
