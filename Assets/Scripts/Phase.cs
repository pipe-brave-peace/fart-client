using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Phase : MonoBehaviour {

    [System.Serializable]
    public struct ENEMY_IN_TIME
    {
        public GameObject Enemy;
        public float Timer;         // カウントタイマー
    }

    [SerializeField]
    private List< ENEMY_IN_TIME > m_Enemy;

    void Start()
    {
        // 敵を非表示する
        for (int i = 0; i < m_Enemy.Count(); ++i)
        {
            // このリストからにこの敵を除外
            if (m_Enemy[i].Enemy == null)
            {
                m_Enemy.Remove(m_Enemy[i]);
                continue;
            }
            m_Enemy[i].Enemy.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update () {
        
        for( int i = 0; i < m_Enemy.Count(); ++i)
        {
            // タイマーのカウント
            ENEMY_IN_TIME obj = m_Enemy[i];
            obj.Timer -= Time.deltaTime;
            m_Enemy[i] = obj;

            // 時間が来た？
            if (m_Enemy[i].Timer <= 0.0f)
            {
                // 敵を表示
                m_Enemy[i].Enemy.SetActive(true);
                // このリストからにこの敵を除外
                m_Enemy.Remove(m_Enemy[i]);
                continue;
            }
        }
	}
}
