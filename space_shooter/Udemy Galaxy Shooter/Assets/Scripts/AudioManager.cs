using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private bool _isStarted;
    private GameObject _backgroundMusic;
    private List<AudioClip> _gameMusic;
    private Dictionary<string, AudioClip> _weaponsFire;
    private Dictionary<string, AudioClip> _effectsMusic;
    private AudioSource _musicSource;
    private bool _musicPlaying;

    void Start()
    {
        _isStarted = false;
        _musicPlaying = false;
        _gameMusic = new List<AudioClip>();
        _effectsMusic = new Dictionary<string, AudioClip>();
        _weaponsFire = new Dictionary<string, AudioClip>();

        LoadAssets("Effects");
        LoadAssets("Music");
        LoadAssets("Weapons");

        _backgroundMusic = new GameObject("BackgroundMusic");
        _backgroundMusic.transform.parent = transform;
        _backgroundMusic.AddComponent<AudioSource>();

        _musicSource = _backgroundMusic.GetComponent<AudioSource>();
        if (_musicSource != null) { _musicSource.loop = true; }

        _isStarted = true;
    }

    public bool IsStarted() { return _isStarted; }

    void LoadAssets(string asset)
    {
        string aPath = "Audio/" + asset;
        Debug.Log("AudioManager::LoadAssets() :: Attempting to load from " + aPath);
        AudioClip[] obj = Resources.LoadAll<AudioClip>(aPath);
        Debug.Log("Found " + obj.Length + " objects in " + aPath);
        foreach (AudioClip aud in obj)
        {
            Debug.Log("AudioManager::LoadAssets() :: Attempting to find objects. Found " + aud.name);
            switch (asset)
            {
                case "Effects":
                    if (aud.name.Contains("Explosion")) _effectsMusic.Add("Explosion", aud);
                    if (aud.name.Contains("PowerUp")) _effectsMusic.Add("PowerUp", aud);
                    break;
                case "Music":
                    _gameMusic.Add(aud);
                    break;
                case "Weapons":
                    string _wName = aud.name.Split("_"[0])[0];
                    Debug.Log("AudioManager::LoadAssets() :: Adding " + _wName + " sound for " + aud);
                    _weaponsFire.Add(_wName, aud);
                    break;
            }
        }
    }

    public void SetMusic(int clip)
    {
        if (_gameMusic != null && _gameMusic.Count > 0)
        {
            _musicSource.clip = _gameMusic[clip % _gameMusic.Count];

            Debug.Log("AudioManager::SetMusic() :: Swapping tracks to " + _musicSource.clip);
            _musicSource.volume = 0.5f;
            _musicSource.Play();
            _musicPlaying = true;
        }
    }

    public AudioClip GetEffectSound(string what)
    {
        return _effectsMusic[what];
    }

    public AudioClip GetWeaponSound(string what)
    {
        if (what == null || what == "") { return GetWeaponSound(); }

        if (_weaponsFire.ContainsKey(what)) { return _weaponsFire[what]; }
        else return GetWeaponSound();
    }

    public AudioClip GetWeaponSound() { return _weaponsFire["Laser"]; }

    void Update() { if (!_musicPlaying) { SetMusic(0); } }
}
