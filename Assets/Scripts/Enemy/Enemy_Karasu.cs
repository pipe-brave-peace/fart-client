using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Karasu : MonoBehaviour {

    [SerializeField]
    TextMesh Debug_State_Text;
    [SerializeField]
    GameObject[] m_NavObj;      // 目標オブジェクト
    [SerializeField]
    float m_Speed = 0.05f;      // スピード
    [SerializeField]
    float m_EatSpeed = 1.0f;      // 食べるスピード

    private Enemy_State m_State;
    private GameObject m_TargetObj;

    // Use this for initialization
    void Start()
    {
        // ターゲットの代入
        m_TargetObj = m_NavObj[0];
        // モードの取得
        m_State = GetComponent<Enemy_State>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move;

        // 状態判定
        switch (m_State.GetState())
        {
            case Enemy_State.STATE.NORMAL:   // 通常
                Debug_State_Text.text = "STATE:Normal";
                m_State.SetState(Enemy_State.STATE.MOVE);
                break;

            case Enemy_State.STATE.MOVE:     // 移動
                Debug_State_Text.text = "STATE:Move";
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
                    m_State.SetState(Enemy_State.STATE.EAT);
                }
                break;

            case Enemy_State.STATE.EAT:      // 食べる
                Debug_State_Text.text = "STATE:Eat";
                if (m_TargetObj == null)
                {
                    break;
                }
                Life target_life = m_TargetObj.GetComponent<Life>();
                target_life.SubLife(Time.deltaTime * m_EatSpeed);
                // 食べ終わった？
                if ( target_life.GetLife() <= 0)
                {
                    Destroy(m_TargetObj.gameObject);
                    // 満腹になる
                    m_State.SetState(Enemy_State.STATE.SATIETY);
                    // 退却座標の代入
                    m_TargetObj = m_NavObj[1];
                }
                break;

            case Enemy_State.STATE.SATIETY:  // 満腹
                Debug_State_Text.text = "STATE:満腹";
                // 目標に着いた？
                if (m_TargetObj == null)
                {
                    // カラスを消す
                    Destroy(gameObject.transform.parent.gameObject);
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

            case Enemy_State.STATE.ESCAPE:   // 逃げる
                Debug_State_Text.text = "STATE:FadeOut";
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
