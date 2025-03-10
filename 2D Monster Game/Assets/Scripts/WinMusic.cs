using UnityEngine;

public class WinMusic : MonoBehaviour
{
    public AudioSource winSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Play the music
        winSource.Play();
    }
}
