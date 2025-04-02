using System.Linq;
using UnityEngine;

namespace CustomizableControls
{
    public interface IDamageDealer
    {
        Component DamageSource { get; }
        int Damage { get; }

        bool CanDealDamage(Component target, Vector3 position)
        {
            IDamageable[] damageables = target.GetComponents<IDamageable>();
            if (damageables.Length == 0) return false;

            return damageables.Any(damageable => damageable.CanTakeDamage(this, position));
        }

        bool DealDamage(Component target, Vector3 position)
        {
            IDamageable[] damageables = target.GetComponents<IDamageable>();
            if (damageables.Length == 0) return false;

            bool hasDealtDamage = false;
            foreach (var damageable in damageables)
            {
                if (damageable.TakeDamage(this, position)) hasDealtDamage = true;
            }

            return hasDealtDamage;
        }
    }
}