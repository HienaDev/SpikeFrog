using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBody : MonoBehaviour
{

    [SerializeField] private TongueScript tongue;
    private Vector3 defaultRotation;
    private bool changedRotation;

    // Start is called before the first frame update
    void Start()
    {
        changedRotation = false;
        defaultRotation = transform.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (!tongue.IsGrappling() && !changedRotation)
        {
            transform.localEulerAngles = defaultRotation;
            changedRotation = true;
        }
        else if(!tongue.IsGrappling())
        {
            defaultRotation = transform.localEulerAngles;
        }
        else if(tongue.IsGrappling())
        {
            changedRotation = false;
            transform.LookAt(tongue.GetGrapplePoint());
        }
    }
}
