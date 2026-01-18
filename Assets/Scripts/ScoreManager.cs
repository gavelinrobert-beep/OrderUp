using UnityEngine;

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
        
        // Events for UI updates
        public event System.Action<int> OnScoreChanged;
        public event System.Action<int> OnOrderCompleted;
        
        public int CurrentScore => currentScore;
        public int OrdersCompleted => ordersCompleted;
        public int StandardOrdersCompleted => standardOrdersCompleted;
        public int ExpressOrdersCompleted => expressOrdersCompleted;
        
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
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnRoundStart -= ResetScore;
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
        public void CompleteOrder(bool isExpress, int points)
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
            
            AddScore(points);
            OnOrderCompleted?.Invoke(ordersCompleted);
            
            Debug.Log($"ScoreManager: Order completed! Total orders: {ordersCompleted} " +
                     $"(Standard: {standardOrdersCompleted}, Express: {expressOrdersCompleted})");
        }
        
        /// <summary>
        /// Gets a summary of the current game statistics
        /// TODO: Expand with more detailed statistics
        /// </summary>
        public string GetGameSummary()
        {
            return $"Final Score: {currentScore}\n" +
                   $"Orders Completed: {ordersCompleted}\n" +
                   $"Standard Orders: {standardOrdersCompleted}\n" +
                   $"Express Orders: {expressOrdersCompleted}";
        }
    }
}
