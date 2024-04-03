using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGrounded : MonoBehaviour
{

    public bool Grounded {  get; private set; } 

    private void OnTriggerEnter(Collider other)
    {
        Grounded = true;
    }

    private void OnTriggerExit(Collider other)
    {
        Grounded = false;
    }
}
