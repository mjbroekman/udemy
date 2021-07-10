using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    private GameObject _brokenCrate;

    private void Start()
    {
        _brokenCrate = Resources.Load<GameObject>("Objects/Wooden_Crate_Cracked");
    }

    public void DestroyCrate()
    {
        GameObject _tempCrate = Instantiate(_brokenCrate, transform.position, transform.rotation);
        transform.gameObject.SetActive(false);
        Destroy(this.gameObject, 10f);
        Destroy(_tempCrate, 5f);
    }
}
