using UnityEngine;

public enum ItemType
{
    SpeedBoost,
    DamageBoost
}

public enum ItemRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    public ItemType itemType;
    public ItemRarity rarity;

    public float value;
    public float dropChance;   

    public Sprite icon;
}