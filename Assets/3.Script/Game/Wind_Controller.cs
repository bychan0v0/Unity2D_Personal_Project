using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind_Controller : MonoBehaviour
{
    [Header("Wind Settings")]
    [SerializeField] private float Speed = 10f;
    [SerializeField] private float windStrength = 1.0f;
    [SerializeField] private float changeInterval = 5.0f;

    private float width;
    private float timer = 0f;
    private int windDirection = -1;

    public Vector2 WindForce
    {
        get { return new Vector2(windDirection * windStrength, 0f); }
    }

    private void Start()
    {
        width = GetComponent<BoxCollider2D>().size.x;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= changeInterval)
        {
            timer = 0f;
            windDirection *= -1;
        }

        WindMove();
    }

    private void WindMove()
    {
        if (windDirection == -1)
        {
            transform.Translate(Vector3.left * Speed * Time.deltaTime);

            if (transform.position.x <= -width)
            {
                Vector2 offset = new Vector2(width * 3f, 0);
                transform.position = (Vector2)transform.position + offset;
            }
        }
        else
        {
            transform.Translate(Vector3.right * Speed * Time.deltaTime);

            if (transform.position.x >= width)
            {
                Vector2 offset = new Vector2(width * -3f, 0);
                transform.position = (Vector2)transform.position + offset;
            }
        }
    }
}
