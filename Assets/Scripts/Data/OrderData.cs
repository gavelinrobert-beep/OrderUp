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
    /// Defines special requirements for handling an order.
    /// </summary>
    [System.Flags]
    public enum SpecialRequirements
    {
        None = 0,
        Fragile = 1 << 0,      // Handle with care
        Refrigeration = 1 << 1, // Requires cold storage
        Hazardous = 1 << 2      // Special handling required
    }
    
    /// <summary>
    /// ScriptableObject defining an order with required products, special requirements, and priority visualization.
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
        
        [Header("Special Requirements")]
        [Tooltip("Special handling requirements for this order")]
        public SpecialRequirements specialRequirements = SpecialRequirements.None;
        
        [Header("Priority Visualization")]
        [Tooltip("Color used to highlight this order's priority in the UI")]
        public Color priorityColor = Color.white;
        
        [Tooltip("Icon to display for this order's priority level")]
        public Sprite priorityIcon;
        
        [Tooltip("Display order priority as urgency level (1-5, where 5 is most urgent)")]
        [Range(1, 5)]
        public int priorityLevel = 1;
        
        /// <summary>
        /// Checks if this order has a specific special requirement.
        /// </summary>
        public bool HasRequirement(SpecialRequirements requirement)
        {
            return (specialRequirements & requirement) != 0;
        }
        
        /// <summary>
        /// Returns a formatted string of all special requirements.
        /// </summary>
        public string GetRequirementsDescription()
        {
            if (specialRequirements == SpecialRequirements.None)
                return "No special requirements";
                
            System.Collections.Generic.List<string> requirements = new System.Collections.Generic.List<string>();
            
            if (HasRequirement(SpecialRequirements.Fragile))
                requirements.Add("Fragile");
            if (HasRequirement(SpecialRequirements.Refrigeration))
                requirements.Add("Refrigeration Required");
            if (HasRequirement(SpecialRequirements.Hazardous))
                requirements.Add("Hazardous Material");
                
            return string.Join(", ", requirements);
        }
    }
}
