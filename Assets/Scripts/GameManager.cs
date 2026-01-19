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
        [SerializeField] private bool isPaused = false;
        [SerializeField] private float remainingTime = 0f;
        
        // Events for other systems to subscribe to
        public event System.Action OnRoundStart;
        public event System.Action OnRoundEnd;
        public event System.Action OnRoundSummary;
        public event System.Action OnPause;
        public event System.Action OnResume;
        public event System.Action<float> OnTimerUpdate; // passes remaining time
        
        public bool IsRoundActive => isRoundActive;
        public bool IsPaused => isPaused;
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
            if (Application.isPlaying)
            {
                StartRound();
            }
        }
        
        private void Update()
        {
            if (isRoundActive && !isPaused)
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
            isPaused = false;
            remainingTime = roundDuration;
            Time.timeScale = 1f;
            
            OnRoundStart?.Invoke();
        }
        
        /// <summary>
        /// Ends the current round
        /// </summary>
        public void EndRound()
        {
            Debug.Log("GameManager: Ending round");
            isRoundActive = false;
            isPaused = false;
            remainingTime = 0f;
            Time.timeScale = 1f;
            
            OnRoundEnd?.Invoke();
            OnRoundSummary?.Invoke();
            
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
        /// </summary>
        public void PauseRound()
        {
            if (!isRoundActive || isPaused)
            {
                return;
            }

            isPaused = true;
            Time.timeScale = 0f;
            OnPause?.Invoke();
        }
        
        /// <summary>
        /// Resumes the round
        /// </summary>
        public void ResumeRound()
        {
            if (!isPaused)
            {
                return;
            }

            isPaused = false;
            Time.timeScale = 1f;
            OnResume?.Invoke();
        }
    }
}
