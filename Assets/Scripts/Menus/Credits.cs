using UnityEngine;

public class Credits : MonoBehaviour
{
    [SerializeField] private MenusManager menusScript;
    private Animator animator;

    void OnEnable()
    {
        animator = GetComponent<Animator>();
        animator.Play("credits");
    }

    public void Finished()
    {
        menusScript.CloseCredits();
        if(menusScript.FinalCredits)
            menusScript.GoToMainMenu();
        menusScript.EndOfFinalCredits();
    }
}