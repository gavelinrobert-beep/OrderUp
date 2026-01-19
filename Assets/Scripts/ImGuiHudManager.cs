using UnityEngine;
using OrderUp.Core;

namespace OrderUp.UI
{
    /// <summary>
    /// Immediate mode UI overlay for score, timer, and round summary.
    /// </summary>
    public class ImGuiHudManager : MonoBehaviour
    {
        private const int HudWindowIdBase = 9000; // Start high to avoid IMGUI control ID collisions.
        private static int nextWindowId = HudWindowIdBase;

        [Header("HUD Window")]
        [SerializeField] private bool showHudWindow = true;
        [SerializeField] private Rect hudWindowRect = new Rect(16f, 16f, 260f, 140f);

        [Header("Summary Window")]
        [SerializeField] private bool showSummaryWindow = true;
        [SerializeField] private Rect summaryWindowRect = new Rect(16f, 170f, 320f, 180f);

        private int hudWindowId;
        private int summaryWindowId;
        private bool summaryVisible;
        private string summaryText = string.Empty;

        private void Awake()
        {
            hudWindowId = nextWindowId++;
            summaryWindowId = nextWindowId++;
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnRoundStart += HandleRoundStart;
                GameManager.Instance.OnRoundSummary += HandleRoundSummary;
            }
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnRoundStart -= HandleRoundStart;
                GameManager.Instance.OnRoundSummary -= HandleRoundSummary;
            }
        }

        private void OnGUI()
        {
            if (showHudWindow)
            {
                hudWindowRect = GUILayout.Window(hudWindowId, hudWindowRect, DrawHudWindow, "Order Up HUD");
            }

            if (showSummaryWindow && summaryVisible)
            {
                summaryWindowRect = GUILayout.Window(summaryWindowId, summaryWindowRect, DrawSummaryWindow, "Round Summary");
            }
        }

        private void DrawHudWindow(int windowId)
        {
            ScoreManager scoreManager = ScoreManager.Instance;
            GameManager gameManager = GameManager.Instance;
            GameStateManager stateManager = GameStateManager.Instance;

            int score = scoreManager != null ? scoreManager.CurrentScore : 0;
            int round = stateManager != null ? stateManager.CurrentRound : 0;
            string state = GetStateLabel(gameManager, stateManager);

            float remainingTime = gameManager != null ? gameManager.RemainingTime : 0f;

            GUILayout.Label($"Score: {score}");
            GUILayout.Label($"Round: {round}");
            GUILayout.Label($"State: {state}");
            GUILayout.Label($"Time Remaining: {FormatTime(remainingTime)}");
            GUI.DragWindow();
        }

        private void DrawSummaryWindow(int windowId)
        {
            GUILayout.Label(summaryText);
            GUI.DragWindow();
        }

        private void HandleRoundStart()
        {
            summaryVisible = false;
            summaryText = string.Empty;
        }

        private void HandleRoundSummary()
        {
            summaryVisible = true;
            summaryText = ScoreManager.Instance != null
                ? ScoreManager.Instance.GetGameSummary()
                : "No score summary available.";
        }

        private static string GetStateLabel(GameManager gameManager, GameStateManager stateManager)
        {
            if (stateManager != null)
            {
                return stateManager.CurrentState.ToString();
            }

            if (gameManager != null && gameManager.IsRoundActive)
            {
                return GameStateManager.GameState.InRound.ToString();
            }

            return GameStateManager.GameState.WaitingForRound.ToString();
        }

        private static string FormatTime(float seconds)
        {
            float clampedSeconds = Mathf.Max(0f, seconds);
            int minutes = Mathf.FloorToInt(clampedSeconds / 60f);
            int remainingSeconds = Mathf.FloorToInt(clampedSeconds % 60f);
            return $"{minutes:00}:{remainingSeconds:00}";
        }
    }
}
