using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InCameraCheck : MonoBehaviour
{

    private Camera cam;
    private MeshRenderer meshRenderer;
    private Plane[] cameraFrustum;
    private Collider col;

    private float timerToCheckIfInCamera;
    private float justChecked;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        meshRenderer = GetComponent<MeshRenderer>();
        col = GetComponent<Collider>();

        timerToCheckIfInCamera = 0.5f;
        justChecked = 0f;
    }



    private void FixedUpdate()
    {
        if (Time.time - justChecked > timerToCheckIfInCamera)
        {
            CheckIfInbounds();
            justChecked = Time.time;
        }
    }

    private void CheckIfInbounds()
    {
        cameraFrustum = GeometryUtility.CalculateFrustumPlanes(cam);

        if (ControlCamera.targetableObjects.Contains(gameObject.transform))
            ControlCamera.targetableObjects.Remove(gameObject.transform);

        if (GeometryUtility.TestPlanesAABB(cameraFrustum, col.bounds))
        {
            ControlCamera.targetableObjects.Add(gameObject.transform);
        }

    }
}
