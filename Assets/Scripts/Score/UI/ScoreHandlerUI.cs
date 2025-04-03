using System;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace Rampage.Score.UI
{
    public class ScoreHandlerUI : MonoBehaviour
    {
        [SerializeField] private ScoreHandler scoreHandler;
        [SerializeField] private TMP_Text scoreText;

        private string scoreTextPrefix;

        protected virtual void Awake()
        {
            Assert.IsNotNull(scoreHandler);
            Assert.IsNotNull(scoreText);

            scoreHandler.ScoreChanged.AddListener(UpdateUI);

            scoreTextPrefix = scoreText.text;
        }

        protected virtual void Start()
        {
            UpdateUI();
        }

        protected virtual void UpdateUI()
        {
            scoreText.text = scoreTextPrefix + scoreHandler.Score;
        }
    }
}