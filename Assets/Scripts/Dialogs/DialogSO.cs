using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Dialog", menuName = "Dialog")]
public class DialogSO : ScriptableObject
{
    public List<DialogLine> dialogLines;
    public UnityEvent onDialogEnd;
}
