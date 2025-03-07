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
    [SerializeField] private Text promptText;      // "Press Space" �ȳ� ���� (Canvas UI Text)
    [SerializeField] private GameObject menuPanel;   // ���� �޴� �г� (�̾��ϱ�, �����ϱ�, ���۹�, �����ϱ� ��ư)
    [SerializeField] private GameObject continuePanel; // �̾��ϱ�(Continue) �г� (ScrollRect ����)
    [SerializeField] private GameObject manipulatePanel;
    [SerializeField] private Continue_Controller continueController; // �̾��ϱ�(Continue) �г� (ScrollRect ����)
    [SerializeField] private Manipulate_Controller manipulateController; // �̾��ϱ�(Continue) �г� (ScrollRect ����)

    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.5f; // �ȳ� ���� FadeOut �ð�

    [Header("Menu Items")]
    // �޴� �׸����� ����� UI Text ������Ʈ�� (��: �ε��� 0: �̾��ϱ�, 1: �����ϱ�, 2: ���۹�, 3: �����ϱ�)
    public List<Text> menuItems;

    [Header("Cursor Settings (Optional)")]
    [SerializeField] private RectTransform cursor;       // ���õ� �׸� �� Ŀ�� �̹���
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

        // �޴� �гΰ� �̾��ϱ� �г� �ʱ⿡�� ��� ��Ȱ��ȭ)            
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

        // Ŀ���� �޴��� �� �� ���� ���̵���, �ʱ⿡�� ��Ȱ��ȭ
        if (cursor != null)
            cursor.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!canActivate)
        {
            return;
        }

        // �޴��� ���� Ȱ��ȭ���� �ʾҰ�, �����̽�Ű�� ������ �޴� Ȱ��ȭ (�ΰ�� �״�� ����)
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
                // �޴��� �߸� Ŀ���� Ȱ��ȭ
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
            // ���õ� �޴� �׸��� RectTransform
            RectTransform itemRect = menuItems[currentIndex].GetComponent<RectTransform>();
            // ��ư�� �߾� ��ġ (���� ��ǥ)
            Vector3 worldCenter = itemRect.TransformPoint(itemRect.rect.center);

            // Ŀ���� �θ� Canvas��� �����մϴ�.
            Canvas canvas = cursor.GetComponentInParent<Canvas>();
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();

            // ���� ��ǥ�� ��ũ�� ��ǥ�� ��ȯ (Screen Space - Overlay��� ī�޶� ���ڴ� null)
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, worldCenter);
            // ��ũ�� ��ǥ�� ĵ������ ���� ��ǥ�� ��ȯ�մϴ�.
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out localPoint);
            // Ŀ���� ��ư�� ���ʿ� ��ġ�ϵ��� x�� ������ ����
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
        continueController.UpdateCursor(); // Ŀ�� ��ġ ����
    }

    private void EnableInput()
    {
        canActivate = true;
    }

}
