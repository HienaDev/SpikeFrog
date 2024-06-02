using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownObject : MonoBehaviour
{

    private Transform target;
    private float speed;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        target = null;
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            // Calculate the direction from the current position to the target
            Vector3 direction = (target.position - transform.position).normalized;

            // Calculate the new position based on the speed and the direction
            Vector3 newPosition = transform.position + speed * Time.deltaTime * direction;

            // Update the position of the object
            transform.position = newPosition;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        EnemyManager enemyManager = collision.gameObject.GetComponent<EnemyManager>();
        LeonManager leonManager = collision.gameObject.GetComponent<LeonManager>();
        DestroyByAttack destroyByAttack = collision.gameObject.GetComponent<DestroyByAttack>();

        if (enemyManager != null) { enemyManager.TakeDamage(50); }
        else if(leonManager != null) {  leonManager.TakeDamage(50); }
        else if(destroyByAttack != null) { destroyByAttack.Explode(); }

        Debug.Log("Destroyed with collision to " + collision.gameObject.name);

        Invoke(nameof(DestroyObject), 0.02f);
    }

    private void DestroyObject()
    {
        if(Grappling.targetableObjects.Contains(gameObject.transform))
            Grappling.targetableObjects.Remove(gameObject.transform);

        Destroy(gameObject);
    }

    public void SetTargetToFlyTo(Transform target, float speed)
    {
        Debug.Log("Target is " + target.gameObject.name);
        this.target = target;
        this.speed = speed;
        rb.velocity = Vector3.zero;
    }
}
