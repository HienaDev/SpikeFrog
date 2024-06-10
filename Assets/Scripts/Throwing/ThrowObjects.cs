using UnityEngine;

public class ThrowObjects : MonoBehaviour
{

    [SerializeField] private GameObject rightHand;
    private GameObject throwingObject;

    [SerializeField] private float speed = 500;
     
    int throwLayer;

    private bool thrown = true;

    private Transform targetPoint;

    // Start is called before the first frame update
    void Start()
    {
        throwingObject = null;
        throwLayer = LayerMask.NameToLayer("Throwable");

        targetPoint = null;
    }


    private void Update()
    {

        if (Input.GetMouseButtonDown(1) && throwingObject != null)
        {
            bool hasTarget = false;
            // Get the mouse position in screen coordinates
            Vector3 mouseScreenPosition = Input.mousePosition;

            // Convert the mouse screen position to world space
            Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);
            RaycastHit hit;

            targetPoint = Grappling.instance.GetClosestObject();
            Vector3 targetPosition = Vector3.zero;

            Debug.Log("Closest object: " + Grappling.instance.GetClosestObject());

            if(targetPoint == null)
            {
                //// Check if the ray hits something in the world
                if (Physics.Raycast(ray, out hit))
                {
                    // If the ray hits an object, set the target point to the hit point
                    targetPosition = hit.point;
                }
                else
                {
                    // If the ray doesn't hit anything, set the target point to a point far away in the direction of the ray
                    targetPosition = ray.GetPoint(1000);
                }
            }
            else
            {
                hasTarget = true;
            }
            

            // Compute the direction from the spawn point to the target point
            Vector3 direction = (targetPosition - throwingObject.transform.position).normalized;

            throwingObject.AddComponent<ThrownObject>();

            if(!hasTarget)
            {
                throwingObject.GetComponent<Rigidbody>().isKinematic = false;
                throwingObject.GetComponent<Rigidbody>().useGravity = false;
                throwingObject.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
                throwingObject.GetComponent<Rigidbody>().velocity = direction * speed;

            }        

            Invoke(nameof(ResetObject), 0.1f);

        }
    }

    private void ResetObject()
    {
        ThrownObject to = throwingObject.GetComponent<ThrownObject>();
        if (targetPoint != null) 
        {
            to.SetTargetToFlyTo(targetPoint, speed);
            targetPoint = null;
        }

        
        thrown = true;
        throwingObject = null;
        
    }

    private void FixedUpdate()
    {
        if(throwingObject != null && !thrown)
        {
            //Debug.Log(throwingObject);
            throwingObject.layer = throwLayer;
            throwingObject.transform.position = rightHand.transform.position;
            throwingObject.transform.rotation = rightHand.transform.rotation * Quaternion.Euler(90, 0, 0);
        }

        
            
    }

    public void SetTarget(GameObject target)
    {
        thrown = false;
        this.throwingObject = target;
    }

    public void ResetTarget() => this.throwingObject = null;

    public GameObject GetTarget() => throwingObject;
}
