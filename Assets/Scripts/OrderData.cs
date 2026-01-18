using UnityEngine;
using System.Collections.Generic;

namespace OrderUp.Data
{
    /// <summary>
    /// Defines the type of order: Standard or Express
    /// </summary>
    public enum OrderType
    {
        Standard,
        Express
    }

    /// <summary>
    /// Defines the difficulty of an order.
    /// </summary>
    public enum OrderDifficulty
    {
        Easy,
        Medium,
        Hard
    }
    
    /// <summary>
    /// ScriptableObject defining an order with required products.
    /// TODO: Add order complexity variations and special requirements.
    /// </summary>
    [CreateAssetMenu(fileName = "NewOrder", menuName = "OrderUp/Order Data")]
    public class OrderData : ScriptableObject
    {
        [Header("Order Info")]
        [Tooltip("Unique identifier for this order")]
        public string orderId;
        
        [Tooltip("Type of order: Standard or Express")]
        public OrderType orderType = OrderType.Standard;

        [Tooltip("Difficulty tier for this order")]
        public OrderDifficulty difficulty = OrderDifficulty.Easy;

        [Header("Customer")]
        [Tooltip("Name of the customer placing the order")]
        public string customerName;

        [Tooltip("Additional notes from the customer")]
        [TextArea(2, 4)]
        public string customerNotes;

        [Header("Requirements")]
        [Tooltip("List of products required for this order")]
        public List<ProductData> requiredProducts = new List<ProductData>();
        
        [Header("Scoring")]
        [Tooltip("Base points for completing this order")]
        public int basePoints = 50;
        
        [Tooltip("Bonus points for express orders")]
        public int expressBonus = 25;
        
        [Tooltip("Time limit for express orders (in seconds)")]
        public float expressTimeLimit = 60f;
        
        // TODO: Add special requirements (e.g., fragile handling)
        // TODO: Add order priority visualization data
    }
}
