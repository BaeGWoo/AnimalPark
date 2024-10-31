using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TileManager : MonoBehaviour
{
    [SerializeField] static int[,] TileMap=new int[8,8];
    [SerializeField] GameObject[] Blocks;
    
    public Dictionary<string, int> SceneNumber;
    private int sceneNumber = -1;
    string[] SceneName = new string[7] { "Nature", "Island", "Desert", "City", "Winter", "Space", "Lobby" };

    [SerializeField] GameObject BlackTile;
    [SerializeField] GameObject WhiteTile;

    private static TileManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            SceneNumber = new Dictionary<string, int>();
            for (int i = 0; i < SceneName.Length; i++)
            {
                SceneNumber[SceneName[i]] = i;

            }

        }

        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);


        
    }

   

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene,LoadSceneMode mode)
    {
       
        sceneNumber = SceneNumber[LevelManager.SceneName];
        if (sceneNumber < 6)
        {
            CreateTileMap();
        }

        EventSystem eventSystem = FindObjectOfType<EventSystem>();

        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystem = eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }
    }


    public void CreateTileMap()
    {
        sceneNumber = SceneNumber[LevelManager.SceneName];
        int BlockNumber = sceneNumber * 2;
        GameObject curTile = Blocks[BlockNumber];
        for (int i = 0; i < 8; i++)
        {
            BlockNumber = BlockNumber == sceneNumber*2 ? BlockNumber + 1 : BlockNumber - 1;
            curTile = Blocks[BlockNumber];

            for (int j = 0; j < 8; j++)
            {
                GameObject tile = Instantiate(curTile);
                tile.transform.position = new Vector3(i * 2, curTile.transform.position.y, j * 2);
                BlockNumber = BlockNumber == sceneNumber * 2 ? BlockNumber + 1 : BlockNumber - 1;

                curTile = Blocks[BlockNumber];
            }
        }


    }
}
