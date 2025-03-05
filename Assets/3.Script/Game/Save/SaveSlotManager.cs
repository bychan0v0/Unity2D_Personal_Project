using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveSlotManager : MonoBehaviour
{
    public static SaveSlotManager Instance;

    // ���� ���õ� ���� ���� ���� ���
    public string currentSaveFolder;

    private string autosaveParentDir;

    private void Awake()
    {
        // �̱��� ���� �� �� ��ȯ �� ����
        if (Instance == null)
        {
            Instance = this;
            autosaveParentDir = Path.Combine(Application.dataPath, "Autosaves");

            if (!Directory.Exists(autosaveParentDir))
            {
                Directory.CreateDirectory(autosaveParentDir);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // �� ���� ���� �� ȣ��: ���� �ð��� ������� ���ο� ���� ����
    public void CreateNewSaveSlot()
    {
        string folderName = "Save_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string folderPath = Path.Combine(autosaveParentDir, folderName);    
        Directory.CreateDirectory(folderPath);
        currentSaveFolder = folderPath;
    }

    public string[] GetSaveFolders()
    {
        if (Directory.Exists(autosaveParentDir))
        {
            string[] allFolders = Directory.GetDirectories(autosaveParentDir);

            // �ֽż����� ���� ��, ���� 5���� ��ȯ
            return allFolders
                .OrderByDescending(folder => Directory.GetCreationTime(folder)) // �ֽż� ����
                .Take(5) // ���� 5�� ����
                .ToArray();
        }
        return new string[0];
    }
}
