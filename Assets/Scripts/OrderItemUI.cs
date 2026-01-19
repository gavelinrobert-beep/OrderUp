using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using OrderUp.Data;

namespace OrderUp.UI
{
    /// <summary>
    /// Handles UI presentation for a single order entry in the order list.
    /// </summary>
    public class OrderItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI orderTypeText;
        [SerializeField] private TextMeshProUGUI productListText;
        [SerializeField] private TextMeshProUGUI timerText;

        private Image backgroundImage;
        private OrderData order;
        private float spawnTime;
        private bool hasExpressTimer;

        private void Awake()
        {
            EnsureLayout();
            EnsureTextFields();
            TryGetComponent(out backgroundImage);
        }

        /// <summary>
        /// Initializes the UI with data from the supplied order.
        /// </summary>
        public void Initialize(OrderData orderData)
        {
            order = orderData;
            spawnTime = Time.time;
            RefreshContent();
        }

        private void Update()
        {
            if (!hasExpressTimer || order == null)
            {
                return;
            }

            UpdateTimer();
        }

        /// <summary>
        /// Checks if this UI entry matches the supplied order.
        /// </summary>
        public bool MatchesOrder(OrderData orderData)
        {
            if (order == null || orderData == null)
            {
                return false;
            }

            bool hasOrderId = !string.IsNullOrEmpty(order.orderId);
            bool hasTargetId = !string.IsNullOrEmpty(orderData.orderId);
            if (hasOrderId && hasTargetId)
            {
                return order.orderId == orderData.orderId;
            }

            return order == orderData;
        }

        /// <summary>
        /// Highlights the UI when an order is completed.
        /// </summary>
        public void PlayCompletionFeedback(Color textColor, Color backgroundColor)
        {
            ApplyTextColor(orderTypeText, textColor);
            ApplyTextColor(productListText, textColor);
            ApplyTextColor(timerText, textColor);

            if (backgroundImage != null)
            {
                backgroundImage.color = backgroundColor;
            }
        }

        private void RefreshContent()
        {
            if (order == null)
            {
                return;
            }

            if (orderTypeText != null)
            {
                orderTypeText.text = BuildOrderHeader();
            }

            if (productListText != null)
            {
                productListText.text = BuildProductList();
            }

            hasExpressTimer = order.orderType == OrderType.Express;
            if (timerText != null)
            {
                timerText.gameObject.SetActive(hasExpressTimer);
            }

            if (hasExpressTimer)
            {
                UpdateTimer();
            }
        }

        private string BuildOrderHeader()
        {
            string orderLabel = order.orderType == OrderType.Express ? "Express Order" : "Standard Order";
            if (!string.IsNullOrEmpty(order.orderId))
            {
                orderLabel = $"{orderLabel} ({order.orderId})";
            }

            return orderLabel;
        }

        private string BuildProductList()
        {
            if (order.requiredProducts == null || order.requiredProducts.Count == 0)
            {
                return "Products: -";
            }

            StringBuilder builder = new StringBuilder("Products:");
            for (int i = 0; i < order.requiredProducts.Count; i++)
            {
                ProductData product = order.requiredProducts[i];
                string productName = product != null && !string.IsNullOrEmpty(product.productName)
                    ? product.productName
                    : product != null && !string.IsNullOrEmpty(product.productId)
                        ? product.productId
                        : "Unknown";
                builder.AppendLine();
                builder.Append($"- {productName}");
            }

            return builder.ToString();
        }

        private static void ApplyTextColor(TextMeshProUGUI textField, Color color)
        {
            if (textField != null)
            {
                textField.color = color;
            }
        }

        private void UpdateTimer()
        {
            if (timerText == null || order == null)
            {
                return;
            }

            float remaining = Mathf.Max(0f, order.expressTimeLimit - (Time.time - spawnTime));
            int remainingSeconds = Mathf.CeilToInt(remaining);
            timerText.text = $"Express: {remainingSeconds}s";
        }

        private void EnsureLayout()
        {
            if (!TryGetComponent(out VerticalLayoutGroup layoutGroup))
            {
                layoutGroup = gameObject.AddComponent<VerticalLayoutGroup>();
                layoutGroup.childAlignment = TextAnchor.UpperLeft;
                layoutGroup.childControlHeight = true;
                layoutGroup.childControlWidth = true;
                layoutGroup.childForceExpandHeight = false;
                layoutGroup.childForceExpandWidth = true;
                layoutGroup.spacing = 4f;
            }

            if (!TryGetComponent(out ContentSizeFitter sizeFitter))
            {
                sizeFitter = gameObject.AddComponent<ContentSizeFitter>();
                sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
        }

        private void EnsureTextFields()
        {
            if (orderTypeText == null)
            {
                orderTypeText = CreateText("OrderTypeText", 20, FontStyles.Bold);
            }

            if (productListText == null)
            {
                productListText = CreateText("ProductListText", 16, FontStyles.Normal);
            }

            if (timerText == null)
            {
                timerText = CreateText("ExpressTimerText", 14, FontStyles.Italic);
            }
        }

        private TextMeshProUGUI CreateText(string name, int fontSize, FontStyles fontStyle)
        {
            GameObject textObject = new GameObject(name, typeof(RectTransform));
            textObject.transform.SetParent(transform, false);
            TextMeshProUGUI textComponent = textObject.AddComponent<TextMeshProUGUI>();
            textComponent.fontSize = fontSize;
            textComponent.fontStyle = fontStyle;
            textComponent.alignment = TextAlignmentOptions.Left;
            textComponent.enableWordWrapping = true;
            textComponent.text = string.Empty;

            if (TMP_Settings.defaultFontAsset != null)
            {
                textComponent.font = TMP_Settings.defaultFontAsset;
            }

            return textComponent;
        }
    }
}
