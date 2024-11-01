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

        float changeInterval = 0.1f; // 0.5�ʸ��� �̹��� ��ü
        float nextChangeTime = changeInterval; // ���� �̹��� ��ü �ð�

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(next);
        asyncLoad.allowSceneActivation = false;


        while (elapsedTime < loadTime)
        {
            yield return null;
            elapsedTime += Time.deltaTime;

            MoveDucks((int)(elapsedTime / changeInterval) % loadingImages.Length);
            nextChangeTime += changeInterval; // ���� ��ü �ð��� ������Ʈ

            // �ε� �� ������Ʈ
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
