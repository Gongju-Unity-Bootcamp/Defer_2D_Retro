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
        // �÷��̾��� ���� ���θ� �������� ����
        PC = FindObjectOfType<Player_Controller>();
    }

    // Update�� �� �����Ӹ��� ȣ��
    // FixedUpdate�� Fixed Timestep�� ������ ���� ���� ������ �������� ȣ��
    // �ұ�Ģ�� Update�� �޸� �����ϰ� ȣ��ǹǷ� ����ȿ�� ���� ������ �� ����
    void FixedUpdate()
    {
        // ī�޶��� ��ġ = �÷��̾��� ��ġ ��ǥ + ������
        targetVector = new Vector3(player.position.x + offsetX, player.position.y + 2.5f + offsetY, -10f + offsetZ);

        // ī�޶��� ��ġ�� targetVector�� �ε巴�� �̵�
        transform.position = Vector3.Lerp(transform.position, targetVector, Time.deltaTime * cameraSpeed);

        OffsetCtrl();
    }

    /// <summary>
    /// Ű �Է¿� ���� ī�޶� ������ ��ȭ �Լ�
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
