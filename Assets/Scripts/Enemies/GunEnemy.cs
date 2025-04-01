using CustomizableControls.Attacks.Shooting;
using UnityEngine;
using UnityEngine.Assertions;

namespace Rampage.Enemies
{
    [SelectionBase]
    public class GunEnemy : MonoBehaviour
    {
        [SerializeField] private GunController gunController;

        protected virtual void Awake()
        {
            Assert.IsNotNull(gunController);
        }
    }
}