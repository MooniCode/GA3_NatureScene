using UnityEngine;

public class Console : MonoBehaviour
{
    [SerializeField] GameObject consoleWindow;
    public bool isConsoleOpen = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            if (consoleWindow.activeSelf)
            {
                consoleWindow.SetActive(false);
                isConsoleOpen = false;
            }
            else
            {
                consoleWindow.SetActive(true);
                isConsoleOpen = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
