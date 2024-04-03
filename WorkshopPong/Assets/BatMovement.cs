using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatMovement : MonoBehaviour
{

    [SerializeField] private float speed;

    [SerializeField] private KeyCode up;
    [SerializeField] private KeyCode down;

    private Rigidbody2D rb;
    private Vector2 velocity;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        velocity = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {

        velocity = Vector2.zero;

        if(Input.GetKey(up))
        {
            velocity.y = speed;
        }

        if (Input.GetKey(down))
        {
            velocity.y = -speed;
        }

        rb.velocity = velocity;

    }
}
