using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    [Header("플레이어 위치")]
    [SerializeField] private Transform playerTransform;

    private float floorHeight = 10f;
    private float cameraYOffset = 5f;

    private void LateUpdate()
    {
        int currentFloor = Mathf.FloorToInt(playerTransform.position.y / floorHeight);
        float targetCameraY = currentFloor * floorHeight + cameraYOffset;

        transform.position = new Vector3(transform.position.x, targetCameraY, transform.position.z);
    }
}
