using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamage : MonoBehaviour
{

    private float damage = 20;



    private void OnTriggerEnter(Collider other)
    {
        EnemyManager enemy = other.GetComponent<EnemyManager>();

        if(enemy != null )
        {
            enemy.TakeDamage(damage);
        }
    }
}
