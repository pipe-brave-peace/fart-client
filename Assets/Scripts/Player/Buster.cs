using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WiimoteApi;

public class Buster : MonoBehaviour
{
    [SerializeField]
    Tank m_Tank = null;

    [SerializeField]
    GameObject m_ExplosionObject;

    [SerializeField]
    GameObject m_GasBulletObject;

    [SerializeField]
    float m_VecPow = 0.0f;

    [SerializeField]
    float m_FartsValue = 0.0f;

    [SerializeField]
    RawImage m_FartsUI = null;

    [SerializeField]
    Image m_ReticleUI = null;

    [SerializeField]
    WiimoteSharing m_WiimoteSharing = null;

    [SerializeField]
    int m_nTime;

    [SerializeField]
    Player m_Player;

    [SerializeField]
    GameObject BusterPoint;

    [SerializeField]
    GameObject m_BusterExplosion;

    [SerializeField]
    PlayerAll m_PlayerAll;

    [SerializeField]
    StageManager m_StageManager;

    [SerializeField]
    LED m_LED;

    public bool m_bBuster;

    int m_nOldTime;

    Vector2 m_OldPoint;

    bool m_bGasFlg = false;

    private List<Joycon> m_joycons;
    private List<Wiimote> m_wiimotes;

    private Wiimote wiimote1 = null;
    private Wiimote wiimote2 = null;

    private Joycon m_joyconR1;
    private Joycon m_joyconR2;

    Vector2 point;

    bool m_bStart;

    [SerializeField]
    int m_nStartTime;

    Quaternion OldRot;

    void Start()
    {
        point = new Vector2(0, 0);

        m_OldPoint = new Vector2(0, 0);

        m_nOldTime = m_nTime;

        m_joycons = JoyconManager.Instance.j;

        int count = 0;

        OldRot = transform.rotation;

        for (int i = 0; i < m_joycons.Count; i++)
        {
            if (!m_joycons[i].isLeft)
            {
                if (count == 0)
                {
                    m_joyconR1 = m_joycons[i];
                }
                else
                {
                    m_joyconR2 = m_joycons[i];
                }
                count++;
            }
        }
    }

