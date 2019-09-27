using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private AudioSource _e_sounds;
    private AudioManager _audioManager;

    // Start is called before the first frame update
    void Start()
    {
        _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        if (_audioManager == null) Debug.LogError("Enemy_base::Start() :: We have a problem. The audioManager is null");
        else
        {
            this.gameObject.AddComponent<AudioSource>();
            _e_sounds = this.GetComponent<AudioSource>();
            _e_sounds.clip = _audioManager.GetEffectSound("Explosion");
            if (_e_sounds == null || _e_sounds.clip == null) { Debug.LogError("Enemy_base::Start() :: Something went wrong and the AudioSource or clip are null"); }
            _e_sounds.loop = false;
            _e_sounds.playOnAwake = false;
            _e_sounds.volume = 0.5f;
            _e_sounds.pitch = 0.35f;
        }
        Destroy(this.gameObject, 2.5f);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
