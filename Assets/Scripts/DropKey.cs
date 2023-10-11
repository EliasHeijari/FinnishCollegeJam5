using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropKey : MonoBehaviour
{
    [SerializeField] private Transform dropTransform;
    [SerializeField] private GameObject keyPrefab;

    public void SpawnKey()
    {
        GameObject key = Instantiate(keyPrefab, dropTransform);
        key.transform.parent = null;
    }
}
