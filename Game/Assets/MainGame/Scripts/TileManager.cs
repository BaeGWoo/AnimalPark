using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField] static int[,] TileMap=new int[8,8];
    [SerializeField] GameObject BlackTile;
    [SerializeField] GameObject WhiteTile;
    private void Awake()
    {
        GameObject curTile= BlackTile;
        for(int i = 0; i < 8; i++)
        {
            curTile = curTile == BlackTile ? WhiteTile : BlackTile;
            for (int j = 0; j < 8; j++)
            {
              
                GameObject tile = Instantiate(curTile);
                tile.transform.position = new Vector3(i*2, 0.2f,j*2);
                curTile = curTile == BlackTile ? WhiteTile : BlackTile;
            }
        }
    }
}
