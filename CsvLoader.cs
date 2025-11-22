using UnityEngine;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// A structure to hold the position data for a single point in time.
/// </summary>
public struct TrajectoryPoint
{
    public float Time;
    public Vector3 Position; // Unity coordinates: X (North), Y (-Z, Up), Z (East)
}
/// <summary>
/// Handles loading and parsing of the CSV file into a list of TrajectoryPoint structs.
/// </summary>
public class CsvLoader : MonoBehaviour
{
    // Public method to load the data, callable from other scripts
    public static List<TrajectoryPoint> LoadTrajectory(TextAsset csvFile)
    {
        var points = new List<TrajectoryPoint>();

        if (csvFile == null)
        {
            Debug.LogError("CSV File is missing! Please assign the ex_no_fov_3000.csv file to the script component.");
            return points;
        }

        // Read all lines from the text asset
        string[] lines = csvFile.text.Split('\n');

        // Skip the header line (line[0]) and process from the second line (index 1)
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            // Split the line by commas
            string[] values = line.Split(',');

            // Ensure the line has enough values (we need Time, X, Y, Z, which are the first 4 columns)
            if (values.Length < 4)
            {
                Debug.LogWarning($"Skipping line {i + 1} due to insufficient columns.");
                continue;
            }

            // Attempt to parse the values
            if (float.TryParse(values[0], out float time) &&
                float.TryParse(values[1], out float missileX) &&
                float.TryParse(values[2], out float missileY) &&
                float.TryParse(values[3], out float missileZ))
            {
                // *** CRITICAL COORDINATE CONVERSION ***
                // CSV: X=North, Y=East, Z=Down
                // Unity: X=North, Y=Up, Z=Forward/East
                // Mapping: X_unity = X_csv, Y_unity = -Z_csv (to flip Down to Up), Z_unity = Y_csv
                
                Vector3 position = new Vector3(missileX, -missileZ, missileY);

                points.Add(new TrajectoryPoint
                {
                    Time = time,
                    Position = position
                });
            }
            else
            {
                Debug.LogWarning($"Could not parse line {i + 1}: {line}");
            }
        }

        Debug.Log($"Successfully loaded {points.Count} trajectory points.");
        return points;
    }
}