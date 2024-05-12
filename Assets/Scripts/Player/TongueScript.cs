using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueScript : MonoBehaviour
{

    private LineRenderer lr;
    private Vector3 grapplePoint;
    [SerializeField] private LayerMask grappeable;
    [SerializeField] private Transform tongueTip;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform player;
    [SerializeField] private float maxDistance;
    private SpringJoint joint;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            StopGrapple();
        }
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    private void StartGrapple()
    {
        RaycastHit hit;

        if (Physics.Raycast(cam.position, cam.forward, out hit, maxDistance, grappeable))
        {
            Debug.Log("grappling");


            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * 0.02f;
            joint.minDistance = distanceFromPoint * 0.1f;

            joint.spring = 1.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
        }
    }

    private void DrawRope()
    {
        if (!joint) return;
        lr.SetPosition(0, tongueTip.position);
        lr.SetPosition(1, grapplePoint);
    }

    private void StopGrapple()
    {
        lr.positionCount = 0;
        Destroy(joint);
    }

    public bool IsGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplePoint() => grapplePoint;
}
