using UnityEngine;
using OrderUp.Data;

namespace OrderUp.Gameplay
{
    /// <summary>
    /// Represents an item that can be picked from a shelf and placed in a cart
    /// </summary>
    public class PickableItem : MonoBehaviour
    {
        [Header("Item Data")]
        [Tooltip("Product data for this item")]
        [SerializeField] private ProductData productData;

        [Header("State")]
        [SerializeField] private bool isPicked = false;

        public ProductData ProductData => productData;
        public bool IsPicked => isPicked;

        /// <summary>
        /// Sets the product data for this item (used during spawning)
        /// </summary>
        /// <param name="product">The product data to assign</param>
        public void SetProductData(ProductData product)
        {
            productData = product;
            if (product != null && !string.IsNullOrEmpty(product.productName))
            {
                gameObject.name = $"Item_{product.productName}";
            }
        }

        /// <summary>
        /// Attempts to pick this item
        /// </summary>
        /// <returns>True if item was successfully picked</returns>
        public bool TryPick()
        {
            if (isPicked)
            {
                return false;
            }

            isPicked = true;
            Debug.Log($"PickableItem: Picked {productData?.productName ?? "Unknown Item"}");
            return true;
        }

        /// <summary>
        /// Places the item back (e.g., when removed from cart)
        /// </summary>
        public void Place()
        {
            isPicked = false;
            Debug.Log($"PickableItem: Placed {productData?.productName ?? "Unknown Item"}");
        }

        private void OnValidate()
        {
            if (productData != null && string.IsNullOrEmpty(gameObject.name))
            {
                gameObject.name = $"Item_{productData.productName}";
            }
        }
    }
}
