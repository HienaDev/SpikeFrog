using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCollider : MonoBehaviour
{

    private SaveManager saveManager;

    // Start is called before the first frame update
    void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerCombat playerCombat = other.GetComponent<PlayerCombat>();

        if (playerCombat != null)
        {
            saveManager.QuickLoadGame();
        }
    }
}
