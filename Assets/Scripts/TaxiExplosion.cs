using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaxiExplosion : MonoBehaviour
{
    [SerializeField]
    private GameObject _taxi;
    [SerializeField]
    private GameObject _wrekedCar;
    [SerializeField]
    private GameObject _explosionPrefab;

    public void Explode()
    {
        Instantiate(_explosionPrefab, _taxi.transform.position, Quaternion.identity);
        _taxi.SetActive(false);
        _wrekedCar.SetActive(true);
    }
}
