using UnityEngine;
using Mirror;
using OrderUp.Data;
using OrderUp.Gameplay;
using OrderUp.Audio;

namespace OrderUp.Player
{
    /// <summary>
    /// Handles player interactions with game objects (picking items, using carts, packing)
    /// Extends PlayerController with role-specific interaction mechanics
    /// </summary>
    [RequireComponent(typeof(PlayerRoleManager))]
    public class PlayerInteractionController : NetworkBehaviour
    {
        [Header("Interaction Settings")]
        [Tooltip("Maximum distance for interactions")]
        [SerializeField] private float interactionRange = 2f;

        [Tooltip("Layer mask for interactable objects")]
        [SerializeField] private LayerMask interactableLayer = ~0;

        [Header("Picker State")]
        [Tooltip("Item currently being held")]
        [SerializeField] private PickableItem heldItem = null;

        [Tooltip("Cart currently being used")]
        [SerializeField] private Cart currentCart = null;

        [Header("Packer State")]
        [Tooltip("Packing station currently being used")]
        [SerializeField] private PackingStation currentStation = null;

        [Tooltip("Order being packed")]
        [SerializeField] private OrderData currentOrder = null;

        private PlayerRoleManager roleManager;

        private void Awake()
        {
            roleManager = GetComponent<PlayerRoleManager>();
        }

        private void Update()
        {
            // Only allow local player to interact
            if (!isLocalPlayer) return;

            HandleInteractionInput();
            HandleRoleToggle();
        }

        private void HandleRoleToggle()
        {
            // Toggle role with 'R' key
            if (Input.GetKeyDown(KeyCode.R))
            {
                roleManager.ToggleRole();
            }
        }

        private void HandleInteractionInput()
        {
            // Interact with 'E' key
            if (Input.GetKeyDown(KeyCode.E))
            {
                TryInteract();
            }

            // Drop/Cancel with 'Q' key
            if (Input.GetKeyDown(KeyCode.Q))
            {
                TryDropOrCancel();
            }

            // Complete action with 'F' key (for packers to complete orders)
            if (Input.GetKeyDown(KeyCode.F))
            {
                TryCompleteAction();
            }
        }

        private void TryInteract()
        {
            if (roleManager.CanPick())
            {
                HandlePickerInteraction();
            }
            else if (roleManager.CanPack())
            {
                HandlePackerInteraction();
            }
        }

        private void HandlePickerInteraction()
        {
            // Check for nearby interactable objects
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange, interactableLayer);

            foreach (Collider hitCollider in hitColliders)
            {
                // Try to pick an item
                if (heldItem == null)
                {
                    PickableItem item = hitCollider.GetComponent<PickableItem>();
                    if (item != null && !item.IsPicked && item.TryPick())
                    {
                        heldItem = item;
                        Debug.Log($"PlayerInteraction: Picked up {item.ProductData?.productName ?? "item"}");
                        
                        // Play audio and visual feedback
                        if (AudioManager.Instance != null)
                            AudioManager.Instance.PlayItemPickup();
                        if (VisualFeedbackManager.Instance != null)
                            VisualFeedbackManager.Instance.ShowItemPickup(item.transform.position);
                        
                        return;
                    }
                }

                // Try to interact with a cart
                Cart cart = hitCollider.GetComponent<Cart>();
                if (cart != null)
                {
                    if (heldItem != null && currentCart == null)
                    {
                        // Add item to cart
                        if (cart.TryAddItem(heldItem))
                        {
                            heldItem = null;
                            Debug.Log("PlayerInteraction: Added item to cart");
                            
                            // Play audio feedback
                            if (AudioManager.Instance != null)
                                AudioManager.Instance.PlayCartInteraction();
                        }
                        return;
                    }
                    else if (currentCart == null)
                    {
                        // Start using cart
                        currentCart = cart;
                        Debug.Log("PlayerInteraction: Started using cart");
                        
                        // Play audio feedback
                        if (AudioManager.Instance != null)
                            AudioManager.Instance.PlayCartInteraction();
                        
                        return;
                    }
                }
            }
        }

        private void HandlePackerInteraction()
        {
            // Check for nearby interactable objects
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange, interactableLayer);

            foreach (Collider hitCollider in hitColliders)
            {
                // Try to interact with a packing station
                PackingStation station = hitCollider.GetComponent<PackingStation>();
                if (station != null)
                {
                    if (currentStation == null)
                    {
                        // Start using packing station
                        currentStation = station;
                        currentStation.SetOccupied(true);
                        Debug.Log("PlayerInteraction: Started using packing station");
                        return;
                    }
                }

                // Try to interact with a cart to take items
                if (currentStation != null)
                {
                    Cart cart = hitCollider.GetComponent<Cart>();
                    if (cart != null && !cart.IsEmpty)
                    {
                        // Take item from cart and pack it
                        PickableItem item = cart.TryRemoveItem();
                        if (item != null)
                        {
                            if (currentStation.TryPackItem(item))
                            {
                                Debug.Log($"PlayerInteraction: Packed {item.ProductData?.productName ?? "item"}");
                                
                                // Play audio feedback
                                if (AudioManager.Instance != null)
                                    AudioManager.Instance.PlayItemPickup();
                            }
                        }
                        return;
                    }
                }
            }
        }

        private void TryDropOrCancel()
        {
            if (roleManager.CanPick())
            {
                // Drop held item
                if (heldItem != null)
                {
                    heldItem.Place();
                    heldItem.transform.SetParent(null);
                    heldItem = null;
                    Debug.Log("PlayerInteraction: Dropped item");
                    
                    // Play audio feedback
                    if (AudioManager.Instance != null)
                        AudioManager.Instance.PlayItemDrop();
                }
                // Stop using cart
                else if (currentCart != null)
                {
                    currentCart = null;
                    Debug.Log("PlayerInteraction: Stopped using cart");
                }
            }
            else if (roleManager.CanPack())
            {
                // Stop using packing station
                if (currentStation != null)
                {
                    currentStation.SetOccupied(false);
                    currentStation = null;
                    currentOrder = null;
                    Debug.Log("PlayerInteraction: Stopped using packing station");
                }
            }
        }

        private void TryCompleteAction()
        {
            if (roleManager.CanPack() && currentStation != null)
            {
                // TODO: Implement UI-based order selection instead of always using first order
                // For MVP, select first active order as a simple implementation
                if (Core.OrderManager.Instance != null)
                {
                    var activeOrders = Core.OrderManager.Instance.ActiveOrders;
                    if (activeOrders.Count > 0)
                    {
                        OrderData orderToComplete = activeOrders[0];
                        
                        // Apply label
                        if (currentStation.TryApplyLabel(orderToComplete))
                        {
                            Debug.Log($"PlayerInteraction: Applied label for order {orderToComplete.orderId}");
                            
                            // Try to complete the order
                            if (currentStation.TryCompleteOrder())
                            {
                                Debug.Log($"PlayerInteraction: Completed order {orderToComplete.orderId}");
                                
                                // Play audio and visual feedback
                                if (AudioManager.Instance != null)
                                    AudioManager.Instance.PlayOrderComplete();
                                if (VisualFeedbackManager.Instance != null)
                                    VisualFeedbackManager.Instance.ShowOrderComplete(currentStation.transform.position);
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning("PlayerInteraction: No active orders to complete");
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Visualize interaction range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
        }
    }
}
