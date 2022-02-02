using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script makes the sound persistent in between the game sessions.
public class DataSaver : MonoBehaviour
{
    public static DataSaver Instance;
    public float volume;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        volume = 0.2f; // sets the volume at the start
        DontDestroyOnLoad(gameObject);
    }
}
