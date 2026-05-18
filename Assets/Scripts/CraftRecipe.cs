using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Recycle Shop/Craft Recipe")]
public class CraftRecipe : ScriptableObject
{
    [Header("Malzemeler")]
    public RecyclableItem ingredientA;
    public RecyclableItem ingredientB;

    [Header("Sonuç")]
    public RecyclableItem result;
    public Sprite resultSprite; 
    public string resultName;
    public int marketValue;

    [Header("Quality")]
    public int minQuality = 50;
    public int maxQuality = 100;
}