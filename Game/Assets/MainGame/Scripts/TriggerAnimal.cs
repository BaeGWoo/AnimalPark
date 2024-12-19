using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerAnimal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Hunter")
        {
            //FindAnyObjectByType<LoadManager>().GetComponent<LoadManager>().LoadScene(gameObject.name);
            SceneManager.LoadScene(gameObject.name);
        }
    } 
}
