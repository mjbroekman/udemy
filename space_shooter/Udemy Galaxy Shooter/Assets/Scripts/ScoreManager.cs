using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager m_instance;

    public static ScoreManager _instance
    {
        get
        {
            if (m_instance == null) { m_instance = new GameObject("ScoreManager").AddComponent<ScoreManager>(); }
            return m_instance;
        }
    }

    void Awake()
    {
        if (m_instance == null) { m_instance = this; }
        else if (m_instance != this) { Destroy(gameObject); }

        DontDestroyOnLoad(gameObject);
    }

    //void Start()
    //{
    //    DontDestroyOnLoad(gameObject);
    //}

    //void Update()
    //{
    //}

    public List<HighScore> GetHighScores()
    {
        List<HighScore> _highScores = new List<HighScore>();
        for (int i = 1; i <= 10; i++)
        {
            if (PlayerPrefs.HasKey("HighScore" + i + "_Score") && PlayerPrefs.HasKey("HighScore" + i + "_Name"))
            {
                HighScore tmp = new HighScore { name = PlayerPrefs.GetString("HighScore" + i + "_Name"), score = PlayerPrefs.GetInt("HighScore" + i + "_Score") };
                _highScores.Add(tmp);
            }
        }

        return _highScores;
    }

    public int GetHighScores(int index)
    {
        List<HighScore> _highScores = new List<HighScore>();
        for (int i = 1; i <= 10; i++)
        {
            if (PlayerPrefs.HasKey("HighScore" + i + "_Score") && PlayerPrefs.HasKey("HighScore" + i + "_Name"))
            {
                HighScore tmp = new HighScore { name = PlayerPrefs.GetString("HighScore" + i + "_Name"), score = PlayerPrefs.GetInt("HighScore" + i + "_Score") };
                _highScores.Add(tmp);
            }
        }
        if (index < _highScores.Count) { return _highScores[index].score; }
        else { return 0; }
    }

    public int UpdateHighScores(int score)
    {
        List<HighScore> _highScores = GetHighScores();
        Debug.Log("ScoreManager::UpdateHighScores() :: Found " + _highScores.Count + " high scores.");
        string _playerName = PlayerPrefs.HasKey("PlayerName") ? PlayerPrefs.GetString("PlayerName") : "Player";
        HighScore tmp = new HighScore { name = _playerName, score = score };
        bool _added = false;
        for (int i = 1; i <= _highScores.Count; i++)
        {
            if (!_added && tmp.score > _highScores[i - 1].score)
            {
                Debug.Log("ScoreManager::UpdateHighScores() :: Added new high score into position " + (i - 1));
                _highScores.Insert((i - 1), tmp);
                _added = true;
            }
        }
        if (!_added)
        {
            Debug.Log("ScoreManager::UpdateHighScores() :: Added new high score to the end of the list.");
            _highScores.Add(tmp);
        }

        for (int i = 0; i < 10 && i < _highScores.Count; i++)
        {
            Debug.Log("ScoreManager::UpdateHighScores() :: Set High Score #" + (i + 1) + " to " + _highScores[i].score);
            PlayerPrefs.SetString("HighScore" + (i + 1) + "_Name", _highScores[i].name);
            PlayerPrefs.SetInt("HighScore" + (i + 1) + "_Score", _highScores[i].score);
        }
        return GetHighScores(0);
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}

public class HighScore
{
    public string name;
    public int score;
}
