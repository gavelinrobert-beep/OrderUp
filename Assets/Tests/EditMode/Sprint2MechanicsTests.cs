using NUnit.Framework;
using UnityEngine;
using OrderUp.Gameplay;
using OrderUp.Data;
using OrderUp.Core;

namespace OrderUp.Tests
{
    public class Sprint2MechanicsTests
    {
        private GameObject pickableItemObject;
        private GameObject cartObject;
        private GameObject boxObject;
        private GameObject packingStationObject;
        private ProductData testProduct;
        private OrderData testOrder;

        [SetUp]
        public void SetUp()
        {
            // Create test product
            testProduct = ScriptableObject.CreateInstance<ProductData>();
            testProduct.productId = "TEST_PRODUCT";
            testProduct.productName = "Test Product";
            testProduct.basePoints = 10;

            // Create test order
            testOrder = ScriptableObject.CreateInstance<OrderData>();
            testOrder.orderId = "TEST_ORDER";
            testOrder.orderType = OrderType.Standard;
            testOrder.basePoints = 50;
            testOrder.requiredProducts = new System.Collections.Generic.List<ProductData> { testProduct };
        }

        [TearDown]
        public void TearDown()
        {
            if (pickableItemObject != null)
                Object.DestroyImmediate(pickableItemObject);
            if (cartObject != null)
                Object.DestroyImmediate(cartObject);
            if (boxObject != null)
                Object.DestroyImmediate(boxObject);
            if (packingStationObject != null)
                Object.DestroyImmediate(packingStationObject);
            if (testProduct != null)
                Object.DestroyImmediate(testProduct);
            if (testOrder != null)
                Object.DestroyImmediate(testOrder);
        }

        [Test]
        public void PickableItem_CanBePicked()
        {
            pickableItemObject = new GameObject("TestItem");
            PickableItem item = pickableItemObject.AddComponent<PickableItem>();

            // Set product data using reflection
            var field = typeof(PickableItem).GetField("productData",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(item, testProduct);

            Assert.IsFalse(item.IsPicked);
            bool picked = item.TryPick();
            Assert.IsTrue(picked);
            Assert.IsTrue(item.IsPicked);

            // Can't pick again
            bool pickedAgain = item.TryPick();
            Assert.IsFalse(pickedAgain);
        }

        [Test]
        public void Cart_CanAddAndRemoveItems()
        {
            cartObject = new GameObject("TestCart");
            Cart cart = cartObject.AddComponent<Cart>();

            pickableItemObject = new GameObject("TestItem");
            PickableItem item = pickableItemObject.AddComponent<PickableItem>();

            // Set product data
            var field = typeof(PickableItem).GetField("productData",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(item, testProduct);

            Assert.IsTrue(cart.IsEmpty);
            Assert.AreEqual(0, cart.CurrentCount);

            bool added = cart.TryAddItem(item);
            Assert.IsTrue(added);
            Assert.IsFalse(cart.IsEmpty);
            Assert.AreEqual(1, cart.CurrentCount);

            PickableItem removedItem = cart.TryRemoveItem();
            Assert.IsNotNull(removedItem);
            Assert.IsTrue(cart.IsEmpty);
            Assert.AreEqual(0, cart.CurrentCount);
        }

        [Test]
        public void Cart_RespectsCapacity()
        {
            cartObject = new GameObject("TestCart");
            Cart cart = cartObject.AddComponent<Cart>();

            // Set max capacity to 2 using reflection
            var field = typeof(Cart).GetField("maxCapacity",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(cart, 2);

            // Add items up to capacity
            for (int i = 0; i < 2; i++)
            {
                GameObject itemObj = new GameObject($"TestItem{i}");
                PickableItem item = itemObj.AddComponent<PickableItem>();
                cart.TryAddItem(item);
            }

            Assert.IsTrue(cart.IsFull);

            // Try to add one more (should fail)
            GameObject extraItemObj = new GameObject("ExtraItem");
            PickableItem extraItem = extraItemObj.AddComponent<PickableItem>();
            bool added = cart.TryAddItem(extraItem);
            Assert.IsFalse(added);
            Object.DestroyImmediate(extraItemObj);
        }

        [Test]
        public void Box_CanAddProductsAndApplyLabel()
        {
            boxObject = new GameObject("TestBox");
            Box box = boxObject.AddComponent<Box>();

            Assert.IsTrue(box.IsEmpty);
            Assert.IsFalse(box.IsLabeled);

            bool added = box.TryAddProduct(testProduct);
            Assert.IsTrue(added);
            Assert.IsFalse(box.IsEmpty);
            Assert.AreEqual(1, box.CurrentCount);

            bool labeled = box.TryApplyLabel(testOrder);
            Assert.IsTrue(labeled);
            Assert.IsTrue(box.IsLabeled);
            Assert.IsTrue(box.IsReadyToShip());
        }

        [Test]
        public void Box_CannotLabelEmptyBox()
        {
            boxObject = new GameObject("TestBox");
            Box box = boxObject.AddComponent<Box>();

            bool labeled = box.TryApplyLabel(testOrder);
            Assert.IsFalse(labeled);
            Assert.IsFalse(box.IsLabeled);
        }

        [Test]
        public void PackingStation_CanPackItemsAndCompleteOrders()
        {
            // Create managers
            GameObject orderManagerObj = new GameObject("OrderManager");
            OrderManager orderManager = orderManagerObj.AddComponent<OrderManager>();

            GameObject scoreManagerObj = new GameObject("ScoreManager");
            ScoreManager scoreManager = scoreManagerObj.AddComponent<ScoreManager>();

            packingStationObject = new GameObject("TestStation");
            PackingStation station = packingStationObject.AddComponent<PackingStation>();

            pickableItemObject = new GameObject("TestItem");
            PickableItem item = pickableItemObject.AddComponent<PickableItem>();

            // Set product data
            var field = typeof(PickableItem).GetField("productData",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(item, testProduct);

            // Pack item
            bool packed = station.TryPackItem(item);
            Assert.IsTrue(packed);

            // Apply label
            bool labeled = station.TryApplyLabel(testOrder);
            Assert.IsTrue(labeled);

            // Clean up managers
            Object.DestroyImmediate(orderManagerObj);
            Object.DestroyImmediate(scoreManagerObj);
        }
    }
}
