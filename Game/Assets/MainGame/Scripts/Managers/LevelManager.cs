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


    // ��ư Ŭ�� �� ��ư �̸��� �ش��ϴ� ������ �̵�
    // ������ ������ �̵��ϱ� ���� loadManager�� �̿��ؼ� ����ũ �ε��� �̿���
    // ���� �ҿ����� ���¿��� �������� ���� ����
    public void ClickMap(UnityEngine.UI.Button button)
    {
        Hunter.SetActive(true);
        mapPanel.SetActive(false);

        TileManager.SetActive(true);
        AIManager.SetActive(true);
        SceneName = button.name;
        loadManager.LoadScene(button.name);
    }


   // �����ܰ��� �������θ� �̿��ؼ� �κ������ ��ư�� Ȱ��ȭ�� �Ǻ�
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

    // �� �ѹ��� �̿��Ͽ� �� �ܰ��� ���� �ܰ� ������ ��������Ʈ�� �ݿ�
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
