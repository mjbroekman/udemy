using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool _is_GameOver;

    // Start is called before the first frame update
    void Start()
    {
        _is_GameOver = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_is_GameOver)
        {
            if (Input.GetKey(KeyCode.R))
            {
                _is_GameOver = false;
                // Main game play is in Scene 0
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
        Debug.Log("GameManager::SetGameState() :: Setting gameState to " + gState);
        _is_GameOver = gState;
    }

}
