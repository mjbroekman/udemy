using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving_Platform : MonoBehaviour
{
    [SerializeField]
    private Transform _targetA;

    [SerializeField]
    private Transform _targetB;

    [SerializeField]
    private float _speed;

    [SerializeField]
    private Transform _target;
   
    [SerializeField]
    private UIManager _uiManager;

    // Start is called before the first frame update
    void Start()
    {
        // Get handles on the start and end positions for our moving platform
        _targetA = GameObject.Find("Point_A").GetComponent<Transform>();
        if (_targetA == null)
        {
            Debug.LogError("_targetA is NULL");
        }

        _targetB = GameObject.Find("Point_B").GetComponent<Transform>();
        if(_targetB == null)
        {
            Debug.LogError("_targetB is NULL");
        }
        // Starting position
        transform.position = _targetA.position;

        // Set initial target
        _target = _targetB;

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("_uiManager is NULL");
        }

        _speed = _uiManager.GetWorldSpeed();

    }

    // Update is called once per frame
    void Update()
    {
        _speed = _uiManager.GetWorldSpeed();

    }

    // FixedUpdate is needed to move the child player with the platform
    void FixedUpdate()
    {
        if (Vector3.Distance(_targetA.position, transform.position) < 0.1)
        {
            _target = _targetB;
        }

        if (Vector3.Distance(_targetB.position, transform.position) < 0.1)
        {
            _target = _targetA;
        }
        transform.position = Vector3.MoveTowards(transform.position, _target.position, Time.deltaTime * _speed);
    }

    // If an object lands on the platform, set the platform as the parent so we move with it
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            other.transform.SetParent(this.transform);
    }

    // If an object leaves the platform, set the object parent to null
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            other.transform.SetParent(null);
    }

}
