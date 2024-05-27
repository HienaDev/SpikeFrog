using UnityEngine;

public class CheckGrounded : MonoBehaviour
{
    [SerializeField] private float coyoteTime;
    public bool Grounded { get; set; }
    private float justLeftGround;
    private bool canToggleGround;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("WorldBoundary"))
            Grounded = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("WorldBoundary"))
        {
            justLeftGround = Time.time;
            canToggleGround = true;
        }
            
    }

    private void FixedUpdate()
    {
        if (Time.time - justLeftGround > coyoteTime && canToggleGround)
        {
            Grounded = false;
            canToggleGround = false;
        }
    }
}
