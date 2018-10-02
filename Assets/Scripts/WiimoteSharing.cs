using UnityEngine;
using WiimoteApi;
using UnityEngine.UI;

public class WiimoteSharing : MonoBehaviour {
    private Wiimote wiimote;

    [SerializeField]
    Image image;

    private int counter = 0;
    private int sendWiimoteCount = 3;

    void Start()
    {
        WiimoteManager.FindWiimotes();
    }

    void Update()
    {
        if (!WiimoteManager.HasWiimote()) { return; }

        counter += 1;
        if (counter < sendWiimoteCount)
        {
            return;
        }
        counter = 0;

        wiimote = WiimoteManager.Wiimotes[0];

        int ret;
        do
        {
            ret = wiimote.ReadWiimoteData();
        } while (ret > 0);

        var keyA = wiimote.Button.a;
        var keyB = wiimote.Button.b;

        float[] pointer = wiimote.Ir.GetPointingPosition();
        var point = new Vector2(pointer[0], pointer[1]);

        image.rectTransform.anchorMax = Vector2.Lerp(image.rectTransform.anchorMax, point, 0.5f);;
        image.rectTransform.anchorMin = Vector2.Lerp(image.rectTransform.anchorMin, point, 0.5f); ;

        Debug.Log(point);
    }
}