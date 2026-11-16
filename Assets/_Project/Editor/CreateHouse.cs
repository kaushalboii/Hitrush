using UnityEngine;
using UnityEditor;

namespace Hitrush.Editor
{
    public class CreateHouse : MonoBehaviour
    {
        [MenuItem("Hitrush/Create Simple House")]
        public static void CreateSimpleHouse()
        {
            // Create house parent
            GameObject house = new GameObject("SimpleHouse");
            
            // Create materials
            Material wallMat = CreateMaterial("HouseWallMat", new Color(0.9f, 0.7f, 0.5f)); // Warm beige
            Material roofMat = CreateMaterial("HouseRoofMat", new Color(0.6f, 0.2f, 0.1f)); // Terracotta
            Material doorMat = CreateMaterial("HouseDoorMat", new Color(0.4f, 0.2f, 0.1f)); // Dark brown
            Material windowMat = CreateMaterial("HouseWindowMat", new Color(0.6f, 0.8f, 1f)); // Light blue
            Material frameMat = CreateMaterial("HouseFrameMat", new Color(0.3f, 0.3f, 0.3f)); // Dark gray
            
            // === WALLS ===
            // Floor
            CreateCube(house, "Floor", new Vector3(0, 0.5f, 0), new Vector3(5, 1, 5), wallMat);
            
            // Four walls (with gaps for door/windows)
            // Front wall (with door opening)
            CreateCube(house, "FrontWall_Left", new Vector3(-1.5f, 2, -2.5f), new Vector3(1.5f, 3, 0.5f), wallMat);
            CreateCube(house, "FrontWall_Right", new Vector3(1.5f, 2, -2.5f), new Vector3(1.5f, 3, 0.5f), wallMat);
            CreateCube(house, "FrontWall_Top", new Vector3(0, 4, -2.5f), new Vector3(5, 1, 0.5f), wallMat);
            
            // Back wall (with window)
            CreateCube(house, "BackWall_Bottom", new Vector3(0, 1.5f, 2.5f), new Vector3(5, 2, 0.5f), wallMat);
            CreateCube(house, "BackWall_Top", new Vector3(0, 4, 2.5f), new Vector3(5, 1, 0.5f), wallMat);
            CreateCube(house, "BackWindow", new Vector3(0, 2.5f, 2.51f), new Vector3(1.5f, 1.5f, 0.1f), windowMat);
            CreateCube(house, "BackWindowFrame", new Vector3(0, 2.5f, 2.52f), new Vector3(1.6f, 1.6f, 0.1f), frameMat);
            
            // Left wall
            CreateCube(house, "LeftWall", new Vector3(-2.5f, 2, 0), new Vector3(0.5f, 3, 5), wallMat);
            CreateCube(house, "LeftWindow", new Vector3(-2.51f, 2.5f, 0), new Vector3(0.1f, 1.5f, 1.5f), windowMat);
            CreateCube(house, "LeftWindowFrame", new Vector3(-2.52f, 2.5f, 0), new Vector3(0.1f, 1.6f, 1.6f), frameMat);
            
            // Right wall
            CreateCube(house, "RightWall", new Vector3(2.5f, 2, 0), new Vector3(0.5f, 3, 5), wallMat);
            CreateCube(house, "RightWindow", new Vector3(2.51f, 2.5f, 0), new Vector3(0.1f, 1.5f, 1.5f), windowMat);
            CreateCube(house, "RightWindowFrame", new Vector3(2.52f, 2.5f, 0), new Vector3(0.1f, 1.6f, 1.6f), frameMat);
            
            // === ROOF ===
            // Simple pyramid roof using scaled cubes
            CreateCube(house, "Roof_Base", new Vector3(0, 4.5f, 0), new Vector3(5.5f, 0.5f, 5.5f), roofMat);
            CreateCube(house, "Roof_Mid", new Vector3(0, 5.2f, 0), new Vector3(4f, 0.5f, 4f), roofMat);
            CreateCube(house, "Roof_Top", new Vector3(0, 5.8f, 0), new Vector3(2.5f, 0.5f, 2.5f), roofMat);
            CreateCube(house, "Roof_Peak", new Vector3(0, 6.2f, 0), new Vector3(1f, 1f, 1f), roofMat);
            
            // === DOOR ===
            CreateCube(house, "Door", new Vector3(0, 1.5f, -2.51f), new Vector3(1f, 3, 0.1f), doorMat);
            CreateCube(house, "DoorFrame", new Vector3(0, 1.5f, -2.52f), new Vector3(1.1f, 3.1f, 0.1f), frameMat);
            CreateCube(house, "DoorKnob", new Vector3(0.3f, 1.5f, -2.53f), new Vector3(0.1f, 0.1f, 0.1f), frameMat);
            
            // === CHIMNEY ===
            CreateCube(house, "Chimney", new Vector3(1.5f, 6f, 1f), new Vector3(0.8f, 2f, 0.8f), new Color(0.5f, 0.3f, 0.2f));
            
            // Center the house
            house.transform.position = Vector3.zero;
            
            // Save as prefab
            string prefabPath = "Assets/_Project/Prefabs/Environment/SimpleHouse.prefab";
            PrefabUtility.SaveAsPrefabAsset(house, prefabPath);
            
            // Clean up
            DestroyImmediate(house);
            
            Debug.Log($"Simple house created at {prefabPath}");
        }
        
        private static Material CreateMaterial(string name, Color color)
        {
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.name = name;
            mat.color = color;
            
            // Save material
            string path = $"Assets/_Project/Art/Materials/{name}.mat";
            AssetDatabase.CreateAsset(mat, path);
            return mat;
        }
        
        private static GameObject CreateCube(GameObject parent, string name, Vector3 localPos, Vector3 scale, Material material)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.SetParent(parent.transform);
            cube.transform.localPosition = localPos;
            cube.transform.localScale = scale;
            
            if (material != null)
            {
                cube.GetComponent<Renderer>().material = material;
            }
            
            // Remove collider for visual-only parts (optional)
            // DestroyImmediate(cube.GetComponent<BoxCollider>());
            
            return cube;
        }
        
        private static GameObject CreateCube(GameObject parent, string name, Vector3 localPos, Vector3 scale, Color color)
        {
            Material mat = CreateMaterial(name + "_Mat", color);
            return CreateCube(parent, name, localPos, scale, mat);
        }
    }
}
