using UnityEngine;
using WiimoteApi;
using UnityEngine.UI;

public class WiimoteSharing : MonoBehaviour {

    private Wiimote wiimote1;
    private Wiimote wiimote2;

    private int counter = 0;
    private int sendWiimoteCount = 3;

    void Start()
    {
        WiimoteManager.FindWiimotes();
    }

    void Update()
    {
        if (!WiimoteManager.HasWiimote()) { return; }

        wiimote1 = WiimoteManager.Wiimotes[0];
        wiimote2 = WiimoteManager.Wiimotes[1];

        int ret;
        do
        {
            ret = wiimote1.ReadWiimoteData();
            ret = wiimote2.ReadWiimoteData();
        } while (ret > 0);

    }

    public Wiimote GetWiimote(int index)
    {
        return WiimoteManager.Wiimotes[index];
    }
}