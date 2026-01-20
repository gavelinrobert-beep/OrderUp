using UnityEngine;

namespace OrderUp.Player
{
    /// <summary>
    /// Defines the role types available to players
    /// </summary>
    public enum PlayerRole
    {
        Picker,
        Packer
    }

    /// <summary>
    /// Manages a player's role and role-specific behaviors
    /// </summary>
    public class PlayerRoleManager : MonoBehaviour
    {
        [Header("Role Settings")]
        [Tooltip("The current role of this player")]
        [SerializeField] private PlayerRole currentRole = PlayerRole.Picker;

        [Header("Role Colors")]
        [Tooltip("Color for Picker role visualization")]
        [SerializeField] private Color pickerColor = new Color(0.5f, 0.8f, 0.5f);

        [Tooltip("Color for Packer role visualization")]
        [SerializeField] private Color packerColor = new Color(0.5f, 0.5f, 0.8f);

        public PlayerRole CurrentRole => currentRole;

        private void Start()
        {
            ApplyRoleVisualization();
        }

        /// <summary>
        /// Changes the player's role
        /// </summary>
        /// <param name="newRole">The new role to assign</param>
        public void SetRole(PlayerRole newRole)
        {
            currentRole = newRole;
            ApplyRoleVisualization();
            Debug.Log($"PlayerRoleManager: Role changed to {newRole}");
        }

        /// <summary>
        /// Toggles between Picker and Packer roles
        /// </summary>
        public void ToggleRole()
        {
            PlayerRole newRole = currentRole == PlayerRole.Picker ? PlayerRole.Packer : PlayerRole.Picker;
            SetRole(newRole);
        }

        /// <summary>
        /// Applies visual indicators for the current role
        /// </summary>
        private void ApplyRoleVisualization()
        {
            // Find renderer on player or children
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            Color roleColor = currentRole == PlayerRole.Picker ? pickerColor : packerColor;

            foreach (Renderer renderer in renderers)
            {
                // Skip certain renderers if needed
                if (renderer.gameObject.CompareTag("UI"))
                {
                    continue;
                }

                renderer.material.color = roleColor;
            }
        }

        /// <summary>
        /// Checks if the player can perform picker actions
        /// </summary>
        /// <returns>True if player is a Picker</returns>
        public bool CanPick()
        {
            return currentRole == PlayerRole.Picker;
        }

        /// <summary>
        /// Checks if the player can perform packer actions
        /// </summary>
        /// <returns>True if player is a Packer</returns>
        public bool CanPack()
        {
            return currentRole == PlayerRole.Packer;
        }
    }
}
