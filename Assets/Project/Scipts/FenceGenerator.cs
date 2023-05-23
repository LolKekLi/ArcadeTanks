using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class FenceGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject _fencePart;

    [SerializeField, Min(0)]
    private int _fenceCount;

    [SerializeField]
    private float _offset;

    private void Start()
    {
        if (Application.isPlaying)
        {
            CorrectChildCount();
            Destroy(this);
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (!Application.isPlaying)
        {
            CorrectChildCount();
        }
    }

    private void CorrectChildCount()
    {
      
        if (transform.childCount < _fenceCount)
        {
            for (int i = transform.childCount; i < _fenceCount; i++)
            {
                var fence = Instantiate(_fencePart, transform);
                fence.transform.localPosition = new Vector3(_offset * i, 0, 0);
            }
        }
        else if(_fenceCount < transform.childCount)
        {
            for (int i = transform.childCount-1; i >= _fenceCount; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
    }
#endif
}