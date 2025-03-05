using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // �÷��̾��� ��ġ
    public Vector3 playerPosition;
}

[System.Serializable]
public class SaveDataCollection
{
    // ���� ���� �����͸� ������ ����Ʈ
    public List<GameData> saves = new List<GameData>();
}
