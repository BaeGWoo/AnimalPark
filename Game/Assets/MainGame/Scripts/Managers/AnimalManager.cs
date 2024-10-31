using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimalManager : MonoBehaviour
{
    [SerializeField] GameObject[] curAnimals;

    public GameObject[] GetCurrentAnimals() { return curAnimals; }

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
        Debug.Log("Animal");
        FindAnyObjectByType<AIManager>().GetComponent<AIManager>().getAnimalList(curAnimals);
    }
}
