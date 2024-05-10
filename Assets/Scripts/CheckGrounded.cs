using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGrounded : MonoBehaviour
{

    public bool Grounded {  get; private set; } 

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("WorldBoundary"))
            Grounded = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("WorldBoundary"))
            Grounded = false;
    }
}
