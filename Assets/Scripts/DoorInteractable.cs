using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractable : MonoBehaviour, IInteractable {

    private Animator animator;
    private bool isOpen = false;
    [SerializeField] private bool isLocked = true;
    [SerializeField] private PlayerKey playerKey;
    [SerializeField] private string interactText = "Open/Close" + "\n   Door";
    [SerializeField] private AudioClip DoorLockedClip;
    [SerializeField] private AudioClip DoorUnlockedClip;
    [SerializeField] private AudioClip DoorOpenClip;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    public void ToggleDoor() {
        isOpen = !isOpen;
        animator.SetBool("IsOpen", isOpen);
    }

    public void Interact(Transform interactorTransform) {
        if (!isLocked)
        {
            ToggleDoor();
            interactText = "Open/Close" + "\n   Door";
            AudioSource.PlayClipAtPoint(DoorOpenClip, transform.position);
        }
        else if (playerKey.HasKey())
        {
            OpenDoorWithKey(playerKey.HasKey());
            interactText = "Door unlocked";
            AudioSource.PlayClipAtPoint(DoorUnlockedClip, transform.position);
        }
        else
        {
            // Door is locked and player is trying to open it
            interactText = "Door is locked!" + "\n   find the key to open";
            AudioSource.PlayClipAtPoint(DoorLockedClip, transform.position);
        }
    }

    public void OpenDoorWithKey(bool hasKey)
    {
        if (hasKey)
        {
            isLocked = false;
            playerKey.SetKey(false);
            interactText = "Open/Close" + "\n   Door";
        }
    }

    public string GetInteractText() {
        return interactText;
    }

    public Transform GetTransform() {
        return transform;
    }

    public bool IsOpen()
    {
        return isOpen;
    }
}