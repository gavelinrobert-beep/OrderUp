using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private TextMeshProUGUI roundText;
        [SerializeField] private Transform orderListContainer;
        [SerializeField] private GameObject orderItemPrefab; // TODO: Create order item prefab

        [Header("Order Completion Feedback")]
        [SerializeField] private float orderCompletionDelay = 1.25f;
        [SerializeField] private Color completionTextColor = new Color(0.2f, 0.9f, 0.4f, 1f);
        [SerializeField] private Color completionBackgroundColor = new Color(0.2f, 0.9f, 0.4f, 0.2f);

        [Header("End Game UI")]
        [SerializeField] private GameObject roundSummaryPanel;
        [SerializeField] private TextMeshProUGUI summaryText;

        private readonly List<OrderItemUI> activeOrderItems = new List<OrderItemUI>();
        private readonly HashSet<OrderItemUI> completingOrderItems = new HashSet<OrderItemUI>();
        private bool hasWarnedMissingPrefab;
        
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

            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.OnRoundChanged += UpdateRound;
            }
            
            if (OrderManager.Instance != null)
            {
                OrderManager.Instance.OnOrderSpawned += OnOrderSpawned;
                OrderManager.Instance.OnOrderCompleted += OnOrderCompleted;
                OrderManager.Instance.OnOrderExpired += OnOrderExpired;
            }

            EnsureOrderListLayout();
            PopulateExistingOrders();
            
            // Initialize UI
            UpdateScore(0);
            UpdateRound(GameStateManager.Instance != null ? GameStateManager.Instance.CurrentRound : 0);
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

            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.OnRoundChanged -= UpdateRound;
            }
            
            if (OrderManager.Instance != null)
            {
                OrderManager.Instance.OnOrderSpawned -= OnOrderSpawned;
                OrderManager.Instance.OnOrderCompleted -= OnOrderCompleted;
                OrderManager.Instance.OnOrderExpired -= OnOrderExpired;
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

        private void UpdateRound(int round)
        {
            if (roundText != null)
            {
                roundText.text = $"Round: {round}";
            }
        }
        
        /// <summary>
        /// Called when a new order is spawned
        /// TODO: Create visual representation of order in the order list
        /// </summary>
        private void OnOrderSpawned(OrderData order)
        {
            Debug.Log($"GameUIManager: Order spawned - {order.orderId} ({order.orderType})");

            if (orderListContainer == null)
            {
                Debug.LogWarning("GameUIManager: Order list container is not assigned.");
                return;
            }

            EnsureOrderListLayout();
            OrderItemUI orderItem = CreateOrderItem(order);
            if (orderItem != null)
            {
                activeOrderItems.Add(orderItem);
            }
        }
        
        /// <summary>
        /// Called when an order is completed
        /// TODO: Add visual feedback for order completion
        /// </summary>
        private void OnOrderCompleted(OrderData order)
        {
            Debug.Log($"GameUIManager: Order completed - {order.orderId}");

            OrderItemUI orderItem = FindOrderItem(order);
            if (orderItem == null)
            {
                return;
            }

            if (!TryBeginCompletionFeedback(orderItem))
            {
                Debug.LogWarning($"GameUIManager: Completion feedback already in progress for order {order.orderId}.");
                return;
            }
            orderItem.PlayCompletionFeedback(completionTextColor, completionBackgroundColor);
            if (orderCompletionDelay <= 0f)
            {
                RemoveOrderItem(orderItem);
                return;
            }

            StartCoroutine(RemoveOrderItemAfterDelay(orderItem, orderCompletionDelay));
        }

        private void OnOrderExpired(OrderData order)
        {
            Debug.Log($"GameUIManager: Order expired - {order.orderId}");
            RemoveOrderItem(order);
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

        private void PopulateExistingOrders()
        {
            if (OrderManager.Instance == null || activeOrderItems.Count > 0)
            {
                return;
            }

            List<OrderData> existingOrders = OrderManager.Instance.ActiveOrders;
            for (int i = 0; i < existingOrders.Count; i++)
            {
                OnOrderSpawned(existingOrders[i]);
            }
        }

        private OrderItemUI CreateOrderItem(OrderData order)
        {
            GameObject orderObject;
            if (orderItemPrefab != null)
            {
                orderObject = Instantiate(orderItemPrefab, orderListContainer);
            }
            else
            {
                if (!hasWarnedMissingPrefab)
                {
                    Debug.LogWarning("GameUIManager: Order item prefab is not assigned, creating UI elements at runtime.");
                    hasWarnedMissingPrefab = true;
                }

                orderObject = new GameObject("OrderItem", typeof(RectTransform));
                orderObject.transform.SetParent(orderListContainer, false);
            }

            if (!string.IsNullOrEmpty(order.orderId))
            {
                orderObject.name = $"OrderItem_{order.orderId}";
            }

            OrderItemUI orderItem = orderObject.GetComponent<OrderItemUI>();
            if (orderItem == null)
            {
                orderItem = orderObject.AddComponent<OrderItemUI>();
            }

            orderItem.Initialize(order);
            return orderItem;
        }

        private OrderItemUI FindOrderItem(OrderData order)
        {
            for (int i = activeOrderItems.Count - 1; i >= 0; i--)
            {
                OrderItemUI orderItem = activeOrderItems[i];
                if (orderItem == null)
                {
                    activeOrderItems.RemoveAt(i);
                    continue;
                }

                if (orderItem.MatchesOrder(order))
                {
                    return orderItem;
                }
            }

            return null;
        }

        private void RemoveOrderItem(OrderData order)
        {
            OrderItemUI orderItem = FindOrderItem(order);
            if (orderItem == null)
            {
                return;
            }

            RemoveOrderItem(orderItem);
        }

        private void RemoveOrderItem(OrderItemUI orderItem)
        {
            completingOrderItems.Remove(orderItem);
            if (orderItem == null)
            {
                return;
            }

            activeOrderItems.Remove(orderItem);
            Destroy(orderItem.gameObject);
        }

        private IEnumerator RemoveOrderItemAfterDelay(OrderItemUI orderItem, float delay)
        {
            yield return new WaitForSeconds(delay);
            RemoveOrderItem(orderItem);
        }

        private bool TryBeginCompletionFeedback(OrderItemUI orderItem)
        {
            return completingOrderItems.Add(orderItem);
        }

        private void EnsureOrderListLayout()
        {
            if (orderListContainer == null)
            {
                return;
            }

            VerticalLayoutGroup layoutGroup = orderListContainer.GetComponent<VerticalLayoutGroup>();
            if (layoutGroup == null)
            {
                layoutGroup = orderListContainer.gameObject.AddComponent<VerticalLayoutGroup>();
                layoutGroup.childAlignment = TextAnchor.UpperRight;
                layoutGroup.childControlHeight = true;
                layoutGroup.childControlWidth = true;
                layoutGroup.childForceExpandHeight = false;
                layoutGroup.childForceExpandWidth = true;
                layoutGroup.spacing = 8f;
            }

            ContentSizeFitter sizeFitter = orderListContainer.GetComponent<ContentSizeFitter>();
            if (sizeFitter == null)
            {
                sizeFitter = orderListContainer.gameObject.AddComponent<ContentSizeFitter>();
                sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
        }
    }
}
