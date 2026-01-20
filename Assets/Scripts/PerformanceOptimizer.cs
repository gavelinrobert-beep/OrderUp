using UnityEngine;
using System.Collections.Generic;

namespace OrderUp.Core
{
    /// <summary>
    /// Manages object pooling and performance optimizations for the game.
    /// Reduces garbage collection overhead and improves frame rates.
    /// </summary>
    public class PerformanceOptimizer : MonoBehaviour
    {
        public static PerformanceOptimizer Instance { get; private set; }

        [Header("Pooling Settings")]
        [SerializeField] private int initialPoolSize = 20;
        [SerializeField] private bool allowPoolGrowth = true;

        [Header("Performance Settings")]
        [SerializeField] private int targetFrameRate = 60;
        [SerializeField] private bool enableVSync = true;

        private Dictionary<GameObject, Queue<GameObject>> objectPools = new Dictionary<GameObject, Queue<GameObject>>();
        private Dictionary<GameObject, Transform> poolParents = new Dictionary<GameObject, Transform>();

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

            InitializePerformanceSettings();
        }

        private void InitializePerformanceSettings()
        {
            // Set target frame rate
            Application.targetFrameRate = targetFrameRate;

            // Configure VSync
            QualitySettings.vSyncCount = enableVSync ? 1 : 0;

            Debug.Log($"PerformanceOptimizer: Target FPS set to {targetFrameRate}, VSync: {enableVSync}");
        }

        /// <summary>
        /// Creates an object pool for the specified prefab.
        /// </summary>
        public void CreatePool(GameObject prefab, int size = 0)
        {
            if (prefab == null) return;
            if (objectPools.ContainsKey(prefab)) return;

            int poolSize = size > 0 ? size : initialPoolSize;
            Queue<GameObject> pool = new Queue<GameObject>();

            // Create pool parent for organization
            Transform poolParent = new GameObject($"Pool_{prefab.name}").transform;
            poolParent.SetParent(transform);
            poolParents[prefab] = poolParent;

            // Pre-instantiate objects
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(prefab, poolParent);
                obj.SetActive(false);
                pool.Enqueue(obj);
            }

            objectPools[prefab] = pool;
            Debug.Log($"PerformanceOptimizer: Created pool for {prefab.name} with {poolSize} objects");
        }

        /// <summary>
        /// Gets an object from the pool or creates a new one if needed.
        /// </summary>
        public GameObject GetFromPool(GameObject prefab)
        {
            if (prefab == null) return null;

            // Create pool if it doesn't exist
            if (!objectPools.ContainsKey(prefab))
            {
                CreatePool(prefab);
            }

            Queue<GameObject> pool = objectPools[prefab];

            // Get object from pool or create new one
            GameObject obj;
            if (pool.Count > 0)
            {
                obj = pool.Dequeue();
            }
            else if (allowPoolGrowth)
            {
                obj = Instantiate(prefab, poolParents[prefab]);
                Debug.LogWarning($"PerformanceOptimizer: Pool for {prefab.name} grew - consider increasing initial size");
            }
            else
            {
                Debug.LogError($"PerformanceOptimizer: Pool for {prefab.name} exhausted and growth is disabled");
                return null;
            }

            obj.SetActive(true);
            return obj;
        }

        /// <summary>
        /// Returns an object to the pool.
        /// </summary>
        public void ReturnToPool(GameObject prefab, GameObject obj)
        {
            if (prefab == null || obj == null) return;

            if (!objectPools.ContainsKey(prefab))
            {
                Debug.LogWarning($"PerformanceOptimizer: No pool exists for {prefab.name}");
                Destroy(obj);
                return;
            }

            obj.SetActive(false);
            obj.transform.SetParent(poolParents[prefab]);
            objectPools[prefab].Enqueue(obj);
        }

        /// <summary>
        /// Clears all object pools.
        /// </summary>
        public void ClearAllPools()
        {
            foreach (var pool in objectPools.Values)
            {
                while (pool.Count > 0)
                {
                    GameObject obj = pool.Dequeue();
                    if (obj != null)
                    {
                        Destroy(obj);
                    }
                }
            }

            objectPools.Clear();
            poolParents.Clear();

            Debug.Log("PerformanceOptimizer: All pools cleared");
        }

        /// <summary>
        /// Gets current pool statistics.
        /// </summary>
        public string GetPoolStats()
        {
            System.Text.StringBuilder stats = new System.Text.StringBuilder();
            stats.AppendLine("Object Pool Statistics:");

            foreach (var kvp in objectPools)
            {
                string prefabName = kvp.Key != null ? kvp.Key.name : "Unknown";
                int available = kvp.Value.Count;
                stats.AppendLine($"  {prefabName}: {available} available");
            }

            return stats.ToString();
        }

        /// <summary>
        /// Gets current performance statistics.
        /// </summary>
        public string GetPerformanceStats()
        {
            float fps = 1f / Time.deltaTime;
            return $"FPS: {fps:F1} | Target: {targetFrameRate} | VSync: {enableVSync}";
        }
    }
}
