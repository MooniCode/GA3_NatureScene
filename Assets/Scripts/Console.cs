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
                Cursor.lockState = CursorLockMode.Locked;
                consoleWindow.SetActive(false);
                isConsoleOpen = false;
            }
            else
            {
                Cursor.lockState= CursorLockMode.None;
                consoleWindow.SetActive(true);
                isConsoleOpen = true;
            }
        }
    }
}
