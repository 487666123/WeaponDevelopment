using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace WeaponDevelopment.Items;

/// <summary>
/// 强化石
/// </summary>
public class EnhancementStone : ModItem, IEnhancement
{
    public int TotalExp => Exp * Item.stack;
    public int Exp => 1000;

    public override void SetDefaults()
    {
        Item.rare = ItemRarityID.Green;
        Item.maxStack = 999;
        Item.width = 14;
        Item.height = 24;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        var itemNameIndex = tooltips.FindIndex(item => item.Name.Equals("ItemName"));

        if (itemNameIndex >= 0)
        {
            tooltips.Insert(++itemNameIndex, new(Mod, nameof(Exp), $"经验值 {Exp:+0;-0;0}")
            {
                OverrideColor = Color.Red
            });
        }
    }
}