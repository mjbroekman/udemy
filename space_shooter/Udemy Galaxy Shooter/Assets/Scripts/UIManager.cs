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

    private Dictionary<string, Text> _textObjects;

    private int _lastBGUpdate;

    // Start is called before the first frame update
    void Start()
    {
        //_scoreText = GetComponentInChildren<Text>();
        //_scoreText.text = "Score: 0";

        _textObjects = new Dictionary<string, Text>();
        LoadAssets("UI/Text");

        if (_textObjects.ContainsKey("Score_Text") && _textObjects["Score_Text"] != null)
        {
            _scoreText = Instantiate(_textObjects["Score_Text"], transform);
            _scoreText.text = "Score: ";
        }
        _textObjects["Score_Text"].text = "Score: 0";

        _background = GameObject.Find("Background").GetComponent<SpriteRenderer>();
        if (_background == null)
        {
            Debug.LogError("Unable to find background component");
        }
        ResetBackground();
        _lastBGUpdate = 0;
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
        Debug.Log("Attempting to load from " + prefabPath);
        foreach (var guid in AssetDatabase.FindAssets("t:Object", new[] { prefabPath }))
        {
            if (asset.Contains("Text"))
            {
                Text newObj = AssetDatabase.LoadAssetAtPath<Text>(AssetDatabase.GUIDToAssetPath(guid));

                if (newObj != null)
                {
                    Debug.Log("Loading " + newObj.name);
                    _textObjects.Add(newObj.name, newObj);
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
                Debug.Log("Adding sprite for " + _newSprite.name);
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

        Image _livesImage = GameObject.Find("Lives_Display").GetComponent<Image>();
        if (_livesImage != null)
        {
            _livesImage.sprite = _lifeSprites[lives];
            if (lives == 0) { DisplayGameOver(); }
        }
        else
        {
            Debug.LogError("Can't find the Lives_Display object or the Image component of it");
        }
    }

    public void UpdateBackground()
    {
        switch (Random.Range(0, 3))
        {
            case 0:
                FlipBackground(true, false);
                break;
            case 1:
                FlipBackground(false, true);
                break;
            case 2:
                if (_background.color.b < 1f)
                {
                    _background.color = new Color(_background.color.r, _background.color.g, _background.color.b + 0.1f, _background.color.a);
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
            Text GameOver = Instantiate(_textObjects["Game_Over_Text"], transform);
            GameOver.text = "GAME OVER";
        }
    }

    public void UpdateScore(int score)
    {
        _scoreText.text = "Score: " + score;
        //_textObjects["Score_Text"].text = "Score: " + score;
        if ((int)(score / 100) > _lastBGUpdate) { UpdateBackground(); _lastBGUpdate = (int)(score / 100); }
    }
}
