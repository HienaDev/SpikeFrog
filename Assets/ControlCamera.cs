using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCamera : MonoBehaviour
{

    [SerializeField] private Transform occlusionPivot;
    [SerializeField] private float deocclusionThreshold;
    [SerializeField] private float deocclusionVelocity;

    [SerializeField] private float rotationVelocityFactor;

    [SerializeField] private float maxPitchUpAngle;
    [SerializeField] private float maxPitchDownAngle;

    [SerializeField] private float resetYawSpeed;

    [SerializeField] private float zoomAccelerationFactor;
    [SerializeField] private float zoomDeceleration;
    [SerializeField] private float zoomMinDistance;
    [SerializeField] private float zoomMaxDistance;

    private Transform cameraTransform;
    private float zoomAcceleration;
    private float zoomVelocity;
    private float zoomPosition;
    private Vector3 deocclusionVector;

    private void Start()
    {
        cameraTransform = GetComponentInChildren<Camera>().transform;

        zoomVelocity = 0f;

        deocclusionVector = new Vector3(0, 0, deocclusionThreshold);
    }

    private void Update()
    {
        UpdatePitch(); // Look up and down
        UpdateYaw(); // Look around
        UpdateZoom();

        PreventOcclusion();
    }



    private void PreventOcclusion()
    {

        Debug.DrawLine(occlusionPivot.position, cameraTransform.position - cameraTransform.TransformDirection(deocclusionVector));

        if (Physics.Linecast(occlusionPivot.position, cameraTransform.position - cameraTransform.TransformDirection(deocclusionVector), out RaycastHit hitInfo))
        {

            if(hitInfo.collider.CompareTag("WorldBoundary"))
            {
                cameraTransform.position = hitInfo.point + cameraTransform.TransformDirection(deocclusionVector);
            }
            else
            {
                Vector3 position = cameraTransform.localPosition;

                position.z += deocclusionVelocity * Time.deltaTime;

                cameraTransform.localPosition = position;
            }

        }
        else
        {
            RevertDeocclusion();
        }
    }

    private void RevertDeocclusion()
    {
        Vector3 localPosition = cameraTransform.localPosition;

        if(localPosition.z > zoomPosition)
        {
            localPosition.z = Mathf.Max(localPosition.z - deocclusionVelocity * Time.deltaTime, zoomPosition);

            Vector3 worldPosition = transform.TransformPoint(localPosition);

            if(!Physics.Linecast(occlusionPivot.position, worldPosition - cameraTransform.TransformDirection(deocclusionVector)))
            {
                cameraTransform.localPosition = localPosition;
            }
        }
    }

    private void UpdateZoom()
    {
        UpdateZoomAcceleration();
        UpdateZoomVelocity();
        UpdateZoomPosition();
    }

    private void UpdateZoomAcceleration()
    {
        zoomAcceleration = Input.GetAxis("Zoom") * zoomAccelerationFactor;  
    }

    private void UpdateZoomVelocity()
    {
        if (zoomAcceleration != 0f)
        {
            zoomVelocity += zoomAcceleration * Time.deltaTime;
        }
        else if (zoomVelocity > 0f)
        {
            zoomVelocity -= zoomDeceleration * Time.deltaTime;
            zoomVelocity = Mathf.Max(zoomVelocity, 0f);
        }
        else
        {
            zoomVelocity += zoomDeceleration * Time.deltaTime;
            zoomVelocity = Mathf.Min(zoomVelocity, 0f);
        }

    }

    private void UpdateZoomPosition()
    {
        if (zoomVelocity != 0f)
        {
            Vector3 position = cameraTransform.localPosition;

            position.z += zoomVelocity * Time.deltaTime;

            if(position.z < -zoomMaxDistance || position.z > -zoomMinDistance) 
            {
                zoomVelocity = 0f;

                position.z = Mathf.Clamp(position.z, -zoomMaxDistance, -zoomMinDistance);
            }

            cameraTransform.localPosition = position;

            zoomPosition = position.z;
        }
    }

    

    private void UpdatePitch()
    {
        Vector3 rotation = transform.localEulerAngles;

        rotation.x -= Input.GetAxis("Mouse Y") * rotationVelocityFactor;

        if(rotation.x < 180f) 
            rotation.x = Mathf.Min(rotation.x, maxPitchUpAngle);
        else
            rotation.x = Mathf.Max(rotation.x, maxPitchDownAngle);

        transform.localEulerAngles = (rotation); 
    }

    private void UpdateYaw()
    {
        // Rotates camera
        //if (Input.GetButton("Camera"))
        //{
        //    Vector3 rotation = transform.localEulerAngles;

        //    rotation.y += Input.GetAxis("Mouse X");

        //    transform.localEulerAngles = (rotation);
        //}
        //else
        //    ResetYaw();

        Vector3 rotation = transform.localEulerAngles;

           rotation.y += Input.GetAxis("Mouse X");

           transform.localEulerAngles = (rotation);
    }

    private void ResetYaw()
    {
        Vector3 rotation = transform.localEulerAngles;

        if(rotation.y != 0f)
        {
            if (rotation.y < 180f)
                rotation.y = Mathf.Max(rotation.y - resetYawSpeed * Time.deltaTime, 0f);
            else
                rotation.y = Mathf.Max(rotation.y + resetYawSpeed * Time.deltaTime, 0f);
        }

        transform.localEulerAngles = (rotation);
    }
}
