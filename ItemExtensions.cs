using Terraria;

namespace WeaponDevelopment;

public static class ItemExtensions
{
    public static bool IsWeapon(this Item item) => item.damage > 0 && item.maxStack == 1;
}