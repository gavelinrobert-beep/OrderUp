using UnityEngine;
using Mirror;

namespace OrderUp.Networking
{
    /// <summary>
    /// Simple UI for hosting and joining games
    /// Uses IMGUI for quick prototyping
    /// </summary>
    public class NetworkUI : MonoBehaviour
    {
        private NetworkManager networkManager;

        private void Start()
        {
            networkManager = NetworkManager.singleton;
            if (networkManager == null)
            {
                Debug.LogError("NetworkUI: NetworkManager not found!");
            }
        }

        private void OnGUI()
        {
            if (networkManager == null) return;

            // Set GUI style
            GUI.skin.label.fontSize = 20;
            GUI.skin.button.fontSize = 18;

            int yOffset = 20;
            int buttonWidth = 200;
            int buttonHeight = 40;

            // Show status
            GUILayout.BeginArea(new Rect(20, yOffset, 400, 400));
            GUILayout.Label($"Network Status: {GetNetworkStatus()}");
            GUILayout.Space(20);

            // Show buttons based on network state
            if (!NetworkClient.isConnected && !NetworkServer.active)
            {
                if (GUILayout.Button("Host Game", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
                {
                    networkManager.StartHost();
                    Debug.Log("NetworkUI: Starting host...");
                }

                GUILayout.Space(10);

                if (GUILayout.Button("Join Game", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
                {
                    networkManager.StartClient();
                    Debug.Log("NetworkUI: Joining game...");
                }

                GUILayout.Space(10);

                if (GUILayout.Button("Server Only", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
                {
                    networkManager.StartServer();
                    Debug.Log("NetworkUI: Starting server...");
                }
            }
            else
            {
                GUILayout.Label($"Players: {NetworkServer.connections.Count}");
                GUILayout.Space(10);

                if (GUILayout.Button("Disconnect", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
                {
                    if (NetworkServer.active && NetworkClient.isConnected)
                    {
                        networkManager.StopHost();
                    }
                    else if (NetworkClient.isConnected)
                    {
                        networkManager.StopClient();
                    }
                    else if (NetworkServer.active)
                    {
                        networkManager.StopServer();
                    }
                    Debug.Log("NetworkUI: Disconnecting...");
                }
            }

            GUILayout.EndArea();
        }

        private string GetNetworkStatus()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                return "Host (Server + Client)";
            }
            else if (NetworkServer.active)
            {
                return "Server Only";
            }
            else if (NetworkClient.isConnected)
            {
                return "Client";
            }
            else
            {
                return "Offline";
            }
        }
    }
}
