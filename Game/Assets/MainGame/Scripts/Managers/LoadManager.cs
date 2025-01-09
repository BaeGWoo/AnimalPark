using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadManager : MonoBehaviour
{
    [SerializeField] Sprite[] loadingImages;
    [SerializeField] Image[] ducks;
    [SerializeField] Slider loadingBar;
    [SerializeField] GameObject loadingCanvas;

    [SerializeField] Texture2D[] mouseImage;
    [SerializeField] SoundManager soundManager;
  

    private static LoadManager instance;
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

        Cursor.SetCursor(mouseImage[0], Vector2.zero, CursorMode.Auto);
        
    }

   

    // �� ���� �ҿ����ϰ� �������� ���� �����ϱ� ���� ����ũ�ε��� �̿�
    // ĵ������ Ȱ��ȭ �� �ڷ�ƾ �Լ� ȣ��
    public void LoadScene(string sceneName)
    {
        loadingCanvas.SetActive(true);
        StartCoroutine(LoadNextScene(sceneName));
        
        if (sceneName== "Lobby")
        {
            Cursor.SetCursor(mouseImage[0], Vector2.zero, CursorMode.Auto);
        }

        else
        {
            Cursor.SetCursor(mouseImage[1], Vector2.zero, CursorMode.Auto);
        }
    }


    IEnumerator LoadNextScene(string next)
    {
        float loadTime = 2f;
        float elapsedTime = 0f;

        float changeInterval = 0.1f; //�ε� �� �̹��� ��ȯ �ӵ�
        float nextChangeTime = changeInterval; 

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Island");
        asyncLoad.allowSceneActivation = false;
        //soundManager.MoveStage(); // �ε�BGM ����
         
        // �ε� �� ����� �����̴� �̹��� ����
        while (elapsedTime < loadTime)
        {
            yield return null;
            elapsedTime += Time.deltaTime;

            MoveDucks((int)(elapsedTime / changeInterval) % loadingImages.Length);
            nextChangeTime += changeInterval;

            loadingBar.value = Mathf.Lerp(loadingBar.value, elapsedTime / loadTime, Time.deltaTime); 
        }


        while (!asyncLoad.isDone)
        {
            yield return null;

            // ����� 0.9 �̸��� ��
            if (asyncLoad.progress < 0.9f)
            {
                loadingBar.value = asyncLoad.progress;
            }
            else
            {
                // ������ �ε� �Ϸ�
                loadingBar.value = 1f;

                // �� ��ȯ ���
                asyncLoad.allowSceneActivation = true;
                //aiManager.ActiveHintPanel();
                yield return new WaitForSeconds(0.5f);


                FindAnyObjectByType<TileManager>().GetComponent<TileManager>().CreateTileMap();
                FindAnyObjectByType<Hunter>().GetComponent<Hunter>().SetInitialState();
                FindAnyObjectByType<Hunter>().GetComponent<Hunter>().setCamera(Camera.main);
                //aiManager.StartTurn();
                FindAnyObjectByType<AIManager>().GetComponent<AIManager>().StartTurn();
                FindAnyObjectByType<SoundManager>().GetComponent<SoundManager>().MoveStage();
                FindAnyObjectByType<SoundManager>().GetComponent<SoundManager>().SetBGMPlayer();
                loadingBar.value = 0;
                loadingCanvas.SetActive(false);
                FindAnyObjectByType<SoundManager>().GetComponent<SoundManager>().BGMPlay();
            }
        }
    }


    public void ReLoadScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        ExitScene();       
        LoadScene(sceneName);
    }


    public void ExitScene()
    {
        //aiManager.ResetAnimalList();
        //AIManager.SetActive(false);

        SceneManager.LoadScene("Lobby");
        //soundManager.EnterLobby();

        // Hunter �ʱ�ȭ
        FindAnyObjectByType<Hunter>().GetComponent<Hunter>().SetInitialState();
    }



    public void MoveDucks(int index)
    {
        for(int i = 0; i < ducks.Length; i++)
        {
            ducks[i].sprite = loadingImages[index];
        }
    }
   
}
