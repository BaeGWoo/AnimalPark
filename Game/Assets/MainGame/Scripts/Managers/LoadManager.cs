using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] GameObject ClearPanel;
    [SerializeField] GameObject HintPanel;

    [SerializeField] bool stageTrigger;
    [SerializeField] int curLevel;
    [SerializeField] int imageNumber;
    [SerializeField] Sprite[] stageImage;
    [SerializeField] Image levelImage;
    [SerializeField] GameObject[] state;
    [SerializeField] Button LeftButton;
    [SerializeField] Button RightButton;
    [SerializeField] Sprite[] HintList;
    [SerializeField] Image hintImage;
    [SerializeField] string[] sceneName;
    [SerializeField] AIManager aiManager;
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
        stageTrigger = false;
        LeftButton.interactable = false;
        sceneName =new string[]{ "GameStart","Nature","Island","Dessert","City","Space"};
    }

    private void Update()
    {
        if(aiManager == null)
        {
            aiManager = FindAnyObjectByType<AIManager>();
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PausePanel.SetActive(true);
        }

        if (!stageTrigger)
        {
            if (aiManager != null)
            {
                if (!aiManager.GetComponent<AIManager>().getTurnState())
                {
                    ClearPanel.SetActive(true);
                    stageTrigger = true;
                }


                if (Hunter.Health <= 0)
                {
                    StartCoroutine(PausePanelOn());
                    stageTrigger = true;
                }
            }           
        }
    }

  

    public void LoadScene(int level)
    {
        FindAnyObjectByType<SoundManager>().GetComponent<SoundManager>().BGMOff();
        //ClearPanel.SetActive(false);
        PausePanel.SetActive(false);
        // 로비로 이동
        if (level == 0)
        {
            StartCoroutine(LoadNextScene(sceneName[0]));
            LobbyCanvas.SetActive(true);
            Cursor.SetCursor(mouseImage[0], Vector2.zero, CursorMode.Auto);
            levelImage.sprite = stageImage[curLevel];
            imageNumber = curLevel;
            state[1].SetActive(true);
            //PausePanel.SetActive(false);

        }

        else
        {
            LobbyCanvas.SetActive(false);
            loadingCanvas.SetActive(true);
            Cursor.SetCursor(mouseImage[1], Vector2.zero, CursorMode.Auto);
            StartCoroutine(LoadNextScene(sceneName[curLevel+1]));
        }
       
    }

    public void LevelUpLoadScene(int state)
    {
        curLevel++;
        ClearPanel.SetActive(false);

        LoadScene(state);
        imageNumber = curLevel;
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

                int index = SceneManager.GetActiveScene().buildIndex;
                if (index > 0)
                {
                    FindAnyObjectByType<TileManager>().GetComponent<TileManager>().CreateTileMap();
                    //FindAnyObjectByType<Hunter>().GetComponent<Hunter>().SetInitialState();
                    //FindAnyObjectByType<Hunter>().GetComponent<Hunter>().setCamera(Camera.main);
                    //aiManager.StartTurn();
                    FindAnyObjectByType<AIManager>().GetComponent<AIManager>().StartTurn();
                    FindAnyObjectByType<SoundManager>().GetComponent<SoundManager>().MoveStage();
                }
                FindAnyObjectByType<SoundManager>().GetComponent<SoundManager>().SetBGMPlayer();
                loadingBar.value = 0;
                loadingCanvas.SetActive(false);
                FindAnyObjectByType<SoundManager>().GetComponent<SoundManager>().BGMPlay();
                stageTrigger = false;
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
            RightButton.interactable = true;
            if (imageNumber > 0)
            {
                levelImage.sprite = stageImage[--imageNumber];
                if (imageNumber == 0)
                    LeftButton.interactable = false;
            }
        }

        else if (button.name == "Right")
        {
            LeftButton.interactable = true;
            if (imageNumber < stageImage.Length-1)
            {
                levelImage.sprite = stageImage[++imageNumber];
                if (imageNumber == stageImage.Length - 1)
                    RightButton.interactable = false;
            }
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


    IEnumerator PausePanelOn()
    {
        yield return new WaitForSeconds(1.0f);
        FindAnyObjectByType<SoundManager>().GetComponent<SoundManager>().BGMOff();

        PausePanel.SetActive(true);
    }

    public void CloseHintPanel()
    {
        HintPanel.SetActive(false);
    }

    public void OpenHintPanel()
    {
        HintPanel.SetActive(true);
        hintImage.sprite = HintList[curLevel*2];
    }
}
