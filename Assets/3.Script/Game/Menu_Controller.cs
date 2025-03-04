using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Menu_Controller : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Text promptText;      // "Press Space" �ȳ� ���� (Canvas�� �ִ� UI Text)
    [SerializeField] private GameObject menuPanel; // �޴� �г� (�̾��ϱ�, �����ϱ�, ���۹�, �����ϱ� ��ư���� ���Ե� GameObject)

    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.5f; // �ȳ��� FadeOut �ð�

    private bool menuActivated = false;

    private void Start()
    {
        // �޴� �г��� ó���� ��Ȱ��ȭ
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }
    }

    private void Update()
    {
        // �޴��� ���� Ȱ��ȭ���� �ʾҰ�, �����̽�Ű�� ������ �� ó��
        if (!menuActivated && Input.GetKeyDown(KeyCode.Space))
        {
            ActivateMenu();
        }
    }

    private void ActivateMenu()
    {
        menuActivated = true;

        // �ȳ� ������ ���̵� �ƿ���Ű��, �Ϸ�Ǹ� �޴� �г� Ȱ��ȭ
        if (promptText != null)
        {
            promptText.DOFade(0f, fadeDuration).OnComplete(() =>
            {
                // FadeOut �� �ȳ� ���� GameObject ��Ȱ��ȭ (�ʿ��)
                promptText.gameObject.SetActive(false);

                // �޴� �г� Ȱ��ȭ
                if (menuPanel != null)
                {
                    menuPanel.SetActive(true);
                }
            });
        }
    }
}
