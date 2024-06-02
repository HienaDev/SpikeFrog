using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialog", menuName = "Dialog")]
public class DialogSO : ScriptableObject
{
    public List<DialogLine> dialogLines;
}
