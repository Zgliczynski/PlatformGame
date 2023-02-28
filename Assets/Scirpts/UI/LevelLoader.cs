using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private Animator transition;

    [SerializeField] private float transitionTime = 1f;

    private void Update()
    {
        LoadingScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator LoadingScene(int levelIndex)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);
    }
}
