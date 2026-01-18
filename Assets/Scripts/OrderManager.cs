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

        [System.Serializable]
        private struct ActiveOrder
        {
            public int instanceId;
            public OrderData orderData;
        }
        
        [Header("Order Pool")]
        [Tooltip("Pool of orders that can be spawned during gameplay")]
        [SerializeField] private List<OrderData> availableOrders = new List<OrderData>();
        
        [Header("Spawn Settings")]
        [Tooltip("Time between order spawns in seconds")]
        [SerializeField] private float spawnInterval = 30f;
        
        [Tooltip("Maximum number of active orders at once")]
        [SerializeField] private int maxActiveOrders = 5;
        
        [Header("Runtime State")]
        [SerializeField] private List<ActiveOrder> activeOrders = new List<ActiveOrder>();
        private float timeSinceLastSpawn = 0f;
        private int nextOrderInstanceId = 0;
        private readonly Dictionary<int, float> orderSpawnTimes = new Dictionary<int, float>();
        
        // Events for UI and other systems
        public event System.Action<OrderData> OnOrderSpawned;
        public event System.Action<OrderData> OnOrderCompleted;
        public event System.Action<OrderData> OnOrderExpired;
        
        public List<OrderData> ActiveOrders
        {
            get
            {
                List<OrderData> orders = new List<OrderData>(activeOrders.Count);
                foreach (ActiveOrder activeOrder in activeOrders)
                {
                    orders.Add(activeOrder.orderData);
                }
                return orders;
            }
        }
        
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
            nextOrderInstanceId = 0;
            orderSpawnTimes.Clear();
            
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
            orderSpawnTimes.Clear();
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

            UpdateOrderExpirations();
            
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
            int instanceId = nextOrderInstanceId++;
            activeOrders.Add(new ActiveOrder
            {
                instanceId = instanceId,
                orderData = newOrder
            });
            orderSpawnTimes[instanceId] = Time.time;
            
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
            if (!TryRemoveActiveOrder(order, out ActiveOrder removedOrder))
            {
                Debug.LogWarning($"OrderManager: Trying to complete order {order.orderId} that is not active!");
                return;
            }
            
            orderSpawnTimes.Remove(removedOrder.instanceId);
            
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
        /// </summary>
        /// <param name="order">The order that expired</param>
        public void ExpireOrder(OrderData order)
        {
            if (!TryGetActiveOrder(order, out ActiveOrder activeOrder))
            {
                return;
            }

            ExpireOrderInstance(activeOrder.instanceId);
        }

        private void ExpireOrderInstance(int instanceId)
        {
            if (!TryRemoveActiveOrder(instanceId, out ActiveOrder removedOrder))
            {
                return;
            }

            orderSpawnTimes.Remove(removedOrder.instanceId);
            Debug.Log($"OrderManager: Order {removedOrder.orderData.orderId} expired!");
            OnOrderExpired?.Invoke(removedOrder.orderData);
        }

        private void UpdateOrderExpirations()
        {
            List<int> expiredInstanceIds = null;

            for (int i = 0; i < activeOrders.Count; i++)
            {
                ActiveOrder activeOrder = activeOrders[i];
                if (activeOrder.orderData.orderType != OrderType.Express)
                {
                    continue;
                }

                if (!orderSpawnTimes.TryGetValue(activeOrder.instanceId, out float spawnTime))
                {
                    continue;
                }

                if (Time.time - spawnTime >= activeOrder.orderData.expressTimeLimit)
                {
                    if (expiredInstanceIds == null)
                    {
                        expiredInstanceIds = new List<int>();
                    }
                    expiredInstanceIds.Add(activeOrder.instanceId);
                }
            }

            if (expiredInstanceIds == null)
            {
                return;
            }

            foreach (int instanceId in expiredInstanceIds)
            {
                ExpireOrderInstance(instanceId);
            }
        }

        private bool TryGetActiveOrder(OrderData order, out ActiveOrder activeOrder)
        {
            for (int i = 0; i < activeOrders.Count; i++)
            {
                if (activeOrders[i].orderData == order)
                {
                    activeOrder = activeOrders[i];
                    return true;
                }
            }

            activeOrder = default;
            return false;
        }

        private bool TryRemoveActiveOrder(OrderData order, out ActiveOrder removedOrder)
        {
            for (int i = 0; i < activeOrders.Count; i++)
            {
                if (activeOrders[i].orderData == order)
                {
                    removedOrder = activeOrders[i];
                    activeOrders.RemoveAt(i);
                    return true;
                }
            }

            removedOrder = default;
            return false;
        }

        private bool TryRemoveActiveOrder(int instanceId, out ActiveOrder removedOrder)
        {
            for (int i = 0; i < activeOrders.Count; i++)
            {
                if (activeOrders[i].instanceId == instanceId)
                {
                    removedOrder = activeOrders[i];
                    activeOrders.RemoveAt(i);
                    return true;
                }
            }

            removedOrder = default;
            return false;
        }
    }
}
