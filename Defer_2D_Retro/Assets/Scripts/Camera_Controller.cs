using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    [Header("Player")]
    public Transform player;
    public Player_Controller PC;

    [Header("Offsets")]
    public Vector3 targetVector;
    public float offsetX = 0;
    public float offsetY = 0;
    public float offsetZ = 0;

    [Header("Speed")]
    public float cameraSpeed = 8.0f;

    // Start is called before the first frame update
    void Start()
    {
        // 플레이어의 앉음 여부를 가져오기 위함
        PC = FindObjectOfType<Player_Controller>();
    }

    // Update는 매 프레임마다 호출
    // FixedUpdate는 Fixed Timestep에 설정된 값에 따라 일정한 간격으로 호출
    // 불규칙한 Update와 달리 일정하게 호출되므로 물리효과 등을 조정할 때 사용됨
    void FixedUpdate()
    {
        // 카메라의 위치 = 플레이어의 위치 좌표 + 오프셋
        targetVector = new Vector3(player.position.x + offsetX, player.position.y + 2.5f + offsetY, -10f + offsetZ);

        // 카메라의 위치를 targetVector로 부드럽게 이동
        transform.position = Vector3.Lerp(transform.position, targetVector, Time.deltaTime * cameraSpeed);

        OffsetCtrl();
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
                offsetX = -5;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                offsetX = 5;
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
}
