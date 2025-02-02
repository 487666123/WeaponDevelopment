using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace WeaponDevelopment.GlobalItems;

public class EnhancementItem : GlobalItem
{
    public override bool InstancePerEntity => true;

    #region 强化等级上限 & 基本强化经验 & 强化经验

    public static readonly int EnhancementLevelCap = 100;
    public static readonly int BaseEXP = 1000;
    public static readonly int EXPPerLevel = 1000;

    #endregion

    /// <summary>
    /// 强化等级
    /// </summary>
    public int Level = 1;
    public int MaxLevel = 50;

    /// <summary>
    /// 强化经验
    /// </summary>
    public int EXP;
    public int RequiredEXP => Level * EXPPerLevel;

    /// <summary>
    /// 更新状态
    /// </summary>
    /// <param name="item"></param>
    public void AapplyLevelBonuses(Item item)
    {
        if (!item.IsWeapon()) return;

        // 暴击
        item.crit += GetCritBonus();

        // 名字
        item.ClearNameOverride();
        item.SetNameOverride(item.Name + $" lv.{Level}");
    }

    public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
    }

    public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (float.IsNaN(velocity.X) || float.IsNaN(velocity.Y) || velocity == Vector2.Zero) return;

        var shootSpeed = velocity.Length() * GetShootSpeedMultiplier();
        velocity = Vector2.Normalize(velocity) * shootSpeed;
    }

    public int GetCritBonus() => (int)Math.Round(Level * 0.5f);

    public readonly float ManaCostPerLevel = -0.25f / 100f;
    public float GetManaCostBonus() => MathF.Round(Level * ManaCostPerLevel, 2);

    public readonly float DamageMultiplierPerLevel = 1f / 100f;
    public float GetDamageMultiplier() => 1 + Level * DamageMultiplierPerLevel;

    public readonly float ShootSpeedPerLevel = 0.25f / 100f;
    public float GetShootSpeedMultiplier() => 1 + Level * ShootSpeedPerLevel;

    public readonly float UseSpeedPerLevel = 0.5f / 100f;
    public float GetUseSpeedMultiplier() => 1 + Level * UseSpeedPerLevel;

    // 修改伤害
    public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage) => damage *= GetDamageMultiplier();

    // 修改使用速度
    public override float UseSpeedMultiplier(Item item, Player player) => GetUseSpeedMultiplier();

    // 修改法力消耗
    public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult) => reduce += GetManaCostBonus();

    // 重置重铸
    public override void PostReforge(Item item) => item.ResetPrefix();

    public override void ModifyTooltips(Item entity, List<TooltipLine> tooltips)
    {
        if (!entity.IsWeapon()) return;

        var itemNameIndex = tooltips.FindIndex(item => item.Name.Equals("ItemName"));

        if (itemNameIndex >= 0)
        {
            // ==========1==========
            tooltips.Insert(++itemNameIndex, new(Mod, "-1-", "---------------"));

            // 强化等级
            tooltips.Insert(++itemNameIndex, new(Mod, nameof(Level), $"等级 {Level}/{MaxLevel}")
            {
                OverrideColor = Color.DeepPink
            });

            // 强化经验 (等级满了就不显示经验值了)
            if (Level < 100)
            {
                tooltips.Insert(++itemNameIndex, new(Mod, nameof(EXP), $"经验 {EXP}/{RequiredEXP}")
                {
                    OverrideColor = Color.HotPink
                });
            }

            // ==========2==========
            tooltips.Insert(++itemNameIndex, new(Mod, "-2-", "---------------"));

            // 伤害
            tooltips.Insert(++itemNameIndex, new(Mod, nameof(EXP), $"伤害 {(GetDamageMultiplier() - 1) * 100:+0;-0;0}%")
            {
                OverrideColor = Color.OrangeRed
            });

            // 暴击
            tooltips.Insert(++itemNameIndex, new(Mod, nameof(EXP), $"暴击 {GetCritBonus():+0;-0;0}%")
            {
                OverrideColor = Color.LightYellow
            });

            // 速度
            tooltips.Insert(++itemNameIndex, new(Mod, nameof(EXP), $"速度 {(GetUseSpeedMultiplier() - 1) * 100:+0;-0;0}%")
            {
                OverrideColor = Color.YellowGreen
            });

            // 魔力消耗
            if (entity.mana > 0) // 消耗魔力的武器才会显示
            {
                tooltips.Insert(++itemNameIndex, new(Mod, nameof(EXP), $"魔力消耗 {GetManaCostBonus() * 100f:+0;-0;0}%")
                {
                    OverrideColor = Color.BlueViolet
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

    #region {Net Send & Receive} and {Save & Load Data}

    public override void NetSend(Item item, BinaryWriter writer)
    {
        writer.Write7BitEncodedInt(Level);
        writer.Write7BitEncodedInt(EXP);
    }

    public override void NetReceive(Item item, BinaryReader reader)
    {
        Level = reader.Read7BitEncodedInt();
        EXP = reader.Read7BitEncodedInt();

        AapplyLevelBonuses(item);
    }

    public override void SaveData(Item item, TagCompound tag)
    {
        tag[nameof(Level)] = Level;
        tag[nameof(EXP)] = EXP;
    }

    public override void LoadData(Item item, TagCompound tag)
    {
        if (tag.TryGet<int>(nameof(Level), out var levelValue))
        {
            Level = levelValue;
        }
        else { Level = 1; }

        if (tag.TryGet<int>(nameof(EXP), out var EXPValue))
        {
            EXP = EXPValue;
        }
        else { EXP = 0; }

        AapplyLevelBonuses(item);
    }

    #endregion
}