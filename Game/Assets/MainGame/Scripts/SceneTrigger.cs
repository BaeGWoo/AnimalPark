using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{
    [SerializeField] GameObject[] sceneTriggers;

    [SerializeField] int curLevel;
    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneOnLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneOnLoaded;
    }

    private void SceneOnLoaded(Scene scene, LoadSceneMode mode)
    {
        //curLevel = FindAnyObjectByType<LevelManager>().GetComponent<LevelManager>().GetCurLevel();

        for(int i = 0; i < curLevel; i++)
        {
            sceneTriggers[i].SetActive(false);
        }
    }


}
