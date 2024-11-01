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
    [SerializeField] GameObject TileManager;
    [SerializeField] GameObject clearPanel;
    [SerializeField] GameObject mapPanel;
    private AIManager aiManager;
    private TileManager tileManager;
    private AnimalManager animalManager;
    private static LevelManager instance;
    public static string SceneName;

   
    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }

        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        aiManager = AIManager.GetComponent<AIManager>();
        tileManager=TileManager.GetComponent<TileManager>();    
        Level = new bool[Maps.Length];
        SceneName = "Lobby";

       
    }

    private void Start()
    {
        LinkMaps();
    }

  

    // 로비로 이동했을때만 호출
    public void LinkMaps()
    {
        mapPanel.SetActive(true);
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
        //SceneManager.LoadScene("Lobby");
        //LinkMaps();
    }

    public void ClickMap(UnityEngine.UI.Button button)
    {
        Hunter.SetActive(true);
        mapPanel.SetActive(false);
        
        TileManager.SetActive(true);
        AIManager.SetActive(true);



        SceneName =button.name;

        //SceneManager.LoadScene(button.name);
        SetSceneName(button.name);
        SceneManager.LoadScene("Loading");
       
       
        tileManager.CreateTileMap();      
        aiManager.StartTurn();
        //aiManager.gameObject.SetActive(true);
        //aiManager.StartTurn();

    }

    public void SetSceneName(string name)
    {
        SceneName = name;
    }

    public string GetSceneName()
    {
        return SceneName;
    }
}
