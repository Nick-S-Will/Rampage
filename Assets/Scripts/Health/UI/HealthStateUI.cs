using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Rampage.Health.UI
{
    public class HealthStateUI : MonoBehaviour
    {
        private float FullFillWidth => healthState.MaxHealth * baseFillWidth;

        [SerializeField] private HealthState healthState;
        [SerializeField] private Image backImage, fillImage;

        private float baseFillWidth;

        protected virtual void Awake()
        {
            Assert.IsNotNull(healthState);
            Assert.IsNotNull(backImage);
            Assert.IsNotNull(fillImage);

            baseFillWidth = fillImage.rectTransform.sizeDelta.x;

            healthState.HealthChanged.AddListener(UpdateUI);
            backImage.rectTransform.sizeDelta = new Vector2(FullFillWidth, backImage.rectTransform.sizeDelta.y);
            fillImage.rectTransform.sizeDelta = new Vector2(FullFillWidth, backImage.rectTransform.sizeDelta.y);
        }

        public virtual void UpdateUI()
        {
            float healthPercent = (float)healthState.Health / healthState.MaxHealth;
            Vector2 newSize = new(healthPercent * FullFillWidth, fillImage.rectTransform.sizeDelta.y);
            fillImage.rectTransform.sizeDelta = newSize;
        }
    }
}