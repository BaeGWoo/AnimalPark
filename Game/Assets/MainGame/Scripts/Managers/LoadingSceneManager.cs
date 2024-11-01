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
        StartCoroutine(LoadNextScene(FindAnyObjectByType<LevelManager>().GetComponent<LevelManager>().GetSceneName()));
    }

    private void Start()
    {
     //   StartCoroutine(LoadNextScene("Lobby"));
    }


    IEnumerator LoadNextScene(string next)
    {
        float loadTime = 3f;
        float elapsedTime = 0f;

        float changeInterval = 0.1f; // 0.5초마다 이미지 교체
        float nextChangeTime = changeInterval; // 다음 이미지 교체 시간

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(next);
        asyncLoad.allowSceneActivation = false;


        while (elapsedTime < loadTime)
        {
            yield return null;
            elapsedTime += Time.deltaTime;

            MoveDucks((int)(elapsedTime / changeInterval) % loadingImages.Length);
            nextChangeTime += changeInterval; // 다음 교체 시간을 업데이트

            // 로딩 바 업데이트
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
