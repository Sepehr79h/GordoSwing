using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{

    public void ResetGame()
    {
        SaveSystem.ResetPlayer();
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
