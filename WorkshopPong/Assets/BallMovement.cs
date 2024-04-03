using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{

    [SerializeField] private float speedX;
    [SerializeField] private float speedY;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.velocity = new Vector2 (speedX, speedY);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
