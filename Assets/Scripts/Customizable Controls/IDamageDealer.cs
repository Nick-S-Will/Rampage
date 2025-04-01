using UnityEngine;

namespace CustomizableControls
{
    public interface IDamageDealer
    {
        Component DamageSource { get; }
        int Damage { get; }
    }
}