using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public int number;
    public void NextScene()
    {
        SceneManager.LoadScene(number);
    }

    [SerializeField] private AudioSource audio;
    public void PlayButtonClick()
    {
        if (audio != null) { audio.Play(); }
    }
}
