using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    [SerializeField] GameObject CreditPanel;
    [SerializeField] GameObject trainObject;
    private float trainSpeed = 3.5f;
    private bool GameEnd = false;
    // Start is called before the first frame update
    private void Update()
    {
        if (GameEnd)
        {
            if(Input.GetKeyUp(KeyCode.Space)) {
                Debug.Log("END");
                Quit();
            }
        }
    }

    public void TrainStart()
    {
        StartCoroutine(trainAway());
    }

    IEnumerator trainAway()
    {
        StartCoroutine(trainMove());
        yield return null;
       
    }


    IEnumerator trainMove()
    {

        while (trainObject.transform.position.x <= 30)
        {
            trainObject.transform.position = new Vector3
                (trainObject.transform.position.x + (trainSpeed * Time.deltaTime), trainObject.transform.position.y, trainObject.transform.position.z);
            yield return null;
        }
        yield return null;
        CreditPanel.SetActive(true);
        GameEnd = true;
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else 
    Application.Quit();
#endif
    }
}
