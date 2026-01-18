using UnityEngine;

namespace OrderUp.Data
{
    /// <summary>
    /// ScriptableObject defining a product that can be picked and packed.
    /// TODO: Add sprite/icon, physical prefab reference, rarity, etc.
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
        
        [Header("Gameplay Properties")]
        [Tooltip("Weight of the product (affects carrying capacity)")]
        public float weight = 1f;
        
        [Tooltip("Base points earned when this product is included in a completed order")]
        public int basePoints = 10;
        
        // TODO: Add product icon/sprite
        // TODO: Add 3D prefab reference for warehouse shelf placement
        // TODO: Add product category/type enum
        // TODO: Add rarity or availability settings
    }
}
