using UnityEngine;

namespace OrderUp.Environment
{
    /// <summary>
    /// WarehouseBuilder creates the basic warehouse environment
    /// Includes floor, walls, picking zone, and packing zone
    /// </summary>
    public class WarehouseBuilder : MonoBehaviour
    {
        [Header("Warehouse Dimensions")]
        [SerializeField] private float warehouseWidth = 20f;
        [SerializeField] private float warehouseLength = 30f;
        [SerializeField] private float wallHeight = 4f;

        [Header("Zone Settings")]
        [SerializeField] private Color pickingZoneColor = new Color(0.5f, 0.8f, 0.5f, 0.3f);
        [SerializeField] private Color packingZoneColor = new Color(0.5f, 0.5f, 0.8f, 0.3f);

        private void Start()
        {
            BuildWarehouse();
        }

        private void BuildWarehouse()
        {
            CreateFloor();
            CreateWalls();
            CreatePickingZone();
            CreatePackingZone();
            CreateLighting();
            
            Debug.Log("WarehouseBuilder: Warehouse environment created");
        }

        private void CreateFloor()
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.position = Vector3.zero;
            floor.transform.localScale = new Vector3(warehouseWidth / 10f, 1f, warehouseLength / 10f);
            
            // Set floor material to gray
            Renderer renderer = floor.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(0.4f, 0.4f, 0.4f);
            }

            floor.transform.parent = transform;
        }

        private void CreateWalls()
        {
            // North wall
            CreateWall("NorthWall", 
                new Vector3(0f, wallHeight / 2f, warehouseLength / 2f), 
                new Vector3(warehouseWidth, wallHeight, 0.2f));

            // South wall
            CreateWall("SouthWall", 
                new Vector3(0f, wallHeight / 2f, -warehouseLength / 2f), 
                new Vector3(warehouseWidth, wallHeight, 0.2f));

            // East wall
            CreateWall("EastWall", 
                new Vector3(warehouseWidth / 2f, wallHeight / 2f, 0f), 
                new Vector3(0.2f, wallHeight, warehouseLength));

            // West wall
            CreateWall("WestWall", 
                new Vector3(-warehouseWidth / 2f, wallHeight / 2f, 0f), 
                new Vector3(0.2f, wallHeight, warehouseLength));
        }

        private void CreateWall(string name, Vector3 position, Vector3 scale)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.position = position;
            wall.transform.localScale = scale;
            
            // Set wall material to light gray
            Renderer renderer = wall.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(0.8f, 0.8f, 0.8f);
            }

            wall.transform.parent = transform;
        }

        private void CreatePickingZone()
        {
            GameObject pickingZone = GameObject.CreatePrimitive(PrimitiveType.Plane);
            pickingZone.name = "PickingZone";
            
            // Position in the back half of the warehouse
            pickingZone.transform.position = new Vector3(0f, 0.05f, warehouseLength / 4f);
            pickingZone.transform.localScale = new Vector3(warehouseWidth / 10f, 1f, warehouseLength / 20f);
            
            // Set semi-transparent green material
            Renderer renderer = pickingZone.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Standard"));
                mat.color = pickingZoneColor;
                mat.SetFloat("_Mode", 3); // Transparent mode
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
                renderer.material = mat;
            }

            // Disable collider so it doesn't interfere with movement
            Collider collider = pickingZone.GetComponent<Collider>();
            if (collider != null)
            {
                Destroy(collider);
            }

            pickingZone.transform.parent = transform;
        }

        private void CreatePackingZone()
        {
            GameObject packingZone = GameObject.CreatePrimitive(PrimitiveType.Plane);
            packingZone.name = "PackingZone";
            
            // Position in the front half of the warehouse
            packingZone.transform.position = new Vector3(0f, 0.05f, -warehouseLength / 4f);
            packingZone.transform.localScale = new Vector3(warehouseWidth / 10f, 1f, warehouseLength / 20f);
            
            // Set semi-transparent blue material
            Renderer renderer = packingZone.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Standard"));
                mat.color = packingZoneColor;
                mat.SetFloat("_Mode", 3); // Transparent mode
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
                renderer.material = mat;
            }

            // Disable collider so it doesn't interfere with movement
            Collider collider = packingZone.GetComponent<Collider>();
            if (collider != null)
            {
                Destroy(collider);
            }

            packingZone.transform.parent = transform;
        }

        private void CreateLighting()
        {
            // Create directional light if one doesn't exist
            Light[] lights = FindObjectsOfType<Light>();
            bool hasDirectionalLight = false;

            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    hasDirectionalLight = true;
                    break;
                }
            }

            if (!hasDirectionalLight)
            {
                GameObject lightObject = new GameObject("Directional Light");
                Light light = lightObject.AddComponent<Light>();
                light.type = LightType.Directional;
                light.color = Color.white;
                light.intensity = 1f;
                lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
                lightObject.transform.parent = transform;
            }
        }
    }
}
