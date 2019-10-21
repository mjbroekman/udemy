using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private Text _highScore;
    private Text _credits;
    private IEnumerator _creditScroll;
    private string[] _creditText;
    private Text[] _textObjs;
    private ScoreManager _scoreManager;

    void Start()
    {
        GetObjects();
        InitHighScore();
        InitCredits();
    }

    void Update()
    {
    }

    private void GetObjects()
    {
        _textObjs = FindObjectsOfType<Text>();
        for (int i = 0; i < _textObjs.Length; i++)
        {
            if (_textObjs[i] != null)
            {
                switch (_textObjs[i].name)
                {
                    case "High_Score": { _highScore = _textObjs[i]; break; }
                    case "Credits": { _credits = _textObjs[i]; break; }
                }
            }
        }
        _scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        if (_scoreManager == null) Debug.LogError("UIManager::Start() :: Unable to find ScoreManager / TopScores component");
    }

    private void InitHighScore()
    {
        if (_highScore == null) { Debug.LogError("MainMenu::Start() :: Unable to find the High Score object"); }
        else
        {
            Debug.Log("MainMenu::Start() :: Setting up high score display");
            _highScore.text = "High Score\n" + _scoreManager.GetHighScores(0);
        }
    }

    private void InitCredits()
    {
        _credits.text = "";
        _credits.horizontalOverflow = HorizontalWrapMode.Wrap;
        _creditText = new string[]
        {
            "",
            "\t\tCredits",
            "",
            "Created 2019 by Maarten Broekman",
            "",
            "Made as part of Jonathan Weinberger's Udemy course 'The Ultimate Guide to Game Development with Unity 2019'.",
            "",
            "",
            "All game assets provided by Jonathan Weinberger and GameDevHQ except as noted below.",
            "",
            "UI elements (except title image) and font from Unity Technologies' \"Unity Samples: UI\" asset pack",
            "",
            "Nebula backgrounds (not galaxy background) from DinV Studio's \"Dynamic Space Background Lite\" asset pack",
            "",
            "Enemy weapons fire sounds from Eric Berzins' \"Ultra SF Game Audio Weapons Pack v.1\" asset pack",
            "",
            "",
            "Game logic customized by Maarten Broekman",
            "Effects logic customized by Maarten Broekman",
            "",
            "",
            "All code was written by Maarten Broekman",
            ""
        };
        _creditScroll = CreditsScroller();
        StartCoroutine(_creditScroll);
    }

    public IEnumerator CreditsScroller()
    {
        int i = 0;
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            _credits.text += "\n" + _creditText[i];
            i++;
            if (i >= _creditText.Length) { _credits.text = ""; i = 0; }
        }
    }

    public void QuitGame() { Application.Quit(); }

    public void LoadGame() { SceneManager.LoadScene(1); }

    public void ShowScores() { SceneManager.LoadScene(2); }

    public void HelpInfo() { SceneManager.LoadScene(3); }
}
