using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Logo_Controller : MonoBehaviour
{
    [Header("Logo Animation Settings")]
    [SerializeField] private SpriteRenderer logoSprite;
    [SerializeField] private float moveDuration = 2f;
    [SerializeField] private float moveDistance = 2f;

    [SerializeField] private float shakeDuration = 0.8f;
    [SerializeField] private float shakeStrength = 0.1f;

    private Text_Controller textController;

    private void Start()
    {
        textController = FindObjectOfType<Text_Controller>();

        Color initColor = logoSprite.color;

        initColor.a = 0f;
        logoSprite.color = initColor;

        Sequence seq = DOTween.Sequence();

        seq.Append(logoSprite.transform.DOMoveY(logoSprite.transform.position.y + moveDistance, moveDuration)
            .SetEase(Ease.OutQuad));

        seq.Join(logoSprite.DOFade(1f, moveDuration));

        seq.Append(logoSprite.transform.DOShakePosition(shakeDuration, new Vector3(0, shakeStrength, 0), 10, 90, false, false));
        seq.AppendCallback(() => {
            textController.StartBlinking();
        });
    }
}
