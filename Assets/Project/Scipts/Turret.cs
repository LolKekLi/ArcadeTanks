using System;
using System.Runtime.InteropServices;
using Project;
using UnityEngine;

public class Turret : MonoBehaviour, IDamagable
{
    [SerializeField]
    private TankController _tankController;

    [SerializeField]
    private EnemyController _enemysController;
    
    public bool IsDied
    {
        get
        {
            if (_tankController)
            {
                return _tankController.IsDied;
            }

            if (_enemysController)
            {
                return _enemysController.IsDied;
            }

            return false;

        }
    }

    public void TakeDamage(float damage)
    {
        if (_tankController)
        {
             _tankController.TakeDamage(damage);
             
             return;
        }

        if (_enemysController)
        {
            _enemysController.TakeDamage(damage);
        }
     
    }
}