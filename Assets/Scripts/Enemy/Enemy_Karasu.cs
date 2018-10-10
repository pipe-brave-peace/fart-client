using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Karasu : MonoBehaviour {

    [SerializeField]
    TextMesh Debug_Mode_Text;
    [SerializeField]
    GameObject[] m_NavObj;      // 目標オブジェクト
    [SerializeField]
    float m_Speed = 0.05f;      // スピード

    private Enemy_Mode m_Mode;
    private GameObject m_TargetObj;

    // Use this for initialization
    void Start()
    {
        // ターゲットの代入
        m_TargetObj = m_NavObj[0];
        // モードの取得
        m_Mode = GetComponent<Enemy_Mode>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move;

        // 状態判定
        switch (m_Mode.GetMode())
        {
            case Enemy_Mode.MODE.NORMAL:   // 通常
                Debug_Mode_Text.text = "MODE:Normal";
                m_Mode.SetMode(Enemy_Mode.MODE.MOVE);
                break;

            case Enemy_Mode.MODE.MOVE:     // 移動
                Debug_Mode_Text.text = "MODE:Move";
                // 目標がなくなった？
                if (m_TargetObj == null)
                {
                    // 再検索
                    m_TargetObj = SerchCrops();          // 農作物をサーチ
                    break;
                }
                //対象の位置の方向を向く
                Vector3 target = m_TargetObj.transform.position;
                target.y = transform.position.y;       // y軸無視
                transform.LookAt(target);
                // 目標へ移動
                move = m_TargetObj.transform.position - transform.position;   // 目的へのベクトル
                move = move.normalized;                                             // 正規化
                transform.position += move * m_Speed;

                // 目標が農作物？
                if( m_TargetObj.tag != "Crops") { break; }
                // 近い？
                if (Vector3.Distance(transform.position, m_TargetObj.transform.position) <= 1.0f)
                {
                    // 食べる状態に変更
                    m_Mode.SetMode(Enemy_Mode.MODE.EAT);
                }
                break;

            case Enemy_Mode.MODE.EAT:      // 食べる
                Debug_Mode_Text.text = "MODE:Eat";
                // 食べ終わった？
                if (m_TargetObj == null)
                {
                    // 満腹になる
                    m_Mode.SetMode(Enemy_Mode.MODE.SATIETY);
                    // 退却座標の代入
                    m_TargetObj = m_NavObj[1];
                }
                break;

            case Enemy_Mode.MODE.SATIETY:  // 満腹
                Debug_Mode_Text.text = "MODE:満腹";
                // 目標に着いた？
                if (m_TargetObj == null)
                {
                    // 自分を消す
                    Destroy(gameObject);
                    return;
                }

                //対象の位置の方向を向く
                target = m_TargetObj.transform.position;
                target.y = transform.position.y;       // y軸無視
                transform.LookAt(target);
                // 目標へ移動
                move = m_TargetObj.transform.position - transform.position;   // 目的へのベクトル
                move = move.normalized;                                             // 正規化
                transform.position += move * m_Speed * 0.5f;
                break;

            case Enemy_Mode.MODE.ESCAPE:   // 逃げる
                Debug_Mode_Text.text = "MODE:FadeOut";
                break;
        }
    }

    // リストから目標を取得
    GameObject SerchCrops()
    {
        //目標を配列で取得する
        foreach (GameObject obs in m_NavObj)
        {
            if (obs == null) continue;
            if( obs.tag == "Crops")
            {
                return obs;
            }
        }
        // リストがなくなったらnull
        return null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Point")
        {
            Destroy(other.gameObject);
        }
    }
}
