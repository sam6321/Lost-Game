using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("MainScene");
    }
}
