using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class UIManager : MonoBehaviour
{

    private Text _scoreText;
    private SpriteRenderer _background;

    private readonly string _spritePath = "Assets/Sprites/UI/";
    private Sprite[] _lifeSprites;
    private Image _livesImage;

    private Dictionary<string, Text> _textObjects;
    private Dictionary<string, Image> _imageObjects;

    private int _lastBGUpdate;

    public bool _uiLoaded;

    private bool _is_GameOver;
    private Text GameOver;

    private IEnumerator _waitForInput;
    private bool _gameReady;

    private SpawnManager _spawnManager;
    private GameManager _gameManager;

    // Start is called before the first frame update
    void Start()
    {
        //_scoreText = GetComponentInChildren<Text>();
        //_scoreText.text = "Score: 0";

        _textObjects = new Dictionary<string, Text>();
        LoadAssets("UI/Text");
        _imageObjects = new Dictionary<string, Image>();
        LoadAssets("UI/Images");

        if (_textObjects.ContainsKey("Score_Text") && _textObjects["Score_Text"] != null)
        {
            _scoreText = Instantiate(_textObjects["Score_Text"], transform);
            _scoreText.text = "Score: ";
        }
        _textObjects["Score_Text"].text = "Score: 0";

        if (_imageObjects.ContainsKey("Lives_Display") && _imageObjects["Lives_Display"] != null)
        {
            GameObject _livesDisplay = GameObject.Find("Lives_Display");
            _livesImage = _livesDisplay == null
                ? Instantiate(_imageObjects["Lives_Display"], transform)
                : GameObject.Find("Lives_Display").GetComponent<Image>();
            InitLives();
        }

        _background = GameObject.Find("Background").GetComponent<SpriteRenderer>();
        if (_background == null)
        {
            Debug.LogError("UIManager::Start() :: Unable to find background component");
        }
        ResetBackground();
        _lastBGUpdate = 0;

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("UIManager::Start() :: This is going to be a problem. We don't have a SpawnManager active.");
        }

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("UIManager::Start() :: We have a problem. The gameManager is null");
        }

        _gameManager.SetGameState(false);
        _is_GameOver = _gameManager.GetGameState();
        _uiLoaded = true;
        _waitForInput = WaitForInput();
    }

    public bool IsStarted()
    {
        return _uiLoaded;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    /// <summary>
    /// Load up the assets that the SpawnManager will control
    /// </summary>
    /// <param name="asset"></param>
    private void LoadAssets(string asset)
    {
        string prefabPath = "Assets/Prefabs/" + asset;
        Debug.Log("UIManager::LoadAssets() :: Attempting to load from " + prefabPath);
        foreach (var guid in AssetDatabase.FindAssets("t:Object", new[] { prefabPath }))
        {
            if (asset.Contains("Text"))
            {
                Text newObj = AssetDatabase.LoadAssetAtPath<Text>(AssetDatabase.GUIDToAssetPath(guid));

                if (newObj != null)
                {
                    Debug.Log("UIManager::LoadAssets() :: Loading " + newObj.name);
                    _textObjects.Add(newObj.name, newObj);
                }
            }
            if (asset.Contains("Image"))
            {
                Image newObj = AssetDatabase.LoadAssetAtPath<Image>(AssetDatabase.GUIDToAssetPath(guid));

                if (newObj != null)
                {
                    Debug.Log("UIManager::LoadAssets() :: Loading " + newObj.name);
                    _imageObjects.Add(newObj.name, newObj);
                }
            }
        }
    }

    private void InitLives()
    {
        _lifeSprites = new Sprite[4];
        foreach (var guid in AssetDatabase.FindAssets("t:Sprite", new[] { _spritePath + "Lives" }))
        {
            Sprite _newSprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guid));
            if (_newSprite != null)
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
    }

    public void UpdateLives(int lives)
    {
        if (_lifeSprites == null) { InitLives(); }

        if (_livesImage != null && lives >= 0 && lives <= 3)
        {
            _livesImage.sprite = _lifeSprites[lives];
            if (lives == 0) { _gameManager.SetGameState(true); DisplayGameOver(); }
        }
        else
        {
            Debug.Log("UIManager::UpdateLives() :: Can't find the Lives_Display object or the Image component of it. Are we trying to update it before it's initialized? Do we have an invalid number of lives?  Lives == " + lives + " Image == " + _livesImage);
        }
    }

    public void UpdateBackground()
    {
        switch (Random.Range(0, 3))
        {
            case 0:
                FlipBackground(true, false);
                Debug.Log("UIManager::UpdateBackground() :: Flipped over X axis.");
                break;
            case 1:
                FlipBackground(false, true);
                Debug.Log("UIManager::UpdateBackground() :: Flipped over Y axis.");
                break;
            case 2:
                if (_background.color.b < 1f)
                {
                    _background.color = new Color(_background.color.r, _background.color.g, _background.color.b + 0.1f, _background.color.a);
                    Debug.Log("UIManager::UpdateBackground() :: Blue shifted. Current Blue == " + _background.color.b);
                }
                break;
        }
    }

    private void FlipBackground(bool flipX, bool flipY)
    {
        if (flipX)
        {
            _background.flipX = !_background.flipX;
        }
        if (flipY)
        {
            _background.flipY = !_background.flipY;
        }
    }

    public void ResetBackground()
    {
        _background.flipX = false;
        _background.flipY = false;
    }

    public void DisplayGameOver()
    {
        if (_textObjects.ContainsKey("Game_Over_Text") && _textObjects["Game_Over_Text"] != null)
        {
            GameOver = Instantiate(_textObjects["Game_Over_Text"], transform);
            GameOver.text = "GAME OVER";
            _is_GameOver = true;
            StartCoroutine(_waitForInput);
        }
    }

    private void FlickerGameOver()
    {
        if (_is_GameOver)
        {
            GameOver.enabled = !GameOver.enabled;
        }
    }

    IEnumerator WaitForInput()
    {
        while (_is_GameOver)
        {
            yield return new WaitForSeconds(0.5f);
            FlickerGameOver();
        }
    }

    public void UpdateScore(int score)
    {
        _scoreText.text = "Score: " + score;
        if ((int)(score / 100) > _lastBGUpdate) { UpdateBackground(); _lastBGUpdate = (int)(score / 100); }
    }
}
