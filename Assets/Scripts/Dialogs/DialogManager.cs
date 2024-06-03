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
    [SerializeField] private List<Camera>    dialogCameras;
    [SerializeField] private GameObject[]    playerUI;

    private Queue<DialogLine> dialogLines;
    private Camera            currentDialogCamera;
    private bool              isTyping = false;
    private bool              fullSentenceDisplayed = false;
    private string            currentSentence;
    private Coroutine         typingCoroutine;

    private void Start()
    {
        dialogLines = new Queue<DialogLine>();
    }

    public void StartDialog(DialogSO dialog)
    {
        StopPlayerFromMoving();

        DeactivatePlayerUI();

        dialogBox.SetActive(true);

        dialogLines.Clear();

        foreach (DialogLine line in dialog.dialogLines)
        {
            dialogLines.Enqueue(line);
        }

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
        typingCoroutine = StartCoroutine(TypeSentence(line.sentence));

        SwitchToDialogCamera(line.cameraIndex);
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        fullSentenceDisplayed = false;
        currentSentence = sentence;
        dialogText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogText.text += letter;

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        fullSentenceDisplayed = true;

        yield return new WaitForSeconds(delayBeforeNextLine);
        DisplayNextSentence();
    }

    private void EndDialog()
    {
        dialogBox.SetActive(false);

        ActivatePlayerUI();
        LetPlayerMove();
        SwitchToMainCamera();
    }

    private void DeactivatePlayerUI()
    {
        foreach (GameObject ui in playerUI)
        {
            ui.SetActive(false);
        }
    }

    private void ActivatePlayerUI()
    {
        foreach (GameObject ui in playerUI)
        {
            ui.SetActive(true);
        }
    }

    private void StopPlayerFromMoving()
    {
        playerMovement.StopMoving();
        playerCombat.enabled = false;
        playerMovement.enabled = false;
    }

    private void LetPlayerMove()
    {
        playerMovement.enabled = true;
        playerCombat.enabled = true;
    }

    private void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && dialogBox.activeSelf)
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogText.text = currentSentence;
                isTyping = false;
                fullSentenceDisplayed = true;
                StartCoroutine(DelayBeforeNextLine());
            }
            else if (fullSentenceDisplayed)
            {
                DisplayNextSentence();
            }
        }
    }

    private IEnumerator DelayBeforeNextLine()
    {
        yield return new WaitForSeconds(delayBeforeNextLine);

        DisplayNextSentence();
    }

    private void SwitchToDialogCamera(int cameraIndex)
    {
        if (currentDialogCamera != null)
        {
            currentDialogCamera.gameObject.SetActive(false);
        }

        if (cameraIndex >= 0 && cameraIndex < dialogCameras.Count)
        {
            currentDialogCamera = dialogCameras[cameraIndex];
            currentDialogCamera.gameObject.SetActive(true);
        }
        else
        {
            currentDialogCamera = mainCamera;
        }
    }

    private void SwitchToMainCamera()
    {
        if (currentDialogCamera != null)
        {
            currentDialogCamera.gameObject.SetActive(false);
        }
        mainCamera.gameObject.SetActive(true);
        currentDialogCamera = mainCamera;
    }
}