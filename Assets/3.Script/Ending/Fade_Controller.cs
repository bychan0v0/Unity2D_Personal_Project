using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Fade_Controller : MonoBehaviour
{
    public static Fade_Controller Instance; // �̱��� ���� ����

    private Image fadeImage; // ���̵� ȿ���� ���� �̹���
    [SerializeField] private float fadeDuration = 1f; // ���̵� ��/�ƿ� ���� �ð�

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ���� ����Ǿ ����
            SceneManager.sceneLoaded += OnSceneLoaded; // ���� ����� �� �ڵ� ����
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindFadeImage();
        FadeIn();
    }

    private void FindFadeImage()
    {
        GameObject fadeObj = GameObject.Find("EndPanel"); // �� ������ ���̵� �г� ã��
        if (fadeObj != null)
        {
            fadeImage = fadeObj.GetComponent<Image>();
        }
    }

    // �� ���� �� ���̵� �� (��ο� ȭ�� �� �����)
    public void FadeIn()
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.color = new Color(0, 0, 0, 1);
        fadeImage.DOFade(0f, fadeDuration).OnComplete(() =>
        {
            fadeImage.gameObject.SetActive(false);
        });
    }

    // �� ��ȯ �� ���̵� �ƿ� (���� ȭ�� �� ��ο���) + �� ����
    public void FadeOutAndLoadScene(string sceneName)
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.DOFade(1f, fadeDuration).OnComplete(() =>
        {
            SceneManager.LoadScene(sceneName);
        });
    }
}
