using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GrodedChecker : MonoBehaviour
{
    [SerializeField]
    private string _earthTag;

    public bool IsGrounded
    {
        get;
        private set;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_earthTag))
        {
            IsGrounded = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_earthTag))
        {
            IsGrounded = false;
        }
    }
}