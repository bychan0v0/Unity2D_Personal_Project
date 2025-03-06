using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // �÷��̾��� ��ġ
    public Vector3 playerPosition;
    public bool isWind;
}

[System.Serializable]
public class SaveDataCollection
{
    // ���� ���� �����͸� ������ ����Ʈ
    public List<GameData> saves = new List<GameData>();
}
