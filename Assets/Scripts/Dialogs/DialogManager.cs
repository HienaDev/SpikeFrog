using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    [System.Serializable]
    public class CameraSetup
    {
        public Camera    camera;
        public Transform lookAtTarget;
        public float     zoomDistance = 0f;
    }

    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject      dialogBox;
    [SerializeField] private Camera          mainCamera;
    [SerializeField] private PlayerMovement  playerMovement;
    [SerializeField] private PlayerCombat    playerCombat;
    [SerializeField] private float           typingSpeed = 0.05f;
    [SerializeField] private float           delayBeforeNextLine = 2f;
    [SerializeField] private AudioSource     audioSource;
    [SerializeField] private MenusManager    menusManager;
    [SerializeField] private GameObject[]    playerUI;
    [SerializeField] private List<CameraSetup> dialogCameras;

    private Queue<DialogLine> dialogLines;
    private Camera            currentDialogCamera;
    private bool              isTyping = false;
    private bool              fullSentenceDisplayed = false;
    private string            currentSentence;
    private Coroutine         typingCoroutine;
    private DialogSO          currentDialog;
    private DialogTrigger     currentTrigger;

    private void Start()
    {
        dialogLines = new Queue<DialogLine>();
    }

    public void StartDialog(DialogSO dialog, DialogTrigger trigger)
    {
        currentDialog = dialog;
        currentTrigger = trigger;

        StopPlayerFromMoving();
        DeactivatePlayerUI();

        dialogBox.SetActive(true);
        dialogLines.Clear();

        currentTrigger?.onDialogStart.Invoke();

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

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        if (line.voiceClip != null)
        {
            audioSource.clip = line.voiceClip;

            audioSource.pitch = Random.Range(0.9f, 1.1f);

            if(line.randomStart)
                audioSource.time = Random.Range(audioSource.clip.length / 2, audioSource.clip.length);

            audioSource.Play();
        }
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

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // Trigger the event after dialog ends
        currentTrigger?.onDialogEnd.Invoke();
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
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && dialogBox.activeSelf && !menusManager.IsPaused)
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogText.text = currentSentence;
                isTyping = false;
                fullSentenceDisplayed = true;
                StartCoroutine(DelayBeforeNextLine());
            }
            else if (fullSentenceDisplayed && !menusManager.IsPaused)
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
            currentDialogCamera = dialogCameras[cameraIndex].camera;
            currentDialogCamera.gameObject.SetActive(true);

            Transform lookAtTarget = dialogCameras[cameraIndex].lookAtTarget;
            float zoomDistance = dialogCameras[cameraIndex].zoomDistance;

            if (lookAtTarget != null)
            {
                currentDialogCamera.transform.LookAt(lookAtTarget);

                if (zoomDistance > 0)
                {
                    Vector3 direction = (lookAtTarget.position - currentDialogCamera.transform.position).normalized;
                    currentDialogCamera.transform.position = lookAtTarget.position - direction * zoomDistance;
                }
            }
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