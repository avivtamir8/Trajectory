using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Controls the movement of the assigned GameObject (the Interceptor Sphere) 
/// along the trajectory loaded from the CSV file.
/// </summary>
public class TrajectoryPlayer : MonoBehaviour
{
    [Header("Data Source")]
    [Tooltip("Drag your ex_no_fov_3000.csv file here from the Assets folder.")]
    public TextAsset csvFile;

    [Header("Playback Settings")]
    [Range(1f, 100f)]
    [Tooltip("Multiplier for the simulation time. 1.0 is real-time.")]
    public float PlaybackSpeed = 10.0f; 

    // Internal state variables
    private List<TrajectoryPoint> trajectoryData;
    private float currentTime = 0.0f;
    private int currentPointIndex = 0;

    void Start()
    {
        // 1. Load the data
        trajectoryData = CsvLoader.LoadTrajectory(csvFile);

        if (trajectoryData == null || trajectoryData.Count == 0)
        {
            Debug.LogError("Trajectory data could not be loaded or is empty. Cannot start playback.");
            return;
        }

        // 2. Initialize position
        // Move the interceptor to the first point of the trajectory
        transform.position = trajectoryData[0].Position;
        
        Debug.Log($"Starting playback from position: {transform.position}");
    }

    void Update()
    {
        // Check if data is loaded and there are points left to play
        if (trajectoryData == null || trajectoryData.Count < 2 || currentPointIndex >= trajectoryData.Count - 1)
        {
            return; // Stop playback when all points are processed
        }

        // 1. Advance simulation time based on real-time and playback speed
        currentTime += Time.deltaTime * PlaybackSpeed;

        // 2. Find the correct point index based on the current time
        // The data is 120Hz, so we check which point's time is greater than the current simulation time.
        
        while (currentPointIndex < trajectoryData.Count - 1 && trajectoryData[currentPointIndex + 1].Time <= currentTime)
        {
            currentPointIndex++;
        }

        // Check if we reached the end again after advancing the index
        if (currentPointIndex >= trajectoryData.Count - 1)
        {
            Debug.Log("Trajectory playback finished.");
            return;
        }
        
        // 3. Interpolate between the current point and the next point for smooth movement
        
        TrajectoryPoint startPoint = trajectoryData[currentPointIndex];
        TrajectoryPoint endPoint = trajectoryData[currentPointIndex + 1];

        // Calculate the interpolation factor (t)
        float totalDuration = endPoint.Time - startPoint.Time;
        float timeSinceLastPoint = currentTime - startPoint.Time;

        // The factor t is between 0 (at startPoint) and 1 (at endPoint)
        float t = totalDuration > 0 ? timeSinceLastPoint / totalDuration : 1f;
        
        // Use Vector3.Lerp for linear interpolation between the two positions
        transform.position = Vector3.Lerp(startPoint.Position, endPoint.Position, t);
        
        // Optional: Draw a debug line to visually trace the path in the scene view
        // This is a great way to verify the path geometry.
        Debug.DrawLine(startPoint.Position, endPoint.Position, Color.red, 0.01f, false);
    }
}