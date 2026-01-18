using UnityEngine;

namespace OrderUp.Data
{
    /// <summary>
    /// Defines product category for filtering or grouping.
    /// </summary>
    public enum ProductCategory
    {
        Uncategorized
    }

    /// <summary>
    /// ScriptableObject defining a product that can be picked and packed.
    /// Includes references for UI icon and world prefab.
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
        
        // TODO: Add rarity or availability settings
    }
}
