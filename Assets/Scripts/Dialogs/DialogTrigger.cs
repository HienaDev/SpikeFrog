using UnityEngine;
using UnityEngine.Events;

public class DialogTrigger : MonoBehaviour
{
    public DialogSO         dialog;
    public DialogManager    dialogManager;
    public UnityEvent       onDialogStart;
    public UnityEvent       onDialogEnd;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerDialog();
            gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }

    public void TriggerDialog()
    {
        dialogManager.StartDialog(dialog, this);
    }
}