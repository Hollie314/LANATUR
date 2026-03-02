using UnityEngine;

public class Screenshots : MonoBehaviour
{
    void TakeScreenshot()
    {
        ScreenCapture.CaptureScreenshot("portfolio.png");
    }
}
