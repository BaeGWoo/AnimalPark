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
    public string name;   // 동물의 이름
    public float Health;    // 체력
    public float Attack;  // 공격력
}

[System.Serializable]
public class AnimalDataList
{
    public List<AnimalData> animal;  // 여러 동물 데이터 리스트
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

            // 동물 데이터 리스트를 Dictionary에 저장
            foreach (var animal in dataList.animal)
            {
                // 각 동물의 이름을 키로, 체력과 공격력을 리스트로 저장
                List<float> status = new List<float> { animal.Attack, animal.Health };
                animalStatus[animal.name] = status; // 이름을 키로 사용하여 상태 정보를 저장
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
            // 동물의 이동

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    TileMap[i, j] = 0;
                }
            }
            yield return null;

            AnimalMove();
            yield return new WaitForSeconds(1.0f); // 필요에 따라 시간 조정

            AnimalAttack();
            yield return new WaitForSeconds(1.0f);
            
            if (CheckAniamlAttack()){yield return new WaitForSeconds(3.0f);}

            else{ yield return new WaitForSeconds(1.5f); }

            AnimalUnAttackBox();
          
            // Hunter의 이동
            HunterMove();

            while (Hunter.Moveable) {  yield return null;}

           

            // Hunter의 공격           
            hunter.Attack();

            // Hunter의 이동이 끝나기를 대기
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



    #region 동물 동작 제어
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


    // 현재 공격가능한 개체 탐색
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
        // 기존에 있던 썸네일 모두 삭제
        foreach (Transform child in animalCanvas.transform)
        {
            if (child.CompareTag("Animal"))
            {
                Destroy(child.gameObject);  
            }
        }
       
        float offset = -145f; 
        Vector3 thumbnailPosition = new Vector3(0, offset, 0);

        // 새로운 리스트에 맞게 프리팹 생성
        for (int i = 0; i < Animals.Length; i++)
        {
            // 각 인덱스 번호에 맞는 동물 스프라이트 할당
            GameObject animalThumbnail = Instantiate(animalImagePrefab, animalCanvas.transform);
            string animalName = Regex.Replace(Animals[i].name, @"\d", "");
            animalThumbnail.transform.Find("Thumbnail").GetComponent<Image>().sprite = Resources.Load<Sprite>(animalName);

            // 각 인덱스별 위치설정
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
        // 배열을 리스트로 변환
        List<GameObject> animalList = new List<GameObject>(Animals);

        // 해당 동물 GameObject를 리스트에서 삭제
        if (animalList.Contains(animal))
        {
            animalList.Remove(animal);
        }

        // 리스트를 다시 배열로 변환
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
