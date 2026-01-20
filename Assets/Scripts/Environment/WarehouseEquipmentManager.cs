using UnityEngine;
using System.Collections.Generic;
using OrderUp.Gameplay;

namespace OrderUp.Environment
{
    /// <summary>
    /// Spawns and manages carts and packing stations in the warehouse
    /// </summary>
    public class WarehouseEquipmentManager : MonoBehaviour
    {
        [Header("Cart Settings")]
        [Tooltip("Number of carts to spawn")]
        [SerializeField] private int cartCount = 2;

        [Tooltip("Cart spawn positions")]
        [SerializeField] private Vector3 cartSpawnArea = new Vector3(0f, 0f, 0f);

        [Header("Packing Station Settings")]
        [Tooltip("Number of packing stations to spawn")]
        [SerializeField] private int packingStationCount = 2;

        [Tooltip("Packing zone center position")]
        [SerializeField] private Vector3 packingZoneCenter = new Vector3(0f, 0f, -7.5f);

        [Header("Prefabs")]
        [Tooltip("Cart prefab (optional)")]
        [SerializeField] private GameObject cartPrefab;

        [Tooltip("Packing station prefab (optional)")]
        [SerializeField] private GameObject packingStationPrefab;

        private List<GameObject> spawnedCarts = new List<GameObject>();
        private List<GameObject> spawnedStations = new List<GameObject>();

        private void Start()
        {
            SpawnEquipment();
        }

        /// <summary>
        /// Spawns all warehouse equipment
        /// </summary>
        public void SpawnEquipment()
        {
            ClearEquipment();
            SpawnCarts();
            SpawnPackingStations();
        }

        /// <summary>
        /// Spawns carts in the warehouse
        /// </summary>
        private void SpawnCarts()
        {
            for (int i = 0; i < cartCount; i++)
            {
                Vector3 position = CalculateCartPosition(i);
                GameObject cart = CreateCart(position, i);
                spawnedCarts.Add(cart);
            }

            Debug.Log($"WarehouseEquipmentManager: Spawned {cartCount} carts");
        }

        /// <summary>
        /// Spawns packing stations in the packing zone
        /// </summary>
        private void SpawnPackingStations()
        {
            for (int i = 0; i < packingStationCount; i++)
            {
                Vector3 position = CalculateStationPosition(i);
                GameObject station = CreatePackingStation(position, i);
                spawnedStations.Add(station);
            }

            Debug.Log($"WarehouseEquipmentManager: Spawned {packingStationCount} packing stations");
        }

        /// <summary>
        /// Calculates the position for a cart
        /// </summary>
        private Vector3 CalculateCartPosition(int index)
        {
            float xOffset = (index - (cartCount - 1) / 2f) * 3f;
            return new Vector3(xOffset, 0f, cartSpawnArea.z);
        }

        /// <summary>
        /// Calculates the position for a packing station
        /// </summary>
        private Vector3 CalculateStationPosition(int index)
        {
            float xOffset = (index - (packingStationCount - 1) / 2f) * 5f;
            return new Vector3(xOffset, 0f, packingZoneCenter.z);
        }

        /// <summary>
        /// Creates a cart at the specified position
        /// </summary>
        private GameObject CreateCart(Vector3 position, int index)
        {
            GameObject cart;

            if (cartPrefab != null)
            {
                cart = Instantiate(cartPrefab, position, Quaternion.identity, transform);
            }
            else
            {
                // Create a basic cart using primitives
                cart = new GameObject($"Cart_{index}");
                cart.transform.position = position;
                cart.transform.SetParent(transform);

                // Create cart body
                GameObject cartBody = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cartBody.name = "CartBody";
                cartBody.transform.SetParent(cart.transform);
                cartBody.transform.localPosition = Vector3.up * 0.5f;
                cartBody.transform.localScale = new Vector3(1f, 1f, 1.5f);

                Renderer renderer = cartBody.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = new Color(0.7f, 0.7f, 0.7f); // Gray color
                }

                // Add sphere collider for easier interaction
                SphereCollider collider = cart.AddComponent<SphereCollider>();
                collider.radius = 1.5f;
                collider.isTrigger = false;
            }

            // Add Cart component
            Cart cartComponent = cart.GetComponent<Cart>();
            if (cartComponent == null)
            {
                cartComponent = cart.AddComponent<Cart>();
            }

            return cart;
        }

        /// <summary>
        /// Creates a packing station at the specified position
        /// </summary>
        private GameObject CreatePackingStation(Vector3 position, int index)
        {
            GameObject station;

            if (packingStationPrefab != null)
            {
                station = Instantiate(packingStationPrefab, position, Quaternion.identity, transform);
            }
            else
            {
                // Create a basic packing station using primitives
                station = new GameObject($"PackingStation_{index}");
                station.transform.position = position;
                station.transform.SetParent(transform);

                // Create station table
                GameObject table = GameObject.CreatePrimitive(PrimitiveType.Cube);
                table.name = "StationTable";
                table.transform.SetParent(station.transform);
                table.transform.localPosition = Vector3.up * 0.75f;
                table.transform.localScale = new Vector3(2f, 0.1f, 2f);

                Renderer renderer = table.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = new Color(0.5f, 0.5f, 0.8f); // Blue-ish color
                }

                // Create legs
                for (int i = 0; i < 4; i++)
                {
                    GameObject leg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    leg.name = $"Leg_{i}";
                    leg.transform.SetParent(station.transform);
                    
                    float xPos = (i % 2 == 0) ? -0.8f : 0.8f;
                    float zPos = (i < 2) ? -0.8f : 0.8f;
                    leg.transform.localPosition = new Vector3(xPos, 0.35f, zPos);
                    leg.transform.localScale = new Vector3(0.1f, 0.7f, 0.1f);

                    Renderer legRenderer = leg.GetComponent<Renderer>();
                    if (legRenderer != null)
                    {
                        legRenderer.material.color = new Color(0.3f, 0.3f, 0.3f);
                    }
                }

                // Add box collider for interaction
                BoxCollider collider = station.AddComponent<BoxCollider>();
                collider.center = Vector3.up * 0.75f;
                collider.size = new Vector3(2f, 1.5f, 2f);
                collider.isTrigger = false;
            }

            // Add PackingStation component
            PackingStation stationComponent = station.GetComponent<PackingStation>();
            if (stationComponent == null)
            {
                stationComponent = station.AddComponent<PackingStation>();
            }

            // Set box position reference
            Transform boxPosition = station.transform.Find("BoxPosition");
            if (boxPosition == null)
            {
                GameObject boxPosObj = new GameObject("BoxPosition");
                boxPosObj.transform.SetParent(station.transform);
                boxPosObj.transform.localPosition = Vector3.up * 1f;
                boxPosition = boxPosObj.transform;
            }

            // Set box position using the public setter
            stationComponent.SetBoxPosition(boxPosition);

            return station;
        }

        /// <summary>
        /// Clears all spawned equipment
        /// </summary>
        public void ClearEquipment()
        {
            foreach (GameObject cart in spawnedCarts)
            {
                if (cart != null)
                {
                    Destroy(cart);
                }
            }
            spawnedCarts.Clear();

            foreach (GameObject station in spawnedStations)
            {
                if (station != null)
                {
                    Destroy(station);
                }
            }
            spawnedStations.Clear();
        }
    }
}
