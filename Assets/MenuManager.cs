using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    public void PlayGame() {
        Debug.Log("PLAY THE GAME LIL BRO");
        SceneManager.LoadScene("SampleScene"); 
    }

    public void QuitGame() {
        Application.Quit();
    }

}
