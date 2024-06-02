using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField] private DialogSO dialog;
    [SerializeField] private DialogManager dialogManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dialogManager.StartDialog(dialog);
            gameObject.SetActive(false);
        }
    }
}