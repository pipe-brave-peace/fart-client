using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

[RequireComponent(typeof(Life))]
[RequireComponent(typeof(Enemy_State))]
[RequireComponent(typeof(Enemy_Score))]

public class Enemy_Boss_Eat_Kuma : MonoBehaviour {

    private const int FOOTSTEP_SOUND_TIME = 50;
    private const float FEAR_TIME = 5.0f;
    private const float CRY_TIME = 1.0f;

    [SerializeField]
    GameObject m_PlayerObj;
    //[SerializeField]
    //TextMesh Debug_State_Text;
    [Header("吼える場所")]
    [SerializeField]
    GameObject CryPoint;
    [Header("農作物を荒らすスピード")]
    [SerializeField]
    float m_EatSpeed = 1.0f;
    [Header("農作物の優先順位")]
    [SerializeField]
    List<GameObject> m_NavCrops;    // 農作物リスト
    [Header("逃げるまでの時間")]
    [SerializeField]
    float m_EscapeTimer;
    [Header("逃げる時の目的地")]
    [SerializeField]
    GameObject m_FadePoint;         // 退却ポイント

    [Header("以下編集しないこと！")]
    [SerializeField]
    SkinnedMeshRenderer m_Color;        // 自分の色
    [SerializeField]
    GameObject m_DamageEffect;      // 弾の爆発エフェクト
    [SerializeField]
    GameObject m_EscapeEffect;      // 退却時汗のエフェクト
    [SerializeField]
    GameObject m_BuffEffect;        // オナラ匂いのエフェクト

    private Enemy_State     m_State;            // 状態
    private NavMeshAgent    m_Nav;              // ナビメッシュ
    private GameObject      m_TargetObj;        // 移動目標
    private Vector3         m_FadePos;          // 退却時向かうの座標
    private int             m_CropIndex;        // 農作物リストのインデックス
    private Animator        m_Animator;         // アニメション
    private bool            m_isCry;            // 吼えたかどうか
    private float           m_CryTimer;         // 吼えるまでのカウント
    private bool            m_isBuff;           // オナラスプレー受けたかどうか
    private float           m_FearTimer;        // 怯む時間
    private GameObject      m_CryEffect;        // 吼えるのエフェクト
    private Color m_FadeColor;    // 退却時の色の変化用

    private int m_FootStepSoundTime;
    private bool m_bCrySoundOn;
    private bool m_bConfSoundOn;

    private bool m_bSplaySound;

    // 初期化
    void Start()
    {
        // コンポーネントの取得
        m_State      = GetComponent<Enemy_State>();
        m_Nav        = GetComponent<NavMeshAgent>();
        m_Animator   = GetComponent<Animator>();

        // 変数の初期化
        m_FadeColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);      // 現在の色をセット
        m_FootStepSoundTime = 0;
        m_CropIndex     = 0;
        m_isCry         = false;
        m_CryTimer      = CRY_TIME;
        m_isBuff        = false;                 // オナラスプレーに攻撃されていない
        m_FearTimer     = FEAR_TIME;
        // 退却ポイントがない：生成座標を代入
        // 退却ポイントがある：退却ポイント座標を代入
        if (m_FadePoint == null)
        {
            m_FadePos = transform.position;
        }
        else
        {
            m_FadePos = m_FadePoint.transform.position;
        }

        // スコアセット
        Enemy_Score score = GetComponent<Enemy_Score>();
        score.SetScore(Score_List.Enemy.Kuma);

        // nullリストの除外
        for (int i = 0; i < m_NavCrops.Count; ++i)
        {
            if (m_NavCrops[i] == null)
            {
                m_NavCrops.Remove(m_NavCrops[i]);
                continue;
            }
        }
        
