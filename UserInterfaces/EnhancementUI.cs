using System;
using Microsoft.Xna.Framework;
using SilkyUIFramework.Attributes;
using SilkyUIFramework.BasicElements;
using SilkyUIFramework.Extensions;
using Steamworks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using WeaponDevelopment.GlobalItems;

namespace WeaponDevelopment.UserInterfaces;

[AutoloadUI("Vanilla: Radial Hotbars", "EnhancementUI")]
public class EnhancementUI : BasicBody
{
    public override bool Enabled => true;

    public override void OnInitialize()
    {
        var view = new SUIDraggableView()
        {
            CornerRadius = new Vector4(12f),
            DragIncrement = new Vector2(5f),
            HAlign = 0.5f,
            VAlign = 0.5f,
            BgColor = Color.Black * 0.25f,
            Display = Display.Flexbox,
            LayoutDirection = LayoutDirection.Column,
            Gap = new Vector2(4f),
        }.Join(this);
        view.SetPadding(4f);

        var title = new SUIText()
        {
            Text = "装备强化",
            TextScale = 0.75f,
            TextAlign = new Vector2(0.5f),
        }.Join(view);

        var itemContainer = new View
        {
            Display = Display.Flexbox,
            LayoutDirection = LayoutDirection.Row,
            Gap = new Vector2(4f),
        }.Join(view);

        var weaponSlot = new SUIItemSlot()
        {
            CornerRadius = new Vector4(8f),
        }.Join(itemContainer);
        weaponSlot.BorderColor = Color.Black * 0.5f;
        weaponSlot.BgColor = Color.Black * 0.25f;
        weaponSlot.SetSize(52, 52);

        var stonelot = new SUIItemSlot()
        {
            CornerRadius = new Vector4(8f),
            ItemScale = 0.846f,
        }.Join(itemContainer);
        stonelot.BorderColor = Color.Black * 0.5f;
        stonelot.BgColor = Color.Black * 0.25f;
        stonelot.SetSize(52, 52);

        var button = new SUIText
        {
            CornerRadius = new Vector4(8f),
            Border = 2f,
            BorderColor = Color.Black * 0.5f,
            BgColor = Color.Black * 0.25f,
            Text = "强化",
            TextAlign = new Vector2(0.5f),
            DragIgnore = false,
        }.Join(view);
        button.SetSize(itemContainer.GetDimensions().Width, 30f);

        title.SetWidth(itemContainer.GetDimensions().Width);

        button.OnLeftMouseDown += (_, _) =>
        {
            var item = weaponSlot.Item;
            if (!item.IsWeapon()) return;

            if (item.TryGetGlobalItem<ItemEnhancement>(out var enhancement))
            {
                enhancement.ItemLevel.Enhance(stonelot.Item);

                item.prefix = 0;
                item.Refresh(false);

                item.position = Main.LocalPlayer.Center - item.Size / 2f;
                PopupText.NewText(PopupTextContext.ItemPickupToVoidContainer, item, 1, true);
                SoundEngine.PlaySound(SoundID.Item37);
            }
        };
    }
}