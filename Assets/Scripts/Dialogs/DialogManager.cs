using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject      dialogBox;
    [SerializeField] private Camera          mainCamera;
    [SerializeField] private PlayerMovement  playerMovement;
    [SerializeField] private PlayerCombat    playerCombat;
    [SerializeField] private float           typingSpeed = 0.05f;
    [SerializeField] private float           delayBeforeNextLine = 2f;

    private Queue<DialogLine> dialogLines;
    private Camera            currentDialogCamera;
    private bool              isTyping = false;

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
        StopAllCoroutines();
        StartCoroutine(TypeSentence(line.sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
        yield return new WaitForSeconds(delayBeforeNextLine);
        DisplayNextSentence();
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
        if (Input.GetMouseButtonDown(0) && dialogBox.activeSelf && !isTyping)
        {
            StopAllCoroutines();
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