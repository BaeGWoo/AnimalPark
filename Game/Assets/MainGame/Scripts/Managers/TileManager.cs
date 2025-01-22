using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class TileManager : MonoBehaviour
{
    [SerializeField] GameObject[] Blocks;
    [SerializeField] GameObject targetBlock;
    private static TileManager instance;
    [SerializeField] int[,] TileMap = new int[8, 8];
    [SerializeField] GameObject[,] TileBlocks=new GameObject[8,8];

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
        int BlockNumber = FindAnyObjectByType<LoadManager>().GetComponent<LoadManager>().GetCurLevel() * 2;
        
        GameObject curTile = Blocks[BlockNumber];
        for (int i = 0; i < 8; i++)
        {
            BlockNumber = BlockNumber % 2 == 0 ? BlockNumber + 1 : BlockNumber - 1;
            for (int j = 0; j < 8; j++)
            {
                BlockNumber = BlockNumber % 2 == 0 ? BlockNumber+1  : BlockNumber-1;
                curTile = Blocks[BlockNumber];
                GameObject tile = Instantiate(curTile);
                tile.transform.position = new Vector3(j * 2, curTile.transform.position.y, i * 2);
                TileBlocks[i,j] = tile;
            }
        }
    }


    
    public void InitialLizeTileMap()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                TileMap[i, j] = 0;
            }
        }
    }

    public bool CheckTileMap(int x, int y)
    {
        return TileMap[x, y]==0;
    }

    public void insertTileMap(int x, int y,int value)
    {
        TileMap[x, y] = value;
    }

    public void SetMoveableTile(Vector3 hunter,float moveDebuff)
    {
        float distance;
       
        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                distance = Math.Abs(hunter.x - i*2) + Math.Abs(hunter.z - j*2);
                if (distance <= (6-moveDebuff) && distance >= 1 && CheckTileMap(i, j))
                {
                    if (gameObject != targetBlock)
                    {
                        TileBlocks[j, i].GetComponent<Block>().SetMaterial(false);                  
                    }
                }
            }
        }        
    }

    public void SetAttackableTile(Vector3 hunter)
    {
        float distance;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                distance = Math.Abs(hunter.x - i * 2) + Math.Abs(hunter.z - j * 2);
                if (distance <= 4 && distance >= 1 && CheckTileMap(i, j))
                {
                    if (gameObject != targetBlock)
                    {
                        TileBlocks[j, i].GetComponent<Block>().SetMaterial(false);

                    }
                }

            }
        }
    }



    public void TileBlockReset()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                TileBlocks[i, j].GetComponent<Block>().SetMaterial(true);
            }
        }
    }

    public void PrintTileMap()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if(TileMap[i, j]==1)
                Debug.Log(i+" , "+j);
            }
        }
    }
}
