using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using static UnityEditor.Progress;

[System.Serializable]
public class AnimalData
{
    public string name;   // ������ �̸�
    public float Health;    // ü��
    public float AttackDMG;  // ���ݷ�
    public int SkillCount;
}

[System.Serializable]
public class AnimalDataArray
{
    public AnimalData[] animal;
}



public class AIManager : MonoBehaviour
{
    public AnimalData animalData;
    public Dictionary<string, List<float>> animalStatus=new Dictionary<string, List<float>>();
    public static Dictionary<string, GameObject[]> animalArray;
    private Dictionary<string, AnimalData> animalDictionary;

    [SerializeField] GameObject[] Animals;   
    
    [SerializeField] Hunter hunter;

    [SerializeField] GameObject animalImagePrefab;
    [SerializeField] Canvas animalCanvas;
    [SerializeField] GameObject clearPanel;
    [SerializeField] bool OnPlay;

    [SerializeField] GameObject SoundManager;
    private SoundManager soundManager;
 

    private void Awake()
    {
       //soundManager = SoundManager.GetComponent<SoundManager>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneOnLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneOnLoaded;
    }

    private void SceneOnLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine("SetStageData");
        OnPlay = true;
    }

    public void SetStageData(){
        TextAsset StatusList = Resources.Load<TextAsset>("AnimalStatus");

        if (animalStatus != null)
        {
            //AnimalDataList dataList = JsonUtility.FromJson<AnimalDataList>(StatusList.text);
            AnimalData[] animals = JsonUtility.FromJson<AnimalDataArray>(StatusList.ToString()).animal;
            // ���� ������ ����Ʈ�� Dictionary�� ����
            animalDictionary = new Dictionary<string, AnimalData>();

            foreach (var animal in animals)
            {
                animalDictionary.Add(animal.name, animal);
                //Debug.Log(animal.name);
            }

            foreach (var animal in Animals)
            {
                //Debug.Log(animal.name);
                if (animalDictionary.ContainsKey(animal.name))
                {
                    AnimalData animalData = animalDictionary[animal.name];
                    animal.GetComponent<Monster>().SetAnimalStatus(
                        animalData.AttackDMG,
                        animalData.Health,
                        animalData.SkillCount
                        );

                    //Debug.Log($"Found: {animalData.name}, Health: {animalData.Health}, Attack: {animalData.AttackDMG}, Skills: {animalData.SkillCount}");
                }
                Vector3 animalPosition=animal.transform.position;
                FindAnyObjectByType<TileManager>().GetComponent<TileManager>().insertTileMap(
                    (int)animalPosition.x / 2, (int)animalPosition.z / 2, 1);               
            }


        }
    }

    public void StartTurn()
    {
        StartCoroutine(ActiveAiManager());
    }


    IEnumerator ActiveAiManager()
    {      
        FindAnyObjectByType<TileManager>().GetComponent<TileManager>().InitialLizeTileMap();
        yield return null;

        UpdateAnimalList();
        StartCoroutine(TurnManager());
    }

    
    private IEnumerator TurnManager()
    {
        while (Hunter.Health>=0) 
        {
            AnimalAct();
            yield return new WaitForSeconds(1.0f);

            if (CheckAniamlAttack()) { yield return new WaitForSeconds(3.0f); }
            else{ yield return new WaitForSeconds(1.5f); }

           
            yield return new WaitForSeconds(0.5f);

            hunter.Move();
            while (Hunter.Moveable) { yield return null; }

            hunter.Attack();
            while (Hunter.Attackable) { yield return null; }
            

            if (Animals.Length <= 0)
            {
                OnPlay = false;
                break;
            }

            if (Hunter.Health <= 0)
            {
                break;
            }
        }
       
    }




   

    public void AnimalAct()
    {
        for (int i = 0; i < Animals.Length; i++)
        {
            UpdateAnimalPosition();
            Animals[i].GetComponent<Monster>().GetAttackAble();
            Animals[i].GetComponent<Monster>().AnimalAct();
        }
    }

    public void UpdateAnimalPosition()
    {
        for (int i = 0; i < Animals.Length; i++)
        {
            Animals[i].GetComponent<Monster>().UpdateAnimalPosition();
        }
    }


    public bool CheckAniamlAttack()
    {
        bool check = true;

        for (int i = 0; i < Animals.Length; i++)
        {
            if (Animals[i].GetComponent<Monster>().GetAttackAble())
                return false;
        }

        return check;
    }


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
            hpSlider.value = Animals[i].GetComponent<Monster>().GetHP()/ Animals[i].GetComponent<Monster>().GetMaxHp();
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

    public void AddAnimal(GameObject animal)
    {
        List<GameObject> animalList = new List<GameObject>(Animals);
        //animal.GetComponent<Animal>().SetAnimalStatus(animalStatus[animal.name][0], animalStatus[animal.name][1], (int)animalStatus[animal.name][2]);

        if (animalDictionary.ContainsKey(animal.name))
        {
            AnimalData animalData = animalDictionary[animal.name];
            animal.GetComponent<Monster>().SetAnimalStatus(
                animalData.Health,
                animalData.AttackDMG,
                animalData.SkillCount
                );

            //Debug.Log($"Found: {animalData.name}, Health: {animalData.Health}, Attack: {animalData.AttackDMG}, Skills: {animalData.SkillCount}");
        }


        animalList.Add(animal);

        Animals = animalList.ToArray();
        UpdateAnimalList();
    }


    public bool getTurnState()
    {
        return OnPlay;
    }
   


}
