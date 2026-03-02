using UnityEngine;

public class Screenshots : MonoBehaviour
{
    public void TakeScreenshot()
    {
        ScreenCapture.CaptureScreenshot("portfolio.png");
    }
}
