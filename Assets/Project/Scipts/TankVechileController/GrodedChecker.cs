using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GrodedChecker : MonoBehaviour
{
    [SerializeField]
    private string _earthTag;

    [field: SerializeField]
    public bool IsGrounded
    {
        get;
        private set;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(_earthTag))
        {
            IsGrounded = true;
        }
        else
        {
            IsGrounded = false;
        }
    }
}