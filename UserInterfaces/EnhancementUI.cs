using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SilkyUIFramework;
using SilkyUIFramework.Attributes;
using SilkyUIFramework.BasicElements;
using SilkyUIFramework.Extensions;
using SilkyUIFramework.Graphics2D;
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

        ToTop.LeftMouseDown += (_, _) =>
        {
            var item = WeaponSlot.Item;
            if (!item.IsWeapon()) return;

            if (item.TryGetGlobalItem<ItemEnhancement>(out var enhancement))
            {
                if (MaterialContainer.Children.FirstOrDefault() is not MaterialSlot stoneSlot) return;

                enhancement.ItemLevel.Enhance(stoneSlot?.Item);

                item.Refresh(false);

                item.position = Main.LocalPlayer.Center - item.Size / 2f;
                PopupText.NewText(PopupTextContext.ItemPickupToVoidContainer, item, 1, true);
                SoundEngine.PlaySound(SoundID.Item37);
            }

            UpdateInfo(item);
        };

        SetButtonHoverAnimation(ToTop);
        SetButtonHoverAnimation(ToOne);
    }

    private static void SetButtonHoverAnimation(UITextView button)
    {
        button.OnUpdateStatus += (gameTime) =>
        {
            var cyanBlue = new Color(100, 230, 230);
            button.BorderColor = button.HoverTimer.Lerp(Color.Black, cyanBlue) * 0.5f;
            button.BackgroundColor = button.HoverTimer.Lerp(Color.Black, cyanBlue) * 0.25f;
            button.TextColor = button.HoverTimer.Lerp(Color.White, cyanBlue);
            button.TextBorderColor = button.HoverTimer.Lerp(Color.Black, Color.Transparent);
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

    protected override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        if (BlurMakeSystem.BlurAvailable && !Main.gameMenu)
        {
            if (BlurMakeSystem.SingleBlur)
            {
                var batch = Main.spriteBatch;
                batch.End();
                BlurMakeSystem.KawaseBlur();
                batch.Begin(0, null, SamplerState.PointClamp, null, SilkyUI.RasterizerStateForOverflowHidden, null,
                    SilkyUI.TransformMatrix);
            }

            SDFRectangle.SampleVersion(BlurMakeSystem.BlurRenderTarget,
                Bounds.Position * Main.UIScale, Bounds.Size * Main.UIScale, BorderRadius * Main.UIScale,
                Matrix.Identity);
        }

        base.Draw(gameTime, spriteBatch);
    }
}