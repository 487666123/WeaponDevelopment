using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;
using WeaponDevelopment.Items;

namespace WeaponDevelopment;

public class WNPC : GlobalNPC
{
    public override void ModifyGlobalLoot(GlobalLoot globalLoot)
    {
        globalLoot.Add(ItemDropRule.Common(ModContent.ItemType<EnhancementStone>(), 5, 1, 10));
    }
}