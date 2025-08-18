using System.Linq;
using Microsoft.Xna.Framework;
using SilkyUIFramework;
using SilkyUIFramework.Attributes;
using SilkyUIFramework.BasicElements;
using SilkyUIFramework.Extensions;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using WeaponDevelopment.GlobalItems;

namespace WeaponDevelopment.UserInterfaces;

[RegisterUI("Vanilla: Radial Hotbars", "EnhancementUI")]
public partial class EnhancementUI : BasicBody
{
    public static EnhancementUI Instance { get; private set; }

    public EnhancementUI() => Instance = this;

    protected override void OnInitialize()
    {
        InitializeComponent();

        BorderColor = SUIColor.Border * 0.75f;
        BackgroundColor = SUIColor.Background * 0.75f;

        WeaponSlot.BorderColor = Color.Black * 0.5f;
        WeaponSlot.BackgroundColor = Color.Black * 0.25f;

        WeaponSlot.ItemChanged += (_, args) => { UpdateInfo(args.NewValue); };

        for (var i = 0; i < 5; i++)
        {
            var stoneSlot = new MaterialSlot()
            {
                BorderColor = Color.Black * 0.5f,
                BorderRadius = new Vector4(8f),
                BackgroundColor = Color.Black * 0.25f,
                ItemScale = 0.846f,
            }.Join(MaterialContainer);
            stoneSlot.SetSize(48f, 48f);
        }

        Button.LeftMouseDown += (_, _) =>
        {
            var item = WeaponSlot.Item;
            if (!item.IsWeapon()) return;

            if (item.TryGetGlobalItem<ItemEnhancement>(out var enhancement))
            {
                if (MaterialContainer.Children.First() is not MaterialSlot stoneSlot) return;

                enhancement.ItemLevel.Enhance(stoneSlot?.Item);

                item.Refresh(false);

                item.position = Main.LocalPlayer.Center - item.Size / 2f;
                PopupText.NewText(PopupTextContext.ItemPickupToVoidContainer, item, 1, true);
                SoundEngine.PlaySound(SoundID.Item37);
            }

            UpdateInfo(item);
        };

        Button.OnUpdateStatus += (gameTime) =>
        {
            Button.BorderColor = Button.HoverTimer.Lerp(Color.Black * 0.5f, new Color(100, 230, 230) * 0.75f);
            Button.BackgroundColor = Button.HoverTimer.Lerp(Color.Black * 0.25f, new Color(100, 230, 230) * 0.2f);
            Button.TextColor = Button.HoverTimer.Lerp(Color.White, new Color(100, 230, 230));
            Button.TextBorderColor = Button.HoverTimer.Lerp(Color.Black, Color.Black * 0f);
        };
    }

    private void SetDefaultInfo()
    {
        WeaponName.Text = "???";
        WeaponLevel.Text = "???";
        ProgressLevel.Progress = 0;
        WeaponExp.Text = "???";
        ProgressExp.Progress = 0;
    }

    private void UpdateInfo(Item item)
    {
        if (item.TryGetGlobalItem<ItemEnhancement>(out var itemEnhancement))
        {
            var itemLevel = itemEnhancement.ItemLevel;

            WeaponName.Text = item.HoverName;
            WeaponLevel.Text = $"lv.{itemLevel.Level} / {itemLevel.LevelCap}";
            ProgressLevel.Progress = itemLevel.Level / (float)itemLevel.LevelCap;
            WeaponExp.Text = $"Exp.{itemLevel.Exp} / {itemLevel.ExpCap}";
            ProgressExp.Progress = itemLevel.Exp / (float)itemLevel.ExpCap;
        }
        else SetDefaultInfo();
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    protected override void UpdateStatus(GameTime gameTime)
    {
        base.UpdateStatus(gameTime);
    }
}