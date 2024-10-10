using UnityEngine;

public interface AnimalInterface
{
    void Move();

    public void Move1(Vector3 curPosition, Vector3[] movePoint, Vector3[] moveDirection)
    {
        AIManager.TileMap[(int)(curPosition.x / 2), (int)(curPosition.z / 2)] = 0;
        Vector3 target = Hunter.HunterPosition;
        //Vector3 curPosition = new Vector3(transform.position.x, 0, transform.position.z);

        float distance = 100;
        int minDirection = 0;



        // 이동할 위치 중 target과 가장 인접한 위치 찾기
        for (int i = 0; i < movePoint.Length; i++)
        {
            float temp;
            temp = Mathf.Abs((movePoint[i].x - target.x) + (movePoint[i].z - target.z));

            if (movePoint[i].x >= -0.01f && movePoint[i].x <= 14 && movePoint[i].z >= -0.01f && movePoint[i].z <= 14)
            {
                if (AIManager.TileMap[(int)(movePoint[i].x / 2), (int)(movePoint[i].z / 2)] != 1)
                {
                    if (temp < distance)
                    {
                        distance = temp;
                        minDirection = i;
                    }
                }
            }
        }
        AIManager.TileMap[(int)(movePoint[minDirection].x / 2), (int)(movePoint[minDirection].z) / 2] = 1;
        //StartCoroutine(JumpToPosition(new Vector3(movePoint[minDirection].x, 0, movePoint[minDirection].z), jumpDuration, jumpHeight));
        //JumpToPosition(new Vector3(movePoint[minDirection].x, 0, movePoint[minDirection].z), jumpDuration, jumpHeight);
    }
    void Attack();

}
