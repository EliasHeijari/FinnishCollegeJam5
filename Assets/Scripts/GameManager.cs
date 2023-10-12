using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject startGuide;
    [SerializeField] private AudioSource mainAudioSource;
    private bool startChecked = false;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        startGuide.SetActive(true);
        mainAudioSource.Pause();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !startChecked)
        {
            mainAudioSource.Play();
            startChecked = true;
            startGuide.SetActive(false);
        }
    }
}
