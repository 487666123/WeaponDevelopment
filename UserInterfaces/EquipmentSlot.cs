using SilkyUIFramework.Attributes;
using SilkyUIFramework.BasicElements;
using Terraria;

namespace WeaponDevelopment.UserInterfaces;

[XmlElementMapping("EquipmentSlot")]
public class EquipmentSlot : SUIItemSlot
{
    public override bool CanPutInItemSlot(Item item)
    {
        return item.IsWeapon() || item.IsAir;
    }
}