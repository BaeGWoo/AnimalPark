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

   


   // �����ܰ��� �������θ� �̿��ؼ� �κ������ ��ư�� Ȱ��ȭ�� �Ǻ�
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

    // �� �ѹ��� �̿��Ͽ� �� �ܰ��� ���� �ܰ� ������ ��������Ʈ�� �ݿ�
    public void LevelUp()
    {
        Level[curLevel++] = true;
        clearPanel.SetActive(false);
    }

   


    
   
}
