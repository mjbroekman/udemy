using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private CharacterController _controller;

    [SerializeField]
    private float _speed;

    [SerializeField]
    private float _gravity = 9.81f;

    [SerializeField]
    private float _jump;

    [SerializeField]
    private bool _isJumping;

    [SerializeField]
    private Vector3 _velocity;

    [SerializeField]
    private float _sensitivity;

    [SerializeField]
    private GameObject _weapon;

    private GameObject _hitMarkerPrefab;
    private AudioClip _weaponFx;
    private AudioSource _weaponFxSrc;

    [SerializeField]
    private int _currentAmmo; 

    [SerializeField]
    private int _maxAmmo;

    private UIManager _uiManager;

    private bool _isReloading;
    private float _reloadTime;
    private float _lastReload;
    private float _weaponSpeed;
    private float _lastShot;

    public float ReloadTime { get => _reloadTime; set => _reloadTime = value; }
    public bool IsReloading { get => _isReloading; set => _isReloading = value; }
    public float LastReload { get => _lastReload; set => _lastReload = value; }
    public float WeaponSpeed { get => _weaponSpeed; set => _weaponSpeed = value; }
    public float LastShot { get => _lastShot; set => _lastShot = value; }
    public int MaxAmmo { get => _maxAmmo; set => _maxAmmo = value; }
    public int CurrentAmmo { get => _currentAmmo; set => _currentAmmo = value; }
    public float Sensitivity { get => _sensitivity; set => _sensitivity = value; }
    public bool IsJumping { get => _isJumping; set => _isJumping = value; }
    public float Speed { get => _speed; set => _speed = value; }
    public float Gravity { get => _gravity; set => _gravity = value; }
    public float Jump { get => _jump; set => _jump = value; }

    // Start is called before the first frame update
    void Start()
    {
        Speed = 3.5f;
        Jump = 15f;
        IsJumping = false;
        IsReloading = false;
        ReloadTime = 1.5f;
        LastReload = 0;
        MaxAmmo = 50;
        WeaponSpeed = 0.1f;
        LastShot = 0;
        _velocity = Vector3.zero;

        // user setting
        Sensitivity = 1f;

        // Grab the player controller
        _controller = GetComponent<CharacterController>();
        
        // Grab the UIManager script
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null) Debug.LogError("UIManager is null. Can't find user interface yet.");
        else _uiManager.InitUI();

        // Give the player a full clip to start
        SetAmmo(MaxAmmo);

        // Get all the fun resources to make sparkly bits and noises.
        _weapon = GameObject.Find("Weapon");
        _weaponFx = Resources.Load<AudioClip>("Sounds/Shoot_Sound");
        _weaponFxSrc = GetComponent<AudioSource>();
        _hitMarkerPrefab = Resources.Load<GameObject>("Effects/Hit_Marker");

        // Hide and lock the mouse into the screen
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        bool hitEscape = Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace);

        // If we hit R, initiate the reloading process
        if (Input.GetKeyDown(KeyCode.R) && !IsReloading)
        {
            SetAmmo(MaxAmmo);
            IsReloading = true;
            LastReload = Time.time;
        }

        // If we're locked into the game screen, allow the player to do stuff
        if (Cursor.lockState == CursorLockMode.Locked) {
            // If the player is pressing Mouse1 AND we're not in our reload delay AND we're not in our weapon firing delay, shoost
            if (Input.GetMouseButton(0) && Time.time > (LastReload + ReloadTime) && Time.time > (LastShot + WeaponSpeed))
            {
                IsReloading = false;
                ShootWeapon();
            }
            else if (!Input.GetMouseButton(0)) StopShooting();

            // Allow for movement
            CalculateMovement();

            // Allow for the potential to escape out
            if (hitEscape)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        else if (Cursor.lockState == CursorLockMode.None)
        {
            // Allow for the potential to escape back in
            if (hitEscape) { 
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    // Set our ammo is a new value and update the UIManager
    void SetAmmo(int newAmmo)
    {
        CurrentAmmo = newAmmo;
        _uiManager.SetAmmo(CurrentAmmo);
    }

    void ShootWeapon()
    {
        //if left click
        // cast ray from center of main camera
        Ray rayOrigin = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hitInfo;

        // Set up the weapon muzzle flash particle system.
        ParticleSystem flash = _weapon.GetComponentInChildren<ParticleSystem>(true);
        ParticleSystem.MainModule flashMain = flash.main;

        // Stop shooting and exit this method if we have last than 1 round left
        if (CurrentAmmo < 1)
        {
            StopShooting();
            return;
        }

        // Set the time of the last shot
        LastShot = Time.time;

        // If the muzzle flash is not emitting, start emitting again
        if (! flash.isEmitting)
        {
            flashMain.playOnAwake = true;
            flash.Play();
        }

        // If the weapon sound effect is NOT playing, then start it up again
        if (!_weaponFxSrc.isPlaying)
        {
            Debug.Log("Fx is not playing, start playing it you goober." + _weaponFx);
            _weaponFxSrc.clip = _weaponFx;
            _weaponFxSrc.Play();
        }

        // Use a round of ammo
        SetAmmo(CurrentAmmo - 1);

        // Raycast to see if we hit something worthwhile
        if (Physics.Raycast(rayOrigin, out hitInfo))
        {
            // Do this if we're in the Unity Editor
            Debug.Log("Raycast collision detected with " + hitInfo.transform.name);
            // instantiate the hit marker
            GameObject _tempMarker = Instantiate(_hitMarkerPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            // Have the marker destroy itself to make sure we clean up objects nicely
            Destroy(_tempMarker, 0.15f);
        }
    }

    // Stop shooting...
    void StopShooting()
    {
        // Get the muzzle flash particle system
        ParticleSystem flash = _weapon.GetComponentInChildren<ParticleSystem>(true);
        ParticleSystem.MainModule flashMain = flash.main;

        // If the particle system is emitting or still playing, then stop it.
        if (flash.isEmitting || flash.isPlaying)
        {
            flashMain.playOnAwake = false;
            flash.Stop();
        }

        // If the weapon sound is still playing, then stop it
        if (_weaponFxSrc.isPlaying)
        {
            Debug.Log("Fx is playing, cut it out you goober." + _weaponFxSrc);
            _weaponFxSrc.Stop();
        }
    }

    void CalculateMovement()
    {
        float eastwest = Input.GetAxis("Horizontal");
        float northsouth = Input.GetAxis("Vertical");
        bool jump = Input.GetKeyDown(KeyCode.Space);

        Vector3 direction = new Vector3(eastwest,0f, northsouth);

        // Apply speed to direction to get velocity
        _velocity = direction * Speed;

        if (jump && !IsJumping)
        {
            direction.y = Jump;
            IsJumping = true;
        }
        else if ((_controller.collisionFlags & CollisionFlags.Below) != 0)
        {
            IsJumping = false;
            direction.y = 0.0f;
        }
        else
        {
            _velocity.y -= Gravity;
        }

        // convert from localspace to worldspace
        _velocity = transform.transform.TransformDirection(_velocity);

        // Move the player based on how much time has passed since the last update
        _controller.Move(_velocity * Time.deltaTime);
    }

    public float GetSensitivity()
    {
        return Sensitivity;
    }

}
