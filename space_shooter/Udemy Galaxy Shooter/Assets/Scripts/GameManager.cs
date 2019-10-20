using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool _is_GameOver;
    private bool _is_GamePaused;
    private bool _isStarted;
    private int _difficultyLevel = 1;
    private AudioManager _audioManager;
    private GameObject _pauseMenu;
    private Animator _pmAnimator;
    private UIManager _uiManager;

    void Start()
    {
        _isStarted = false;
        _difficultyLevel = 1;
        _is_GamePaused = false;
        _uiManager = null;

        _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        if (_audioManager == null) Debug.LogError("GameManager::Start() :: We have a problem. The audioManager is null");
        else { _audioManager.SetMusic(_difficultyLevel); }

        _pauseMenu = GameObject.Find("Pause_Menu_Panel");
        if (_pauseMenu == null) { Debug.LogError("GameManager::Start() :: Unable to find Pause Menu"); }
        else
        {
            _pauseMenu.SetActive(false);
            _pmAnimator = _pauseMenu.GetComponent<Animator>();
            if (_pmAnimator == null) { Debug.LogError("GameManager::Start() :: Unable to grab pause menu animator."); }
        }

        _is_GameOver = true;
        _isStarted = true;
    }

    public bool IsStarted()
    {
        return _isStarted;
    }

    void Update()
    {
        if (_is_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                _is_GameOver = false;
                SceneManager.LoadScene(1);
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                GoMainMenu();
            }
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q))
            {
                Application.Quit();
            }
        }
        if (!_is_GamePaused && Input.GetKeyDown(KeyCode.P))
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        _pauseMenu.SetActive(true);
        _pmAnimator.SetBool("isPaused", true);
        Time.timeScale = 0;
    }

    public void UnPause()
    {
        _pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void GoMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void GameOver()
    {
    }

    public bool GetGameState()
    {
        return _is_GameOver;
    }

    public void SetGameState(bool gState)
    {
        _is_GameOver = gState;
    }

    public int GetLevel()
    {
        return _difficultyLevel;
    }

    public void IncreaseLevel()
    {
        SetLevel(_difficultyLevel + 1);
    }

    public void SetLevel(int level)
    {
        if (_uiManager == null)
        {
            _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
            if (_uiManager == null) Debug.LogError("GameManager::SetLevel() :: Houston, we have a problem. There is no UIManager in the scene.");
        }
        _difficultyLevel = level;
        Debug.Log("GameManager::SetLevel() :: Game Level is now " + _difficultyLevel);
        _audioManager.SetMusic(_difficultyLevel);
        _uiManager.UpdateLevelText(_difficultyLevel);
    }

    public float[] GetScreenBoundaries(GameObject who)
    {
        return GetScreenBoundaries(who.tag);
    }

    public float[] GetScreenBoundaries(string who)
    {
        switch (who)
        {
            case "Player": return new float[] { -10f, 10f, -4f, 4f };
            case "Asteroid": return new float[] { -9.5f, 9.5f, -5f, 5.5f };
            default: return new float[] { -10f, 10f, -5f, 6f };
        }
    }

}
