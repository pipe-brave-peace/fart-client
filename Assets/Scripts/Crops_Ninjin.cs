using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crops_Ninjin : MonoBehaviour {

    [SerializeField]
    int m_Life = 5;
    [SerializeField]
    float m_DamageTime = 2.0f;
    [SerializeField]
    TextMesh Debug_HP_Text;      // 残りの農作物
    [SerializeField]
    float m_DamageCnt = 0.0f;

    // Use this for initialization
    void Start () {
        Debug_HP_Text.text = m_Life.ToString();
    }
	
	// Update is called once per frame
	void Update () {
	}

    // ダメージ処理
    public void Damage(int Damage)
    {
        m_Life -= Damage;
    }

    // 当たり続けるの時
    void OnTriggerStay(Collider other)
    {
        // 獣に侵入された？
        if (other.gameObject.tag == "Enemy")
        {
            // ダメージ時間のカウント
            m_DamageCnt += Time.deltaTime;
            // カウント到達した？
            if(m_DamageCnt >= m_DamageTime)
            {
                m_Life--;               // ライフを減る
                m_DamageCnt = 0.0f;     // カウントの初期化
                Debug_HP_Text.text = m_Life.ToString();     // HP表示

                // 死亡した？
                if (m_Life <= 0)
                {

                    Destroy(this.gameObject);
                }
            }
        }
    }
}
