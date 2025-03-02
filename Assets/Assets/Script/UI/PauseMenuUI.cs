using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1.0f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void PauseGlobal()
    {
  
        Time.timeScale = 0f;
      //  GameIsPaused = true;
    }

    public void ResumeGlobal()
    {
      
        Time.timeScale = 1.0f;
        GameIsPaused = false;
    }


    public void LoadMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("UI");
    }
}
