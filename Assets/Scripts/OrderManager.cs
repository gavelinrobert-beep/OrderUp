using UnityEngine;
using System.Collections.Generic;
using OrderUp.Data;

namespace OrderUp.Core
{
    /// <summary>
    /// Manages order spawning, tracking, and completion.
    /// Spawns orders during the round and handles order lifecycle.
    /// </summary>
    public class OrderManager : MonoBehaviour
    {
        public static OrderManager Instance { get; private set; }
        
        [Header("Order Pool")]
        [Tooltip("Pool of orders that can be spawned during gameplay")]
        [SerializeField] private List<OrderData> availableOrders = new List<OrderData>();
        
        [Header("Spawn Settings")]
        [Tooltip("Time between order spawns in seconds")]
        [SerializeField] private float spawnInterval = 30f;
        
        [Tooltip("Maximum number of active orders at once")]
        [SerializeField] private int maxActiveOrders = 5;
        
        [Header("Runtime State")]
        [SerializeField] private List<OrderData> activeOrders = new List<OrderData>();
        private float timeSinceLastSpawn = 0f;
        
        // Events for UI and other systems
        public event System.Action<OrderData> OnOrderSpawned;
        public event System.Action<OrderData> OnOrderCompleted;
        public event System.Action<OrderData> OnOrderExpired;
        
        public List<OrderData> ActiveOrders => new List<OrderData>(activeOrders);
        
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
                GameManager.Instance.OnRoundStart += OnRoundStart;
                GameManager.Instance.OnRoundEnd += OnRoundEnd;
            }
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnRoundStart -= OnRoundStart;
                GameManager.Instance.OnRoundEnd -= OnRoundEnd;
            }
        }
        
        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.IsRoundActive)
            {
                UpdateOrderSpawning();
            }
        }
        
        /// <summary>
        /// Called when a round starts
        /// </summary>
        private void OnRoundStart()
        {
            Debug.Log("OrderManager: Round started, spawning initial orders");
            activeOrders.Clear();
            timeSinceLastSpawn = 0f;
            
            // Spawn initial orders
            SpawnInitialOrders();
        }
        
        /// <summary>
        /// Called when a round ends
        /// </summary>
        private void OnRoundEnd()
        {
            Debug.Log("OrderManager: Round ended, clearing orders");
            activeOrders.Clear();
        }
        
        /// <summary>
        /// Spawns initial orders at the start of the round
        /// </summary>
        private void SpawnInitialOrders()
        {
            // Spawn 2-3 initial orders to get players started
            int initialOrderCount = Mathf.Min(3, maxActiveOrders);
            for (int i = 0; i < initialOrderCount; i++)
            {
                SpawnOrder();
            }
        }
        
        /// <summary>
        /// Updates order spawning logic
        /// </summary>
        private void UpdateOrderSpawning()
        {
            timeSinceLastSpawn += Time.deltaTime;
            
            if (timeSinceLastSpawn >= spawnInterval && activeOrders.Count < maxActiveOrders)
            {
                SpawnOrder();
                timeSinceLastSpawn = 0f;
            }
        }
        
        /// <summary>
        /// Spawns a new order from the available pool
        /// TODO: Implement smarter order selection (difficulty scaling, variety)
        /// </summary>
        private void SpawnOrder()
        {
            if (availableOrders.Count == 0)
            {
                Debug.LogWarning("OrderManager: No orders available to spawn!");
                return;
            }
            
            // Pick a random order from the pool
            OrderData newOrder = availableOrders[Random.Range(0, availableOrders.Count)];
            activeOrders.Add(newOrder);
            
            Debug.Log($"OrderManager: Spawned new order: {newOrder.orderId} (Type: {newOrder.orderType})");
            OnOrderSpawned?.Invoke(newOrder);
        }
        
        /// <summary>
        /// Marks an order as completed
        /// TODO: Add validation that order requirements are actually met
        /// </summary>
        /// <param name="order">The order to complete</param>
        public void CompleteOrder(OrderData order)
        {
            if (!activeOrders.Contains(order))
            {
                Debug.LogWarning($"OrderManager: Trying to complete order {order.orderId} that is not active!");
                return;
            }
            
            activeOrders.Remove(order);
            
            // Calculate points
            int points = order.basePoints;
            if (order.orderType == OrderType.Express)
            {
                points += order.expressBonus;
            }
            
            // Update score
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.CompleteOrder(order.orderType == OrderType.Express, points);
            }
            
            Debug.Log($"OrderManager: Completed order {order.orderId} for {points} points");
            OnOrderCompleted?.Invoke(order);
        }
        
        /// <summary>
        /// Handles order expiration (for express orders)
        /// TODO: Implement express order timing and expiration
        /// </summary>
        /// <param name="order">The order that expired</param>
        public void ExpireOrder(OrderData order)
        {
            if (!activeOrders.Contains(order))
            {
                return;
            }
            
            activeOrders.Remove(order);
            Debug.Log($"OrderManager: Order {order.orderId} expired!");
            OnOrderExpired?.Invoke(order);
        }
    }
}
