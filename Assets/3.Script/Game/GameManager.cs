using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("��� �� �迭")]
    [SerializeField] private GameObject[] floors;

    [Header("�÷��̾� ��ġ")]
    [SerializeField] private Transform player;

    private float floorHeight = 10f;

    private void Update()
    {
        int currentFloor = Mathf.FloorToInt(player.position.y / floorHeight);

        for (int i = 0; i < floors.Length; i++)
        {
            if (i == currentFloor || i == currentFloor - 1 || i == currentFloor + 1)
            {
                floors[i].SetActive(true);
            }
            else
            {
                floors[i].SetActive(false);
            }
        }
    }
}
