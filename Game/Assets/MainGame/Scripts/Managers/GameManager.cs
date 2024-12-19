using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 타일 생성
    [SerializeField] GameObject[] Blocks;
    string[] SceneName = new string[7] { "Nature", "Island", "Desert", "City", "Winter", "Space", "Lobby" };

    // 게임 불러오기
    [SerializeField] int curLevel = -1;
    private string filePath;
    [System.Serializable]
    public class GameData
    {
        public Game game;
        public Monster monster;
    }

    [System.Serializable]
    public class Game
    {
        public int level;
    }

    [System.Serializable]
    public class Monster
    {
        public float health;
        public float dmg;
    }



    private void Awake()
    {
        filePath = Application.persistentDataPath + "/GameData.json";  // 파일 경로 설정
        LoadData();
    }

  


    //타일 생성 함수
    public void CreateTileMap()
    {   
        int BlockNumber = SceneManager.loadedSceneCount * 2;
        GameObject curTile = Blocks[BlockNumber];
        for (int i = 0; i < 8; i++)
        {
            BlockNumber = BlockNumber == SceneManager.loadedSceneCount * 2 ? BlockNumber + 1 : BlockNumber - 1;
            curTile = Blocks[BlockNumber];

            for (int j = 0; j < 8; j++)
            {
                GameObject tile = Instantiate(curTile);
                tile.transform.position = new Vector3(i * 2, curTile.transform.position.y, j * 2);
                BlockNumber = BlockNumber == SceneManager.loadedSceneCount * 2 ? BlockNumber + 1 : BlockNumber - 1;

                curTile = Blocks[BlockNumber];
            }
        }
    }


    public void LoadData()
    {
        if (File.Exists(filePath))  // 파일이 존재하면
        {
            string json = File.ReadAllText(filePath);  // JSON 파일 읽기
            GameData data = JsonUtility.FromJson<GameData>(json);  // JSON을 GameData 객체로 역직렬화

            curLevel = data.game.level;  // 저장된 level 값
            Debug.Log(curLevel);
        }
        else
        {
            Debug.Log("No saved data found.");
        }
    }

    public void SaveData(int level)
    {
        GameData data = new GameData();
        data.game = new Game();
        data.game.level = level;
        
        string json = JsonUtility.ToJson(data);  // GameData 객체를 JSON으로 변환
        File.WriteAllText(filePath, json);  // JSON을 파일로 저장 
    }


}
