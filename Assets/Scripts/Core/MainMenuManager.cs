using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using OrderUp.Audio;

namespace OrderUp.UI
{
    /// <summary>
    /// Manages the main menu scene including start game and quit functionality.
    /// </summary>
    public class MainMenuManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button quitButton;
        
        [Header("Scene Settings")]
        [SerializeField] private string gameSceneName = "Main";
        
        private void Start()
        {
            // Set up button listeners
            if (startGameButton != null)
            {
                startGameButton.onClick.AddListener(OnStartGameClicked);
            }
            
            if (quitButton != null)
            {
                quitButton.onClick.AddListener(OnQuitClicked);
            }
        }
        
        /// <summary>
        /// Handles the Start Game button click.
        /// Loads the main game scene.
        /// </summary>
        public void OnStartGameClicked()
        {
            Debug.Log("MainMenuManager: Starting game - Loading Main scene");
            
            // Play UI click sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayUIClick();
            }
            
            // Load the main game scene
            SceneManager.LoadScene(gameSceneName);
        }
        
        /// <summary>
        /// Handles the Quit button click.
        /// Exits the application.
        /// </summary>
        public void OnQuitClicked()
        {
            Debug.Log("MainMenuManager: Quitting application");
            
            // Play UI click sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayUIClick();
            }
            
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}
