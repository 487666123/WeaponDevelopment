using SilkyUIFramework.Attributes;
using SilkyUIFramework.BasicElements;
using Terraria;
using WeaponDevelopment.Items;

namespace WeaponDevelopment.UserInterfaces;

[XmlElementMapping("MaterialSlot")]
public class MaterialSlot : SUIItemSlot
{
    public override bool CanPutInItemSlot(Item item)
    {
        return item.IsAir || item.ModItem is EnhancementStone;
    }
}