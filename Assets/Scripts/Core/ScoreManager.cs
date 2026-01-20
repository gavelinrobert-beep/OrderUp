using UnityEngine;
using OrderUp.Data;

namespace OrderUp.Core
{
    /// <summary>
    /// Manages team score throughout the game.
    /// Singleton pattern for easy access from order completion systems.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }
        
        [Header("Score State")]
        [SerializeField] private int currentScore = 0;
        [SerializeField] private int ordersCompleted = 0;
        [SerializeField] private int standardOrdersCompleted = 0;
        [SerializeField] private int expressOrdersCompleted = 0;
        [SerializeField] private int missedExpressOrders = 0;
        [SerializeField] private float totalCompletionTime = 0f;
        [SerializeField] private int timedOrdersCompleted = 0;
        
        // Events for UI updates
        public event System.Action<int> OnScoreChanged;
        public event System.Action<int> OnOrderCompleted;
        
        public int CurrentScore => currentScore;
        public int OrdersCompleted => ordersCompleted;
        public int StandardOrdersCompleted => standardOrdersCompleted;
        public int ExpressOrdersCompleted => expressOrdersCompleted;
        public int MissedExpressOrders => missedExpressOrders;
        public float AverageCompletionTime => HasCompletionTimeData ? totalCompletionTime / timedOrdersCompleted : 0f;
        public bool HasCompletionTimeData => timedOrdersCompleted > 0;
        
        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        private void Start()
        {
            // Subscribe to game events
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnRoundStart += ResetScore;
            }

            if (OrderManager.Instance != null)
            {
                OrderManager.Instance.OnOrderExpired += HandleOrderExpired;
            }
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnRoundStart -= ResetScore;
            }

            if (OrderManager.Instance != null)
            {
                OrderManager.Instance.OnOrderExpired -= HandleOrderExpired;
            }
        }
        
        /// <summary>
        /// Resets score at the start of a new round
        /// </summary>
        public void ResetScore()
        {
            Debug.Log("ScoreManager: Resetting score for new round");
            currentScore = 0;
            ordersCompleted = 0;
            standardOrdersCompleted = 0;
            expressOrdersCompleted = 0;
            missedExpressOrders = 0;
            totalCompletionTime = 0f;
            timedOrdersCompleted = 0;
            
            OnScoreChanged?.Invoke(currentScore);
        }
        
        /// <summary>
        /// Adds points to the team score
        /// </summary>
        /// <param name="points">Points to add</param>
        public void AddScore(int points)
        {
            currentScore += points;
            Debug.Log($"ScoreManager: Added {points} points. Total: {currentScore}");
            
            OnScoreChanged?.Invoke(currentScore);
        }
        
        /// <summary>
        /// Records completion of an order
        /// </summary>
        /// <param name="isExpress">Whether the order was an express order</param>
        /// <param name="points">Points earned from the order</param>
        /// <param name="completionTime">Optional time it took to complete the order (in seconds)</param>
        public void CompleteOrder(bool isExpress, int points, float? completionTime = null)
        {
            ordersCompleted++;
            
            if (isExpress)
            {
                expressOrdersCompleted++;
            }
            else
            {
                standardOrdersCompleted++;
            }
            
            if (completionTime.HasValue)
            {
                totalCompletionTime += completionTime.Value;
                timedOrdersCompleted++;
            }

            AddScore(points);
            OnOrderCompleted?.Invoke(ordersCompleted);
            
            Debug.Log($"ScoreManager: Order completed! Total orders: {ordersCompleted} " +
                     $"(Standard: {standardOrdersCompleted}, Express: {expressOrdersCompleted})");
        }

        /// <summary>
        /// Records an express order that expired before completion.
        /// </summary>
        public void RecordMissedExpressOrder()
        {
            missedExpressOrders++;
            Debug.Log($"ScoreManager: Express order missed. Total missed express: {missedExpressOrders}");
        }
        
        /// <summary>
        /// Gets a comprehensive summary of the current game statistics.
        /// Includes score, order completions by type, missed orders, and average completion time.
        /// </summary>
        public string GetGameSummary()
        {
            string averageCompletionSummary = HasCompletionTimeData
                ? $"{AverageCompletionTime:0.0}s"
                : "N/A";

            return $"Final Score: {currentScore}\n" +
                   $"Orders Completed: {ordersCompleted}\n" +
                   $"Standard Orders: {standardOrdersCompleted}\n" +
                   $"Express Orders: {expressOrdersCompleted}\n" +
                   $"Missed Express Orders: {missedExpressOrders}\n" +
                   $"Average Completion Time: {averageCompletionSummary}";
        }

        private void HandleOrderExpired(OrderData order)
        {
            if (order == null || order.orderType != OrderType.Express)
            {
                return;
            }

            RecordMissedExpressOrder();
        }
    }
}
