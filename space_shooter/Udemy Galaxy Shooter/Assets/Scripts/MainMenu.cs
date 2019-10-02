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
    }

    private void InitHighScore()
    {
        if (_highScore == null) { Debug.LogError("MainMenu::Start() :: Unable to find the High Score object"); }
        else
        {
            Debug.Log("MainMenu::Start() :: Setting up high score display");
            _highScore.text = "High Score\n";
            _highScore.text += PlayerPrefs.HasKey("CurrentHighScore") ? PlayerPrefs.GetInt("CurrentHighScore") : 0;
        }
    }

    public void ShowScores()
    {
    }

    private void InitCredits()
    {
        _credits.text = "";
        _credits.horizontalOverflow = HorizontalWrapMode.Overflow;
        _creditText = new string[]
        {
            "",
            "\t\tCredits",
            "",
            "Created 2019 by Maarten Broekman",
            "",
            "",
            "Made as part of Jonathan Weinberger's Udemy course:",
            "The Ultimate Guide to Game Development with Unity 2019.",
            "",
            "",
            "Game Assets provided by Jonathan Weinberger and GameDevHQ",
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

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(1);
    }
}
