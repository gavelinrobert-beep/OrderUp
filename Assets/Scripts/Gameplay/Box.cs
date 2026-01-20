using UnityEngine;
using System.Collections.Generic;
using OrderUp.Data;

namespace OrderUp.Gameplay
{
    /// <summary>
    /// Represents a box that can hold items for packing
    /// Boxes are filled at packing stations and completed with labels
    /// </summary>
    public class Box : MonoBehaviour
    {
        [Header("Box Settings")]
        [Tooltip("Maximum number of items the box can hold")]
        [SerializeField] private int maxCapacity = 5;

        [Header("State")]
        [SerializeField] private List<ProductData> packedProducts = new List<ProductData>();
        [SerializeField] private bool isLabeled = false;
        [SerializeField] private OrderData assignedOrder = null;

        public int CurrentCount => packedProducts.Count;
        public int MaxCapacity => maxCapacity;
        public bool IsFull => packedProducts.Count >= maxCapacity;
        public bool IsEmpty => packedProducts.Count == 0;
        public bool IsLabeled => isLabeled;
        public OrderData AssignedOrder => assignedOrder;

        /// <summary>
        /// Attempts to add a product to the box
        /// </summary>
        /// <param name="product">The product to add</param>
        /// <returns>True if product was successfully added</returns>
        public bool TryAddProduct(ProductData product)
        {
            if (product == null)
            {
                Debug.LogWarning("Box: Attempted to add null product");
                return false;
            }

            if (IsFull)
            {
                Debug.LogWarning("Box: Box is full, cannot add more products");
                return false;
            }

            packedProducts.Add(product);
            Debug.Log($"Box: Added {product.productName} to box ({CurrentCount}/{MaxCapacity})");
            return true;
        }

        /// <summary>
        /// Applies a label to the box for a specific order
        /// </summary>
        /// <param name="order">The order this box is for</param>
        /// <returns>True if label was successfully applied</returns>
        public bool TryApplyLabel(OrderData order)
        {
            if (order == null)
            {
                Debug.LogWarning("Box: Attempted to apply null order label");
                return false;
            }

            if (IsEmpty)
            {
                Debug.LogWarning("Box: Cannot label an empty box");
                return false;
            }

            if (isLabeled)
            {
                Debug.LogWarning("Box: Box is already labeled");
                return false;
            }

            isLabeled = true;
            assignedOrder = order;
            Debug.Log($"Box: Applied label for order {order.orderId}");
            return true;
        }

        /// <summary>
        /// Gets all products currently in the box
        /// </summary>
        /// <returns>Read-only list of products</returns>
        public IReadOnlyList<ProductData> GetProducts()
        {
            return packedProducts.AsReadOnly();
        }

        /// <summary>
        /// Checks if this box is ready to be shipped
        /// </summary>
        /// <returns>True if box has items and is labeled</returns>
        public bool IsReadyToShip()
        {
            return !IsEmpty && isLabeled;
        }

        /// <summary>
        /// Resets the box to empty state
        /// </summary>
        public void Reset()
        {
            packedProducts.Clear();
            isLabeled = false;
            assignedOrder = null;
            Debug.Log("Box: Reset to empty state");
        }
    }
}
