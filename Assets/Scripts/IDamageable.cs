using UnityEngine;

namespace Rampage
{
    public interface IDamageable
    {
        bool TakeDamage(Vector3 position);
    }
}