using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind_Controller : MonoBehaviour
{
    [Header("Wind Settings")]
    [SerializeField] private float Speed = 10f;
    [SerializeField] private float windStrength = 1.0f;
    [SerializeField] private float changeInterval = 5.0f;
    [SerializeField] private float speedSmoothTime = 1f;

    private float width;
    private float timer = 0f;
    private int windDirection = -1;
    private float currentSpeed = 0f;
    private float speedSmoothVelocity = 0f;

    public Vector2 WindForce
    {
        get 
        {
            float normalized = currentSpeed / Speed;

            return new Vector2(normalized * windStrength, 0f);
        }
    }

    private void Start()
    {
        width = GetComponent<BoxCollider2D>().size.x;

        currentSpeed = windDirection * Speed;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= changeInterval)
        {
            timer = 0f;
            windDirection *= -1;
        }

        float targetSpeed = windDirection * Speed;
        
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        WindMove();
    }

    private void WindMove()
    {
        transform.Translate(Vector3.right * currentSpeed * Time.deltaTime); 

        if (transform.position.x <= -width)
        {
            Vector2 offset = new Vector2(width * 2f, 0);
            transform.position = (Vector2)transform.position + offset;
        }

        if (transform.position.x >= width)
        {
            Vector2 offset = new Vector2(width * -2f, 0);
            transform.position = (Vector2)transform.position + offset;
        }
    }
}
