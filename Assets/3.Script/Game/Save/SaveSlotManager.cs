using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveSlotManager : MonoBehaviour
{
    public static SaveSlotManager Instance;

    // 현재 선택된 저장 슬롯 폴더 경로
    public string currentSaveFolder;

    private string autosaveParentDir;

    private void Awake()
    {
        // 싱글턴 설정 및 씬 전환 간 유지
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

    // 새 게임 시작 시 호출: 현재 시간을 기반으로 새로운 폴더 생성
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

            // 최신순으로 정렬 후, 상위 5개만 반환
            return allFolders
                .OrderByDescending(folder => Directory.GetCreationTime(folder)) // 최신순 정렬
                .Take(5) // 상위 5개 선택
                .ToArray();
        }
        return new string[0];
    }
}
