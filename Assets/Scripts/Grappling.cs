using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{

    private PlayerMovement playerMovement;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform gunTip;
    [SerializeField] private LayerMask whatsGrappleable;
    private LineRenderer lineRenderer;

    [SerializeField] private float maxGrappleDistance;
    [SerializeField] private float grappleDelayTime;
    [SerializeField] private float overshootYAxis; 

    private Vector3 grapplePoint;

    [SerializeField] private float grapplingCd;
    private float grapplingCdTimer;

    [SerializeField] private KeyCode grappleKey = KeyCode.Mouse1;
    private bool grappling;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(grappleKey))
        {
            StartGrapple();
        }

        if(grapplingCdTimer > 0)
        {
            grapplingCdTimer -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        if(grappling)
            lineRenderer.SetPosition(0, gunTip.position);
    }

    private void StartGrapple()
    {
        if (grapplingCdTimer > 0) return;

        grappling = true;

        playerMovement.ActivateGrapple();

        playerMovement.EnableFreeze();

        RaycastHit hit;

        if ((Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatsGrappleable)))
        {
            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;


            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {
        playerMovement.DisableFreeze();

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) 
        {
            highestPointOnArc = overshootYAxis;
        }

        playerMovement.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }

    private void StopGrapple()
    {

        playerMovement.DisableFreeze();

        grappling = false;

        grapplingCdTimer = grapplingCd;

        lineRenderer.enabled = false;

    }
}
