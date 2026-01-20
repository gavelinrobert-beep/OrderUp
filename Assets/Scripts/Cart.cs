using UnityEngine;
using System.Collections.Generic;
using OrderUp.Data;

namespace OrderUp.Gameplay
{
    /// <summary>
    /// Represents a cart that can hold items picked by pickers
    /// Items can be added by pickers and removed by packers
    /// </summary>
    public class Cart : MonoBehaviour
    {
        [Header("Cart Settings")]
        [Tooltip("Maximum number of items the cart can hold")]
        [SerializeField] private int maxCapacity = 10;

        [Header("State")]
        [SerializeField] private List<PickableItem> items = new List<PickableItem>();

        public int CurrentCount => items.Count;
        public int MaxCapacity => maxCapacity;
        public bool IsFull => items.Count >= maxCapacity;
        public bool IsEmpty => items.Count == 0;

        /// <summary>
        /// Attempts to add an item to the cart
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <returns>True if item was successfully added</returns>
        public bool TryAddItem(PickableItem item)
        {
            if (item == null)
            {
                Debug.LogWarning("Cart: Attempted to add null item");
                return false;
            }

            if (IsFull)
            {
                Debug.LogWarning("Cart: Cart is full, cannot add more items");
                return false;
            }

            items.Add(item);
            
            // Parent the item to the cart for visual representation
            item.transform.SetParent(transform);
            item.transform.localPosition = Vector3.up * (0.5f + items.Count * 0.2f);
            
            Debug.Log($"Cart: Added {item.ProductData?.productName ?? "Unknown Item"} to cart ({CurrentCount}/{MaxCapacity})");
            return true;
        }

        /// <summary>
        /// Attempts to remove an item from the cart
        /// </summary>
        /// <returns>The removed item, or null if cart is empty</returns>
        public PickableItem TryRemoveItem()
        {
            if (IsEmpty)
            {
                Debug.LogWarning("Cart: Cart is empty, cannot remove items");
                return null;
            }

            PickableItem item = items[items.Count - 1];
            items.RemoveAt(items.Count - 1);
            
            Debug.Log($"Cart: Removed {item.ProductData?.productName ?? "Unknown Item"} from cart ({CurrentCount}/{MaxCapacity})");
            return item;
        }

        /// <summary>
        /// Gets all items currently in the cart
        /// </summary>
        /// <returns>Read-only list of items</returns>
        public IReadOnlyList<PickableItem> GetItems()
        {
            return items.AsReadOnly();
        }

        /// <summary>
        /// Clears all items from the cart
        /// </summary>
        public void Clear()
        {
            items.Clear();
            Debug.Log("Cart: Cleared all items");
        }
    }
}
