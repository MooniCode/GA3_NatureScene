using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System;
using System.Text;

public class SceneLoader : MonoBehaviour
{
    // Scene names matching their indices
    private readonly string[] sceneNames = new string[]
    {
        "Empty Scene",
        "FBX Scene",
        "glTF Scene",
        "Compressed glTF Scene"
    };

    private Stopwatch loadTimer;
    private static SceneLoader instance;
    private string logFilePath;

    void Awake()
    {
        // Singleton pattern to prevent duplicates
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        loadTimer = new Stopwatch();
        InitializeLogFile();
    }

    private void InitializeLogFile()
    {
        // Create file path next to executable
        logFilePath = Application.dataPath + "/../SceneLoadTimes.txt";

        // Create header if file doesn't exist
        if (!File.Exists(logFilePath))
        {
            StringBuilder header = new StringBuilder();
            header.AppendLine("================================================================================");
            header.AppendLine("SCENE LOAD TIME ANALYSIS");
            header.AppendLine("Generated: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            header.AppendLine("================================================================================");
            header.AppendLine();

            File.WriteAllText(logFilePath, header.ToString());
            UnityEngine.Debug.Log($"[Scene Loader] Created log file at: {logFilePath}");
        }
    }

    void Update()
    {
        // Check for number key inputs
        if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
        {
            LoadSceneByIndex(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            LoadSceneByIndex(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            LoadSceneByIndex(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            LoadSceneByIndex(3);
        }
    }

    private void LoadSceneByIndex(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            StartCoroutine(LoadSceneAsync(sceneIndex));
        }
        else
        {
            UnityEngine.Debug.LogWarning($"Scene index {sceneIndex} is out of range! Total scenes in build: {SceneManager.sceneCountInBuildSettings}");
        }
    }

    private IEnumerator LoadSceneAsync(int sceneIndex)
    {
        loadTimer.Reset();
        loadTimer.Start();

        string sceneName = sceneIndex < sceneNames.Length ? sceneNames[sceneIndex] : $"Scene {sceneIndex}";
        UnityEngine.Debug.Log($"[Scene Loader] Starting to load: {sceneName} (Index: {sceneIndex})");

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        asyncLoad.allowSceneActivation = false;

        // Wait until the scene is almost loaded
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // Activate the scene
        asyncLoad.allowSceneActivation = true;

        // Wait until fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        loadTimer.Stop();

        // Log detailed information
        LogSceneLoadInfo(sceneIndex, sceneName, loadTimer.Elapsed.TotalSeconds);
    }

    private void LogSceneLoadInfo(int sceneIndex, string sceneName, double loadTime)
    {
        Scene loadedScene = SceneManager.GetActiveScene();

        // Console log
        UnityEngine.Debug.Log($"Scene Loaded: {sceneName} | Index: {sceneIndex} | Load Time: {loadTime:F3}s ({loadTime * 1000:F0}ms) | Root Objects: {loadedScene.rootCount} | Path: {loadedScene.path}");

        // Write to text file
        WriteToLogFile(sceneName, sceneIndex, loadTime, loadedScene);
    }

    private void WriteToLogFile(string sceneName, int sceneIndex, double loadTimeSeconds, Scene loadedScene)
    {
        try
        {
            StringBuilder logEntry = new StringBuilder();

            logEntry.AppendLine("--------------------------------------------------------------------------------");
            logEntry.AppendLine($"Timestamp:        {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
            logEntry.AppendLine($"Scene Name:       {sceneName}");
            logEntry.AppendLine($"Scene Index:      {sceneIndex}");
            logEntry.AppendLine($"Load Time:        {loadTimeSeconds:F4} seconds ({loadTimeSeconds * 1000:F2} ms)");
            logEntry.AppendLine($"Root Objects:     {loadedScene.rootCount}");
            logEntry.AppendLine();
            logEntry.AppendLine("Root Object Names:");

            GameObject[] rootObjects = loadedScene.GetRootGameObjects();
            for (int i = 0; i < rootObjects.Length; i++)
            {
                logEntry.AppendLine($"  {i + 1}. {rootObjects[i].name}");
            }

            logEntry.AppendLine();

            File.AppendAllText(logFilePath, logEntry.ToString());

            UnityEngine.Debug.Log($"[Scene Loader] Data written to: {logFilePath}");
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"[Scene Loader] Failed to write to log file: {e.Message}");
        }
    }
}