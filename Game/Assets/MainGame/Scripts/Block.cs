using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    
    [SerializeField] float curdistance;
    
    public Material redMaterial; // 빨간색 재질
    private Material originalMaterial; // 원래 재질
    private Renderer objectRenderer;
    
    public Vector3 curPostiion;
    
    public bool moveableArea = false;
    public float padding = 2; // x 축 간격
    public static GameObject targetblock;
   
    private float speed = 3.0f;

    private enum MoveStage { MovingX, MovingZ, Done }
    private MoveStage currentStage = MoveStage.MovingX;


    //public float distance;
    void Awake()
    {
        // 기본 Material값 저장
        objectRenderer = GetComponent<Renderer>();
        originalMaterial = objectRenderer.material;
        currentStage = MoveStage.Done;
    }

    private void Start()
    {
        curPostiion = transform.position;

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

        if (Hunter.fireball)
        {
            StartCoroutine(AttackAbleDirection());
        }

    }

    public IEnumerator MoveableArea()
    {
        float distance;
        distance = Math.Abs(Hunter.HunterPosition.x - curPostiion.x) + Math.Abs(Hunter.HunterPosition.z - curPostiion.z);
        //curdistance = distance;

        if (distance <= 4)
        {
            if (this.gameObject != targetblock)
            {
                objectRenderer.material = redMaterial;
                moveableArea = true;
                gameObject.tag = "MoveAbleBlock";
            }

        }
        yield return null; // 다음 프레임까지 대기
    }

    public IEnumerator AttackAbleDirection()
    {
        float distance;
        distance = Math.Abs(Hunter.HunterPosition.x - curPostiion.x) + Math.Abs(Hunter.HunterPosition.z - curPostiion.z);
        //curdistance = distance;
        if (distance <= 2)
        {
            if (this.gameObject != targetblock)
            {
                objectRenderer.material = redMaterial;
                gameObject.tag = "AttackAbleDirection";
            }

        }
        yield return null; // 다음 프레임까지 대기
    }


    public void ChooseAttackDirection()
    {
        if (Input.GetMouseButtonDown(0))
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.transform.gameObject;

                if (!(clickedObject.CompareTag("Block")))
                {
                    return;

                }


                // 클릭한 위치의 x와 z 좌표 추출
                Vector3 clickPosition = hit.point;
                float x = clickPosition.x;
                float z = clickPosition.z;
                int xIndex = Mathf.RoundToInt(x / padding);
                int zIndex = Mathf.RoundToInt(z / padding);
                
                // 블럭의 크기와 간격을 padding에 넣어 정확한 칸의 좌표로 떨어지게 설정
                Vector3 newPosition = new Vector3(xIndex * padding, clickedObject.transform.position.y, zIndex * padding);
                Hunter.HunterRotation.LookAt(newPosition);

                // Hunter에서 LookAt으로 인해 회전하는 것을 방지
                float targetYRotation = Hunter.HunterRotation.eulerAngles.y;
                Hunter.HunterRotation.rotation = Quaternion.Euler(0, targetYRotation % 360, 0);
                StartCoroutine(ReturnArea());

                Hunter.fireball = false;
                Hunter.chooseDirection = true;
               
            }
        }
    }


    public IEnumerator ReturnArea()
    {
        objectRenderer.material = originalMaterial;
        gameObject.tag = "Block";
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
                Debug.Log(hit.transform.position);

                if (clickedObject.CompareTag("Block"))
                {
                    if (!clickedObject.GetComponent<Block>().moveableArea)  return;
                }

                else  return;

                // 클릭한 위치의 x와 z 좌표 추출
                Vector3 clickPosition = hit.point;
                float x = clickPosition.x;
                float z = clickPosition.z;

                // 클릭한 위치에 가장 가까운 칸의 인덱스 계산
                int xIndex = Mathf.RoundToInt(x / padding);
                int zIndex = Mathf.RoundToInt(z / padding);

                // 새 위치 계산
                Vector3 newPosition = new Vector3(xIndex * padding, clickedObject.transform.position.y, zIndex * padding);
                float newDistance = Math.Abs(newPosition.x - curPostiion.x) + Math.Abs(newPosition.z - curPostiion.z);
                

                StartCoroutine(ReturnArea());
                StartCoroutine(MoveHunter(newPosition));
                
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

       
        //float yRotation = Mathf.Round(eulerAngles.y / 90) * 90;
        float yRotation = Hunter.HunterRotation.eulerAngles.y;

        // 회전값을 설정합니다.
        Hunter.HunterRotation.rotation = Quaternion.Euler(0, yRotation, 0);

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

        //float yRotation = Mathf.Round(eulerAngles.y / 90) * 90;
        float yRotation = Hunter.HunterRotation.eulerAngles.y;

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
        Hunter.Moveable = false;
        Hunter.Running = false;
    }

}
