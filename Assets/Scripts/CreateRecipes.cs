#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class CreateRecipes
{
    [MenuItem("Recycle Shop/Create Craft Recipes")]
    public static void CreateAll()
    {
        string folder = "Assets/ScriptableObjects/Recipes";
        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects"))
            AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
        if (!AssetDatabase.IsValidFolder(folder))
            AssetDatabase.CreateFolder("Assets/ScriptableObjects", "Recipes");

        CreateRecipe(folder, "Cam_Metal_Buyutec", "Cam", "Metal", "Büyüteç", 80);
        CreateRecipe(folder, "Cam_Kagit_CerceveliFoto", "Cam", "Kagit", "Çerçeveli Fotoğraf", 60);
        CreateRecipe(folder, "Cam_Plastik_Kavanoz", "Cam", "Plastik", "Kavanoz", 50);
        CreateRecipe(folder, "Metal_Kagit_NotDefteri", "Metal", "Kagit", "Not Defteri", 45);
        CreateRecipe(folder, "Metal_Plastik_Tornavida", "Metal", "Plastik", "Tornavida", 70);
        CreateRecipe(folder, "Kagit_Plastik_Kart", "Kagit", "Plastik", "Kart", 30);
        CreateRecipe(folder, "Cam_Cam_Ayna", "Cam", "Cam", "Ayna", 90);
        CreateRecipe(folder, "Metal_Metal_Plaka", "Metal", "Metal", "Plaka", 65);
        CreateRecipe(folder, "Kagit_Kagit_KartonKutu", "Kagit", "Kagit", "Karton Kutu", 40);
        CreateRecipe(folder, "Plastik_Plastik_Kelepcе", "Plastik", "Plastik", "Kelepçe", 35);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("10 tarif olusturuldu: " + folder);
    }

    static void CreateRecipe(string folder, string fileName,
        string catA, string catB, string resultName, int marketValue)
    {
        CraftRecipe recipe = ScriptableObject.CreateInstance<CraftRecipe>();
        recipe.resultName = resultName;
        recipe.marketValue = marketValue;

        AssetDatabase.CreateAsset(recipe, folder + "/" + fileName + ".asset");
    }
}
#endif