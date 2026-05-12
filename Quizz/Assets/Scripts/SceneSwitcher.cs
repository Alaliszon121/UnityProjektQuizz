using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public int number;
    public void NextScene()
    {
        SceneManager.LoadScene(number);
    }
}
