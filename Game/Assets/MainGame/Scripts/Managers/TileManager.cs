using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TileManager : MonoBehaviour
{
    [SerializeField] GameObject[] Blocks;
    private static TileManager instance;

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

    }

    // 각 씬에 맞는 블록타일을 찾아서 맵을 구성합니다.
    public void CreateTileMap()
    {
        int BlockNumber = FindAnyObjectByType<LevelManager>().GetComponent<LevelManager>().GetCurLevel() * 2;
       
        GameObject curTile = Blocks[BlockNumber];
        for (int i = 0; i < 8; i++)
        {

            for (int j = 0; j < 8; j++)
            {
                BlockNumber = BlockNumber % 2 == 0 ? BlockNumber + 1 : BlockNumber - 1;
                curTile = Blocks[BlockNumber];
                GameObject tile = Instantiate(curTile);
                tile.transform.position = new Vector3(i * 2, curTile.transform.position.y, j * 2);           
            }
        }
    }    
}
