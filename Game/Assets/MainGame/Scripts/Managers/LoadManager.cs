using System.Collections;
using UnityEditor;
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

    [SerializeField] GameObject LobbyCanvas;
    [SerializeField] GameObject PausePanel;
    [SerializeField] int curLevel;
    [SerializeField] int imageNumber;
    [SerializeField] Sprite[] stageImage;
    [SerializeField] Image levelImage;
    [SerializeField] GameObject[] state;
    [SerializeField] string[] sceneName;
    
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
        levelImage.sprite=stageImage[0];
        curLevel = 0;
        imageNumber = curLevel;
        state[1].SetActive(true);
        sceneName =new string[]{ "GameStart","Nature","Island","Dessert","City","Space"};
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PausePanel.SetActive(true);
        }
    }

    // 각 씬이 불완전하게 보여지는 것을 방지하기 위해 페이크로딩을 이용
    // 캔버스를 활성화 후 코루틴 함수 호출
    public void LoadScene(string sceneName)
    {
        loadingCanvas.SetActive(true);
        StartCoroutine(LoadNextScene(sceneName));
        
        if (sceneName== "Lobby")
        {
            LobbyCanvas.SetActive(true);
            Cursor.SetCursor(mouseImage[0], Vector2.zero, CursorMode.Auto);
            levelImage.sprite = stageImage[curLevel];
            imageNumber = curLevel;
            state[1].SetActive(true);
        }

        else
        {
            LobbyCanvas.SetActive(false);
            Cursor.SetCursor(mouseImage[1], Vector2.zero, CursorMode.Auto);
        }
    }

    public void curStage(int level)
    {
        // 로비로 이동
        if (level == 0)
        {
            LobbyCanvas.SetActive(true);
            Cursor.SetCursor(mouseImage[0], Vector2.zero, CursorMode.Auto);
            levelImage.sprite = stageImage[curLevel];
            imageNumber = curLevel;
            state[1].SetActive(true);
            PausePanel.SetActive(false);
        }

        else
        {
            loadingCanvas.SetActive(true);
            Cursor.SetCursor(mouseImage[1], Vector2.zero, CursorMode.Auto);
            StartCoroutine(LoadNextScene(sceneName[curLevel+1]));

            LobbyCanvas.SetActive(false);
        }
       
    }




    IEnumerator LoadNextScene(string next)
    {
        float loadTime = 2f;
        float elapsedTime = 0f;

        float changeInterval = 0.1f; //로딩 시 이미지 전환 속도
        float nextChangeTime = changeInterval; 

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(next);
        asyncLoad.allowSceneActivation = false;
        //soundManager.MoveStage(); // 로딩BGM 설정
         
        // 로딩 시 사용할 움직이는 이미지 설정
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

            // 진행률 0.9 미만일 때
            if (asyncLoad.progress < 0.9f)
            {
                loadingBar.value = asyncLoad.progress;
            }
            else
            {
                // 마지막 로딩 완료
                loadingBar.value = 1f;

                // 씬 전환 허용
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


    public void stageImageChange(Button button)
    {
        for (int i = 0; i < state.Length; i++)
        {
            state[i].SetActive(false);
        }

        if (button.name == "Left")
        {
            if (imageNumber > 0)
                levelImage.sprite = stageImage[--imageNumber];
        }

        else if (button.name == "Right")
        {
            if (imageNumber < stageImage.Length-1)
                levelImage.sprite = stageImage[++imageNumber];
        }

        if (imageNumber < curLevel)
        {
            state[0].SetActive(true);
        }

        else if (imageNumber == curLevel)
        {
            state[1].SetActive(true);
        }

        else
        {
            state[2].SetActive(true);
        }

    }

    public void ClosePausePanel()
    {
        PausePanel.SetActive(false);
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

        // Hunter 초기화
        FindAnyObjectByType<Hunter>().GetComponent<Hunter>().SetInitialState();
    }



    public void MoveDucks(int index)
    {
        for(int i = 0; i < ducks.Length; i++)
        {
            ducks[i].sprite = loadingImages[index];
        }
    }

    public int GetCurLevel()
    {
        return curLevel;
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

}
