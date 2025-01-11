using UnityEngine;


public class ScreenFadeUsage : MonoBehaviour
{
    bool flipflop;

    private void OnEnable()
    {
        if (flipflop)
        {
            this.FadeIn(2);
        }
        else
        {
            this.FadeOut(2);
        }
        flipflop = !flipflop;
    }

    private void OnDestroy()
    {
        this.FadeReset();
    }
}
