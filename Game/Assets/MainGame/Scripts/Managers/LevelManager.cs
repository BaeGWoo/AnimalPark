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
    [SerializeField] GameObject LoadManger;
    [SerializeField] GameObject clearPanel;
    [SerializeField] GameObject mapPanel;
    private AIManager aiManager;
    private TileManager tileManager;
    private LoadingSceneManager loadManager;
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
        loadManager=LoadManger.GetComponent<LoadingSceneManager>();
        Level = new bool[Maps.Length];
        SceneName = "Lobby";

       
    }

    private void Start()
    {
        LinkMaps();
    }

    private void Update()
    {
        if (Level[Maps.Length - 1])
        {
            Hunter.GetComponent<Hunter>().MoveEndingScene();
        }
    }


    // 버튼 클릭 시 버튼 이름에 해당하는 씬으로 이동
    // 씬으로 완전히 이동하기 전에 loadManager를 이용해서 페이크 로딩을 이용해
    // 씬이 불완전한 상태에서 보여지는 것을 방지
    public void ClickMap(UnityEngine.UI.Button button)
    {
        Hunter.SetActive(true);
        mapPanel.SetActive(false);

        TileManager.SetActive(true);
        AIManager.SetActive(true);
        SceneName = button.name;
        loadManager.LoadScene(button.name);
    }


   // 이전단계의 성공여부를 이용해서 로비씬에서 버튼의 활성화를 판별
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

    // 씬 넘버를 이용하여 각 단계의 이전 단계 성공시 레벨리스트에 반영
    public void LevelUp()
    {
        int SceneNumber= SceneManager.GetActiveScene().buildIndex-1;
        Level[SceneNumber] = true;
        clearPanel.SetActive(false);
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