    void Update()
    {

        Debug.Log(m_Tank.m_fTank);

        if (WiimoteManager.Wiimotes != null)
        {
            if (WiimoteManager.Wiimotes.Count > 0)
            {
                wiimote1 = WiimoteManager.Wiimotes[0];
                if (WiimoteManager.Wiimotes.Count > 1)
                {
                    wiimote2 = WiimoteManager.Wiimotes[1];
                }
            }

            int ret = 0;
            do
            {
                if (wiimote1 != null)
                {
                    ret = wiimote1.ReadWiimoteData();
                }
                if (wiimote2 != null)
                {
                    wiimote2.ReadWiimoteData();
                }
            } while (ret > 0);
        }


        Vector3 rayPos = new Vector3(m_ReticleUI.rectTransform.position.x, m_ReticleUI.rectTransform.position.y, m_ReticleUI.rectTransform.position.z);

        Ray ray = Camera.main.ScreenPointToRay(rayPos);

        if (m_StageManager.isGameMode())
        {
            if (transform.rotation.x > 0)
            {
                transform.Rotate(-10, 0, 0);
            }
            else if (transform.rotation.x <= 0)
            {
                m_nStartTime--;

                if (m_nStartTime <= 0)
                {
                    m_bStart = true;
                }
            }
        }

        if (m_bStart) { transform.rotation =   Quaternion.LookRotation(new Vector3(-ray.direction.x, -ray.direction.y, -ray.direction.z)); }

        if (m_Player.GetPlayerNumber() == 0)
        {
            if (wiimote1 != null)
            {
                float[] pointer = wiimote1.Ir.GetPointingPosition();
                point = new Vector2(pointer[0], pointer[1]);

                if (!m_Player.GetStan())
                {
                    if (wiimote1.Button.plus)
                    {
                        m_Tank.Farmer(0.5f);
                    }
            
                    if (wiimote1.Button.minus)
                    {
                        m_Tank.Farmer(0.1f);
                    }
            
                    if (wiimote1.Button.b)
                    {
                        if (!m_Tank.GetFurzFlg())
                        {
                            BulletShot();
                        }
                    }
            
                    if (wiimote1.Button.a)
                    {
                        GasShot();
                    }
                }
            }

            if (!m_Player.GetStan())
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (!m_Tank.GetFurzFlg())
                    {
                        BulletShot();
                    }
                }

                if (!m_bGasFlg)
                {
                    if (Input.GetKey(KeyCode.G))
                    {
                        GasShot();
                        m_bGasFlg = true;
                    }
                }

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    m_Tank.Farmer(0.1f);
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    m_Tank.Farmer(0.5f);
                }
            }

            if (Input.GetKey(KeyCode.A))
            {
                point.x -= 0.01f;
            }

            if (Input.GetKey(KeyCode.D))
            {
                point.x += 0.01f;
            }

            if (Input.GetKey(KeyCode.W))
            {
                point.y += 0.01f;
            }

            if (Input.GetKey(KeyCode.S))
            {
                point.y -= 0.01f;
            }

            if (point.x < 0)
            {
                m_ReticleUI.rectTransform.anchorMax = Vector2.Lerp(m_ReticleUI.rectTransform.anchorMax, m_OldPoint, 0.5f);
                m_ReticleUI.rectTransform.anchorMin = Vector2.Lerp(m_ReticleUI.rectTransform.anchorMin, m_OldPoint, 0.5f);
            }
            else
            {
                m_ReticleUI.rectTransform.anchorMax = Vector2.Lerp(m_ReticleUI.rectTransform.anchorMax, point, 0.5f);
                m_ReticleUI.rectTransform.anchorMin = Vector2.Lerp(m_ReticleUI.rectTransform.anchorMin, point, 0.5f);

                m_OldPoint = point;
            }

            if (m_joyconR1 != null)
            {
                if (!m_Player.GetStan())
                {
                    if (m_joyconR1.GetButtonDown(Joycon.Button.SHOULDER_1))
                    {
                        if (!m_Tank.GetFurzFlg())
                        {
                            BulletShot();
                        }
                    }
                }
            }
        }
        else if (m_Player.GetPlayerNumber() == 1)
        {
            if (wiimote2 != null)
            {
                float[] pointer = wiimote2.Ir.GetPointingPosition();
                point = new Vector2(pointer[0], pointer[1]);

                if (!m_Player.GetStan())
                {
                    if (wiimote2.Button.plus)
                    {
                        m_Tank.Farmer(0.5f);
                    }

                    if (wiimote2.Button.minus)
                    {
                        m_Tank.Farmer(0.1f);
                    }

                    if (wiimote2.Button.b)
                    {
                        if (!m_Tank.GetFurzFlg())
                        {
                            BulletShot();
                        }
                    }

                    if (wiimote2.Button.a)
                    {
                        GasShot();
                    }
                }
            }
            else
            {
                Cursor.visible = false;
                var position = Input.mousePosition;
                m_ReticleUI.rectTransform.position = position;
            }

            if (point.x < 0)
            {
                m_ReticleUI.rectTransform.anchorMax = Vector2.Lerp(m_ReticleUI.rectTransform.anchorMax, m_OldPoint, 0.5f);
                m_ReticleUI.rectTransform.anchorMin = Vector2.Lerp(m_ReticleUI.rectTransform.anchorMin, m_OldPoint, 0.5f);
            }
            else
            {
                m_ReticleUI.rectTransform.anchorMax = Vector2.Lerp(m_ReticleUI.rectTransform.anchorMax, point, 0.5f);
                m_ReticleUI.rectTransform.anchorMin = Vector2.Lerp(m_ReticleUI.rectTransform.anchorMin, point, 0.5f);

                m_OldPoint = point;
            }

            if (!m_Player.GetStan())
            {
                if (Input.GetKeyDown(KeyCode.B))
                {
                    if (!m_Tank.GetFurzFlg())
                    {
                        BulletShot();
                    }
                }

                if (!m_bGasFlg)
                {
                    if (Input.GetKey(KeyCode.N))
                    {
                        GasShot();
                        m_bGasFlg = true;
                    }
                }

                if (Input.GetKeyDown(KeyCode.C))
                {
                    m_Tank.Farmer(0.1f);
                }

                if (Input.GetKeyDown(KeyCode.V))
                {
                    m_Tank.Farmer(0.5f);
                }

                if (m_joyconR2 != null)
                {
                    if (m_joyconR2.GetButtonDown(Joycon.Button.SHOULDER_1))
                    {
                        if (!m_Tank.GetFurzFlg())
                        {
                            BulletShot();
                        }
                    }
                }
            }
        }

        if (m_bGasFlg)
        {
            m_nTime --;
        }

        if (m_nTime <= 0)
        {
            m_nTime = m_nOldTime;
            m_bGasFlg = false;
        }
    }

    //　敵を撃つ
    void BulletShot()
    {
        switch (AllManager.Instance.GetStateScene())
        {
            case AllManager.STATE_SCENE.STATE_TITLE:
                break;

            case AllManager.STATE_SCENE.STATE_STAGE:
                if (m_Tank.m_fTank > 0.6f) { return; }

                SoundManager.Instance.PlaySE(SoundManager.SE_TYPE.ONARA_BAZOOKA);

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                GameObject buster_effect = Instantiate(m_ExplosionObject, transform.position, Quaternion.identity) as GameObject;

                buster_effect.transform.parent = BusterPoint.gameObject.transform;
                buster_effect.transform.localPosition = new Vector3(0, 0, 0);
                buster_effect.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

                if (m_Player.GetPlayerNumber() == 0)
                {

                    ray = Camera.main.ScreenPointToRay(m_ReticleUI.rectTransform.position);
                }
                else if (m_Player.GetPlayerNumber() == 1)
                {
                    ray = Camera.main.ScreenPointToRay(m_ReticleUI.rectTransform.position);
                    //ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                }

                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 600f, LayerMask.GetMask("LifeReticle")))
                {
                    m_LED.Bazooka();

                    int nNumber = m_Player.GetPlayerNumber();

                    var LifeParent = hit.collider.gameObject.transform.parent.gameObject;
                    var EnemyParent = hit.collider.gameObject.transform.parent.gameObject.transform.parent.gameObject;

                    if (LifeParent.transform.childCount == 1)
                    {
                        InfoManager.Instance.AddPlayerScore(nNumber, EnemyParent.GetComponent<Enemy_Score>().GetScore());
                        InfoManager.Instance.AddPlayerCombo(nNumber);
                        InfoManager.Instance.AddPlayerEnemy(nNumber);
                    }

                    if (m_PlayerAll.m_bTankMax)
                    {
                        m_bBuster = true;
                    }
                    GameObject newExplosion = Instantiate(m_BusterExplosion, hit.collider.gameObject.transform.position, Quaternion.identity);
                    newExplosion.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                    Destroy(hit.collider.gameObject);
                }
                else if (Physics.Raycast(ray, out hit, 600f, LayerMask.GetMask("Enemy")))
                {
                    if (hit.collider.gameObject.GetComponent<Enemy_State>().GetState() != Enemy_State.STATE.ESCAPE)
                    {
                        m_LED.Bazooka();

                        int nNumber = m_Player.GetPlayerNumber();

                       InfoManager.Instance.AddPlayerScore(nNumber, hit.collider.gameObject.GetComponent<Enemy_Score>().GetScore());
                       hit.collider.gameObject.GetComponent<Enemy_State>().SetState(Enemy_State.STATE.DAMAGE);

                       if (hit.collider.gameObject.GetComponent<Life>().GetLife() <= 0)
                       {
                           InfoManager.Instance.AddPlayerCombo(nNumber);
                           InfoManager.Instance.AddPlayerEnemy(nNumber);
                       }
                    }
                }

                m_Tank.FartingFarts(-0.3f);
                break;

            case AllManager.STATE_SCENE.STATE_RESULT:
                break;
        }
    }

    void GasShot()
    {
        switch (AllManager.Instance.GetStateScene())
        {
            case AllManager.STATE_SCENE.STATE_TITLE:
                break;

            case AllManager.STATE_SCENE.STATE_STAGE:
                if (m_FartsUI.uvRect.x >= 0.99f) { return; }

                SoundManager.Instance.PlaySE(SoundManager.SE_TYPE.ONARA_SPRAY);

                m_Tank.FartingFarts(-0.01f);

                GameObject newBullet = Instantiate(m_GasBulletObject, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);

                Vector3 force = transform.forward * m_VecPow;

                newBullet.GetComponent<Gas>().SetNumber(m_Player.GetPlayerNumber());

                newBullet.GetComponent<Rigidbody>().AddForce(-force, ForceMode.Impulse);
                break;

            case AllManager.STATE_SCENE.STATE_RESULT:
                break;
        }
    }

    public void BusterSet(bool buse)
    {
        if (buse)
        {
            if (transform.rotation.x > 0)
            {
                transform.Rotate(-10, 0, 0);
            }
        }
        else
        {
            transform.rotation = OldRot;
        }
    }
}