using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class InteractableObject<T> : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<T>(out var component))
        {
            OnInteracted(component);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<T>(out var component))
        {
            OnExited(component);
        }
    }
        
    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<T>(out var component))
        {
            OnStay(component);
        }
    }

    protected virtual void OnStay(T component)
    {
            
    }


    protected virtual void OnExited(T component)
    {
    }

    protected abstract void OnInteracted(T component);
}