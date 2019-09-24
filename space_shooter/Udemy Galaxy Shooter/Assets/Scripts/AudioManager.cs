using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AudioManager : MonoBehaviour
{
    private bool _isStarted;
    private GameObject _backgroundMusic;
    private List<AudioClip> _gameMusic;
    private AudioClip _weaponsFire;
    private Dictionary<string, AudioClip> _effectsMusic;
    private AudioSource _musicSource;

    void Start()
    {
        _isStarted = false;
        _gameMusic = new List<AudioClip>();
        _effectsMusic = new Dictionary<string, AudioClip>();

        LoadAssets("Effects");
        LoadAssets("Music");
        LoadAssets("Weapons");

        _backgroundMusic = new GameObject("BackgroundMusic");
        _backgroundMusic.transform.parent = transform;
        _backgroundMusic.AddComponent<AudioSource>();
        _musicSource = _backgroundMusic.GetComponent<AudioSource>();
        if (_musicSource != null)
        {
            _musicSource.loop = true;
        }
        _isStarted = true;
    }

    public bool IsStarted()
    {
        return _isStarted;
    }

    void LoadAssets(string asset)
    {
        string audioPath = "Assets/Audio/" + asset;
        Debug.Log("AudioManager::LoadAssets() :: Attempting to load from " + audioPath);
        foreach (var guid in AssetDatabase.FindAssets("t:AudioClip", new[] { audioPath }))
        {
            AudioClip newObj = AssetDatabase.LoadAssetAtPath<AudioClip>(AssetDatabase.GUIDToAssetPath(guid));
            Debug.Log("AudioManager::LoadAssets() :: Loading " + newObj.name);
            switch (asset)
            {
                case "Effects":
                    if (newObj.name.Contains("Explosion")) _effectsMusic.Add("Explosion", newObj);
                    if (newObj.name.Contains("PowerUp")) _effectsMusic.Add("PowerUp", newObj);
                    break;
                case "Music":
                    _gameMusic.Add(newObj);
                    break;
                case "Weapons":
                    _weaponsFire = newObj;
                    break;
            }
        }
    }

    public void SetMusic(int clip)
    {
        _musicSource.clip = _gameMusic[clip % _gameMusic.Count];
        Debug.Log("AudioManager::SetMusic() :: Swapping tracks to " + _musicSource.clip);
        _musicSource.volume = 0.5f;
        _musicSource.Play();
    }

    public AudioClip getEffectSound(string what)
    {
        return _effectsMusic[what];
    }

    public AudioClip getWeaponSound()
    {
        return _weaponsFire;
    }

    void Update()
    {

    }
}
