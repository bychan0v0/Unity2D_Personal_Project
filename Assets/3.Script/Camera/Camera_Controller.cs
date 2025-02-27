using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    public Transform playerTransform;  // 플레이어 Transform 할당
    public float floorHeight = 10f;      // 한 층의 높이
    public float cameraYOffset = 5f;     // 한 층의 중앙 위치 (예: 10의 절반)

    void LateUpdate()
    {
        // 플레이어의 현재 층 번호 계산 (예: y가 23이면 2층, 0층부터 시작하는 경우)
        int currentFloor = Mathf.FloorToInt(playerTransform.position.y / floorHeight);

        // 해당 층의 중앙 y 좌표 계산
        float targetCameraY = currentFloor * floorHeight + cameraYOffset;

        // 카메라의 x, z 좌표는 그대로 두고 y 좌표만 스냅
        transform.position = new Vector3(transform.position.x, targetCameraY, transform.position.z);
    }
}
