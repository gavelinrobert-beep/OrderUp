using UnityEngine;

namespace OrderUp.Core
{
    /// <summary>
    /// Tracks overall game state, rounds, and score for UI and gameplay systems.
    /// </summary>
    public class GameStateManager : MonoBehaviour
    {
        public static GameStateManager Instance { get; private set; }

        public enum GameState
        {
            WaitingForRound,
            InRound,
            Paused,
            Summary
        }

        [Header("State Tracking")]
        [SerializeField] private int currentRound = 0;
        [SerializeField] private int currentScore = 0;
        [SerializeField] private GameState currentState = GameState.WaitingForRound;

        public event System.Action<int> OnRoundChanged;
        public event System.Action<int> OnScoreUpdated;
        public event System.Action<GameState> OnStateChanged;

        public int CurrentRound => currentRound;
        public int CurrentScore => currentScore;
        public GameState CurrentState => currentState;

        private bool isGameManagerSubscribed;
        private bool isScoreManagerSubscribed;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            TrySubscribe();
        }

        private void Start()
        {
            TrySubscribe();
        }

        private void OnDisable()
        {
            UnsubscribeFromManagers();
        }

        private void TrySubscribe()
        {
            if (GameManager.Instance != null && !isGameManagerSubscribed)
            {
                GameManager.Instance.OnRoundStart += HandleRoundStart;
                GameManager.Instance.OnRoundEnd += HandleRoundEnd;
                GameManager.Instance.OnRoundSummary += HandleRoundSummary;
                GameManager.Instance.OnPause += HandlePause;
                GameManager.Instance.OnResume += HandleResume;
                isGameManagerSubscribed = true;
                SyncStateFromGameManager();
            }

            if (ScoreManager.Instance != null && !isScoreManagerSubscribed)
            {
                ScoreManager.Instance.OnScoreChanged += HandleScoreChanged;
                HandleScoreChanged(ScoreManager.Instance.CurrentScore);
                isScoreManagerSubscribed = true;
            }
        }

        private void UnsubscribeFromManagers()
        {
            if (GameManager.Instance != null && isGameManagerSubscribed)
            {
                GameManager.Instance.OnRoundStart -= HandleRoundStart;
                GameManager.Instance.OnRoundEnd -= HandleRoundEnd;
                GameManager.Instance.OnRoundSummary -= HandleRoundSummary;
                GameManager.Instance.OnPause -= HandlePause;
                GameManager.Instance.OnResume -= HandleResume;
            }

            if (ScoreManager.Instance != null && isScoreManagerSubscribed)
            {
                ScoreManager.Instance.OnScoreChanged -= HandleScoreChanged;
            }

            isGameManagerSubscribed = false;
            isScoreManagerSubscribed = false;
        }

        private void HandleRoundStart()
        {
            currentRound++;
            SetState(GameState.InRound);
            OnRoundChanged?.Invoke(currentRound);
        }

        private void HandleRoundEnd()
        {
            SetState(GameState.Summary);
        }

        private void HandleRoundSummary()
        {
            SetState(GameState.Summary);
        }

        private void HandlePause()
        {
            SetState(GameState.Paused);
        }

        private void HandleResume()
        {
            SetState(GameState.InRound);
        }

        private void HandleScoreChanged(int score)
        {
            currentScore = score;
            OnScoreUpdated?.Invoke(currentScore);
        }

        private void SyncStateFromGameManager()
        {
            if (GameManager.Instance == null)
            {
                return;
            }

            if (GameManager.Instance.IsRoundActive)
            {
                SetState(GameManager.Instance.IsPaused ? GameState.Paused : GameState.InRound);
            }
            else
            {
                SetState(currentRound > 0 ? GameState.Summary : GameState.WaitingForRound);
            }
        }

        private void SetState(GameState newState)
        {
            if (currentState == newState)
            {
                return;
            }

            currentState = newState;
            OnStateChanged?.Invoke(currentState);
        }
    }
}
