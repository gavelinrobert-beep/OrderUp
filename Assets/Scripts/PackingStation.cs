using UnityEngine;
using OrderUp.Data;
using OrderUp.Core;
using System.Collections.Generic;

namespace OrderUp.Gameplay
{
    /// <summary>
    /// Represents a packing station where packers can pack boxes and complete orders
    /// </summary>
    public class PackingStation : MonoBehaviour
    {
        [Header("Station Settings")]
        [Tooltip("Reference to the box at this station")]
        [SerializeField] private Box currentBox;

        [Tooltip("Transform where the box is positioned")]
        [SerializeField] private Transform boxPosition;

        [Header("State")]
        [SerializeField] private bool isOccupied = false;

        public bool IsOccupied => isOccupied;
        public Box CurrentBox => currentBox;

        private void Start()
        {
            // Initialize with a new box if none is assigned
            if (currentBox == null)
            {
                CreateNewBox();
            }
        }

        /// <summary>
        /// Attempts to pack an item into the box at this station
        /// </summary>
        /// <param name="item">The pickable item to pack</param>
        /// <returns>True if item was successfully packed</returns>
        public bool TryPackItem(PickableItem item)
        {
            if (item == null)
            {
                Debug.LogWarning("PackingStation: Attempted to pack null item");
                return false;
            }

            if (currentBox == null)
            {
                Debug.LogWarning("PackingStation: No box available at station");
                return false;
            }

            if (item.ProductData == null)
            {
                Debug.LogWarning("PackingStation: Item has no product data");
                return false;
            }

            bool success = currentBox.TryAddProduct(item.ProductData);
            if (success)
            {
                // Destroy the physical item after packing
                Destroy(item.gameObject);
                Debug.Log($"PackingStation: Packed {item.ProductData.productName} into box");
            }

            return success;
        }

        /// <summary>
        /// Attempts to apply a label to the current box for a specific order
        /// </summary>
        /// <param name="order">The order to label the box for</param>
        /// <returns>True if label was successfully applied</returns>
        public bool TryApplyLabel(OrderData order)
        {
            if (currentBox == null)
            {
                Debug.LogWarning("PackingStation: No box available at station");
                return false;
            }

            return currentBox.TryApplyLabel(order);
        }

        /// <summary>
        /// Attempts to complete and ship the current box
        /// Validates the order and updates score if valid
        /// </summary>
        /// <returns>True if box was successfully completed</returns>
        public bool TryCompleteOrder()
        {
            if (currentBox == null)
            {
                Debug.LogWarning("PackingStation: No box available at station");
                return false;
            }

            if (!currentBox.IsReadyToShip())
            {
                Debug.LogWarning("PackingStation: Box is not ready to ship (needs items and label)");
                return false;
            }

            OrderData order = currentBox.AssignedOrder;
            if (order == null)
            {
                Debug.LogWarning("PackingStation: Box has no assigned order");
                return false;
            }

            // Validate the order
            List<ProductData> packedProducts = new List<ProductData>(currentBox.GetProducts());
            if (OrderManager.Instance != null && 
                OrderManager.Instance.ValidateOrder(order, packedProducts, out int points))
            {
                // Complete the order
                OrderManager.Instance.CompleteOrder(order);
                
                Debug.Log($"PackingStation: Completed and shipped order {order.orderId} for {points} points");
                
                // Reset the box for next order
                CreateNewBox();
                return true;
            }
            else
            {
                Debug.LogWarning($"PackingStation: Order {order.orderId} validation failed");
                return false;
            }
        }

        /// <summary>
        /// Creates a new empty box at the station
        /// </summary>
        private void CreateNewBox()
        {
            // Clean up old box
            if (currentBox != null)
            {
                Destroy(currentBox.gameObject);
            }

            // Create new box
            GameObject boxObject = new GameObject("Box");
            boxObject.transform.SetParent(boxPosition != null ? boxPosition : transform);
            boxObject.transform.localPosition = Vector3.zero;
            
            currentBox = boxObject.AddComponent<Box>();
            
            Debug.Log("PackingStation: Created new box");
        }

        /// <summary>
        /// Sets the occupied state of the station
        /// </summary>
        /// <param name="occupied">True if a packer is using this station</param>
        public void SetOccupied(bool occupied)
        {
            isOccupied = occupied;
        }
    }
}
