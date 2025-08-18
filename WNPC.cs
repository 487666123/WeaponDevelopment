using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;
using WeaponDevelopment.Items;

namespace WeaponDevelopment;

public class NPCLoot : GlobalNPC
{
    public override void ModifyNPCLoot(NPC npc, Terraria.ModLoader.NPCLoot npcLoot)
    {
        if (npc.damage <= 0) return;
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EnhancementStone>(), 2, 5, 10));
    }
}