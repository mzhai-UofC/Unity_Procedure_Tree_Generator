using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Procedural_Tree_Generator_Window : EditorWindow
{

    /// <summary>
    /// UI settings paramters
    /// </summary>
    Texture2D header_Selection_Texture;
    Texture2D branch_tree_Texture;
    Texture2D pine_tree_Texture;
    Texture2D coconut_tree_Texture;
    Texture2D trunk_Texture;

    Color header_Section_Color = new Color(13f / 255f, 32f / 255f, 44f / 255f, 1f); //dark blue
    Color branch_tree_SectionColor = new Color(85f / 255f, 120f / 255f, 55f / 255f, 1f); //yellow + green
    Color pine_tree_SectionColor = new Color(55f / 255f, 75f / 255f, 55f / 255f, 1f); //dark green
    Color coconut_tree_SectionColor = new Color(0f / 255f, 125f / 255f, 125f / 255f, 1f); // blue
    Color trunk_mat_color = new Color(155f / 255f, 120f / 255f, 55f / 255f, 1f); //brown

    Rect headerSection;
    Rect branch_treeSection;
    Rect pine_treeSection;
    Rect coconut_treeSection;

    /// <summary>
    /// tool settings paramters
    /// </summary>
    public GameObject tree;

    public Material trunkMaterial;
    public Material leavesMaterial;

    public int _recursionLevel = 3;

    public int _basePolygon = 20;
    public int _floorNumber = 40;
    public float _trunkThickness = 0.35f;
    public float _floorHeight = 0.1f;
    private float _firstBranchHeight = 0.2f;
    private float _distorsionCone = 20f;
    public float _twistiness = 8f;
    public float _reductionRate = 0.1f;
    public float _branchDensity = 0.2f;
    public Mesh _leavesMesh;
    public float _leavesSize = 30f;
    [Range(0, 2)]
    private float _randomRatio = 0.005f;
    private Vector2 scrollPosition = new Vector2();
    private GameObject cube;

    [MenuItem("Window/Procedural Tree Generator")]

    /// <summary>
    /// Create the main window of "tree generator" 
    /// </summary>
    static void OpenWindow()
    {
        Procedural_Tree_Generator_Window window = (Procedural_Tree_Generator_Window)GetWindow(typeof(Procedural_Tree_Generator_Window));
        window.minSize = new Vector2(250, 350);
        window.Show();
    }

    /// <summary>
    /// similiar to start() or awake()
    /// </summary>
    void OnEnable()
    {
        InitTextures();
    }

    /// <summary>
    /// Initialize UI 2D texture values and colors
    /// </summary>
    void InitTextures()
    {
        header_Selection_Texture = new Texture2D(1, 1);
        header_Selection_Texture.SetPixel(0, 0, header_Section_Color);
        header_Selection_Texture.Apply();

        branch_tree_Texture = new Texture2D(1, 1);
        branch_tree_Texture.SetPixel(0, 0, branch_tree_SectionColor);
        branch_tree_Texture.Apply();

        pine_tree_Texture = new Texture2D(1, 1);
        pine_tree_Texture.SetPixel(0, 0, pine_tree_SectionColor);
        pine_tree_Texture.Apply();

        coconut_tree_Texture = new Texture2D(1, 1);
        coconut_tree_Texture.SetPixel(0, 0, coconut_tree_SectionColor);
        coconut_tree_Texture.Apply();

        trunk_Texture = new Texture2D(1, 1);
        trunk_Texture.SetPixel(0, 0, trunk_mat_color);
        trunk_Texture.Apply();

        trunkMaterial = new Material(Shader.Find("Standard"));
        AssetDatabase.CreateAsset(trunkMaterial, "Assets/trunkMaterial.mat");
        trunkMaterial.mainTexture = trunk_Texture;

        leavesMaterial = new Material(Shader.Find("Standard"));
        AssetDatabase.CreateAsset(leavesMaterial, "Assets/leavesMaterial.mat");
        leavesMaterial.mainTexture = branch_tree_Texture;

        DrawCube();

    }

    /// <summary>
    /// Define Rect value and add texture based on rect
    /// </summary>
    void DrawLayout()
    {
        //generate on top left
        headerSection.x = 0;
        headerSection.y = 0;
        headerSection.width = Screen.width;
        headerSection.height = 50;

        branch_treeSection.x = 0;
        branch_treeSection.y = 50;
        branch_treeSection.width = Screen.width ;
        branch_treeSection.height = Screen.height - 50;

        //pine_treeSection.x = Screen.width / 3f;
        //pine_treeSection.y = 50;
        //pine_treeSection.width = Screen.width / 3f;
        //pine_treeSection.height = Screen.height - 50;

        //coconut_treeSection.x = 2 * Screen.width / 3f;
        //coconut_treeSection.y = 50;
        //coconut_treeSection.width = Screen.width / 3f;
        //coconut_treeSection.height = Screen.height - 50;

        GUI.DrawTexture(headerSection, header_Selection_Texture);
        GUI.DrawTexture(branch_treeSection, branch_tree_Texture);
        //GUI.DrawTexture(pine_treeSection, pine_tree_Texture);
        //GUI.DrawTexture(coconut_treeSection, coconut_tree_Texture);


    }

    /// <summary>
    /// Draw content of header
    /// </summary>
    void DrawHeader()
    {
        GUILayout.BeginArea(headerSection);
        GUILayout.Label("Procedural Tree Generator");
        GUILayout.EndArea();
    }

    /// <summary>
    /// Draw content of mage region
    /// </summary>
    void Draw_branchTreeSettings()
    {
        GUILayout.BeginArea(branch_treeSection);
        GUILayout.Label("Branch Tree");

        if (!cube) DrawCube();

        if (!trunkMaterial) trunkMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Assets/trunkMaterial.mat");

        if (!leavesMaterial) leavesMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("branch_tree_Texture.mat");

        if (!tree) tree = GameObject.Find("tree");

        Editor e = Editor.CreateEditor(this);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUIStyle.none, GUI.skin.verticalScrollbar);
        e.DrawDefaultInspector();

        EditorGUILayout.EndScrollView();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Generate Branch Tree", GUILayout.Height(50)))
            gen();

        if (GUILayout.Button("Save mesh to Assets", GUILayout.Height(50)))
            saveMesh();

        GUILayout.EndArea();
    }

    /// <summary>
    /// pine tree settings
    /// </summary>
   /* void Draw_pineTreeSettings()
    {
        GUILayout.BeginArea(pine_treeSection);
        GUILayout.Label("Pine Tree");
        GUILayout.EndArea();
    }

    /// <summary>
    /// coconut tree settings
    /// </summary>
    void Draw_coconutTreeSettings()
    {
        GUILayout.BeginArea(coconut_treeSection);
        GUILayout.Label("Coconut Tree");
        GUILayout.EndArea();
    }*/

    /// <summary>
    /// similiar to update(), called one or more time each interaction
    /// </summary>
    private void OnGUI()
    {
        DrawHeader();
        DrawLayout();
        Draw_branchTreeSettings();
        //Draw_pineTreeSettings();
        //Draw_coconutTreeSettings();
    }

    /// <summary>
    /// saving the generated meshed
    /// </summary>
    private void saveMesh()
    {
        AssetDatabase.CreateAsset(tree.GetComponent<MeshFilter>().sharedMesh, "Assets/tree.asset");
        AssetDatabase.SaveAssets();
    }

    static T[] SubArray<T>(T[] data, int index, int length)
    {
        T[] result = new T[length];
        for (int i = index; i < index + length; ++i)
        {
            result[i] = data[i];
        }
        return result;
    }

    int getCloseValue(int val)
    {
        return Random.Range(val - (int)(val * _randomRatio), val + (int)(val * _randomRatio));
    }

    float getCloseValue(float val)
    {
        return Random.Range(val - val * _randomRatio, val + val * _randomRatio);
    }


    /// <summary>
    /// generating tree(truck + leaves)
    /// </summary>
    void gen()
    {
        if (!_leavesMesh)
            _leavesMesh = AssetDatabase.GetBuiltinExtraResource<UnityEngine.Mesh>("Assets/Cube.asset");

        if (!leavesMaterial)
            leavesMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("leavesMaterial.mat");
        if (!trunkMaterial)
            trunkMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("trunkMaterial.mat");
        if (tree)
            DestroyImmediate(tree);
        tree = new GameObject("tree");

        if (cube)
            DestroyImmediate(cube);
        cube = new GameObject("cube");


        int basePolygon = Mathf.Max(3, getCloseValue(_basePolygon));
        Vector3[] startVertices = new Vector3[basePolygon];
        Vector2[] startUv = new Vector2[basePolygon];

        float angularStep = 2f * Mathf.PI / (float)(basePolygon);
        for (int j = 0; j < basePolygon; ++j)
        {
            Vector3 randomness = new Vector3
                (
                Random.Range(-1, 1),
                Random.Range(-1, 1),
                Random.Range(-1, 1)
                ) / 10f;
            Vector3 pos = new Vector3(Mathf.Cos(j * angularStep), 0f, Mathf.Sin(j * angularStep)) + randomness;
            startVertices[j] = _trunkThickness * (pos);
            startUv[j] = new Vector2(j * angularStep, startVertices[j].y);
        }

        Mesh mesh = GenBranch(tree, basePolygon, startVertices, startUv, getCloseValue(_trunkThickness), getCloseValue(_floorHeight), getCloseValue(_floorNumber), new Vector3(), Vector3.up, 0f, getCloseValue(_distorsionCone), getCloseValue(1), getCloseValue(_branchDensity), getCloseValue(_recursionLevel));

        CombineInstance[] combine = new CombineInstance[2];
        // combine[0].mesh = MeshSmoothener.SmoothMesh(mesh.GetSubmesh(0), smoothingPower, MeshSmoothener.Filter.Laplacian);
        combine[0].mesh = mesh.GetSubmesh(0);
        combine[0].transform = tree.transform.localToWorldMatrix;
        combine[1].mesh = mesh.GetSubmesh(1);
        combine[1].transform = tree.transform.localToWorldMatrix;
        mesh.CombineMeshes(combine, false, false);
        // mesh = MeshSmoothener.SmoothMesh(mesh, smoothingPower, MeshSmoothener.Filter.Laplacian);

        mesh.RecalculateNormals();

        tree.AddComponent<MeshFilter>().mesh = mesh;
        tree.AddComponent<MeshRenderer>().materials = new Material[2] { trunkMaterial, leavesMaterial };

    }

    Vector3 ChangeCoordinates(Vector3 input, Vector3 inputNormal, Vector3 newNormal)
    {
        float angle = Vector3.Angle(inputNormal, newNormal);
        Vector3 axis = Vector3.Cross(inputNormal, newNormal);
        Quaternion rot = Quaternion.AngleAxis(angle, axis);
        return rot * input;
    }

    Vector3 getRandomVectorInCone(float coneAngularAmplitude, Vector3 direction)
    {
        return (new Vector3(Random.Range(-coneAngularAmplitude, coneAngularAmplitude), Random.Range(-coneAngularAmplitude, coneAngularAmplitude), Random.Range(-coneAngularAmplitude, coneAngularAmplitude)) / 100f + direction).normalized;
    }

    static bool Happens(float proba)
    {
        return Random.Range(0f, 1f) < proba;
    }

    /// <summary>
    /// generating branches
    /// </summary>
    /// <param name="tree"></param>
    /// <param name="basePolygon"></param>
    /// <param name="startVertices"></param>
    /// <param name="startUv"></param>
    /// <param name="thickness"></param>
    /// <param name="floorHeight"></param>
    /// <param name="floorNumber"></param>
    /// <param name="startingPos"></param>
    /// <param name="startingDirection"></param>
    /// <param name="angularOffset"></param>
    /// <param name="distorsionCone"></param>
    /// <param name="roughness"></param>
    /// <param name="branchDensity"></param>
    /// <param name="recursionLevel"></param>
    /// <returns></returns>
    Mesh GenBranch(GameObject tree, int basePolygon, Vector3[] startVertices, Vector2[] startUv, float thickness, float floorHeight, int floorNumber, Vector3 startingPos, Vector3 startingDirection, float angularOffset, float distorsionCone, float roughness, float branchDensity, int recursionLevel)
    {
        float currentThickness = thickness;
        Mesh mesh = new Mesh();
        mesh.subMeshCount = 2;

        Vector3[] vertices = new Vector3[basePolygon * floorNumber];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles;

        Vector3 leavesPosition = new Vector3();

        float angularStep = 2f * Mathf.PI / (float)(basePolygon);
        Vector3 first = new Vector3(Mathf.Cos(angularOffset), 0f, Mathf.Sin(angularOffset)) * thickness;
        first = ChangeCoordinates(first, Vector3.up, startingDirection);
        first += startingPos;

        for (int i = 0; i < startVertices.Length; ++i)
        {
            vertices[i] = startVertices[i];
            uv[i] = startUv[i];
        }


        triangles = new int[6 * (vertices.Length - basePolygon)];


        Vector3 growDirection = startingDirection;

        Vector3 lastPivot = startingPos;

        for (int i = 1; i < vertices.Length / basePolygon; ++i)
        {

            Vector3 pivot = lastPivot + floorHeight * growDirection;
            lastPivot = pivot;

            for (int j = 0; j < basePolygon; ++j)
            {
                Vector3 randomness = new Vector3
                    (
                    Random.Range(-roughness / 10f, roughness / 10f),
                    Random.Range(-roughness / 10f, roughness / 10f),
                    Random.Range(-roughness / 10f, roughness / 10f)
                    );
                Vector3 pos = new Vector3(Mathf.Cos(j * angularStep + angularOffset), 0f, Mathf.Sin(j * angularStep + angularOffset)) + randomness;
                pos *= currentThickness;
                pos = ChangeCoordinates(pos, new Vector3(0f, 1f, 0f), growDirection);
                vertices[i * basePolygon + j] = pos + pivot;
                uv[i * basePolygon + j] = new Vector2(j * angularStep, vertices[i * basePolygon + j].y);

                triangles[6 * ((i - 1) * basePolygon + j)] = (i - 1) * basePolygon + j;
                triangles[6 * ((i - 1) * basePolygon + j) + 1] = (i) * basePolygon + j;
                triangles[6 * ((i - 1) * basePolygon + j) + 2] = (i - 1) * basePolygon + (j + 1) % basePolygon;
                triangles[6 * ((i - 1) * basePolygon + j) + 3] = (i - 1) * basePolygon + (j + 1) % basePolygon;
                triangles[6 * ((i - 1) * basePolygon + j) + 4] = (i) * basePolygon + j;
                triangles[6 * ((i - 1) * basePolygon + j) + 5] = (i) * basePolygon + (j + 1) % basePolygon;
            }
            if (Happens(branchDensity) && recursionLevel > 0 && i >= floorNumber * _firstBranchHeight && basePolygon >= 4 && i < vertices.Length / basePolygon - 1) // split!
            {
                int subBasePolygon = Random.Range(Mathf.Max(2, basePolygon / 3), Mathf.Min(2 * basePolygon / 3, basePolygon - 1)) + 1;
                int subBasePolygon2 = basePolygon - subBasePolygon + 2;
                int newOffset = Random.Range(0, basePolygon);
                Vector3[] subStartVertices = new Vector3[subBasePolygon];
                Vector2[] subStartUv = new Vector2[subBasePolygon];
                Vector3[] subStartVertices2 = new Vector3[subBasePolygon2];
                Vector2[] subStartUv2 = new Vector2[subBasePolygon2];

                Vector3 subStartingPos = pivot;
                Vector3 mid = new Vector3();
                for (int k = 0; k < subBasePolygon; ++k)
                {
                    int shift = ((k + newOffset) % basePolygon + basePolygon) % basePolygon;
                    subStartVertices[k] = vertices[i * basePolygon + shift];
                    subStartUv[k] = uv[i * basePolygon + shift];
                    mid += subStartVertices[k];
                }

                mid /= subBasePolygon;
                subStartingPos = mid;

                float newAngularOffset = angularOffset + Vector3.SignedAngle(subStartVertices[0] - mid, vertices[i * basePolygon] - pivot, growDirection) * Mathf.Deg2Rad;


                Vector3 subStartingPos2 = pivot;
                Vector3 mid2 = new Vector3();
                for (int k = 0; k < subBasePolygon2; ++k)
                {
                    int shift = ((k + newOffset + subBasePolygon - 1) % basePolygon + basePolygon) % basePolygon;
                    subStartVertices2[k] = vertices[i * basePolygon + shift];
                    subStartUv2[k] = uv[i * basePolygon + shift];
                    mid2 += subStartVertices2[k];
                }
                mid2 /= subBasePolygon2;
                subStartingPos2 = mid2;

                float newAngularOffset2 = angularOffset + Vector3.SignedAngle(subStartVertices2[0] - mid2, vertices[i * basePolygon] - pivot, growDirection) * Mathf.Deg2Rad;

                mesh.vertices = SubArray(vertices, 0, basePolygon * (i + 1));
                mesh.uv = SubArray(uv, 0, basePolygon * (i + 1));
                mesh.SetTriangles(SubArray(triangles, 0, 6 * (i * basePolygon)), 0);
                mesh.RecalculateNormals();

                Mesh res1 = new Mesh();
                res1.subMeshCount = 2;
                Mesh res2 = new Mesh();
                res2.subMeshCount = 2;
                Mesh res = new Mesh();
                res.subMeshCount = 2;

                Mesh branch1 = GenBranch(tree, subBasePolygon, subStartVertices, subStartUv,
                    currentThickness * subBasePolygon / ((float)basePolygon), floorHeight,
                    floorNumber - i, subStartingPos, getRandomVectorInCone(distorsionCone, (growDirection + distorsionCone / 45f * (mid - pivot).normalized).normalized),
                    newAngularOffset, distorsionCone, roughness, branchDensity * 1.1f, recursionLevel - 1);

                Mesh branch2 = GenBranch(tree, subBasePolygon2, subStartVertices2, subStartUv2,
                    currentThickness * subBasePolygon2 / ((float)basePolygon), floorHeight,
                    floorNumber - i, subStartingPos2, getRandomVectorInCone(distorsionCone, (growDirection + distorsionCone / 45f * (mid2 - pivot).normalized).normalized),
                    newAngularOffset2, distorsionCone, roughness, branchDensity * 1.1f, recursionLevel - 1);

                CombineInstance[] combine = new CombineInstance[3];
                combine[1].transform = tree.transform.localToWorldMatrix;
                combine[2].transform = tree.transform.localToWorldMatrix;

                Mesh subMesh1 = mesh.GetSubmesh(0);
                Mesh subMesh11 = branch1.GetSubmesh(0);
                Mesh subMesh12 = branch2.GetSubmesh(0);

                Mesh subMesh2 = mesh.GetSubmesh(1);
                if (subMesh2 == null)
                {
                    subMesh2 = new Mesh();
                }

                Mesh subMesh21 = branch1.GetSubmesh(1);
                if (subMesh21 == null)
                {
                    subMesh21 = new Mesh();
                }

                Mesh subMesh22 = branch2.GetSubmesh(1);
                if (subMesh22 == null)
                {
                    subMesh22 = new Mesh();
                }

                combine[0].mesh = subMesh1;
                combine[0].transform = tree.transform.localToWorldMatrix;
                combine[1].mesh = subMesh11;
                combine[1].transform = tree.transform.localToWorldMatrix;
                combine[2].mesh = subMesh12;
                combine[2].transform = tree.transform.localToWorldMatrix;

                res1.CombineMeshes(combine, true, false);
                res1.RecalculateNormals();

                combine[0].mesh = subMesh2;
                combine[1].mesh = subMesh21;
                combine[2].mesh = subMesh22;

                res2.CombineMeshes(combine, true, false);
                res2.RecalculateNormals();

                combine = new CombineInstance[2];
                combine[0].mesh = res1;
                combine[0].transform = tree.transform.localToWorldMatrix;
                combine[1].mesh = res2;
                combine[1].transform = tree.transform.localToWorldMatrix;

                res.CombineMeshes(combine, false, false);
                res.RecalculateNormals();
                res.Optimize();
                return res;
            }
            currentThickness = Mathf.Pow(Mathf.Asin((floorNumber - i) / (float)(floorNumber) * 2f - 1f) / Mathf.PI + 0.5f, _reductionRate) * thickness;
            growDirection = getRandomVectorInCone(_twistiness, growDirection);

            leavesPosition = pivot;

        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        {

            Mesh trLeavesMesh = new Mesh();
            trLeavesMesh.subMeshCount = 2;

            Vector3[] trVertices = (Vector3[])(_leavesMesh.vertices.Clone());
            int[] leavesTriangles = (int[])(_leavesMesh.triangles.Clone());

            for (int i = 0; i < _leavesMesh.vertices.Length; ++i)
            {
                trVertices[i] *= currentThickness * _leavesSize;
                trVertices[i] += leavesPosition;
            }
            trLeavesMesh.SetVertices(trVertices);
            trLeavesMesh.SetTriangles(leavesTriangles, 0);
            trLeavesMesh.SetNormals((Vector3[])(_leavesMesh.normals.Clone()));
            trLeavesMesh.SetTangents((Vector4[])(_leavesMesh.tangents.Clone()));
            trLeavesMesh.uv = (Vector2[])(_leavesMesh.uv.Clone());

            Mesh res = new Mesh();
            res.subMeshCount = 2;

            CombineInstance[] combine = new CombineInstance[2];
            combine[0].mesh = mesh;
            combine[0].transform = tree.transform.localToWorldMatrix;
            combine[1].mesh = trLeavesMesh;
            combine[1].transform = tree.transform.localToWorldMatrix;
            res.CombineMeshes(combine, false, false);
            res.RecalculateNormals();
            res.Optimize();
            res.OptimizeIndexBuffers();
            res.OptimizeReorderVertexBuffer();
            return res;
        }

    }
    /// <summary>
    /// mesh cube generator for leaves meshes
    /// </summary>
    /// <param name="_lenth"></param>
    /// <param name="_height"></param>
    /// <returns></returns>
    public Vector3[] GetVertex(float _lenth, float _height)
    {
        Vector3[] _rec = new Vector3[] {
            //front
            new Vector3(0,0,0),
            new Vector3(0,_height,0),
            new Vector3(_lenth,0,0),
            new Vector3(0,_height,0),
            new Vector3(_lenth,_height,0),
            new Vector3(_lenth,0,0),
        
            //left
            new Vector3(0,0,0),
            new Vector3(0,0,_lenth),
            new Vector3(0,_height,0),
            new Vector3(0,_height,0),
            new Vector3(0,0,_lenth),
            new Vector3(0,_lenth,_height),
            //right
            new Vector3(_lenth,_lenth,_height),
            new Vector3(_lenth,0,_lenth),
            new Vector3(_lenth,_height,0),
            new Vector3(_lenth,_height,0),
            new Vector3(_lenth,0,_lenth),
            new Vector3(_lenth,0,0),
            //back
            new Vector3(_lenth,0,_lenth),
            new Vector3(_lenth,_height,_lenth),
            new Vector3(0,_height,_lenth),
            new Vector3(_lenth,0,_lenth),
            new Vector3(0,_height,_lenth),
            new Vector3(0,0,_lenth),
 
            //bottom
            new Vector3(0,0,0),
            new Vector3(_lenth,0,0),
            new Vector3(_lenth,0,_lenth),
            new Vector3(_lenth,0,_lenth),
            new Vector3(0,0,_lenth),
            new Vector3(0,0,0),
    
            //top
            new Vector3(0,_height,0),
            new Vector3(0, _height, _lenth),
            new Vector3(_lenth, _height, _lenth),
            new Vector3(_lenth, _height, _lenth),
            new Vector3(_lenth, _height, 0),
            new Vector3(0, _height, 0)

        };
        return _rec;
    }
    public int[] GetSort(Vector3[] _vec)
    {
        int[] rec = new int[_vec.Length];
        for (int i = 0; i < _vec.Length; i++)
        {
            rec[i] = i;
        }
        return rec;
    }
    private Material _mat;
    public void DrawCube()
    {
        if (cube)
            DestroyImmediate(cube);
        cube = new GameObject("cube");

        Mesh mesh = cube.AddComponent<MeshFilter>().mesh;
        MeshRenderer _render = cube.AddComponent<MeshRenderer>();

        mesh.vertices = GetVertex(1, 1);
        mesh.triangles = GetSort(mesh.vertices);
        _render.material = _mat;

        AssetDatabase.CreateAsset(cube.GetComponent<MeshFilter>().sharedMesh, "Assets/cube.asset");
    
    }
}

