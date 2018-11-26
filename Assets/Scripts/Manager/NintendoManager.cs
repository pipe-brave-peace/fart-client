using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEngine;
using System;
using WiimoteApi;
using WiimoteApi.Internal;
public class NintendoManager : MonoBehaviour
{

    //Unity経由でアクセス可能な設定
    public bool EnableIMU = true;
    public bool EnableLocalize = true;

    public static bool Debug_Messages = false;

    //異なるオペレーティングシステム,後続のゼロを行うかどうか
    private const ushort vendor_id = 0x57e;
    private const ushort vendor_id_ = 0x057e;
    private const ushort product_l = 0x2006;
    private const ushort product_r = 0x2007;
    private const ushort product_wiimote = 0x0306;
    private const ushort product_wiimoteplus = 0x0330;


    public List<Joycon> j; // Array of all connected Joy-Cons
    public List<Wiimote> w; // Array of all connected Joy-Cons
    static NintendoManager instance;

    private WiimoteType type;

    public static NintendoManager Instance
    {
        get { return instance; }
    }

    // Use this for initialization
    void Awake()
    {

        if (instance != null) Destroy(gameObject);
        instance = this;
        int i = 0;

        j = new List<Joycon>();
        w = new List<Wiimote>();
        bool isLeft = false;
        HIDapi.hid_init();

        IntPtr ptr = HIDapi.hid_enumerate(vendor_id, 0x0);
        IntPtr top_ptr = ptr;

        if (ptr == IntPtr.Zero)
        {
            ptr = HIDapi.hid_enumerate(vendor_id_, 0x0);
            if (ptr == IntPtr.Zero)
            {
                HIDapi.hid_free_enumeration(ptr);
                Debug.Log("No found!");
            }
        }

        hid_device_info enumerate;
        while (ptr != IntPtr.Zero)
        {
            enumerate = (hid_device_info)Marshal.PtrToStructure(ptr, typeof(hid_device_info));

            Debug.Log(enumerate.product_id);
            if (enumerate.product_id == product_l || enumerate.product_id == product_r)
            {
                if (enumerate.product_id == product_l)
                {
                    isLeft = true;
                    Debug.Log("Left Joy-Con connected.");
                }
                else if (enumerate.product_id == product_r)
                {
                    isLeft = false;
                    Debug.Log("Right Joy-Con connected.");
                }
                else
                {
                    Debug.Log("Non Joy-Con input device skipped.");
                }
                IntPtr handle = HIDapi.hid_open_path(enumerate.path);
                HIDapi.hid_set_nonblocking(handle, 1);
                j.Add(new Joycon(handle, EnableIMU, EnableLocalize & EnableIMU, 0.05f, isLeft));
                ++i;
            }
            else if (enumerate.product_id == product_wiimote || enumerate.product_id == product_wiimoteplus)
            {
                Wiimote remote = null;


                if (enumerate.product_id == product_wiimote)
                {
                    type = WiimoteType.WIIMOTE;
                    Debug.Log("Wiimote connected.");
                }
                else if (enumerate.product_id == product_wiimoteplus)
                {
                    type = WiimoteType.WIIMOTEPLUS;
                    Debug.Log("WiimotePlus connected.");
                }
                else
                {
                    Debug.Log("Non Wiimote input device skipped.");
                }

                if (remote == null)
                {
                    IntPtr handle = HIDapi.hid_open_path(enumerate.path);

                    WiimoteType trueType = type;

                    // Wii U Pro Controllers have the same identifiers as the newer Wii Remote Plus except for product
                    // string (WHY nintendo...)
                    if (enumerate.product_string.EndsWith("UC"))
                        trueType = WiimoteType.PROCONTROLLER;

                    remote = new Wiimote(handle, enumerate.path, trueType);

                    if (Debug_Messages)
                        Debug.Log("Found New Remote: " + remote.hidapi_path);

                    w.Add(remote);

                    HIDapi.hid_set_nonblocking(handle, 1);

                    remote.SendDataReportMode(InputDataType.REPORT_BUTTONS);
                    remote.SendStatusInfoRequest();

                    ++i;
                }
            }

            ptr = enumerate.next;
        }

        HIDapi.hid_free_enumeration(top_ptr);
    }

    void Start()
    {
        for (int i = 0; i < j.Count; ++i)
        {
            Debug.Log(i);
            Joycon jc = j[i];
            byte LEDs = 0x0;
            LEDs |= (byte)(0x1 << i);
            jc.Attach(leds_: LEDs);
            jc.Begin();
        }
    }

    void Update()
    {
        for (int i = 0; i < j.Count; ++i)
        {
            j[i].Update();
        }
    }

    void OnApplicationQuit()
    {
        for (int i = 0; i < j.Count; ++i)
        {
            j[i].Detach();
        }
    }
}