        // 農作物の先頭をターゲットに
        m_TargetObj = SerchCrops();
    }

    // Update is called once per frame
    void Update()
    {
        if ( !m_isCry)
        {
            // 攻撃不可
            m_State.CanSet(false);
            // 近い？
            if (DistanceNoneY(CryPoint.transform.position, 1.0f))
            {
                m_Nav.updatePosition = false;
                MoveHoming(m_PlayerObj.transform.position);
                m_CryTimer -= Time.deltaTime;
                if( m_CryTimer <= 0.0f)
                {
                    m_CryTimer = CRY_TIME;
                    m_isCry = true;
                    m_State.CanSet(true);
                    m_State.SetState(Enemy_State.STATE.CRY);
                    m_Animator.SetBool("ToCry", true);
                }
            }
            else
            {
                if (m_FootStepSoundTime > 0)
                {
                    m_FootStepSoundTime--;
                }
                else if (m_FootStepSoundTime <= 0)
                {
                    m_FootStepSoundTime = FOOTSTEP_SOUND_TIME;
                    SoundManager.Instance.PlaySE(SoundManager.SE_TYPE.BEAR_FOOTSTEP);
                }
                // 目標へ移動
                MoveHoming(CryPoint.transform.position);
            }
            return;
        }
        // 状態判定
        switch (m_State.GetState())
        {
            case Enemy_State.STATE.MOVE:     // 移動
                //Debug_State_Text.text = "STATE:Move";

                // 攻撃不可
                m_State.CanSet(false);
                m_Animator.SetBool("ToMove", false);

                // 目標がなくなった？
                if (m_TargetObj == null)
                {
                    // 再検索
                    m_TargetObj = SerchCrops(m_CropIndex);          // 農作物をサーチ
                    break;
                }
                // 目標へ移動
                MoveHoming(m_TargetObj.transform.position);

                // 近い？
                if (DistanceNoneY(m_TargetObj.transform.position, 1.0f))
                {
                    // 食べる状態に変更
                    m_State.CanSet(true);
                    m_State.SetState(Enemy_State.STATE.EAT);
                    m_Animator.SetBool("ToEat", true);
                    MoveHoming(transform.position);     // 止まる
                }
                break;

            case Enemy_State.STATE.EAT:      // 食べる
                //Debug_State_Text.text = "STATE:食べているよ";

                // 攻撃可能
                m_State.CanSet(true);

                // 食べ終わった？
                if (m_TargetObj == null)
                {
                    // 次を探す
                    m_State.SetState(Enemy_State.STATE.MOVE);
                    m_Animator.SetBool("ToEat", false);
                    break;
                }

                // 農作物体力を減らす
                Life target_life = m_TargetObj.GetComponent<Life>();
                target_life.SubLife(Time.deltaTime * m_EatSpeed);
                // 食べ終わった？
                if (target_life.GetLife() <= 0) { Destroy(m_TargetObj.gameObject); }
                break;

            case Enemy_State.STATE.CRY:
                {
                    //Debug_State_Text.text = "STATE:がおぉぉ！！！";
                    m_State.CanSet(false);

                    // 吼えるエフェクトの再生
                    if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f && m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
                    {
                        CreateCryEffect();
                    }

                    if (!m_bCrySoundOn)
                    {
                        SoundManager.Instance.PlaySE(SoundManager.SE_TYPE.BEAR_ROAR);
                        m_bCrySoundOn = true;
                    }

                    // アニメション終わった？
                    if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f && m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
                    {
                        m_State.CanSet(true);
                        m_State.SetState(Enemy_State.STATE.MOVE);
                        m_Animator.SetBool("ToCry", false);
                        m_Nav.updatePosition = true;
                    }
                    break;
                }

            case Enemy_State.STATE.SPRAY:
                //Debug_State_Text.text = "STATE:見えねぇ！！";   // テスト
                // フラグをスプレーを受けたに変更
                m_isBuff = true;

                if (!m_bSplaySound)
                {
                    SoundManager.Instance.PlaySE(SoundManager.SE_TYPE.ONARASPLAY_HIT);
                    m_bSplaySound = true;
                }

                // 匂いのエフェクトの再生
                m_BuffEffect.SetActive(true);

                // 直前が食事状態なら
                if (m_State.GetStateOld() == Enemy_State.STATE.EAT)
                {
                    // 食事を続く
                    m_State.SetState(Enemy_State.STATE.EAT);
                    break;
                }

                // 移動状態へ
                m_State.SetState(Enemy_State.STATE.MOVE);
                break;

            case Enemy_State.STATE.DAMAGE:      // ダメージ状態
                                                //Debug_State_Text.text = "STATE:痛えぇ！";

                if (!m_isBuff)
                {
                    // 直前が食事状態なら
                    if (m_State.GetStateOld() == Enemy_State.STATE.EAT)
                    {
                        // 食事を続く
                        m_State.SetState(Enemy_State.STATE.EAT);
                        break;
                    }
                    m_State.SetState(Enemy_State.STATE.MOVE);     // 移動状態へ
                    break;
                }
                // フラグをスプレーを受けてないに変更
                m_isBuff = false;

                // エフェクトの生成
                GameObject damage_effect = Instantiate(m_DamageEffect, transform.position, Quaternion.identity) as GameObject;
                // 匂いのエフェクトの再生
                m_BuffEffect.SetActive(false);

                SoundManager.Instance.PlaySE(SoundManager.SE_TYPE.BEAR_SHOUT);

                // 怯む
                m_State.SetState(Enemy_State.STATE.FEAR);
                m_Animator.SetBool("ToFear", true);
                break;

            case Enemy_State.STATE.FEAR:        // 怯む
                 //Debug_State_Text.text = "STATE:怖いよ、怖いよぉ～";

                if (!m_bConfSoundOn)
                {
                    SoundManager.Instance.PlaySE(SoundManager.SE_TYPE.BEAR_CONFUSION);
                    SoundManager.Instance.LoopSE(SoundManager.SE_TYPE.BEAR_CONFUSION);
                    m_bConfSoundOn = true;
                }

                // 攻撃不能
                m_State.CanSet(false);

                m_FearTimer -= Time.deltaTime;
                if( m_FearTimer <= 0.0f)
                {
                    m_bConfSoundOn = false;
                    SoundManager.Instance.StopSE(SoundManager.SE_TYPE.BEAR_CONFUSION);
                    m_FearTimer = FEAR_TIME;
                    m_State.CanSet(true);
                    m_State.SetState(Enemy_State.STATE.MOVE);
                    AnimatorFuraguInit();
                    m_Animator.SetBool("ToMove", true);
                    // 次の農作物を狙う
                    m_CropIndex++;
                    m_TargetObj = SerchCrops(m_CropIndex);          // 農作物をサーチ
                }

                break;

            case Enemy_State.STATE.ESCAPE:   // 逃げる
                //Debug_State_Text.text = "STATE:FadeOut";

                // 攻撃不能
                m_State.CanSet(false);

                // 汗のエフェクトを出す
                m_EscapeEffect.SetActive(true);

                // 消えていく
                m_FadeColor.a -= 0.02f;
                m_Color.material.SetColor("_MainColor", m_FadeColor);

                // 離脱の位置の方向に移動
                MoveHoming(m_FadePos);

                // 汗を止める
                if (m_FadeColor.a <= 0.3f) { m_EscapeEffect.SetActive(false); }

                // クマオブジェクトを消す
                if (DistanceNoneY(m_FadePos, 1.0f)) { Destroy(transform.parent.gameObject); }
                return;
        }

        // 時間来たら逃げる～
        m_EscapeTimer -= Time.deltaTime;
        if (m_EscapeTimer > 0.0f) return;
        if (m_State.GetState() == Enemy_State.STATE.ESCAPE) return;
        if (m_State.GetState() == Enemy_State.STATE.CRY) return;
        if (m_State.GetState() == Enemy_State.STATE.DAMAGE) return;
        if (m_State.GetState() == Enemy_State.STATE.FEAR) return;
        m_State.CanSet(true);
        m_State.SetState(Enemy_State.STATE.ESCAPE);
        m_Animator.Play("Move");
    }

    // Y軸無視でターゲットに向く
    void MoveHoming(Vector3 Target)
    {
        Target.y = transform.position.y;        // y軸無視
        m_Nav.SetDestination(Target);           // ナビメッシュ上の移動
    }

    // Y軸無視でターゲットと近い？
    bool DistanceNoneY(Vector3 Target, float var)
    {
        // Y軸無視の距離算出
        Vector2 this_pos    = new Vector2(transform.position.x, transform.position.z);
        Vector2 target_pos  = new Vector2(Target.x, Target.z);

        // 近い？
        return (Vector2.Distance(this_pos, target_pos) <= var) ? true : false;
    }

    // 農作物リストから目標を取得
    GameObject SerchCrops()
    {
        //目標を配列で取得する
        foreach (GameObject obs in m_NavCrops)
        {
            // nullじゃなかったら返す
            if (obs != null) { return obs; }
        }
        m_State.SetState(Enemy_State.STATE.ESCAPE);
        // リストがなくなったらnull
        return null;
    }
    // 農作物リストからインデックスで目標を取得
    GameObject SerchCrops(int Index)
    {
        // nullリストの除外
        for( int i = 0; i < m_NavCrops.Count; ++i)
        {
            if( m_NavCrops[i] == null)
            {
                m_NavCrops.Remove(m_NavCrops[i]);
                continue;
            }
        }
        // リストなくなった？
        if( m_NavCrops.Count <= 0)
        {
            m_State.SetState(Enemy_State.STATE.ESCAPE);
            return null;
        }
        // インデックスがリストより大きかったら、最初のオブジェクトを返す
        if ( Index >= m_NavCrops.Count ) { Index = 0; }
        m_CropIndex = Index;
        return m_NavCrops[Index];
    }
    void CreateCryEffect()
    {
        // プレハブを取得
        GameObject prefab = (GameObject)Resources.Load("Prefabs/Effect/E_BossCry");

        // プレハブからインスタンスを生成
        m_CryEffect = (GameObject)Instantiate(prefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        // 作成したオブジェクトを子として登録
        m_CryEffect.transform.parent = transform;
        m_CryEffect.transform.localPosition = new Vector3(0.0f, 1.14f, 0.3f);
        m_CryEffect.transform.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        m_CryEffect.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }
    void AnimatorFuraguInit()
    {
        m_Animator.SetBool("ToEat"      , false);
        m_Animator.SetBool("ToCry"      , false);
        m_Animator.SetBool("ToAttack"   , false);
        m_Animator.SetBool("ToFear"     , false);
        m_Animator.SetBool("ToMove"     , false);
        m_Animator.SetBool("ToDamage"   , false);
        m_Animator.SetBool("ToFaint"    , false);
    }
}
