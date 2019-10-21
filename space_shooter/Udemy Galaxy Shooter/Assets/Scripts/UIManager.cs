using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Text _scoreText;
    private int _score;
    private Text _highScoreText;
    private Text _levelText;

    private SpriteRenderer _background;

    // Text Objects to connect to variables
    protected internal readonly string[] _textToObj = { "Score_Text", "High_Score", "Level_Text" };

    private readonly string _spritePath = "Sprites/UI/";
    private Sprite[] _lifeSprites;
    private Image _livesImage;
    private List<Sprite> _backgrounds;
    private Sprite _defBackground;

    private Dictionary<string, Text> _textObjects;
    private Dictionary<string, Image> _imageObjects;

    private int _lastBGUpdate;

    public bool _uiLoaded;

    private bool _is_GameOver;
    private Text GameOver;
    private Text SubText;

    private IEnumerator _waitForInput;

    private SpawnManager _spawnManager;
    private GameManager _gameManager;
    private ScoreManager _scoreManager;

    void Start()
    {
        _textObjects = new Dictionary<string, Text>();
        LoadAssets("Prefabs/UI/Text");
        _imageObjects = new Dictionary<string, Image>();
        LoadAssets("Prefabs/UI/Images");
        _backgrounds = new List<Sprite>();
        _defBackground = null;
        LoadAssets("Sprites/Backgrounds");

        ConfigObjects();
        ResetBackground();

        _lastBGUpdate = 0;
        _score = 0;

        _gameManager.SetGameState(false);
        _is_GameOver = _gameManager.GetGameState();
        _uiLoaded = true;
        _waitForInput = WaitForInput();
    }

    private void ConfigObjects()
    {
        _background = GameObject.Find("Background").GetComponent<SpriteRenderer>();
        if (_background == null) Debug.LogError("UIManager::ConfigObjects() :: Unable to find background sprite renderer");

        _scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        if (_scoreManager == null) Debug.LogError("UIManager::ConfigObjects() :: Unable to find the ScoreManager");

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null) Debug.LogError("UIManager::ConfigObjects() :: Unable to find the SpawnManager.");

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null) Debug.LogError("UIManager::ConfigObjects() :: Unable to find the Game_Manager");

        foreach (string textObj in _textToObj)
        {
            if (_textObjects.ContainsKey(textObj) && _textObjects[textObj] != null)
            {
                switch (textObj)
                {
                    case "Score_Text": _scoreText = Instantiate(_textObjects[textObj], transform); _scoreText.text = "Score:\n"; break;
                    case "High_Score": _highScoreText = Instantiate(_textObjects[textObj], transform); _highScoreText.text = "High Score:\n" + _scoreManager.GetHighScores(0); break;
                    case "Level_Text": _levelText = Instantiate(_textObjects[textObj], transform); UpdateLevelText(_gameManager.GetLevel()); break;
                }
            }
            else { Debug.LogError("UIManager::ConfigObjects() :: Unable to find non-null " + textObj + " object"); }
        }

        if (_imageObjects.ContainsKey("Lives_Display") && _imageObjects["Lives_Display"] != null)
        {
            GameObject _livesDisplay = GameObject.Find("Lives_Display");
            _livesImage = _livesDisplay == null
                ? Instantiate(_imageObjects["Lives_Display"], transform)
                : GameObject.Find("Lives_Display").GetComponent<Image>();
            InitLives();
        }
        else { Debug.LogError("UIManager::ConfigObjects() :: Unable to find Lives_Display image"); }
    }

    public bool IsStarted() { return _uiLoaded; }

    private void Update()
    {
    }

    private void LoadAssets(string asset)
    {
        string aPath = "" + asset;
        Debug.Log("UIManager::LoadAssets() :: Attempting to load from " + aPath);
        if (asset.Contains("Text"))
        {
            Text[] obj = Resources.LoadAll<Text>(aPath);
            Debug.Log("Found " + obj.Length + " objects in " + aPath);
            foreach (Text newObj in obj)
            {
                Debug.Log("UIManager::LoadAssets() :: Loading " + newObj.name);
                _textObjects.Add(newObj.name, newObj);
            }
        }
        if (asset.Contains("Image"))
        {
            Image[] obj = Resources.LoadAll<Image>(aPath);
            Debug.Log("UIManager::LoadAssets() :: Found " + obj.Length + " objects in " + aPath);
            foreach (Image newObj in obj)
            {
                Debug.Log("UIManager::LoadAssets() :: Loading " + newObj.name);
                _imageObjects.Add(newObj.name, newObj);
            }
        }
        if (asset.Contains("Backgrounds"))
        {
            Sprite[] obj = Resources.LoadAll<Sprite>(aPath);
            Debug.Log("UIManager::LoadAssets() :: Found " + obj.Length + " objects in " + aPath);
            foreach (Sprite newObj in obj)
            {
                Debug.Log("UIManager::LoadAssets() :: Loading " + newObj.name);
                _backgrounds.Add(newObj);
                if (newObj.name == "SpaceBG_Overlay") { _defBackground = newObj; }
            }
        }
    }

    private void InitLives()
    {
        _lifeSprites = new Sprite[4];
        string aPath = _spritePath + "Lives";
        Sprite[] obj = Resources.LoadAll<Sprite>(aPath);
        Debug.Log("Found " + obj.Length + " objects in " + aPath);
        foreach (Sprite _newSprite in obj)
        {
            Debug.Log("UIManager::InitLives() :: Adding sprite for " + _newSprite.name);
            switch (_newSprite.name)
            {
                case "no_lives": _lifeSprites[0] = _newSprite; break;
                case "One": _lifeSprites[1] = _newSprite; break;
                case "Two": _lifeSprites[2] = _newSprite; break;
                case "Three": _lifeSprites[3] = _newSprite; break;
            }
        }
    }

    public void UpdateLives(int lives)
    {
        if (_lifeSprites == null) { InitLives(); }

        if (_livesImage != null && lives >= 0 && lives <= 3)
        {
            _livesImage.sprite = _lifeSprites[lives];
            if (lives == 0) { _gameManager.SetGameState(true); UpdateHighScore(); DisplayGameOver(); }
        }
        else
        {
            _gameManager.SetGameState(true);
            UpdateHighScore();
            DisplayGameOver();
            Debug.Log("UIManager::UpdateLives() :: Can't find the Lives_Display object or the Image component of it. Are we trying to update it before it's initialized? Do we have an invalid number of lives?  Lives == " + lives + " Image == " + _livesImage);
        }
    }

    public void UpdateBackground()
    {
        int val = Random.Range(0, 4);
        switch (val)
        {
            case 0:
                FlipBackground(true, false);
                break;
            case 1:
                FlipBackground(false, true);
                break;
            case 2:
            case 3:
                int bg = Random.Range(0, _backgrounds.Count);
                while (_background.sprite.name == _backgrounds[bg].name)
                {
                    bg = Random.Range(0, _backgrounds.Count);
                }
                _background.sprite = _backgrounds[bg];
                break;
        }
    }

    private void FlipBackground(bool flipX, bool flipY)
    {
        if (flipX) _background.flipX = !_background.flipX;
        if (flipY) _background.flipY = !_background.flipY;
    }

    public void ResetBackground()
    {
        _background.flipX = false;
        _background.flipY = false;
        if (_defBackground != null) { _background.sprite = _defBackground; }
    }

    public void DisplayGameOver()
    {
        if (_textObjects.ContainsKey("Game_Over_Text") && _textObjects["Game_Over_Text"] != null)
        {
            GameOver = Instantiate(_textObjects["Game_Over_Text"], transform);
            if (GameOver.transform.childCount > 0)
            {
                Transform gOverChild = GameOver.transform.GetChild(0);
                if (gOverChild != null && gOverChild.gameObject.name == "SubText")
                {
                    SubText = gOverChild.gameObject.GetComponent<Text>();
                    if (SubText != null) { SubText.text = "Press 'R' to Restart. Press 'M' to go to the Main Menu. Press Escape to Quit."; }
                }
            }
            GameOver.text = "GAME OVER";
            _is_GameOver = true;
            StartCoroutine(_waitForInput);
        }
    }

    private void FlickerGameOver() { if (_is_GameOver) { GameOver.enabled = !GameOver.enabled; } }

    IEnumerator WaitForInput()
    {
        while (_is_GameOver)
        {
            yield return new WaitForSeconds(0.5f);
            FlickerGameOver();
        }
    }

    public void UpdateHighScore() { _highScoreText.text = "High Score:\n" + _scoreManager.UpdateHighScores(_score); }

    public void UpdateScore(int score)
    {
        _score = score;
        _scoreText.text = "Score:\n" + _score;
        if ((int)(score / 100) > _lastBGUpdate) { UpdateBackground(); _lastBGUpdate = (int)(score / 100); }
        if ((int)(score / 200) > _gameManager.GetLevel()) { _gameManager.SetLevel((int)(_score / 200)); }
        UpdateLevelText(_gameManager.GetLevel());
    }

    public void UpdateLevelText(int level) { _levelText.text = "Level: " + level; }
}
