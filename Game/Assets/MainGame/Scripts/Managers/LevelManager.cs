using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LevelManager : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Button[] Maps;
    [SerializeField] bool[] Level;
    [SerializeField] GameObject Hunter;
    [SerializeField] GameObject AIManager;
    [SerializeField] GameObject clearPanel;
    private AIManager aiManager;
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
        LinkMaps();
    }

  

    // 로비로 이동했을때만 호출
    public void LinkMaps()
    {

        for (int i = 1; i < Maps.Length; i++)
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

    }

    public void LevelUp()
    {
        int SceneNumber= SceneManager.GetActiveScene().buildIndex-1;
        Level[SceneNumber] = true;
        clearPanel.SetActive(false);
        SceneManager.LoadScene("Lobby");
        LinkMaps();
    }

    public void ClickMap(UnityEngine.UI.Button button)
    {
        SceneManager.LoadScene(button.name);
        SceneName=button.name;
        Hunter.SetActive(true);
        aiManager.StartTurn();
        

    }
}
