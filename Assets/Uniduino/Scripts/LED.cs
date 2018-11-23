using UnityEngine;
using System.Collections;
using Uniduino;

public class LED : MonoBehaviour
{
    [SerializeField]
    bool m_bArduino;

    private Arduino arduino;
    private int[] LED_Num = { 0,0};
    private int[] LED_NumOld = { -1,-1};

    void Start()
    {
        if (m_bArduino) { return; }

        arduino = Arduino.global;
    }

    // タイトルなどのLED処理
    public void Title()
    {
        if (m_bArduino) { return; }

        arduino.Wright("/");
    }
    // タイトルでゲームスタートする時のLED処理
    // ゲージセットすれば点滅終了
    public void ToGame()
    {
        if (m_bArduino) { return; }

        arduino.Wright(".");
    }
    // バズーカ発砲する時のLED処理
    // Title(),ToGame()の時に実行しないこと！
    public void Bazooka()
    {
        if (m_bArduino) { return; }

        arduino.Wright("-");
    }
    // ゲージの処理
    // PlayerIndex：プレイヤーID（0、1）
    // GageVar    ：ゲージ量（0.0～1.0）
    public void Gage(int PlayerIndex, float GageVar)
    {
        if (m_bArduino) { return; }

        LED_Num[PlayerIndex] = ( GageVar >= 1.0f) ? 33 : Mathf.RoundToInt(GageVar * 32);

        if (LED_Num[PlayerIndex] == LED_NumOld[PlayerIndex]) return;
        LED_NumOld[PlayerIndex] = LED_Num[PlayerIndex];

        char LED_Out = (PlayerIndex == 0)? '0' : 'R';
        for (int i = 0; i < LED_Num[PlayerIndex]; ++i)
        {
            LED_Out++;
        }
        arduino.Wright(LED_Out.ToString());
    }
}