using UnityEngine;
using System.Collections.Generic;
using OrderUp.Data;
using OrderUp.Gameplay;

namespace OrderUp.Environment
{
    /// <summary>
    /// Spawns shelves with pickable items in the picking zone
    /// Manages the warehouse inventory system
    /// </summary>
    public class ShelfManager : MonoBehaviour
    {
        [Header("Shelf Settings")]
        [Tooltip("Product data assets available in the warehouse")]
        [SerializeField] private List<ProductData> availableProducts = new List<ProductData>();

        [Tooltip("Number of shelves to spawn")]
        [SerializeField] private int shelfCount = 4;

        [Tooltip("Items per shelf")]
        [SerializeField] private int itemsPerShelf = 5;

        [Header("Spawn Settings")]
        [Tooltip("Picking zone center position")]
        [SerializeField] private Vector3 pickingZoneCenter = new Vector3(0f, 0f, 7.5f);

        [Tooltip("Picking zone size")]
        [SerializeField] private Vector3 pickingZoneSize = new Vector3(15f, 0f, 10f);

        [Header("Prefabs")]
        [Tooltip("Shelf prefab (optional, will create basic shelf if null)")]
        [SerializeField] private GameObject shelfPrefab;

        private List<GameObject> spawnedShelves = new List<GameObject>();
        private List<PickableItem> spawnedItems = new List<PickableItem>();

        private void Start()
        {
            SpawnShelves();
        }

        /// <summary>
        /// Spawns all shelves with items in the picking zone
        /// </summary>
        public void SpawnShelves()
        {
            ClearShelves();

            if (availableProducts.Count == 0)
            {
                Debug.LogWarning("ShelfManager: No products available to spawn");
                return;
            }

            for (int i = 0; i < shelfCount; i++)
            {
                Vector3 shelfPosition = CalculateShelfPosition(i);
                GameObject shelf = CreateShelf(shelfPosition, i);
                spawnedShelves.Add(shelf);

                SpawnItemsOnShelf(shelf.transform, i);
            }

            Debug.Log($"ShelfManager: Spawned {shelfCount} shelves with {itemsPerShelf} items each");
        }

        /// <summary>
        /// Calculates the position for a shelf based on its index
        /// </summary>
        private Vector3 CalculateShelfPosition(int index)
        {
            // Arrange shelves in rows
            int shelvesPerRow = 2;
            int row = index / shelvesPerRow;
            int col = index % shelvesPerRow;

            float xOffset = (col - (shelvesPerRow - 1) / 2f) * 4f;
            float zOffset = pickingZoneCenter.z + (row * 3f) - 2f;

            return new Vector3(xOffset, 0f, zOffset);
        }

        /// <summary>
        /// Creates a shelf at the specified position
        /// </summary>
        private GameObject CreateShelf(Vector3 position, int index)
        {
            GameObject shelf;

            if (shelfPrefab != null)
            {
                shelf = Instantiate(shelfPrefab, position, Quaternion.identity, transform);
            }
            else
            {
                // Create a basic shelf using primitives
                shelf = new GameObject($"Shelf_{index}");
                shelf.transform.position = position;
                shelf.transform.SetParent(transform);

                // Create shelf structure
                GameObject shelfBase = GameObject.CreatePrimitive(PrimitiveType.Cube);
                shelfBase.name = "ShelfBase";
                shelfBase.transform.SetParent(shelf.transform);
                shelfBase.transform.localPosition = Vector3.zero;
                shelfBase.transform.localScale = new Vector3(3f, 2f, 0.5f);

                Renderer renderer = shelfBase.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = new Color(0.6f, 0.4f, 0.2f); // Brown color
                }
            }

            return shelf;
        }

        /// <summary>
        /// Spawns pickable items on a shelf
        /// </summary>
        private void SpawnItemsOnShelf(Transform shelf, int shelfIndex)
        {
            for (int i = 0; i < itemsPerShelf; i++)
            {
                // Pick a random product
                ProductData product = availableProducts[Random.Range(0, availableProducts.Count)];

                // Calculate item position on shelf
                Vector3 itemPosition = new Vector3(
                    ((i - itemsPerShelf / 2f) * 0.5f),
                    1.5f,
                    0f
                );

                // Create item
                GameObject itemObj = CreatePickableItem(product, shelf, itemPosition);
                PickableItem item = itemObj.GetComponent<PickableItem>();
                if (item != null)
                {
                    spawnedItems.Add(item);
                }
            }
        }

        /// <summary>
        /// Creates a pickable item for a product
        /// </summary>
        private GameObject CreatePickableItem(ProductData product, Transform parent, Vector3 localPosition)
        {
            GameObject itemObj;

            // Use product prefab if available
            if (product.prefab != null)
            {
                itemObj = Instantiate(product.prefab, parent);
            }
            else
            {
                // Create a basic item using primitives
                itemObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                itemObj.transform.localScale = Vector3.one * 0.3f;

                Renderer renderer = itemObj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    // Random color for visual variety
                    renderer.material.color = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
                }
            }

            itemObj.name = $"Item_{product.productName}";
            itemObj.transform.SetParent(parent);
            itemObj.transform.localPosition = localPosition;

            // Add PickableItem component
            PickableItem pickableItem = itemObj.GetComponent<PickableItem>();
            if (pickableItem == null)
            {
                pickableItem = itemObj.AddComponent<PickableItem>();
            }

            // Set product data using the public setter
            pickableItem.SetProductData(product);

            return itemObj;
        }

        /// <summary>
        /// Clears all spawned shelves and items
        /// </summary>
        public void ClearShelves()
        {
            foreach (GameObject shelf in spawnedShelves)
            {
                if (shelf != null)
                {
                    Destroy(shelf);
                }
            }
            spawnedShelves.Clear();
            spawnedItems.Clear();
        }

        /// <summary>
        /// Restocks empty shelves
        /// </summary>
        public void RestockShelves()
        {
            // Remove null items (picked items that were destroyed)
            spawnedItems.RemoveAll(item => item == null);

            // Check each shelf and restock if needed
            for (int i = 0; i < spawnedShelves.Count; i++)
            {
                GameObject shelf = spawnedShelves[i];
                if (shelf == null) continue;

                int itemCount = shelf.GetComponentsInChildren<PickableItem>().Length;
                if (itemCount < itemsPerShelf / 2)
                {
                    SpawnItemsOnShelf(shelf.transform, i);
                }
            }
        }

        private void OnDrawGizmos()
        {
            // Visualize picking zone
            Gizmos.color = new Color(0.5f, 0.8f, 0.5f, 0.3f);
            Gizmos.DrawWireCube(pickingZoneCenter, pickingZoneSize);
        }
    }
}
