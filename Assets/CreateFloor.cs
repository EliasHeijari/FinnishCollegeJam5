using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateFloor : MonoBehaviour
{
    [SerializeField] private GameObject tile;
    [SerializeField] private int tileAmount = 3;
    private Vector3 spawnPosz = Vector3.zero;
    private Vector3 spawnPosx = Vector3.zero;
    [SerializeField] private float nextTileNum = 4f;

    private void Start()
    {
        for (int z = 0; z < tileAmount; ++z)
        {
            spawnPosz = new Vector3(spawnPosz.x, spawnPosz.y, spawnPosz.z + nextTileNum);
            Instantiate(tile, spawnPosz, Quaternion.identity);
            for (int x = 0; x < tileAmount; ++x)
            {
                spawnPosx = new Vector3(spawnPosx.x + nextTileNum, spawnPosx.y, spawnPosx.z);
                Instantiate(tile, spawnPosz + spawnPosx, Quaternion.identity);
            }
            spawnPosx = Vector3.zero;
        }
    }
}
