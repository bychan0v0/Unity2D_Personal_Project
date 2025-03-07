using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Menu_Controller : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text promptText;      // "Press Space" 안내 문구 (Canvas UI Text)
    [SerializeField] private GameObject menuPanel;   // 메인 메뉴 패널 (이어하기, 새로하기, 조작법, 종료하기 버튼)
    [SerializeField] private GameObject continuePanel; // 이어하기(Continue) 패널 (ScrollRect 포함)
    [SerializeField] private GameObject manipulatePanel;
    [SerializeField] private Continue_Controller continueController; // 이어하기(Continue) 패널 (ScrollRect 포함)
    [SerializeField] private Manipulate_Controller manipulateController; // 이어하기(Continue) 패널 (ScrollRect 포함)

    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.5f; // 안내 문구 FadeOut 시간

    [Header("Menu Items")]
    // 메뉴 항목으로 사용할 UI Text 컴포넌트들 (예: 인덱스 0: 이어하기, 1: 새로하기, 2: 조작법, 3: 종료하기)
    public List<Text> menuItems;

    [Header("Cursor Settings (Optional)")]
    [SerializeField] private RectTransform cursor;       // 선택된 항목 옆 커서 이미지
    [SerializeField] private float cursorOffsetX = 15f;

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    private int currentIndex = 0;
    public bool menuActivated = false;
    public bool canActivate = false;

    private float menuTimer;
    private bool menuOn;

    private void Start()
    {
        Invoke("EnableInput", 3f);

        continueController = FindObjectOfType<Continue_Controller>();
        manipulateController = FindObjectOfType<Manipulate_Controller>();

        UpdateSelection();
        UpdateCursor();

        // 메뉴 패널과 이어하기 패널 초기에는 모두 비활성화)            
        if (menuPanel != null)
            menuPanel.SetActive(false);
        if (continuePanel != null)
            continuePanel.SetActive(false);
        if (continueController != null)
            continueController.gameObject.SetActive(false);
        if (manipulatePanel != null)
            manipulatePanel.SetActive(false);
        if (manipulateController != null)
            manipulateController.gameObject.SetActive(false);

        // 커서도 메뉴가 뜰 때 같이 보이도록, 초기에는 비활성화
        if (cursor != null)
            cursor.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!canActivate)
        {
            return;
        }

        // 메뉴가 아직 활성화되지 않았고, 스페이스키를 누르면 메뉴 활성화 (로고는 그대로 유지)
        if (!menuActivated && Input.GetKeyDown(KeyCode.Space) && !menuOn)
        {
            Invoke("ActivateMenu", 0.1f);
            menuOn = true;
        }

        if (menuOn)
        {
            menuTimer += Time.deltaTime;
            if (menuTimer >= 0.2f)
            {
                menuOn = false;
            }
        }

        if (menuActivated && menuTimer >= 0.2f)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentIndex = (currentIndex - 1 + menuItems.Count) % menuItems.Count;
                UpdateSelection();
                UpdateCursor();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentIndex = (currentIndex + 1) % menuItems.Count;
                UpdateSelection();
                UpdateCursor();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Invoke("SelectMenuItem", 0.5f);
            }
        }
    }

    private void ActivateMenu()
    {
        menuActivated = true;
        if (promptText != null)
        {
            promptText.DOFade(0f, fadeDuration).OnComplete(() =>
            {
                promptText.gameObject.SetActive(false);
                if (menuPanel != null)
                {
                    menuPanel.SetActive(true);
                }
                // 메뉴가 뜨면 커서도 활성화
                if (cursor != null)
                    cursor.gameObject.SetActive(true);
            });
        }
    }

    void UpdateSelection()
    {
        for (int i = 0; i < menuItems.Count; i++)
        {
            menuItems[i].color = (i == currentIndex) ? selectedColor : normalColor;
        }
    }

    public void UpdateCursor()
    {
        if (cursor != null && menuItems.Count > 0)
        {
            // 선택된 메뉴 항목의 RectTransform
            RectTransform itemRect = menuItems[currentIndex].GetComponent<RectTransform>();
            // 버튼의 중앙 위치 (월드 좌표)
            Vector3 worldCenter = itemRect.TransformPoint(itemRect.rect.center);

            // 커서의 부모가 Canvas라고 가정합니다.
            Canvas canvas = cursor.GetComponentInParent<Canvas>();
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();

            // 월드 좌표를 스크린 좌표로 변환 (Screen Space - Overlay라면 카메라 인자는 null)
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, worldCenter);
            // 스크린 좌표를 캔버스의 로컬 좌표로 변환합니다.
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out localPoint);
            // 커서가 버튼의 왼쪽에 위치하도록 x축 오프셋 적용
            localPoint.x -= cursorOffsetX;

            cursor.anchoredPosition = localPoint;
        }
    }

    void SelectMenuItem()
    {
        switch (currentIndex)
        {
            case 0:
                menuActivated = false;
                if (menuPanel != null)
                    menuPanel.SetActive(false);
                if (continuePanel != null)
                    continuePanel.SetActive(true);
                if (continueController != null)
                    continueController.gameObject.SetActive(true);

                StartCoroutine(DelayedUpdateCursor());
                break;
            case 1:
                SaveSlotManager.Instance.CreateNewSaveSlot();
                SceneManager.LoadScene("Main");
                break;
            case 2:
                menuActivated = false;
                if (menuPanel != null)
                    menuPanel.SetActive(false);
                if (continuePanel != null)
                    manipulatePanel.SetActive(true);
                if (continueController != null)
                    manipulateController.gameObject.SetActive(true);
                break;
            case 3:
                Application.Quit();
                break;
            default:
                break;
        }
    }

    private IEnumerator DelayedUpdateCursor()
    {
        yield return null;
        continueController.UpdateCursor(); // 커서 위치 갱신
    }

    private void EnableInput()
    {
        canActivate = true;
    }

}
