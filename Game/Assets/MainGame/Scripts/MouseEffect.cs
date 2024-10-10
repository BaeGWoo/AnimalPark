using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MouseEffect : MonoBehaviour
{
    public Material redMaterial; // 빨간색 재질
    private Material originalMaterial; // 원래 재질
    private Renderer objectRenderer;
    public Vector3 curPostiion;
    public bool moveableArea = false;
    public float xSpacing = 2; // x 축 간격
    public float zSpacing = 2;  // z 축 간격
    public static GameObject targetblock;
    public float padding = 0;

    private float speed = 3.0f;
    private enum MoveStage { MovingX, MovingZ, Done }
    private MoveStage currentStage = MoveStage.MovingX;


    [SerializeField] float curdistance;
    //public float distance;
    void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        originalMaterial = objectRenderer.material;
        currentStage = MoveStage.Done;

    }

    private void Start()
    {
        curPostiion = transform.position;
        if (CompareTag("BlackTile"))
        {
            padding = 1.8f;
        }
       
    }

    void Update()
    {
        if (Hunter.Moveable && !Hunter.Running)
        {
            StartCoroutine(MoveableArea());
        }

        else
        {
            StartCoroutine(ReturnArea());
        }
        
    }

    public IEnumerator MoveableArea()
    {
        float distance;
        distance = Math.Abs(Hunter.HunterPosition.x - (curPostiion.x - padding)) + Math.Abs(Hunter.HunterPosition.z - curPostiion.z);
        curdistance = distance;
        if (distance <= 4)
        {
            if (this.gameObject != targetblock)
            {
                objectRenderer.material = redMaterial;
                moveableArea = true;

                moveHunterPosition();
            }

        }
        yield return null; // 다음 프레임까지 대기
    }

    public IEnumerator ReturnArea()
    {
        objectRenderer.material = originalMaterial;
        moveableArea = false;
        yield return null; // 다음 프레임까지 대기
    }

    public void moveHunterPosition()
    {
        if (Input.GetMouseButtonDown(0))
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.transform.gameObject;
                Debug.Log(clickedObject);
                if (clickedObject.CompareTag("map") || clickedObject.CompareTag("BlackTile"))
                {
                    if (!clickedObject.GetComponent<MouseEffect>().moveableArea || clickedObject == targetblock)
                    {
                        Debug.Log("not Moveable or targetblock");
                        return;
                    }

                }
                else
                {
                    Debug.Log(clickedObject.tag);
                    return;
                }
                // 클릭한 위치의 x와 z 좌표 추출
                Vector3 clickPosition = hit.point;
                float x = clickPosition.x;
                float z = clickPosition.z;

                // 클릭한 위치에 가장 가까운 칸의 인덱스 계산
                int xIndex = Mathf.RoundToInt(x / xSpacing);
                int zIndex = Mathf.RoundToInt(z / zSpacing);

                // 새 위치 계산
                Vector3 newPosition = new Vector3(xIndex * xSpacing, clickedObject.transform.position.y, zIndex * zSpacing);
                float newDistance = Math.Abs(newPosition.x - curPostiion.x - padding) + Math.Abs(newPosition.z - curPostiion.z);
                // 오브젝트 위치 이동

                StartCoroutine(ReturnArea());
                StartCoroutine(MoveHunter(newPosition));
                //Hunter.HunterPosition = newPosition;
                //Hunter.Moveable = false;
                targetblock = clickedObject;

            }
        }
    }

    private IEnumerator MoveHunter(Vector3 newPosition)
    {
        // X 좌표로 이동
        yield return StartCoroutine(MoveToX(newPosition.x));

        // Z 좌표로 이동
        yield return StartCoroutine(MoveToZ(newPosition.z));

        // 모든 이동이 끝난 후 실행할 다음 행동
        OnReachedDestination();
    }

    private IEnumerator MoveToX(float targetX)
    {
        Hunter.Running = true;
        Vector3 startPosition = Hunter.HunterPosition;
        currentStage = MoveStage.MovingX;
        Vector3 targetPosition = new Vector3(targetX, Hunter.HunterPosition.y, Hunter.HunterPosition.z);
        Hunter.HunterRotation.LookAt(targetPosition);
        Vector3 eulerAngles = Hunter.HunterRotation.eulerAngles;

        // 각도 값을 0~360 범위로 변환합니다.
        float yRotation = Mathf.Round(eulerAngles.y / 90) * 90;

        // 회전값을 설정합니다.
        Hunter.HunterRotation.rotation = Quaternion.Euler(eulerAngles.x, yRotation, eulerAngles.z);

        while (Mathf.Abs(Hunter.HunterPosition.x - targetX) > Mathf.Epsilon)
        {
            float step = speed * Time.deltaTime;
            float newX = Mathf.MoveTowards(Hunter.HunterPosition.x, targetX, step);
            Hunter.HunterPosition = new Vector3(newX, Hunter.HunterPosition.y, Hunter.HunterPosition.z);
            yield return null; // 다음 프레임까지 대기
        }
    }

    private IEnumerator MoveToZ(float targetZ)
    {
        Vector3 startPosition = Hunter.HunterPosition;
        currentStage = MoveStage.MovingZ;
        Vector3 targetPosition = new Vector3(Hunter.HunterPosition.x, Hunter.HunterPosition.y, targetZ);
        Hunter.HunterRotation.LookAt(targetPosition);
        Vector3 eulerAngles = Hunter.HunterRotation.eulerAngles;

        // 각도 값을 0~360 범위로 변환합니다.
        float yRotation = Mathf.Round(eulerAngles.y / 90) * 90;

        // 회전값을 설정합니다.
        Hunter.HunterRotation.rotation = Quaternion.Euler(eulerAngles.x, yRotation, eulerAngles.z);

        while (Mathf.Abs(Hunter.HunterPosition.z - targetZ) > Mathf.Epsilon)
        {
            float step = speed * Time.deltaTime;
            float newZ = Mathf.MoveTowards(Hunter.HunterPosition.z, targetZ, step);
            Hunter.HunterPosition = new Vector3(Hunter.HunterPosition.x, Hunter.HunterPosition.y, newZ);
            yield return null; // 다음 프레임까지 대기
        }
    }

    private void OnReachedDestination()
    {
        currentStage = MoveStage.Done;
        Debug.Log("목표 위치에 도달했습니다!");
        // 다음 행동을 여기에 구현하세요.
        Hunter.Moveable = false;
        Hunter.Running = false;
    }



}
