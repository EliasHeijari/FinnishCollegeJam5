using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneManager : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(LoadFirstScene());
    }

    IEnumerator LoadFirstScene()
    {
        yield return new WaitForSeconds(8f);
        SceneManager.LoadScene(0);
    }
}
