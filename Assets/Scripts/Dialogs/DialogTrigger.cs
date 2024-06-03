using UnityEngine;
using UnityEngine.Events;

public class DialogTrigger : MonoBehaviour
{
    public DialogSO         dialog;
    public DialogManager    dialogManager;
    public UnityEvent       onDialogEnd;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerDialog();
            gameObject.SetActive(false);
        }
    }

    public void TriggerDialog()
    {
        dialogManager.StartDialog(dialog, this);
    }
}