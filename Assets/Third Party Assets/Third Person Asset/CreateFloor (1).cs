using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateFloor : MonoBehaviour
{
    public GameObject tile;
    public int tileAmount = 3;
    private Vector3 spawnPosz = Vector3.zero;
    private Vector3 spawnPosx = Vector3.zero;
    public float nextTileNum = 4f;

    public void CreateTiles()
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


[CustomEditor(typeof(CreateFloor))]
public class CreateFloorEditor : Editor
{
    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();

        CreateFloor createFloor = (CreateFloor)target;

        if (GUILayout.Button("Create Floor", GUILayout.Width(90f)))
        {
            createFloor.CreateTiles();
        }
    }
}
