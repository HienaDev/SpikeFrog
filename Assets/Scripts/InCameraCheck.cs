using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InCameraCheck : MonoBehaviour
{

    Camera cam;
    MeshRenderer meshRenderer;
    Plane[] cameraFrustum;
    Collider collider;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        meshRenderer = GetComponent<MeshRenderer>();
        collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CheckIfInbounds();
        }
    }

    private void CheckIfInbounds()
    {
        cameraFrustum = GeometryUtility.CalculateFrustumPlanes(cam);

        if (GeometryUtility.TestPlanesAABB(cameraFrustum, collider.bounds))
        {
            meshRenderer.sharedMaterial.color = Color.red;
        }
        else
        {
            meshRenderer.sharedMaterial.color = Color.white;
        }
    }
}
