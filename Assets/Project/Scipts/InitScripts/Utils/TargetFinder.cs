using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Project
{
    public static class TargetFinder
    {
        public static T NearestElement<T>(this IEnumerable<T> enumerable, Transform transform) where T : MonoBehaviour
        {
            T element = null;
            float distance = 0f;
        
            foreach (var item in enumerable)
            {
                if (ReferenceEquals(element, null))
                {
                    element = item;
                    distance = (element.transform.position - transform.position).sqrMagnitude;
                }
                else
                {
                    float itemDistance = (item.transform.position - transform.position).sqrMagnitude;
        
                    if (itemDistance < distance)
                    {
                        element = item;
                        distance = itemDistance;
                    }
                }
            }
        
            return element;
        }
    }
}