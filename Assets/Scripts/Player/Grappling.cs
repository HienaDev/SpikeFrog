using Cinemachine;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;
public class Grappling : MonoBehaviour
{

    private PlayerMovement playerMovement;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform gunTip;
    public Transform GunTip { get { return gunTip; } }
    [SerializeField] private LayerMask whatsGrappleable;

    [SerializeField] private float maxGrappleDistance;
    [SerializeField] private float grappleDelayTime;
    [SerializeField] private float overshootYAxis;

    private Vector3 grapplePoint;
    public Vector3 GrapplePoint { get { return grapplePoint; } }

    private GameObject grappledObject;

    [SerializeField] private float grapplingCd;
    private float grapplingCdTimer;

    [SerializeField] private KeyCode grappleKey = KeyCode.Mouse1;
    private bool grappling;
    public bool Grapple { get { return grappling; } }

    [SerializeField] private float fovValue;
    private float defaultFov;
    private CinemachineVirtualCamera activeCamera;

    [SerializeField] private Transform normalCamera;
    private GrapplingRope grapplingRope;

    [SerializeField] private float grabSpeed = 1;
    public static HashSet<Transform> targetableObjects { get; private set; }
    private Transform closestObject;

    private void Awake()
    {
        activeCamera = cam.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        targetableObjects = new HashSet<Transform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        grapplingRope = GetComponent<GrapplingRope>();

        grappledObject = null;

        defaultFov = activeCamera.m_Lens.FieldOfView;

    }

    public void UpdateCamera()
    {
        activeCamera = cam.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(grappleKey))
        {
            StartGrapple();
        }

        if (grapplingCdTimer > 0)
        {
            grapplingCdTimer -= Time.deltaTime;
        }

        if (grappledObject != null)
        {
            grapplePoint = grappledObject.transform.position;
        }
    }

    private void StartGrapple()
    {
        if (grapplingCdTimer > 0) return;

        grapplingCdTimer = grapplingCd;

        grappling = true;

        playerMovement.EnableGrapple();

        List<Transform> objects = new List<Transform>();
        Vector2 posInViewPortClosestObjectToKeep = Vector2.zero;


        closestObject = null;

        RaycastHit hit;

        foreach (Transform t in targetableObjects)
        {
            Debug.Log(t.gameObject.name);
            if (t != null)
            {
                Debug.DrawRay(transform.position, (t.position - transform.position));
                if (Physics.Raycast(transform.position, (t.position - transform.position), out hit, float.PositiveInfinity))
                {
                    Debug.Log(hit.collider.gameObject.name);
                    if (hit.collider.gameObject == t.gameObject)
                    {
                        objects.Add(t);
                    }
                }
                else
                    Debug.Log("wtf");
            }
            else
                Debug.Log("wtf2");
        }

        if (objects.Count > 0)
        {

            closestObject = objects[0];

            for (int i = 0; i <= objects.Count - 1; i++)
            {
                Vector2 posInViewPort = cam.GetComponent<Camera>().WorldToViewportPoint(objects[i].position) - new Vector3(0.5f, 0.5f, 0.5f);
                Vector2 posInViewPortClosestObject = cam.GetComponent<Camera>().WorldToViewportPoint(closestObject.position) - new Vector3(0.5f, 0.5f, 0.5f);
                if (posInViewPort.magnitude < posInViewPortClosestObject.magnitude)
                {
                    closestObject = objects[i];
                }
            }

            posInViewPortClosestObjectToKeep = cam.GetComponent<Camera>().WorldToViewportPoint(closestObject.position) - new Vector3(0.5f, 0.5f, 0.5f);


            // Exception for enemies
            if (closestObject.GetComponentInParent<EnemyManager>() != null)
                closestObject = closestObject.GetComponentInParent<EnemyManager>().gameObject.transform;


        }


        if (closestObject != null && posInViewPortClosestObjectToKeep.magnitude < (Vector2.one * 0.05f).magnitude)
        {
            grapplePoint = closestObject.transform.position;
            grappledObject = closestObject.gameObject;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;


            Invoke(nameof(StopGrapple), grappleDelayTime);
        }
    }

    private void ExecuteGrapple()
    {

        StartCoroutine(GrabEnemy());
    }

    private IEnumerator GrabEnemy()
    {
        float lerpValue = 0f;
        Vector3 initialPos = Vector3.zero;
        if (closestObject != null)
            initialPos = closestObject.position;


        float distanceForSpeed = Vector3.Distance(initialPos, transform.position);

        // Exception for enemies
        if (closestObject.GetComponent<NavMeshAgent>() != null)
            closestObject.GetComponent<NavMeshAgent>().enabled = false;

        while (lerpValue < 1f && Vector3.Distance(closestObject.transform.position, transform.position) > 0.7f)
        {
            closestObject.transform.position = Vector3.Lerp(initialPos, gameObject.transform.position, lerpValue);
            lerpValue += grabSpeed * distanceForSpeed * Time.deltaTime;
            //Debug.Log(lerpValue);
            yield return null;
        }


        // Exception for enemies
        if (closestObject.GetComponent<NavMeshAgent>() != null)
        {
            closestObject.GetComponent<NavMeshAgent>().enabled = true;
            playerMovement.EnableEnemyGrab();
        }
            

        Invoke(nameof(StopGrapple), 0f);
    }

    private void StopGrapple()
    {
        Debug.Log("GRapple stop");
  
        playerMovement.DisableGrapple();

        grapplingRope.ResetRope();

        grappling = false;

        grappledObject = null;

        Invoke(nameof(StopEnemyGrab), 1f);

    }

    private void StopEnemyGrab() => playerMovement.DisableEnemyGrab();

}
