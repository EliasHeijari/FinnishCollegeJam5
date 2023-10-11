using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropKey : MonoBehaviour
{
    [SerializeField] private Transform dropTransform;
    [SerializeField] private GameObject keyPrefab;

    public void SpawnKey()
    {
        GameObject key = Instantiate(keyPrefab, dropTransform.position, Quaternion.identity);
        key.transform.parent = null;
        key.transform.localPosition = Vector3.zero;
        key.transform.position = dropTransform.position;
    }
}
