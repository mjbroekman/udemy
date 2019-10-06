using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class TopScores : MonoBehaviour
{
    private Text[] _textObjs;
    private Text _scoreList, _scoreNames;
    private IEnumerator scoreDisplay;

    public void Start()
    {
        _textObjs = FindObjectsOfType<Text>();
        for (int i = 0; i < _textObjs.Length; i++)
        {
            if (_textObjs[i] != null)
            {
                switch (_textObjs[i].name)
                {
                    case "Score_List_Scores": { _scoreList = _textObjs[i]; break; }
                    case "Score_List_Names": { _scoreNames = _textObjs[i]; break; }
                }
            }
        }
        scoreDisplay = DisplayScores();
        StartCoroutine(scoreDisplay);
    }

    public IEnumerator DisplayScores()
    {
        _scoreList.text = "";
        _scoreNames.text = "";
        for (int i = 1; i <= 10; i++)
        {
            yield return new WaitForSeconds(0.5f);
            if (PlayerPrefs.HasKey("HighScore" + i + "_Score") && PlayerPrefs.HasKey("HighScore" + i + "_Name"))
            {
                _scoreNames.text += PlayerPrefs.GetString("HighScore" + i + "_Name") + "\n";
                _scoreList.text += PlayerPrefs.GetInt("HighScore" + i + "_Score") + "\n";
            }
            else
            {
                _scoreNames.text += ". . . . .\n";
                _scoreList.text += "0\n";
            }
        }
    }

    public void ClearScores()
    {
        StopCoroutine(scoreDisplay);
        scoreDisplay = null;
        for (int i = 1; i <= 10; i++)
        {
            if (PlayerPrefs.HasKey("HighScore" + i + "_Score")) { PlayerPrefs.DeleteKey("HighScore" + i + "_Score"); }
            if (PlayerPrefs.HasKey("HighScore" + i + "_Name")) { PlayerPrefs.DeleteKey("HighScore" + i + "_Name"); }
        }
        scoreDisplay = DisplayScores();
        StartCoroutine(scoreDisplay);
        PlayerPrefs.Save();
    }

    public void ReturnToMain()
    {
        SceneManager.LoadScene(0);
    }
}