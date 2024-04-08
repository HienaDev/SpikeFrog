using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovingTest : MonoBehaviour
{

    [SerializeField] private Transform orientation;
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerObj;
    private Rigidbody rb;

    [SerializeField] private float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        float horizontalInput = Input.GetAxis("Strafe");
        float verticalInput = Input.GetAxis("Forward");

        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (inputDir != Vector3.zero)
        {
            playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir, rotationSpeed * Time.deltaTime);
        }
    }
}
