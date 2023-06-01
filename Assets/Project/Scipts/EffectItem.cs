using UnityEngine;

namespace Project
{
    public class EffectItem : InteractableObject<BusterEffectController>
    {
        [SerializeField]
        private EffectType _type;

        protected override void OnInteracted(BusterEffectController component)
        {
            component.ApplyEffect(_type);
            gameObject.SetActive(false);
        }
    }
}