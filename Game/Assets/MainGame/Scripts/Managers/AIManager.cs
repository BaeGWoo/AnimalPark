using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using UnityEngine.Windows;
using static UnityEngine.EventSystems.EventTrigger;

[System.Serializable]
public class AnimalData
{
    public string name;   // ������ �̸�
    public float Health;    // ü��
    public float Attack;  // ���ݷ�
}

[System.Serializable]
public class AnimalDataList
{
    public List<AnimalData> animal;  // ���� ���� ������ ����Ʈ
}



public class AIManager : MonoBehaviour
{
    public AnimalData animalData;
    public Dictionary<string, List<float>> animalStatus=new Dictionary<string, List<float>>();
    [SerializeField] GameObject[] Animals;
    public static int[,] TileMap = new int[8, 8];
    [SerializeField] Hunter hunter;
    [SerializeField] GameObject animalImagePrefab;
    [SerializeField] Canvas animalCanvas;
    [SerializeField] GameObject clearPanel;
    [SerializeField] int MaxAnimalHP = 3;


    public static bool animalHintPanelActive = false;

    [SerializeField] GameObject SoundManager;

    private static AIManager instance;
    private SoundManager soundManager;
    public static Dictionary<string, GameObject[]> animalArray;

    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
            animalArray = new Dictionary<string, GameObject[]>();
        }

        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        TextAsset StatusList = Resources.Load<TextAsset>("AnimalStatus");
        soundManager = SoundManager.GetComponent<SoundManager>();

        if (animalStatus != null)
        {
            AnimalDataList dataList = JsonUtility.FromJson<AnimalDataList>(StatusList.text);

            // ���� ������ ����Ʈ�� Dictionary�� ����
            foreach (var animal in dataList.animal)
            {
                // �� ������ �̸��� Ű��, ü�°� ���ݷ��� ����Ʈ�� ����
                List<float> status = new List<float> { animal.Attack, animal.Health };
                animalStatus[animal.name] = status; // �̸��� Ű�� ����Ͽ� ���� ������ ����
            }

           
        }
    }

    public void ActiveHintPanel()
    {
        animalHintPanelActive = true;
    }

    public void HintPanelOff(GameObject panel)
    {
        animalHintPanelActive = false;
        panel.SetActive(false);
    }

    public void StartTurn(){StartCoroutine(ActiveAiManager());}



    private void Update()
    {
     
        if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
        {
            hunter.PanelActive();
        }
       
    }


    

    IEnumerator ActiveAiManager()
    {
        while (animalHintPanelActive)
        {
            yield return null;
        }

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                TileMap[i, j] = 0;
            }
        }
        yield return null;
        UpdateAnimalList();
        StartCoroutine(TurnManager());
    }

    
    private IEnumerator TurnManager()
    {
       
       
        while (Hunter.Health>=0) 
        {
            // ������ �̵�

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    TileMap[i, j] = 0;
                }
            }
            yield return null;

            AnimalMove();
            yield return new WaitForSeconds(1.0f); // �ʿ信 ���� �ð� ����

            AnimalAttack();
            yield return new WaitForSeconds(1.0f);
            
            if (CheckAniamlAttack()){yield return new WaitForSeconds(3.0f);}

            else{ yield return new WaitForSeconds(1.5f); }

            AnimalUnAttackBox();
          
            // Hunter�� �̵�
            HunterMove();

            while (Hunter.Moveable) {  yield return null;}

           

            // Hunter�� ����           
            hunter.Attack();

            // Hunter�� �̵��� �����⸦ ���
            while (Hunter.Attackable)
            {
                yield return null;
            }

            if (Animals.Length <= 0||Hunter.Health<=0)
            {
                break;
            }

          

        }
    }

    public void HunterMove()
    {
        for (int i = 0; i < Animals.Length; i++)
        {
            Animals[i].GetComponent<BoxCollider>().enabled = false;
        }
        hunter.Move();
    }



    #region ���� ���� ����
    public void AnimalMove()
    {
        int AnimalSize=GetAnimalsCount();
        for (int i = 0; i < AnimalSize; i++)
        {
            StartCoroutine(AnimalMoveTurn(i));
        }
    }

    IEnumerator AnimalMoveTurn(int index)
    {
        yield return null;

        if(index< Animals.Length)
        {
            Animals[index].GetComponent<Animal>().Move();
            Animals[index].GetComponent<BoxCollider>().enabled = true;
            while (index<Animals.Length&&Animals[index].GetComponent<Animal>().moveable)
            {
               
                yield return null;
            }
        }
        

        

    }

    public void AnimalUnAttackBox()
    {
        for (int i = 0; i < Animals.Length; i++)
        {
            Animals[i].GetComponent<Animal>().UnActiveAttackBox();
        }
    }

    public void AnimalActiveCollider(bool state)
    {
        for (int i = 0; i < Animals.Length; i++)
        {
            Animals[i].GetComponent<BoxCollider>().enabled = state;
        }
    }

    public void AnimalAttack()
    {
        for (int i = 0; i < Animals.Length; i++)
        {
            Animals[i].GetComponent<Animal>().ActiveAttackBox();
        }
    }


    // ���� ���ݰ����� ��ü Ž��
    public bool CheckAniamlAttack()
    {
        bool check = true;

        for (int i = 0; i < Animals.Length; i++)
        {
            if (Animals[i].GetComponent<Animal>().GetAttackAble())
                check = false;
        }

        return check;
    }
    #endregion


    #region AnimalCanvasUpdate

    public void UpdateAnimalList()
    {
        // ������ �ִ� ����� ��� ����
        foreach (Transform child in animalCanvas.transform)
        {
            if (child.CompareTag("Animal"))
            {
                Destroy(child.gameObject);  
            }
        }
       
        float offset = -145f; 
        Vector3 thumbnailPosition = new Vector3(0, offset, 0);

        // ���ο� ����Ʈ�� �°� ������ ����
        for (int i = 0; i < Animals.Length; i++)
        {
            // �� �ε��� ��ȣ�� �´� ���� ��������Ʈ �Ҵ�
            GameObject animalThumbnail = Instantiate(animalImagePrefab, animalCanvas.transform);
            string animalName = Regex.Replace(Animals[i].name, @"\d", "");
            animalThumbnail.transform.Find("Thumbnail").GetComponent<Image>().sprite = Resources.Load<Sprite>(animalName);

            // �� �ε����� ��ġ����
            animalThumbnail.GetComponent<RectTransform>().localPosition += thumbnailPosition * i;

            Slider hpSlider = animalThumbnail.GetComponentInChildren<Slider>();
            hpSlider.value = Animals[i].GetComponent<Animal>().GetHP()/ Animals[i].GetComponent<Animal>().GetMaxHp();
        }

        
    }

    public void ShowNext()
    {
        StartCoroutine(PopUpClearPanel());
    }

  IEnumerator PopUpClearPanel()
    {
        soundManager.SoundPlay("LevelUp");
        yield return new WaitForSeconds(1.5f);
        clearPanel.SetActive(true);
    }


    public void UpdateAnimalHp()
    {
        UpdateAnimalList();
        int index = 0;
        if(Animals.Length <= 0) { return; }
        foreach (Transform child in animalCanvas.transform)
        {
            if (child.CompareTag("Animal")&&index<Animals.Length)
            {
                Slider hpSlider = child.gameObject.GetComponentInChildren<Slider>();
                hpSlider.value = Animals[index++].GetComponent<Animal>().GetHP()/ MaxAnimalHP;
            }
        }
    }

    public void RemoveAnimal(GameObject animal)
    {
        // �迭�� ����Ʈ�� ��ȯ
        List<GameObject> animalList = new List<GameObject>(Animals);

        // �ش� ���� GameObject�� ����Ʈ���� ����
        if (animalList.Contains(animal))
        {
            animalList.Remove(animal);
        }

        // ����Ʈ�� �ٽ� �迭�� ��ȯ
        Animals = animalList.ToArray();
    }

    public int GetAnimalsCount()
    {
        Debug.Log(Animals.Length);
        return Animals.Length;
    }

    #endregion

   
    public void getAnimalList(GameObject[] array)
    {
        string sceneName = LevelManager.SceneName;
        if (sceneName == "Lobby") return;
       

      
        Animals = null;
        Animals=new GameObject[array.Length];

        for(int i = 0; i <array.Length; i++)
        {
            Animals[i]=array[i];
            Animals[i].GetComponent<Animal>().SetAnimalStatus(animalStatus[Animals[i].name][0], animalStatus[Animals[i].name][1]);
        }
        UpdateAnimalList();
    }

    public void ResetAnimalList()
    {
        Animals = null; Animals = new GameObject[0];
    }

}
