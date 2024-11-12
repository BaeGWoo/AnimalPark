using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;




public class LoadingSceneManager : MonoBehaviour
{
    [SerializeField] Sprite[] loadingImages;
    [SerializeField] Image[] ducks;
    [SerializeField] Slider loadingBar;
    [SerializeField] GameObject loadingCanvas;

    [SerializeField] GameObject AIManager;
    [SerializeField] GameObject TileManager;
    [SerializeField] GameObject SoundManager;

    [SerializeField] Texture2D[] mouseImage;

    private AIManager aiManager;
    private TileManager tileManager;
    private SoundManager soundManager;

    private static LoadingSceneManager instance;
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
        aiManager = AIManager.GetComponent<AIManager>();
        tileManager = TileManager.GetComponent<TileManager>();
        soundManager = SoundManager.GetComponent<SoundManager>();
    }

   

    // 각 씬이 불완전하게 보여지는 것을 방지하기 위해 페이크로딩을 이용
    // 캔버스를 활성화 후 코루틴 함수 호출
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

        float changeInterval = 0.1f; // 0.5초마다 이미지 교체
        float nextChangeTime = changeInterval; // 다음 이미지 교체 시간

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(next);
        asyncLoad.allowSceneActivation = false;
        soundManager.MoveStage();
        //soundManager.BGMOff();
         
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
                aiManager.ActiveHintPanel();
                yield return new WaitForSeconds(0.5f);
                tileManager.CreateTileMap();
                aiManager.StartTurn();
                loadingBar.value = 0;
                loadingCanvas.SetActive(false);
                soundManager.BGMPlay();
            }
        }
    }

    public void MoveDucks(int index)
    {
        for(int i = 0; i < ducks.Length; i++)
        {
            ducks[i].sprite = loadingImages[index];
        }
    }
   
}
