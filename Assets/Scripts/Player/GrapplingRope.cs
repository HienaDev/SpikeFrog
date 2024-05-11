using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GrapplingRope : MonoBehaviour
{

    private LineRenderer lineRenderer;
    private Grappling grapplingScript;

    private Vector3 currentGrapplePosition;

    [SerializeField] private int quality;
    private Spring spring;

    [SerializeField] private float ropeSpeed;

    [SerializeField] private float damper;
    [SerializeField] private float strenght;
    [SerializeField] private float velocity;
    [SerializeField] private float waveCount;
    [SerializeField] private float waveHeight;
    [SerializeField] private AnimationCurve effectCurve;


    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        grapplingScript = GetComponent<Grappling>();

        spring = new Spring();
        spring.SetTarget(0);
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    private void DrawRope()
    {
        if (!grapplingScript.Grapple)
        {
            currentGrapplePosition = grapplingScript.GunTip.position;
            spring.Reset();

            if(lineRenderer.positionCount > 0)
            {
                lineRenderer.positionCount = 0;
            }

            return;
        }

        if (lineRenderer.positionCount == 0) 
        {
            spring.SetVelocity(velocity);
            lineRenderer.positionCount = quality + 1;
        }

        spring.SetDamper(damper);
        spring.SetStrength(strenght);
        spring.Update(Time.deltaTime);

        Vector3 grapplePoint = grapplingScript.GrapplePoint;
        Vector3 gunTipPosition = grapplingScript.GunTip.position;
        Vector3 up = Quaternion.LookRotation((grapplePoint - gunTipPosition).normalized) * Vector3.up;



        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * ropeSpeed);

        for (int i = 0; i < quality + 1; i++)
        {
            float delta = i / (float)quality;

            Vector3 offset = up * waveHeight * Mathf.Sin(delta
                            * waveCount * Mathf.PI) * spring.Value 
                            * effectCurve.Evaluate(delta);

            lineRenderer.SetPosition(i, Vector3.Lerp(gunTipPosition, currentGrapplePosition, delta) + offset);
        }
    }

    public void ResetRope()
    {
        //currentGrapplePosition = grapplingScript.GunTip.position;
        lineRenderer.positionCount = 0;
        spring.Reset();
    }
}
