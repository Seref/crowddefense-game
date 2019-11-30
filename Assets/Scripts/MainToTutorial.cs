using UnityEngine;
using UnityEngine.SceneManagement;

public class MainToTutorial : MonoBehaviour
{
    public void GotoMainMenuScene()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void GotoTutorialScene()
    {
        SceneManager.LoadScene("Tutorial");
    }
}
