using UnityEngine;

public class SimpleForward : MonoBehaviour
{

    [SerializeField] private float _moveSpeed = 3;
    [SerializeField] private bool _runMove = false;
    public bool RunMove => _runMove;

    void Update()
    {
        if (!_runMove)
            return;
        Vector3 dir = new Vector3(-1, 0, 0);
        transform.position += dir * _moveSpeed * Time.deltaTime;
    }
}
