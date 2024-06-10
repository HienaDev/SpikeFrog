using UnityEngine;

public class SimpleForward : MonoBehaviour
{
    [SerializeField] private float  moveSpeed = 3;
    [SerializeField] private bool   runMove = false;
    
    public bool RunMove => runMove;

    void Update()
    {
        if (!runMove)
            return;
        Vector3 dir = new Vector3(-1, 0, 0);
        transform.position += dir * moveSpeed * Time.deltaTime;
    }
}
