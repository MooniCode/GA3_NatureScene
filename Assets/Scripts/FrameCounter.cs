using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class FrameCounter : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("How often to update the FPS counter (in seconds)")]
    [Range(0.1f, 2.0f)]
    public float updateInterval = 0.5f;

    [Tooltip("Text component to display the FPS")]
    public TMP_Text fpsText;

    [Header("Display Options")]
    [Tooltip("Format string for the FPS display")]
    public string displayFormat = "{0} FPS";

    [Tooltip("Color when FPS is good")]
    public Color goodFPSColor = Color.green;

    [Tooltip("Color when FPS is acceptable")]
    public Color okayFPSColor = Color.yellow;

    [Tooltip("Color when FPS is poor")]
    public Color badFPSColor = Color.red;

    [Tooltip("Threshold for 'good' FPS")]
    public float goodFPSThreshold = 60f;

    [Tooltip("Threshold for 'okay' FPS")]
    public float okayFPSThreshold = 30f;

    private float accumulatedFrameTime = 0f;
    private int framesAccumulated = 0;
    private float timeLeftForUpdate;
    private float currentFPS = 0f;

    private void Start()
    {
        if (fpsText == null)
        {
            Debug.LogWarning("No Text component assigned to FrameCounter. FPS will not be displayed.");
            enabled = false;
            return;
        }

        timeLeftForUpdate = updateInterval;
    }

    private void Update()
    {
        // Accumulate frame time and count
        accumulatedFrameTime += Time.unscaledDeltaTime;
        framesAccumulated++;

        // Decrease time until next update
        timeLeftForUpdate -= Time.unscaledDeltaTime;

        // Check if it's time to update the displayed FPS
        if (timeLeftForUpdate <= 0f)
        {
            // Calculate average FPS over the update interval
            currentFPS = framesAccumulated / accumulatedFrameTime;

            // Format and display the FPS value
            fpsText.text = string.Format(displayFormat, Mathf.Round(currentFPS));

            // Set color based on FPS thresholds
            if (currentFPS >= goodFPSThreshold)
            {
                fpsText.color = goodFPSColor;
            }
            else if (currentFPS >= okayFPSThreshold)
            {
                fpsText.color = okayFPSColor;
            }
            else
            {
                fpsText.color = badFPSColor;
            }

            // Reset for next update
            timeLeftForUpdate = updateInterval;
            accumulatedFrameTime = 0f;
            framesAccumulated = 0;
        }
    }
}