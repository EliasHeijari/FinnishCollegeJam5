using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WonGame : MonoBehaviour
{
    private DoorInteractable doorInteractable;
    public bool GameWon = false;
    [SerializeField] private GameObject wonScreen;

    private void Start()
    {
        doorInteractable = GetComponent<DoorInteractable>();
    }

    private void Update()
    {
        if (doorInteractable.IsOpen())
        {
            GameWon = true;

            wonScreen.SetActive(true);
            StartCoroutine(LoadMainMenuScene());
        }
    }

    IEnumerator LoadMainMenuScene()
    {
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
