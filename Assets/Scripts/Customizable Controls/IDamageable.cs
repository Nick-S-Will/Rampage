using UnityEngine;

namespace CustomizableControls
{
    public interface IDamageable
    {
        bool CanTakeDamage(IDamageDealer damageDealer, Vector3 position);

        bool TakeDamage(IDamageDealer damageDealer, Vector3 position);
    }
}