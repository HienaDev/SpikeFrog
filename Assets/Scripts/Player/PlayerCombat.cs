using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private float          timeAvaiableForNextCombo;


    private Animator animator;
    private float    timerCombo;
    private bool     puncheable;

    // Start is called before the first frame update
    void Start()
    {
        puncheable = true;
        timerCombo = 0;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        timerCombo += Time.deltaTime;
        // puncheable is reactivated through idle animation
        if (Input.GetMouseButtonDown(0) && puncheable == true)
        {
            puncheable = false;
            animator.SetTrigger("FirstPunch");
            timerCombo = 0;

            playerMovement.UpdateCamera(0, 0);

            // player is unfrozen through idle animation using method DisableFreeze from this method
            playerMovement.EnableFreeze();
        }
        else if (Input.GetMouseButtonDown(0) && !puncheable)// && timerCombo > timeAvaiableForNextCombo)
        {
            animator.SetTrigger("Punch");
            timerCombo = 0;

            playerMovement.UpdateCamera(0, 0);
        }

        // reset trigger after some time so the player doesnt have a buffered attack 
        if (timerCombo > timeAvaiableForNextCombo * 2)
        {
            animator.ResetTrigger("Punch");
        }
    }

    public void ActivatePunch() => puncheable = true;

    public void DisableFreeze() => playerMovement.DisableFreeze();

    [System.Serializable]
    public struct SaveData
    {
        public float timerCombo;
        public bool  puncheable;
    }

    public SaveData GetSaveData()
    {
        SaveData saveData;

        saveData.timerCombo = timerCombo;
        saveData.puncheable = puncheable;

        return saveData;
    }

    public void LoadSaveData(SaveData saveData)
    {
        timerCombo = saveData.timerCombo;
        puncheable = saveData.puncheable;
    }
}
