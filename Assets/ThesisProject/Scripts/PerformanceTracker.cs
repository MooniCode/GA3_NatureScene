using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class PerformanceTracker : MonoBehaviour
{
    [Header("Tracking Settings")]
    [SerializeField] private float sampleInterval = 1f; // Record data every X seconds
    [SerializeField] private float testDuration = 300f; // 5 minutes in seconds (change to 600 for 10 min)
    [SerializeField] private bool startTrackingOnLoad = true;

    private List<float> fpsReadings = new List<float>();
    private List<float> frameTimeReadings = new List<float>();
    private float currentFPS;
    private float deltaTime;
    private bool isTracking = false;
    private float trackingStartTime;
    private float lastSampleTime;
    private string logFilePath;
    private string currentSceneName;

    void Start()
    {
        // Disable VSync
        QualitySettings.vSyncCount = 0;

        // Remove any FPS cap
        Application.targetFrameRate = -1;

        logFilePath = Application.dataPath + "/../PerformanceData.txt";
        currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if (startTrackingOnLoad)
        {
            StartTracking();
        }
    }

    void Update()
    {
        // Calculate FPS every frame
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        currentFPS = 1.0f / deltaTime;

        // Manual start/stop with spacebar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTracking)
                StopTracking();
            else
                StartTracking();
        }

        // If tracking, record samples at intervals
        if (isTracking)
        {
            float elapsed = Time.time - trackingStartTime;

            // Check if it's time for a new sample
            if (Time.time - lastSampleTime >= sampleInterval)
            {
                RecordSample();
                lastSampleTime = Time.time;
            }

            // Stop automatically after test duration
            if (elapsed >= testDuration)
            {
                StopTracking();
            }
        }
    }

    void OnGUI()
    {
        // Display current stats on screen
        int w = Screen.width, h = Screen.height;
        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(10, 10, w, h * 2 / 50);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 50;
        style.normal.textColor = Color.white;

        string statusText = isTracking ? "TRACKING" : "IDLE";
        Color statusColor = isTracking ? Color.green : Color.yellow;

        string text = $"<color=white>Scene: {currentSceneName}</color>\n";
        text += $"<color={(statusColor == Color.green ? "lime" : "yellow")}>{statusText}</color>\n";
        text += $"<color=white>FPS: {currentFPS:F1}</color>\n";
        text += $"<color=white>Frame Time: {deltaTime * 1000:F2}ms</color>\n";

        if (isTracking)
        {
            float elapsed = Time.time - trackingStartTime;
            float remaining = testDuration - elapsed;
            text += $"<color=cyan>Time Remaining: {remaining:F0}s</color>\n";
            text += $"<color=cyan>Samples Collected: {fpsReadings.Count}</color>\n";
        }
        else
        {
            text += $"<color=yellow>Press SPACE to start tracking</color>\n";
        }

        GUI.Label(rect, text, style);
    }

    public void StartTracking()
    {
        fpsReadings.Clear();
        frameTimeReadings.Clear();
        isTracking = true;
        trackingStartTime = Time.time;
        lastSampleTime = Time.time;
        currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        Debug.Log($"[Performance Tracker] Started tracking scene: {currentSceneName}");
    }

    public void StopTracking()
    {
        if (!isTracking || fpsReadings.Count == 0)
            return;

        isTracking = false;
        WriteResults();
        Debug.Log($"[Performance Tracker] Stopped tracking. Results written to: {logFilePath}");
    }

    private void RecordSample()
    {
        fpsReadings.Add(currentFPS);
        frameTimeReadings.Add(deltaTime * 1000); // Convert to milliseconds
    }

    private void WriteResults()
    {
        try
        {
            StringBuilder output = new StringBuilder();

            // Create header if file doesn't exist
            bool fileExists = File.Exists(logFilePath);
            if (!fileExists)
            {
                output.AppendLine("================================================================================");
                output.AppendLine("UNITY PERFORMANCE ANALYSIS");
                output.AppendLine("================================================================================");
                output.AppendLine();
            }

            // Write test results
            output.AppendLine("--------------------------------------------------------------------------------");
            output.AppendLine($"Test Date:        {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
            output.AppendLine($"Scene Name:       {currentSceneName}");
            output.AppendLine($"Test Duration:    {testDuration:F0} seconds");
            output.AppendLine($"Sample Interval:  {sampleInterval:F1} seconds");
            output.AppendLine($"Total Samples:    {fpsReadings.Count}");
            output.AppendLine();

            // Calculate statistics
            float avgFPS = fpsReadings.Average();
            float minFPS = fpsReadings.Min();
            float maxFPS = fpsReadings.Max();
            float avgFrameTime = frameTimeReadings.Average();
            float minFrameTime = frameTimeReadings.Min();
            float maxFrameTime = frameTimeReadings.Max();

            // Calculate 1% and 0.1% lows (common in performance analysis)
            List<float> sortedFPS = fpsReadings.OrderBy(x => x).ToList();
            int onePercentIndex = Mathf.Max(0, (int)(sortedFPS.Count * 0.01f));
            int pointOnePercentIndex = Mathf.Max(0, (int)(sortedFPS.Count * 0.001f));
            float onePercentLow = sortedFPS.Skip(onePercentIndex).Take(Mathf.Max(1, sortedFPS.Count / 100)).Average();
            float pointOnePercentLow = sortedFPS.Skip(pointOnePercentIndex).Take(Mathf.Max(1, sortedFPS.Count / 1000)).Average();

            output.AppendLine("FPS STATISTICS:");
            output.AppendLine($"  Average FPS:      {avgFPS:F2}");
            output.AppendLine($"  Minimum FPS:      {minFPS:F2}");
            output.AppendLine($"  Maximum FPS:      {maxFPS:F2}");
            output.AppendLine($"  1% Low FPS:       {onePercentLow:F2}");
            output.AppendLine($"  0.1% Low FPS:     {pointOnePercentLow:F2}");
            output.AppendLine();

            output.AppendLine("FRAME TIME STATISTICS:");
            output.AppendLine($"  Average:          {avgFrameTime:F2} ms");
            output.AppendLine($"  Minimum:          {minFrameTime:F2} ms");
            output.AppendLine($"  Maximum:          {maxFrameTime:F2} ms");
            output.AppendLine();

            // Memory info
            float totalMemoryMB = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024f * 1024f);
            float reservedMemoryMB = UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / (1024f * 1024f);

            output.AppendLine("MEMORY USAGE:");
            output.AppendLine($"  Total Allocated:  {totalMemoryMB:F2} MB");
            output.AppendLine($"  Total Reserved:   {reservedMemoryMB:F2} MB");
            output.AppendLine();

            // System info
            output.AppendLine("SYSTEM INFORMATION:");
            output.AppendLine($"  Graphics Device:  {SystemInfo.graphicsDeviceName}");
            output.AppendLine($"  Processor:        {SystemInfo.processorType}");
            output.AppendLine($"  System Memory:    {SystemInfo.systemMemorySize} MB");
            output.AppendLine($"  Unity Version:    {Application.unityVersion}");
            output.AppendLine();

            // Optional: Write raw data samples (commented out to keep file clean)
            // output.AppendLine("RAW FPS SAMPLES:");
            // for (int i = 0; i < fpsReadings.Count; i++)
            // {
            //     output.AppendLine($"  Sample {i + 1}: {fpsReadings[i]:F2} FPS ({frameTimeReadings[i]:F2} ms)");
            // }
            // output.AppendLine();

            File.AppendAllText(logFilePath, output.ToString());
        }
        catch (Exception e)
        {
            Debug.LogError($"[Performance Tracker] Failed to write results: {e.Message}");
        }
    }

    void OnDestroy()
    {
        if (isTracking)
        {
            StopTracking();
        }
    }
}