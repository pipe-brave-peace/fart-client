using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

[RequireComponent(typeof(Life))]
[RequireComponent(typeof(Enemy_State))]
[RequireComponent(typeof(Enemy_Score))]

public class Enemy_Kuma : MonoBehaviour {
    
    [SerializeField]
    TextMesh Debug_State_Text;
    [SerializeField]
    Renderer m_Color;           // 自分の色
    [SerializeField]
    float m_EatSpeed = 1.0f;
    
    [SerializeField]
    List<GameObject> m_NavPoints;     // 移動ポイントリスト
    [SerializeField]
    List<GameObject> m_NavCrops;      // 農作物リスト
    [SerializeField]
    PhaseManager m_PhaseManager;
    [SerializeField]
    List<int> m_PhaseLife;

    private GameObject m_TargetObj;     // ターゲットオブジェクト
    private Enemy_State m_State;        // 状態
    private NavMeshAgent m_Nav;         // ナビメッシュ
    private Vector3 m_PosOld;           // 生成座標
    private Life m_Life;                // 体力

    // 初期化
    void Start()
    {
        m_Life = GetComponent<Life>();
        m_State = GetComponent<Enemy_State>();
        //m_TargetObj = GameObject.FindGameObjectWithTag("MainCamera");                         // プレイヤーを取得
        m_TargetObj = GetCrop(m_PhaseManager.GetNowPhaseIndex());
        m_Nav = GetComponent<NavMeshAgent>();               // ナビメッシュの取得
        m_PosOld = transform.position;                      // 満腹後向かう座標のセット
        // スコアセット
        Enemy_Score score = GetComponent<Enemy_Score>();
        score.SetScore(Score_List.Enemy.Kuma);
    }

    // Update is called once per frame
    void Update()
    {
        // 状態判定
        switch (m_State.GetState())
        {
            case Enemy_State.STATE.NORMAL:   // 通常
                Debug_State_Text.text = "STATE:Normal";
                m_State.SetState(Enemy_State.STATE.MOVE);
                break;

            case Enemy_State.STATE.MOVE:     // 移動
                Debug_State_Text.text = "STATE:Move";
                if (m_TargetObj == null)
                {
                    m_TargetObj = GetCrop(m_PhaseManager.GetNowPhaseIndex());
                    break;
                }
                //対象の位置の方向に移動
                MoveHoming(m_TargetObj);

                // 近い？
                if (!DistanceNoneY(m_TargetObj, 5.0f)) { break; }

                // 対象による処理
                if( m_TargetObj.tag == "Crops")
                {
                    // 食事状態に変更
                    m_State.SetState(Enemy_State.STATE.EAT);
                    break;
                }
                // 攻撃状態に変更
                m_State.SetState(Enemy_State.STATE.ATTACK);
                m_Nav.SetDestination(transform.position);       // 移動を止める

                break;
                
            case Enemy_State.STATE.EAT:      // 食べる
                Debug_State_Text.text = "STATE:食べているよ";
                if (m_TargetObj == null) { break; }

                // 農作物のライフを取得
                Life target_life = m_TargetObj.GetComponent<Life>();
                // ライフを削る
                target_life.SubLife(Time.deltaTime * m_EatSpeed);
                // 食べ終わった？
                if (target_life.GetLife() > 0) { break; }

                // 農作物を消す
                Destroy(m_TargetObj.gameObject);
                // 次の農作物を代入
                m_TargetObj = GetCrop(m_PhaseManager.GetNowPhaseIndex());
                // 移動状態へ
                m_State.SetState(Enemy_State.STATE.MOVE);
                break;

            case Enemy_State.STATE.ATTACK:      // 攻撃
                Debug_State_Text.text = "STATE:攻撃している";
                //m_TargetObj = GetCrop();
                m_State.SetState(Enemy_State.STATE.MOVE);
                break;

            case Enemy_State.STATE.DAMAGE:      // ダメージ状態
                Debug_State_Text.text = "STATE:痛えぇ！";
                // 体力を減らす
                m_Life.SubLife(1.0f);

                // 体力がなくなった？
                if (m_Life.GetLife() <= 0)
                {
                    m_State.SetState(Enemy_State.STATE.ESCAPE);     // 離脱状態へ
                }
                m_State.SetState(Enemy_State.STATE.NORMAL);     // 通常状態へ
                break;

            case Enemy_State.STATE.ESCAPE:   // 逃げる
                Debug_State_Text.text = "STATE:FadeOut";

                // 離脱の位置の方向に移動
                m_Nav.SetDestination(m_PosOld);

                // アルファ値を減らす
                Color color = m_Color.material.color;
                color.a -= 0.01f;
                m_Color.material.color = color;

                // 透明になった？
                if (color.a > 0.0f) { break; }

                // 自分を消す
                Destroy(gameObject);
                return;
        }
    }

    // Y軸無視でターゲットに向く
    void MoveHoming(GameObject Target)
    {
        if (Target == null) return;
        Vector3 target = Target.transform.position;
        target.y = transform.position.y;       // y軸無視
        m_Nav.SetDestination(target);
    }

    // Y軸無視でターゲットと近い？
    bool DistanceNoneY(GameObject Target, float var)
    {
        if (Target == null) return false;
        // Y軸無視の距離算出
        Vector2 this_pos = new Vector2(transform.position.x, transform.position.z);
        Vector2 target_pos = new Vector2(Target.transform.position.x, Target.transform.position.z);

        // 近い？
        return (Vector2.Distance(this_pos, target_pos) <= var) ? true : false;
    }

    // リストから次のポイントの取得処理
    GameObject GetNextPoint(int Phase)
    {
        if ( m_NavPoints[0] == null ) return null;
        Destroy( m_NavPoints[0].transform.gameObject );
        m_NavPoints.Remove( m_NavPoints[0] );
        //目標を配列で取得する
        return ( m_NavPoints[0] == null ) ? null : m_NavPoints[0];
    }

    // リストから次の農作物の取得処理
    GameObject GetCrop(int Phase)
    {
        for (int i = 0; i < m_NavCrops.Count(); ++i)
        {
            // このリストからにこの敵を除外
            if (m_NavCrops[i] == null)
            {
                m_NavCrops.Remove(m_NavCrops[i]);
                continue;
            }
        }
        //目標を配列で取得する
        return (m_NavCrops[0] == null) ? null : m_NavCrops[0];
    }
}
