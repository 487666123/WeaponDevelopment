using Terraria.ID;
using Terraria.ModLoader;

namespace WeaponDevelopment.Items;

/// <summary>
/// 进化石
/// </summary>
public class EvolutionStone : ModItem
{
    public override void SetDefaults()
    {
        Item.width = Item.height = 20;
        Item.rare = ItemRarityID.Blue;
        Item.maxStack = 999;
    }
}