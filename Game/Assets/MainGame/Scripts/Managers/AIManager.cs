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
    //private List<Animal> animals = new List<Animal>();
    public static int[,] TileMap = new int[8, 8];
    [SerializeField] Hunter hunter;
    [SerializeField] GameObject animalImagePrefab;
    [SerializeField] Canvas animalCanvas;
    [SerializeField] GameObject clearPanel;
    [SerializeField] int MaxAnimalHP = 3;
    int count = 5;
    


    private void Awake(){DontDestroyOnLoad(gameObject);}
    public void StartTurn(){StartCoroutine(ActiveAiManager());}

    private void Update()
    {
     
        if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("esc");
            hunter.PanelActive();
        }

       
    }

    IEnumerator ActiveAiManager()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject[] temp = GameObject.Find("AnimalManager").GetComponent<AnimalManager>().GetCurrentAnimals();
        Animals = new GameObject[temp.Length];
        for (int i = 0; i < Animals.Length; i++)
        {
            Animals[i] = temp[i];
        }
        StartCoroutine(TurnManager());
        UpdateAnimalList();
    }

    
    private IEnumerator TurnManager()
    {
        while (Hunter.Health>=0) // ������ ��� ����Ǵ� ���� �ݺ�
        {
            // ������ �̵�
            AnimalMove();
            yield return new WaitForSeconds(1.0f); // �ʿ信 ���� �ð� ����
            AnimalAttack();
            yield return new WaitForSeconds(1.0f);
            
            if (CheckAniamlAttack()){yield return new WaitForSeconds(3.0f);}

            else{yield return new WaitForSeconds(1.0f);}

            AnimalUnAttackBox();
          
            // Hunter�� �̵�
            HunterMove();

            while (Hunter.Moveable) { yield return null;}

            

            // Hunter�� ����           
            hunter.Attack();

            // Hunter�� �̵��� �����⸦ ���
            while (Hunter.Attackable)
            {
                AnimalActiveCollider(Hunter.chooseDirection);
                yield return null;
            }
        }
    }




    #region ���� ���� ����
    public void AnimalMove()
    {
        Vector3[] movePoints = null;
        Vector3[] moveDirections = null;
        Animator animator = null;


        for (int i = 0; i < Animals.Length; i++)
        {
            Animals[i].GetComponent<Animal>().Move();
            Animals[i].GetComponent<BoxCollider>().enabled = true;
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


        if (GetAnimalsCount() <= 0)
        {
            clearPanel.SetActive(true);

        }


        // ���� �̹��� �����տ��� �⺻ ��ġ�� �޾ƿ���
        //RectTransform prefabRectTransform = animalImagePrefab.GetComponent<RectTransform>();
        //Vector3 initialPosition = prefabRectTransform.localPosition;
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
            //RectTransform rectTransform = animalFace.GetComponent<RectTransform>();
            //rectTransform.localPosition += thumbnailPosition * i;
            animalThumbnail.GetComponent<RectTransform>().localPosition += thumbnailPosition * i;

            Slider hpSlider = animalThumbnail.GetComponentInChildren<Slider>();
            hpSlider.value = Animals[i].GetComponent<Animal>().GetHP()/ MaxAnimalHP;
        }

        
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
                Debug.Log(Animals[index].name);
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
        return Animals.Length;
    }

    #endregion

    public void HunterMove()
    {
        for(int i = 0; i < Animals.Length; i++)
        {
            Animals[i].GetComponent<BoxCollider>().enabled = false;
        }
        hunter.Move();
    }

  

    
}
