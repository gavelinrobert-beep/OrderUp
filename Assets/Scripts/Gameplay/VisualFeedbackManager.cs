using UnityEngine;

namespace OrderUp.Gameplay
{
    /// <summary>
    /// Provides visual feedback for player interactions and game events.
    /// Manages particle effects, highlights, and other visual indicators.
    /// </summary>
    public class VisualFeedbackManager : MonoBehaviour
    {
        public static VisualFeedbackManager Instance { get; private set; }

        [Header("Particle Effects")]
        [SerializeField] private ParticleSystem itemPickupEffect;
        [SerializeField] private ParticleSystem orderCompleteEffect;
        [SerializeField] private ParticleSystem expressWarningEffect;

        [Header("Highlight Settings")]
        [SerializeField] private Color interactableHighlightColor = new Color(1f, 1f, 0.5f, 0.5f);
        [SerializeField] private Color selectedHighlightColor = new Color(0.5f, 1f, 0.5f, 0.7f);
        [SerializeField] private float highlightPulseSpeed = 2f;

        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Plays a particle effect at the specified position.
        /// </summary>
        private void PlayEffect(ParticleSystem effect, Vector3 position)
        {
            if (effect == null) return;

            ParticleSystem instance = Instantiate(effect, position, Quaternion.identity);
            Destroy(instance.gameObject, instance.main.duration + 1f);
        }

        /// <summary>
        /// Shows item pickup visual feedback.
        /// </summary>
        public void ShowItemPickup(Vector3 position)
        {
            PlayEffect(itemPickupEffect, position);
        }

        /// <summary>
        /// Shows order completion visual feedback.
        /// </summary>
        public void ShowOrderComplete(Vector3 position)
        {
            PlayEffect(orderCompleteEffect, position);
        }

        /// <summary>
        /// Shows express order warning visual feedback.
        /// </summary>
        public void ShowExpressWarning(Vector3 position)
        {
            PlayEffect(expressWarningEffect, position);
        }

        /// <summary>
        /// Adds a highlight to an interactable object.
        /// </summary>
        public void AddInteractableHighlight(GameObject target)
        {
            if (target == null) return;

            InteractableHighlight highlight = target.GetComponent<InteractableHighlight>();
            if (highlight == null)
            {
                highlight = target.AddComponent<InteractableHighlight>();
            }

            highlight.SetHighlight(interactableHighlightColor, highlightPulseSpeed);
        }

        /// <summary>
        /// Removes highlight from an object.
        /// </summary>
        public void RemoveHighlight(GameObject target)
        {
            if (target == null) return;

            InteractableHighlight highlight = target.GetComponent<InteractableHighlight>();
            if (highlight != null)
            {
                highlight.ClearHighlight();
            }
        }

        /// <summary>
        /// Shows a selected highlight on an object.
        /// </summary>
        public void ShowSelected(GameObject target)
        {
            if (target == null) return;

            InteractableHighlight highlight = target.GetComponent<InteractableHighlight>();
            if (highlight == null)
            {
                highlight = target.AddComponent<InteractableHighlight>();
            }

            highlight.SetHighlight(selectedHighlightColor, 0f);
        }
    }

    /// <summary>
    /// Component that manages highlight rendering for an interactable object.
    /// </summary>
    public class InteractableHighlight : MonoBehaviour
    {
        private Renderer[] renderers;
        private Color highlightColor;
        private float pulseSpeed;
        private bool isHighlighted;
        private Material[] originalMaterials;
        private Material[] highlightMaterials;

        private void Awake()
        {
            renderers = GetComponentsInChildren<Renderer>();
            StoreOriginalMaterials();
        }

        private void StoreOriginalMaterials()
        {
            int totalMaterials = 0;
            foreach (var renderer in renderers)
            {
                totalMaterials += renderer.materials.Length;
            }

            originalMaterials = new Material[totalMaterials];
            int index = 0;

            foreach (var renderer in renderers)
            {
                foreach (var mat in renderer.materials)
                {
                    originalMaterials[index++] = mat;
                }
            }
        }

        public void SetHighlight(Color color, float pulse)
        {
            highlightColor = color;
            pulseSpeed = pulse;
            isHighlighted = true;
            ApplyHighlight();
        }

        public void ClearHighlight()
        {
            isHighlighted = false;
            RestoreOriginalMaterials();
        }

        private void ApplyHighlight()
        {
            foreach (var renderer in renderers)
            {
                foreach (var mat in renderer.materials)
                {
                    if (mat.HasProperty("_EmissionColor"))
                    {
                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", highlightColor);
                    }
                    else if (mat.HasProperty("_Color"))
                    {
                        Color originalColor = mat.color;
                        mat.color = Color.Lerp(originalColor, highlightColor, 0.5f);
                    }
                }
            }
        }

        private void RestoreOriginalMaterials()
        {
            foreach (var renderer in renderers)
            {
                foreach (var mat in renderer.materials)
                {
                    if (mat.HasProperty("_EmissionColor"))
                    {
                        mat.DisableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", Color.black);
                    }
                }
            }
        }

        private void Update()
        {
            if (isHighlighted && pulseSpeed > 0)
            {
                float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
                Color pulsedColor = highlightColor * (0.5f + pulse * 0.5f);

                foreach (var renderer in renderers)
                {
                    foreach (var mat in renderer.materials)
                    {
                        if (mat.HasProperty("_EmissionColor"))
                        {
                            mat.SetColor("_EmissionColor", pulsedColor);
                        }
                    }
                }
            }
        }

        private void OnDestroy()
        {
            ClearHighlight();
        }
    }
}
