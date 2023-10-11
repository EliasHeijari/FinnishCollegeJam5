using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKey : MonoBehaviour
{
    private bool hasKey = false;


    public void SetKey(bool hasKey)
    {
        this.hasKey = hasKey;
    }

    public bool HasKey() { return this.hasKey; }
}
