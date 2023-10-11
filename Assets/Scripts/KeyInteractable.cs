using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInteractable : MonoBehaviour, IInteractable
{
    private PlayerKey playerKey;
    [SerializeField] private string playerObjectName = "Player";

    private void Start()
    {
        playerKey = GameObject.Find(playerObjectName).GetComponent<PlayerKey>();
    }
    public void Interact(Transform interactorTransform)
    {
        // Take Key Function
        playerKey.SetKey(true);
        Destroy(gameObject);
    }

    public string GetInteractText()
    {
        return "Take Door Key";
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
