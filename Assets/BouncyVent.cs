using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyVent : MonoBehaviour
{
    [SerializeField] private float force;

    private void OnCollisionEnter(Collision collision)
    {

        PlayerMovement pm = collision.gameObject.GetComponent<PlayerMovement>();

        if (pm != null)
        {
            pm.SetGrounded(false);
        }

        if (collision.rigidbody)
        {
            collision.rigidbody.velocity = transform.TransformDirection(new Vector3(0, force, 0));
        }
    }
}
