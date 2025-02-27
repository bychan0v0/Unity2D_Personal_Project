using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    public Transform playerTransform;  // �÷��̾� Transform �Ҵ�
    public float floorHeight = 10f;      // �� ���� ����
    public float cameraYOffset = 5f;     // �� ���� �߾� ��ġ (��: 10�� ����)

    void LateUpdate()
    {
        // �÷��̾��� ���� �� ��ȣ ��� (��: y�� 23�̸� 2��, 0������ �����ϴ� ���)
        int currentFloor = Mathf.FloorToInt(playerTransform.position.y / floorHeight);

        // �ش� ���� �߾� y ��ǥ ���
        float targetCameraY = currentFloor * floorHeight + cameraYOffset;

        // ī�޶��� x, z ��ǥ�� �״�� �ΰ� y ��ǥ�� ����
        transform.position = new Vector3(transform.position.x, targetCameraY, transform.position.z);
    }
}
