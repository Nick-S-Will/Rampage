using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Rampage.GameHandlers
{
    public class GameOverHandler : MonoBehaviour
    {
        public UnityEvent GameEnded => gameEnded;

        [Header("Events")]
        [SerializeField] private UnityEvent gameEnded;

        public void EndGame()
        {
            gameEnded.Invoke();
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}