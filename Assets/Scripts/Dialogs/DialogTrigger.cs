using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField] private DialogSO       dialog;
    [SerializeField] private DialogManager  dialogManager;
    [SerializeField] private Camera         dialogCamera;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dialogManager.StartDialog(dialog, dialogCamera);
            gameObject.SetActive(false);
        }
    }
}