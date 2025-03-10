using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    // Public method to load a new scene.
    public void LoadLevel()
    {
        // Loading the scene named "Win".
        SceneManager.LoadScene("Win");
    }
}
