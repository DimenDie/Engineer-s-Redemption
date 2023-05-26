using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    InputMaster controls;
    public GameObject pauseMenu;
    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }

    private void Awake()
    {
        controls = new InputMaster();

        controls.Menu.PauseMenu.performed += pause => PauseMenu();
    }
    private void Start()
    {
        Time.timeScale = 1.0f;
    }
    public void StartGame(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ChangeTimeRate(int rate)
    {
        Time.timeScale = rate;
    }

    public void PauseMenu()
    {
        pauseMenu.SetActive(true);
        ChangeTimeRate(0);
    }

}
