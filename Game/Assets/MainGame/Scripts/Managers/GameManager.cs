using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Ÿ�� ����
    [SerializeField] GameObject[] Blocks;
    string[] SceneName = new string[7] { "Nature", "Island", "Desert", "City", "Winter", "Space", "Lobby" };

    // ���� �ҷ�����
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
        filePath = Application.persistentDataPath + "/GameData.json";  // ���� ��� ����
        LoadData();
    }

  


    //Ÿ�� ���� �Լ�
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
        if (File.Exists(filePath))  // ������ �����ϸ�
        {
            string json = File.ReadAllText(filePath);  // JSON ���� �б�
            GameData data = JsonUtility.FromJson<GameData>(json);  // JSON�� GameData ��ü�� ������ȭ

            curLevel = data.game.level;  // ����� level ��
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
        
        string json = JsonUtility.ToJson(data);  // GameData ��ü�� JSON���� ��ȯ
        File.WriteAllText(filePath, json);  // JSON�� ���Ϸ� ���� 
    }


}
