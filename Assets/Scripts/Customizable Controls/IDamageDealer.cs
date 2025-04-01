using UnityEngine;

public interface IDamageDealer
{
    Component DamageSource { get; }
    int Damage { get; }
}