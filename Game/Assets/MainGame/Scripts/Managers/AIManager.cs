using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using UnityEngine.Windows;

public class AIManager : MonoBehaviour
{
    [SerializeField] GameObject[] Animals;
    public static int[,] TileMap = new int[8, 8];
    [SerializeField] Hunter hunter;
    [SerializeField] GameObject animalImagePrefab;
    [SerializeField] Canvas animalCanvas;
    [SerializeField] GameObject clearPanel;
    [SerializeField] int MaxAnimalHP = 3;
   
    private static AIManager instance;
    public static Dictionary<string, GameObject[]> animalArray;

    private void Awake()
    {
       
        if(instance == null)
        {
            instance = this;
            animalArray = new Dictionary<string, GameObject[]>();
        }

        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
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
        for (int i = 0; i < Animals.Length; i++)
        {
            StartCoroutine(AnimalMoveTurn(i));
        }
    }

    IEnumerator AnimalMoveTurn(int index)
    {
        Animals[index].GetComponent<Animal>().Move();
        Animals[index].GetComponent<BoxCollider>().enabled = true;

        while (Animals[index].GetComponent<Animal>().moveable)
        {
            yield return null;
        }

        yield return null;

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
        }
        UpdateAnimalList();
    }

    public void ResetAnimalList()
    {
        Animals = null; Animals = new GameObject[0];
    }

}
