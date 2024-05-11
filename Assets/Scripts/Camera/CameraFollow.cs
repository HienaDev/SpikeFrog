using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [Header("CameraSettings")] private Vector3 cameraVelocity;
    private float cameraSmoothSpeed; // the bigger the number the longer it takes to move the camera

    [SerializeField] private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        cameraSmoothSpeed = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        HandleAllCameraActions();
    }

    public void HandleAllCameraActions()
    {
        if(player != null)
        {
            FollowTarget();
            // rotate around the player
            //collide with objects
        }

    }

    private void FollowTarget()
    {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, 
                                                          player.transform.position, 
                                                          ref cameraVelocity, 
                                                          cameraSmoothSpeed * Time.deltaTime);

        transform.position = targetCameraPosition ;
    
    }
}
