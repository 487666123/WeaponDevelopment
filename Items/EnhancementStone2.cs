using Terraria.ModLoader;
using Terraria.ID;

namespace WeaponDevelopment.Items;

/// <summary>
/// 强化石
/// </summary>
public class EnhancementStone2 : ModItem, IEnhancement
{
    public int EXP => 100;

    public override void SetDefaults()
    {
        Item.rare = ItemRarityID.Green;
        Item.maxStack = 999;
        Item.width = 14;
        Item.height = 24;
    }
}