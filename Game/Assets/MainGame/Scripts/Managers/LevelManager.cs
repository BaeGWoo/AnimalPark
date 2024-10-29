using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] Button[] Maps;
    [SerializeField] bool[] Level;
    [SerializeField] GameObject Hunter;
    [SerializeField] GameObject AIManager;
    private AIManager aiManager;
    //public EventSystem eventSystem;
    public static string SceneName;

   
    private void Awake()
    {
        aiManager= AIManager.GetComponent<AIManager>();
        Level = new bool[Maps.Length];
        DontDestroyOnLoad(gameObject);
        SceneName = "Lobby";

       
    }

    private void Start()
    {
        StartCoroutine(LinkMaps());
    }

  


    IEnumerator LinkMaps()
    {
        while (true)
        {
            for(int i = 1; i < Maps.Length; i++)
            {
                if (!Level[i - 1])
                {
                    Maps[i].interactable = false;
                }
                else
                {
                    Maps[i].interactable = true;
                }
            }

            yield return null;

        }
    }

    public void ClickMap(Button button)
    {
        SceneManager.LoadScene(button.name);
        SceneName=button.name;
        Hunter.SetActive(true);
        aiManager.StartTurn();
        

    }
}
