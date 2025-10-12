using UnityEngine;

public class SceneSwaper : MonoBehaviour
{
    public void SwapScene(int sceneIndex)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }
}
