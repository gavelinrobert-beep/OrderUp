using UnityEngine;
using Mirror;

namespace OrderUp.Networking
{
    /// <summary>
    /// Custom NetworkManager for OrderUp
    /// Handles network setup, player spawning, and connection management
    /// </summary>
    public class OrderUpNetworkManager : NetworkManager
    {
        [Header("OrderUp Settings")]
        [Tooltip("Maximum number of players (2-4)")]
        [SerializeField] private int maxPlayers = 4;

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            // Check if max players reached
            if (numPlayers >= maxPlayers)
            {
                Debug.LogWarning($"OrderUpNetworkManager: Max players ({maxPlayers}) reached. Cannot add more players.");
                conn.Disconnect();
                return;
            }

            base.OnServerAddPlayer(conn);
            Debug.Log($"OrderUpNetworkManager: Player added. Total players: {numPlayers}/{maxPlayers}");
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);
            Debug.Log($"OrderUpNetworkManager: Player disconnected. Total players: {numPlayers}/{maxPlayers}");
        }

        public override void OnClientConnect()
        {
            base.OnClientConnect();
            Debug.Log("OrderUpNetworkManager: Client connected to server");
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            Debug.Log("OrderUpNetworkManager: Client disconnected from server");
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            Debug.Log("OrderUpNetworkManager: Server started");
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            Debug.Log("OrderUpNetworkManager: Server stopped");
        }
    }
}
