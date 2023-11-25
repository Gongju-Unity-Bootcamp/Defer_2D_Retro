using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    [Header("Player")]
    public Transform player;
    public Player_Controller PC;
    public Player_Health PH;

    [Header("Offsets")]
    public Vector3 targetVector;
    public float offsetX = 0;
    public float offsetY = 0;
    public float offsetZ = 0;

    [Header("Speed")]
    public float cameraSpeed = 8.0f;

    [Header("Area Limits")]
    public float minX = -150.0f;
    public float maxX = 150.0f;
    public float minY = -50.0f;
    public float maxY = 50.0f;

    // Start is called before the first frame update
    void Start()
    {
        // 플레이어의 앉음 여부를 가져오기 위함
        PC = FindObjectOfType<Player_Controller>();
        PH = FindObjectOfType<Player_Health>();
    }

    // Update는 매 프레임마다 호출
    // FixedUpdate는 Fixed Timestep에 설정된 값에 따라 일정한 간격으로 호출
    // 불규칙한 Update와 달리 일정하게 호출됨
    void FixedUpdate()
    {
        if (!PH.isDead)
        {
            // 카메라의 위치 = 플레이어의 위치 좌표 + 오프셋
            // Mathf.Clamp로 카메라의 영역을 제한
            targetVector = new Vector3(Mathf.Clamp(player.position.x + offsetX, minX, maxX), Mathf.Clamp(player.position.y + 2.5f + offsetY, minY, maxY), -10f + offsetZ);


            // 카메라의 위치를 targetVector로 부드럽게 이동
            transform.position = Vector3.Lerp(transform.position, targetVector, Time.deltaTime * cameraSpeed);

            OffsetCtrl();
        }
    }

    /// <summary>
    /// 키 입력에 따른 카메라 오프셋 변화 함수
    /// </summary>
    public void OffsetCtrl()
    {
        if (!PC.isHit)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                offsetX = -2;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                offsetX = 2;
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                offsetY = 3f;
            }
            else
            {
                offsetY = 0;
            }

            if (Input.GetKey(KeyCode.DownArrow) && PC.isCrouch)
            {
                offsetZ = 2.5f;
            }
            else
            {
                offsetZ = 0;
            }
        }
    }

    // OnDrawGizmos 함수를 이용하여 제한 영역 표시
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // 상단 가로선
        Gizmos.DrawLine(new Vector3(minX, maxY, 0), new Vector3(maxX, maxY, 0));
        // 하단 가로선
        Gizmos.DrawLine(new Vector3(minX, minY, 0), new Vector3(maxX, minY, 0));
        // 좌측 세로선
        Gizmos.DrawLine(new Vector3(minX, minY, 0), new Vector3(minX, maxY, 0));
        // 우측 세로선
        Gizmos.DrawLine(new Vector3(maxX, minY, 0), new Vector3(maxX, maxY, 0));
    }
}
