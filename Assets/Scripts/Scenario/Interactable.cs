using UnityEngine;

public class Interactable : MonoBehaviour
{
    private bool insideCollider;
    private LoadNextScene lns;

    // Start is called before the first frame update
    void Start()
    {
        insideCollider = false;
        lns = GetComponent<LoadNextScene>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && insideCollider)
        {
            lns.SceneLoad();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
            insideCollider = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            insideCollider = false;
    }
}
