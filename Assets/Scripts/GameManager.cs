using UnityEngine;

namespace OrderUp.Core
{
    /// <summary>
    /// Central GameManager responsible for game state, round timer, and coordination.
    /// Singleton pattern for easy access from other systems.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [Header("Round Settings")]
        [Tooltip("Duration of a game round in seconds")]
        [SerializeField] private float roundDuration = 300f; // 5 minutes
        
        [Header("Runtime State")]
        [SerializeField] private bool isRoundActive = false;
        [SerializeField] private float remainingTime = 0f;
        
        // Events for other systems to subscribe to
        public event System.Action OnRoundStart;
        public event System.Action OnRoundEnd;
        public event System.Action<float> OnTimerUpdate; // passes remaining time
        
        public bool IsRoundActive => isRoundActive;
        public float RemainingTime => remainingTime;
        
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
            // Auto-start round for MVP testing
            // TODO: Remove auto-start and trigger from lobby/menu system
            StartRound();
        }
        
        private void Update()
        {
            if (isRoundActive)
            {
                UpdateTimer();
            }
        }
        
        /// <summary>
        /// Starts a new game round
        /// </summary>
        public void StartRound()
        {
            Debug.Log("GameManager: Starting new round");
            isRoundActive = true;
            remainingTime = roundDuration;
            
            OnRoundStart?.Invoke();
        }
        
        /// <summary>
        /// Ends the current round
        /// </summary>
        public void EndRound()
        {
            Debug.Log("GameManager: Ending round");
            isRoundActive = false;
            remainingTime = 0f;
            
            OnRoundEnd?.Invoke();
            
            // TODO: Trigger round summary UI
            // TODO: Calculate final scores and statistics
        }
        
        /// <summary>
        /// Updates the round timer
        /// </summary>
        private void UpdateTimer()
        {
            remainingTime -= Time.deltaTime;
            
            OnTimerUpdate?.Invoke(remainingTime);
            
            if (remainingTime <= 0f)
            {
                remainingTime = 0f;
                EndRound();
            }
        }
        
        /// <summary>
        /// Pauses the round (for menu/pause screen)
        /// TODO: Implement pause functionality
        /// </summary>
        public void PauseRound()
        {
            // TODO: Implement pause logic
            Time.timeScale = 0f;
        }
        
        /// <summary>
        /// Resumes the round
        /// TODO: Implement resume functionality
        /// </summary>
        public void ResumeRound()
        {
            // TODO: Implement resume logic
            Time.timeScale = 1f;
        }
    }
}
