//by Dante Deketele
//DAE 2023-2024

using System.Collections.Generic;
using UnityEngine;

public class ScreenShotTaker : MonoBehaviour
{
    [SerializeField] private KeyCode shootButton;

    private void Update()
    {
        if (Input.GetKeyDown(shootButton))
        {
            TakeScreenshot();
        }
    }

    static void TakeScreenshot(int index = 0)
    {
        if (Application.isPlaying)
        {
            //Make sure you have the Screenshots folder set up, the script won't generate it/
            string path = "Assets/Screenshots";
            var time = System.DateTime.Now;
            string timestamp = $"{time.Hour}{time.Minute}{time.Second}";
            string fileName = $"{path}/screenshot_{index}-{timestamp}.png";

            ScreenCapture.CaptureScreenshot(fileName);

            Debug.Log("Took screenshot: " + fileName);
        }
    }
}