using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool _is_GameOver;
    private bool _isStarted;
    private float _difficultyLevel = 1;

    // Start is called before the first frame update
    void Start()
    {
        _isStarted = false;
        _difficultyLevel = 1;
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
            if (Input.GetKey(KeyCode.R))
            {
                _is_GameOver = false;
                // Main game play is in Scene 1
                SceneManager.LoadScene(1);
            }
            if (Input.GetKey(KeyCode.M))
            {
                // Load the main menu (scene 0)
                SceneManager.LoadScene(0);
            }
        }
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

    public float GetLevel()
    {
        return _difficultyLevel;
    }

    public void IncreaseLevel()
    {
        _difficultyLevel++;
    }

    public void SetLevel(int level)
    {
        _difficultyLevel = level;
    }

    public float[] GetScreenBoundaries(GameObject who)
    {
        switch (who.tag)
        {
            // return float[] { minX, maxX, minY, maxY }
            case "Player": return new float[] { -10f, 10f, -4f, 4f };
            case "Asteroid": return new float[] { -9.5f, 9.5f, -5f, 5.5f };
            default: return new float[] { -9.5f, 9.5f, -5f, 6f };
        }
    }

    public float[] GetScreenBoundaries(string who)
    {
        switch (who)
        {
            // return float[] { minX, maxX, minY, maxY }
            case "Player": return new float[] { -10f, 10f, -4f, 4f };
            case "Asteroid": return new float[] { -9.5f, 9.5f, -5f, 5.5f };
            default: return new float[] { -10f, 10f, -5f, 6f };
        }
    }

}
