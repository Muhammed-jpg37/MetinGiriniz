using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuButton : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("SortingScene");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
