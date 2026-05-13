using UnityEngine;

[CreateAssetMenu(fileName = "NewRecyclableItem", menuName = "Recycle Shop/Recyclable Item")]
public class RecyclableItem : ScriptableObject
{
    [Header("Kimlik")]
    public string itemName;
    public RecycleCategory category;

    [Header("Sprite'lar")]
    public Sprite dirtySprite;
    public Sprite cleanSprite;
    public Sprite recycledSprite;

    [Header("Temizlik")]
    public CleaningTool requiredTool;
    public int cleaningStepsRequired = 3;

    [Header("Ekonomi")]
    public int baseValue = 50;
    public int bonusPerQuality = 1;
}

public enum RecycleCategory { Plastik, Kagit, Cam, Metal }
public enum CleaningTool { Firca, Sunger, HerIkisi }