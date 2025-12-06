using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class ColorSwapEntity : MonoBehaviour
{
    public Material[] lightMat;
    public Material[] darkMat;

    public bool usingLight = true;

    public UnityEvent<bool> OnEntityColorSwapped;

    private Renderer rend;

    private void OnValidate()
    {
        PopulateMaterials();
    }

    private void PopulateMaterials()
    {
        // Change these to your folder paths
        string lightFolder = "Assets/Game/Mat/Light";
        string darkFolder = "Assets/Game/Mat/Dark";

        lightMat = LoadMaterialsFromFolder(lightFolder);
        darkMat = LoadMaterialsFromFolder(darkFolder);
    }

    private Material[] LoadMaterialsFromFolder(string folderPath)
    {
        string[] guids = AssetDatabase.FindAssets("t:Material", new[] { folderPath });
        Material[] mats = new Material[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            mats[i] = AssetDatabase.LoadAssetAtPath<Material>(path);
        }

        return mats;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ColorSwapManager.Instance.OnColorSwap.AddListener(DoSwap);
        rend = GetComponent<Renderer>();
        if (usingLight)
            ApplyLight();
        else
            ApplyDark();
    }

    private void DoSwap()
    {
        usingLight = !usingLight;
        if (usingLight)
            ApplyLight();
        else
            ApplyDark();

        OnEntityColorSwapped.Invoke(usingLight);
    }

    private void ApplyLight()
    {
        if(rend != null)
            rend.materials = lightMat;
    }

    private void ApplyDark()
    {
        if (rend != null)
            rend.materials = darkMat;
    }
}
