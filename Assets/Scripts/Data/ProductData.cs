using UnityEngine;

namespace OrderUp.Data
{
    /// <summary>
    /// Defines product category for filtering or grouping.
    /// </summary>
    public enum ProductCategory
    {
        Uncategorized,
        Food,
        Electronics,
        Clothing
    }

    /// <summary>
    /// Defines product rarity level affecting spawn rates and availability.
    /// </summary>
    public enum ProductRarity
    {
        Common,      // Frequently available
        Uncommon,    // Moderately available
        Rare,        // Occasionally available
        VeryRare     // Rarely available
    }
    
    /// <summary>
    /// ScriptableObject defining a product that can be picked and packed.
    /// Includes references for UI icon and world prefab, plus rarity and availability settings.
    /// </summary>
    [CreateAssetMenu(fileName = "NewProduct", menuName = "OrderUp/Product Data")]
    public class ProductData : ScriptableObject
    {
        [Header("Basic Info")]
        [Tooltip("Unique identifier for this product")]
        public string productId;
        
        [Tooltip("Display name of the product")]
        public string productName;
        
        [Tooltip("Description of the product")]
        [TextArea(2, 4)]
        public string description;

        [Header("Visuals")]
        [Tooltip("Icon used in UI for this product")]
        public Sprite icon;

        [Tooltip("Prefab reference for the product in the world")]
        public GameObject prefab;

        [Header("Classification")]
        [Tooltip("Category for grouping or filtering products")]
        public ProductCategory category = ProductCategory.Uncategorized;
        
        [Header("Gameplay Properties")]
        [Tooltip("Weight of the product (affects carrying capacity)")]
        public float weight = 1f;
        
        [Tooltip("Base points earned when this product is included in a completed order")]
        public int basePoints = 10;
        
        [Header("Availability")]
        [Tooltip("Rarity level of this product affecting spawn rates")]
        public ProductRarity rarity = ProductRarity.Common;
        
        [Tooltip("Spawn weight used for random selection (higher = more common)")]
        [Range(1, 100)]
        public int spawnWeight = 50;
        
        [Tooltip("Is this product currently available for spawning?")]
        public bool isAvailable = true;
        
        /// <summary>
        /// Gets the rarity multiplier for scoring purposes.
        /// </summary>
        public float GetRarityMultiplier()
        {
            switch (rarity)
            {
                case ProductRarity.Common:
                    return 1.0f;
                case ProductRarity.Uncommon:
                    return 1.5f;
                case ProductRarity.Rare:
                    return 2.0f;
                case ProductRarity.VeryRare:
                    return 3.0f;
                default:
                    return 1.0f;
            }
        }
        
        /// <summary>
        /// Calculates the effective spawn rate based on rarity and spawn weight.
        /// </summary>
        public float GetEffectiveSpawnRate()
        {
            if (!isAvailable) return 0f;
            
            float rarityFactor = 1f;
            switch (rarity)
            {
                case ProductRarity.Uncommon:
                    rarityFactor = 0.7f;
                    break;
                case ProductRarity.Rare:
                    rarityFactor = 0.4f;
                    break;
                case ProductRarity.VeryRare:
                    rarityFactor = 0.2f;
                    break;
            }
            
            return spawnWeight * rarityFactor;
        }
    }
}
