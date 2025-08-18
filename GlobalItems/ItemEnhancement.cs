using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace WeaponDevelopment.GlobalItems;

/// <summary>
/// 等级设定: 每次升级提供属性加成
/// 进阶设定:
///     武器默认等级上限为 50，可以通过 1 个进阶道具突破最高等级，每次进阶提升10级，最高可达 100 级
///     武器有 5 阶，每阶可以提升 20% 属性加成乘数，最高提升 100%
/// </summary>
public class ItemEnhancement : GlobalItem
{
    public override bool InstancePerEntity => true;

    public ItemLevel ItemLevel { get; private set; } = new();

    public override void ModifyItemScale(Item item, Player player, ref float scale)
    {
        if (item.useStyle == ItemUseStyleID.Swing && item.DamageType == DamageClass.Melee)
            scale += ItemLevel.ItemScaleBonus;
    }

    // 修改弹幕速度
    public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (item.DamageType != DamageClass.Ranged || float.IsNaN(velocity.X) || float.IsNaN(velocity.Y) || velocity == Vector2.Zero) return;

        velocity *= ItemLevel.ShootSpeedMultiplier;
    }

    // 修改伤害
    public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
    {
        damage.Base += ItemLevel.BaseDamage;
        damage *= ItemLevel.DamageMultiplier;
    }

    // 修改使用速度
    public override float UseSpeedMultiplier(Item item, Player player)
    {
        return ItemLevel.UseSpeedMultiplier;
    }

    // 修改法力消耗
    public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult)
    {
        reduce += ItemLevel.ManaCostBonus;
    }

    // 重置重铸
    //public override void PostReforge(Item item)
    //{
    //    item.ResetPrefix();
    //}

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (!item.IsWeapon()) return;

        var itemNameIndex = tooltips.FindIndex(item => item.Name.Equals("ItemName"));

        if (itemNameIndex >= 0)
        {
            // ==========1==========
            tooltips.Insert(++itemNameIndex, new(Mod, "-1-", "---------------"));

            var starRating = Math.Clamp(ItemLevel.StarRating, 0, ItemLevel.StarRatingCap);
            // 星级
            tooltips.Insert(++itemNameIndex, new(Mod, nameof(Level), $"星级 {new string('★', starRating) + new string('☆', ItemLevel.StarRatingCap - starRating)}")
            {
                OverrideColor = Color.Yellow
            });

            // 强化等级
            tooltips.Insert(++itemNameIndex, new(Mod, nameof(ItemLevel.Level), $"等级 {ItemLevel.Level}/{ItemLevel.LevelCap}")
            {
                OverrideColor = Color.DeepPink
            });

            // 强化经验 (等级满了就不显示经验值了)
            if (ItemLevel.Level < ItemLevel.MaxLevelCap)
            {
                tooltips.Insert(++itemNameIndex, new(Mod, "Exp", $"经验 {ItemLevel.Exp}/{ItemLevel.ExpCap}")
                {
                    OverrideColor = Color.HotPink
                });
            }

            // ==========2==========
            tooltips.Insert(++itemNameIndex, new(Mod, "-2-", "---------------"));

            // 伤害
            tooltips.Insert(++itemNameIndex, new(Mod, "DamageBonus", $"伤害 {(ItemLevel.DamageMultiplier - 1) * 100:+0;-0;0}% {ItemLevel.BaseDamage:+0.00;-0.00;0}")
            {
                OverrideColor = Color.OrangeRed
            });

            // 暴击
            if (item.DamageType != DamageClass.Summon)
            {
                tooltips.Insert(++itemNameIndex, new(Mod, "CritBonus", $"暴击 {ItemLevel.CritBonus:+0;-0;0}%")
                {
                    OverrideColor = Color.Yellow
                });
            }

            // 使用速度
            tooltips.Insert(++itemNameIndex, new(Mod, "UseSpeedBonus", $"使用速度 {(ItemLevel.UseSpeedMultiplier - 1) * 100:+0;-0;0}%")
            {
                OverrideColor = Color.GreenYellow
            });

            // 弹幕速度
            if (item.DamageType == DamageClass.Ranged && item.shoot is not ProjectileID.None)
            {
                tooltips.Insert(++itemNameIndex, new(Mod, "ShootSpeedBonus", $"弹幕速度 {(ItemLevel.ShootSpeedMultiplier - 1) * 100:+0;-0;0}%")
                {
                    OverrideColor = Color.LawnGreen
                });
            }

            // 魔力消耗
            if (item.mana > 0) // 消耗魔力的武器才会显示
            {
                tooltips.Insert(++itemNameIndex, new(Mod, "ManaCostBonus", $"魔力消耗 {ItemLevel.ManaCostBonus * 100f:+0;-0;0}%")
                {
                    OverrideColor = Color.BlueViolet
                });
            }

            // 大小
            if (item.useStyle == ItemUseStyleID.Swing && item.DamageType == DamageClass.Melee)
            {
                tooltips.Insert(++itemNameIndex, new(Mod, "ItemScaleBonus", $"大小 {ItemLevel.ItemScaleBonus * 100:+0;-0;0}%")
                {
                    OverrideColor = Color.Orange
                });
            }

            // ==========3==========
            tooltips.Insert(++itemNameIndex, new TooltipLine(Mod, "-3-", "---------------"));
        }

        // 显示有什么 tooltips
        // tooltips.Add(new(Mod, "contents", string.Join("\n", tooltips.Select(i => i.Name)))
        // {
        //     OverrideColor = Color.Pink
        // });
    }

    #region IO

    public override void NetSend(Item item, BinaryWriter writer)
    {
        ItemLevel.NetSend(writer);
    }

    public override void NetReceive(Item item, BinaryReader reader)
    {
        ItemLevel.NetReceive(reader);
        ItemLevel.ApplyLevelBonuses(item);
    }

    public override void SaveData(Item item, TagCompound tag)
    {
        tag[nameof(ItemLevel)] = ItemLevel;
    }

    public override void LoadData(Item item, TagCompound tag)
    {
        if (tag.TryGet<ItemLevel>(nameof(ItemLevel), out var itemLevel))
        {
            ItemLevel = itemLevel;
        }

        ItemLevel.ApplyLevelBonuses(item);
    }

    #endregion
}