using Cinemachine;
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

    [SerializeField] private GameObject cameraBrain;
    private Transform cameraTransform;
    private float zoomAcceleration;
    private float zoomVelocity;
    private float zoomPosition;
    private float zoomTargetCameraLevel;

    private Vector3 deocclusionVector;

    [SerializeField] private bool targetting;

    [SerializeField] private GameObject normalCamera;
    [SerializeField] private GameObject targetCamera;

    public static HashSet<Transform> targetableObjects {  get; private set; }

    private void Start()
    {
        //cameraTransform = GetComponentInChildren<Camera>().transform;
        cameraTransform = targetCamera.transform;

        zoomTargetCameraLevel = GetTargetCameraOffset();

        zoomVelocity = 0f;

        zoomPosition = normalCamera.transform.localPosition.z;

        deocclusionVector = new Vector3(0, 0, deocclusionThreshold);

        targetableObjects = new HashSet<Transform>();
    }

    private float GetTargetCameraOffset()
    {
        return (targetCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance);
    }

    private void SetTargetCameraOffset(float value)
    {
        targetCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = value;
    }

    private void Update()
    {

        

        SwapCameras();

        if (!targetting)
        {
            UpdatePitch(); // Look up and down
            UpdateYaw(); // Look around

        }


        UpdateZoom();

        PreventOcclusion();

    }

    private void FixedUpdate()
    {
        if (targetting)
            transform.rotation = targetCamera.transform.rotation;
    }

    private void SwapCameras()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {

            List<Transform> objects = new List<Transform>();

            Transform closestObject = null;

            foreach (Transform t in targetableObjects)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, (t.position - transform.position), out hit, float.PositiveInfinity))
                {
                    if (hit.collider.gameObject == t.gameObject)
                    {
                        Debug.Log(hit.collider.gameObject.name);
                        objects.Add(t);
                    }
                }
            }

            if (objects.Count > 0)
            {
                closestObject = objects[0];

                for (int i = 0; i <= objects.Count - 1; i++)
                {
                    if (Vector3.Distance(objects[i].position, transform.position) < Vector3.Distance(closestObject.position, transform.position))
                    {
                        closestObject = objects[i];
                    }
                }


                targetCamera.GetComponent<CinemachineVirtualCamera>().LookAt = closestObject;


            }

            if(targetCamera.activeSelf || (!targetCamera.activeSelf && closestObject != null))
            {
                targetCamera.SetActive(!targetCamera.activeSelf);
                normalCamera.SetActive(!normalCamera.activeSelf);
            }
            


            Debug.Log(targetableObjects.Count);
            

            if (targetCamera.activeSelf)
            {
                targetting = true;
                targetCamera.transform.position = normalCamera.transform.position;
                targetCamera.transform.rotation = normalCamera.transform.rotation;
                cameraTransform = targetCamera.transform;


                


                Debug.Log(objects.Count);
            }
            else
            {
                targetting = false;
                normalCamera.transform.position = targetCamera.transform.position;
                normalCamera.transform.rotation = targetCamera.transform.rotation;
                cameraTransform = normalCamera.transform;
            }
        }
    }


    private void PreventOcclusion()
    {

        Debug.DrawLine(occlusionPivot.position, cameraTransform.position - cameraTransform.TransformDirection(deocclusionVector), Color.red);

        if (Physics.Linecast(occlusionPivot.position, cameraTransform.position - cameraTransform.TransformDirection(deocclusionVector), out RaycastHit hitInfo))
        {

            if (hitInfo.collider.CompareTag("WorldBoundary"))
            {
                cameraTransform.position = hitInfo.point + cameraTransform.TransformDirection(deocclusionVector);

                SetTargetCameraOffset(GetTargetCameraOffset() - Vector3.Distance(hitInfo.point, cameraTransform.position));
            }
            else
            {
                //Debug.Log(GetTargetCameraOffset());
                SetTargetCameraOffset(GetTargetCameraOffset() - deocclusionVelocity * Time.deltaTime);

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

        // Fixed jittering by changing deocclusion value, check again later

        if (!targetCamera.activeSelf)
        {
            Vector3 localPosition = cameraTransform.localPosition;

            if (localPosition.z > zoomPosition)
            {
                localPosition.z = Mathf.Max(localPosition.z - deocclusionVelocity * Time.deltaTime, zoomPosition);

                Vector3 worldPosition = transform.TransformPoint(localPosition);

                if (!Physics.Linecast(occlusionPivot.position, worldPosition - cameraTransform.TransformDirection(deocclusionVector)))
                {
                    cameraTransform.localPosition = localPosition;
                }
            }
        }
        else
        {
            Vector3 localPosition = cameraTransform.localPosition;

            if (GetTargetCameraOffset() < zoomTargetCameraLevel)
            {
                //Debug.Log(zoomTargetCameraLevel);
                SetTargetCameraOffset(Mathf.Min(GetTargetCameraOffset() + deocclusionVelocity * Time.deltaTime, zoomTargetCameraLevel));

                Vector3 worldPosition = transform.TransformPoint(localPosition);

                Debug.DrawLine(occlusionPivot.position, worldPosition + cameraTransform.TransformDirection(deocclusionVector), Color.blue);
                if (!Physics.Linecast(occlusionPivot.position, worldPosition + cameraTransform.TransformDirection(deocclusionVector)))
                {
                    SetTargetCameraOffset(GetTargetCameraOffset());
                }
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

            SetTargetCameraOffset(GetTargetCameraOffset() - zoomVelocity * Time.deltaTime);

            if (!targetCamera.activeSelf)
            { 
                if (position.z < -zoomMaxDistance || position.z > -zoomMinDistance)
                {
                    zoomVelocity = 0f;

                    position.z = Mathf.Clamp(position.z, -zoomMaxDistance, -zoomMinDistance);
                }
            }
            else
            { 
                if (GetTargetCameraOffset() > zoomMaxDistance || GetTargetCameraOffset() < zoomMinDistance)
                {
                    zoomVelocity = 0f;

                    SetTargetCameraOffset(Mathf.Clamp(GetTargetCameraOffset(), zoomMinDistance, zoomMaxDistance));
                }
            }


            cameraTransform.localPosition = position;

            //Debug.Log(zoomVelocity);



            zoomTargetCameraLevel = GetTargetCameraOffset();

            zoomPosition = position.z;
        }
    }



    private void UpdatePitch()
    {
        Vector3 rotation = transform.localEulerAngles;

        rotation.x -= Input.GetAxis("Mouse Y") * rotationVelocityFactor;

        if (rotation.x < 180f)
            rotation.x = Mathf.Min(rotation.x, maxPitchUpAngle);
        else
            rotation.x = Mathf.Max(rotation.x, maxPitchDownAngle);

        transform.localEulerAngles = (rotation);
    }

    private void UpdateYaw()
    {

        Vector3 rotation = transform.localEulerAngles;

        rotation.y += Input.GetAxis("Mouse X");

        transform.localEulerAngles = (rotation);
    }

}
