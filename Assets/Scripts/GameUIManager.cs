using UnityEngine;
using UnityEngine.UI;
using TMPro;
using OrderUp.Core;
using OrderUp.Data;

namespace OrderUp.UI
{
    /// <summary>
    /// Manages the game UI including timer, score, and order list display.
    /// </summary>
    public class GameUIManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Transform orderListContainer;
        [SerializeField] private GameObject orderItemPrefab; // TODO: Create order item prefab
        
        [Header("End Game UI")]
        [SerializeField] private GameObject roundSummaryPanel;
        [SerializeField] private TextMeshProUGUI summaryText;
        
        private void Start()
        {
            // Subscribe to game events
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnTimerUpdate += UpdateTimer;
                GameManager.Instance.OnRoundSummary += ShowRoundSummary;
            }
            
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.OnScoreChanged += UpdateScore;
            }
            
            if (OrderManager.Instance != null)
            {
                OrderManager.Instance.OnOrderSpawned += OnOrderSpawned;
                OrderManager.Instance.OnOrderCompleted += OnOrderCompleted;
            }
            
            // Initialize UI
            UpdateScore(0);
            if (roundSummaryPanel != null)
            {
                roundSummaryPanel.SetActive(false);
            }
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnTimerUpdate -= UpdateTimer;
                GameManager.Instance.OnRoundSummary -= ShowRoundSummary;
            }
            
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.OnScoreChanged -= UpdateScore;
            }
            
            if (OrderManager.Instance != null)
            {
                OrderManager.Instance.OnOrderSpawned -= OnOrderSpawned;
                OrderManager.Instance.OnOrderCompleted -= OnOrderCompleted;
            }
        }
        
        /// <summary>
        /// Updates the timer display
        /// </summary>
        private void UpdateTimer(float remainingTime)
        {
            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(remainingTime / 60f);
                int seconds = Mathf.FloorToInt(remainingTime % 60f);
                timerText.text = $"{minutes:00}:{seconds:00}";
                
                // Color code timer when time is running low
                if (remainingTime < 60f)
                {
                    timerText.color = Color.red;
                }
                else if (remainingTime < 120f)
                {
                    timerText.color = Color.yellow;
                }
                else
                {
                    timerText.color = Color.white;
                }
            }
        }
        
        /// <summary>
        /// Updates the score display
        /// </summary>
        private void UpdateScore(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {score}";
            }
        }
        
        /// <summary>
        /// Called when a new order is spawned
        /// TODO: Create visual representation of order in the order list
        /// </summary>
        private void OnOrderSpawned(OrderData order)
        {
            Debug.Log($"GameUIManager: Order spawned - {order.orderId} ({order.orderType})");
            
            // TODO: Instantiate order UI element and add to order list
            // TODO: Display order details (products, type, timer for express)
        }
        
        /// <summary>
        /// Called when an order is completed
        /// TODO: Add visual feedback for order completion
        /// </summary>
        private void OnOrderCompleted(OrderData order)
        {
            Debug.Log($"GameUIManager: Order completed - {order.orderId}");
            
            // TODO: Remove order UI element from list
            // TODO: Play completion animation/effect
        }
        
        /// <summary>
        /// Shows the round summary screen at the end of the game
        /// </summary>
        private void ShowRoundSummary()
        {
            if (roundSummaryPanel != null && summaryText != null)
            {
                roundSummaryPanel.SetActive(true);
                
                if (ScoreManager.Instance != null)
                {
                    summaryText.text = ScoreManager.Instance.GetGameSummary();
                }
            }
        }
        
        /// <summary>
        /// Button callback to restart the round
        /// TODO: Connect to button in UI
        /// </summary>
        public void OnRestartButtonClicked()
        {
            if (roundSummaryPanel != null)
            {
                roundSummaryPanel.SetActive(false);
            }
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartRound();
            }
        }
        
        /// <summary>
        /// Button callback to return to main menu
        /// TODO: Implement main menu scene transition
        /// </summary>
        public void OnMainMenuButtonClicked()
        {
            Debug.Log("Return to main menu - TODO: Implement scene transition");
            // TODO: Load main menu scene
        }
    }
}
