using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObjects : MonoBehaviour
{

    [SerializeField] private GameObject rightHand;
    private GameObject target;

    [SerializeField] private float speed;
     
    int throwLayer;

    // Start is called before the first frame update
    void Start()
    {
        target = null;
        throwLayer = LayerMask.NameToLayer("Throwable");
    }


    private void Update()
    {

        if (target != null)
        {
            // Get the mouse position in screen coordinates
            Vector3 mouseScreenPosition = Input.mousePosition;

            // Convert the mouse screen position to world space
            Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);
            RaycastHit hit;

            Vector3 targetPoint;

            // Check if the ray hits something in the world
            if (Physics.Raycast(ray, out hit))
            {
                // If the ray hits an object, set the target point to the hit point
                targetPoint = hit.point;
            }
            else
            {
                // If the ray doesn't hit anything, set the target point to a point far away in the direction of the ray
                targetPoint = ray.GetPoint(1000);
            }

            // Compute the direction from the spawn point to the target point
            Vector3 direction = (targetPoint - target.transform.position).normalized;

            Debug.DrawRay(target.transform.position, direction * 100, Color.yellow);
        }

        if (Input.GetMouseButtonDown(1) && target != null)
        {

            // Get the mouse position in screen coordinates
            Vector3 mouseScreenPosition = Input.mousePosition;

            // Convert the mouse screen position to world space
            Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);
            RaycastHit hit;

            Vector3 targetPoint;

            // Check if the ray hits something in the world
            if (Physics.Raycast(ray, out hit))
            {
                // If the ray hits an object, set the target point to the hit point
                targetPoint = hit.point;
            }
            else
            {
                // If the ray doesn't hit anything, set the target point to a point far away in the direction of the ray
                targetPoint = ray.GetPoint(1000);
            }

            // Compute the direction from the spawn point to the target point
            Vector3 direction = (targetPoint - target.transform.position).normalized;


            target.GetComponent<Rigidbody>().isKinematic = false;
            target.GetComponent<Rigidbody>().useGravity = false;
            target.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
            target.GetComponent<Rigidbody>().velocity = direction * speed;

            target = null;

        }
    }



    private void FixedUpdate()
    {
        if(target != null)
        {
            target.layer = throwLayer;
            target.transform.position = rightHand.transform.position;
            target.transform.rotation = rightHand.transform.rotation * Quaternion.Euler(90, 0, 0);
        }

        
            
    }

    public void SetTarget(GameObject target) => this.target = target;

    public void ResetTarget() => this.target = null;

    public GameObject GetTarget() => target;
}
