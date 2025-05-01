using System;
using UnityEngine;

public class PlayingList : MonoBehaviour
{
    AudioSource[] sources;

    void Start()
    {
        // Get every single audio source in the scene using the updated method.
        sources = UnityEngine.Object.FindObjectsByType<AudioSource>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
    }

    void Update()
    {
        // When a key is pressed, list all the game objects that are playing an audio
        if (Input.GetKeyUp(KeyCode.A))
        {
            foreach (AudioSource audioSource in sources)
            {
                if (audioSource.isPlaying)
                    Debug.Log(audioSource.name + " is playing " + audioSource.clip.name);
            }
            Debug.Log("---------------------------"); // To avoid confusion next time
            Debug.Break(); // Pause the editor
        }
    }
}
