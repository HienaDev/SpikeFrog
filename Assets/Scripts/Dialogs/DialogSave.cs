using System.Collections.Generic;
using UnityEngine;

public class DialogSave : MonoBehaviour
{
    private List<BoxCollider> dialogColliders;

    private void Start()
    {
        dialogColliders = new List<BoxCollider>();

        for (int i = 0; i < transform.childCount; i++)
        {
            BoxCollider collider = transform.GetChild(i).GetComponent<BoxCollider>();
            if (collider != null)
            {
                dialogColliders.Add(collider);
            }
        }
    }

    [System.Serializable]
    public struct SaveData
    {
        public bool[] colliderStates;
    }

    public SaveData GetSaveData()
    {
        SaveData saveData;
        saveData.colliderStates = new bool[dialogColliders.Count];

        for (int i = 0; i < dialogColliders.Count; i++)
        {
            saveData.colliderStates[i] = dialogColliders[i].enabled;
        }

        return saveData;
    }

    public void LoadSaveData(SaveData saveData)
    {
        for (int i = 0; i < saveData.colliderStates.Length; i++)
        {
            if (i < dialogColliders.Count)
            {
                dialogColliders[i].enabled = saveData.colliderStates[i];
            }
        }
    }
}
