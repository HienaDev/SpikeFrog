using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject      dialogBox;
    [SerializeField] private Camera          mainCamera;
    [SerializeField] private PlayerMovement  playerMovement;
    [SerializeField] private PlayerCombat    playerCombat;

    private Queue<DialogLine> dialogLines;
    private Camera            currentDialogCamera;

    void Start()
    {
        dialogLines = new Queue<DialogLine>();
    }

    public void StartDialog(DialogSO dialog, Camera dialogCamera)
    {
        playerMovement.enabled = false;
        playerCombat.enabled = false;

        dialogCamera.gameObject.SetActive(true);

        dialogBox.SetActive(true);

        dialogLines.Clear();

        foreach (DialogLine line in dialog.dialogLines)
        {
            dialogLines.Enqueue(line);
        }

        currentDialogCamera = dialogCamera;
        SwitchToDialogCamera();
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (dialogLines.Count == 0)
        {
            EndDialog();
            return;
        }

        DialogLine line = dialogLines.Dequeue();

        nameText.text = line.speakerName;
        dialogText.text = line.sentence;
    }

    void EndDialog()
    {
        dialogBox.SetActive(false);
        playerMovement.enabled = true;
        playerCombat.enabled = true;
        SwitchToMainCamera();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && dialogBox.activeSelf)
        {
            DisplayNextSentence();
        }
    }

    void SwitchToDialogCamera()
    {
        if (currentDialogCamera != null)
        {
            mainCamera.gameObject.SetActive(false);
            currentDialogCamera.gameObject.SetActive(true);
        }
    }

    void SwitchToMainCamera()
    {
        if (currentDialogCamera != null)
        {
            currentDialogCamera.gameObject.SetActive(false);
        }
        mainCamera.gameObject.SetActive(true);
    }
}