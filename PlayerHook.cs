using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using WeaponDevelopment.GlobalItems;

namespace WeaponDevelopment;

class PlayerHook : ModSystem
{
    public override void Load()
    {
        IL_Player.ItemCheck_Shoot += IL_Player_ItemCheck_Shoot;
    }

    private static void IL_Player_ItemCheck_Shoot(MonoMod.Cil.ILContext il)
    {
        var c = new ILCursor(il);

        //IL_0012: ldfld float32 Terraria.Item::shootSpeed
        //IL_0017: stloc.1
        //IL_0018: ldarg.2
        //IL_0019: ldfld int32 Terraria.Item::damage <|
        //IL_001e: stloc.2
        //IL_001f: ldarg.2
        if (!c.TryGotoNext(MoveType.After,
            i => i.MatchLdfld(typeof(Item), nameof(Item.damage))))
        {
            return;
        }

        // 压入 Player
        c.Emit(OpCodes.Ldarg_0);
        // 压入 Item
        c.Emit(OpCodes.Ldarg_2);

        // 修改
        c.EmitDelegate<Func<int, Player, Item, int>>((damage, player, item) =>
        {
            var modifier = new StatModifier();
            var itemEnhancement = item.GetGlobalItem<ItemEnhancement>();
            itemEnhancement.ModifyWeaponDamage(item, player, ref modifier);
            damage = (int)Math.Max(0, modifier.ApplyTo(damage) + 5E-06f);
            return damage;
        });
    }
}
