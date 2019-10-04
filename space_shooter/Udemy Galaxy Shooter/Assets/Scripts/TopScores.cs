using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class TopScores : MonoBehaviour
{
    private string[] _topScores;
    private Text _scoreList;
    private Text[] _textObjs;
    private IEnumerator scoreDisplay;

    void Start()
    {
        _topScores = PlayerPrefs.HasKey("TopScores") ? PlayerPrefs.GetString("TopScores").Split(","[0]) : new string[0];
        _textObjs = FindObjectsOfType<Text>();
        for (int i = 0; i < _textObjs.Length; i++)
        {
            if (_textObjs[i] != null)
            {
                switch (_textObjs[i].name)
                {
                    case "Score_List_Scores": { _scoreList = _textObjs[i]; break; }
                }
            }
        }
        scoreDisplay = DisplayScores();
        StartCoroutine(scoreDisplay);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { ReturnToMain(); }
    }

    public IEnumerator DisplayScores()
    {
        int i = 0;
        _scoreList.text = "";
        while (i < 10)
        {
            yield return new WaitForSeconds(0.5f);
            if (_topScores.Length > 0 && i < _topScores.Length) { _scoreList.text += _topScores[i] + "\n"; }
            if (i >= _topScores.Length) { _scoreList.text += "0\n"; }
            i++;
        }
    }

    public void ClearScores()
    {
        StopCoroutine(scoreDisplay);
        scoreDisplay = null;
        if (PlayerPrefs.HasKey("TopScores")) { PlayerPrefs.DeleteKey("TopScores"); }
        if (PlayerPrefs.HasKey("CurrentHighScore")) { PlayerPrefs.DeleteKey("CurrentHighScore"); }
        scoreDisplay = DisplayScores();
        StartCoroutine(scoreDisplay);
    }

    public void ReturnToMain()
    {
        SceneManager.LoadScene(0);
    }
}