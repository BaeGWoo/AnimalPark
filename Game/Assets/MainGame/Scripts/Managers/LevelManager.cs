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
    public static int curLevel=0;

    [SerializeField] GameObject Hunter;
    [SerializeField] GameObject AIManager;
    [SerializeField] GameObject LoadManger;
    [SerializeField] GameObject clearPanel;
    [SerializeField] GameObject mapPanel;
    //private AIManager aiManager;
    private AnimalManager animalManager;
    private static LevelManager instance;
    public static string SceneName;
    public bool MaxLevel = false;

   
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


        //aiManager = AIManager.GetComponent<AIManager>();
        //loadManager=LoadManger.GetComponent<LoadingSceneManager>();
        Level = new bool[Maps.Length];
        SceneName = "Lobby";

       
    }

    private void Start()
    {
        LinkMaps();
    }

    private void Update()
    {
        
    }

    public int GetCurLevel()
    {
        return curLevel;
    }

   


   // 이전단계의 성공여부를 이용해서 로비씬에서 버튼의 활성화를 판별
    public void LinkMaps()
    {
        //mapPanel.SetActive(true);
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

        if (Level[Maps.Length - 1])
        {
            mapPanel.SetActive(false);
            Hunter.GetComponent<Hunter>().MoveEndingScene();
        }
    }

    // 씬 넘버를 이용하여 각 단계의 이전 단계 성공시 레벨리스트에 반영
    public void LevelUp()
    {
        Level[curLevel++] = true;
        clearPanel.SetActive(false);
    }

   


    
   
}
