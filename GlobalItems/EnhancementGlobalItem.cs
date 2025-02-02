using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace WeaponDevelopment.GlobalItems;

public class EnhancementGlobalItem : GlobalItem
{
    public override bool InstancePerEntity => true;

    #region 强化等级上限 & 基本强化经验 & 强化经验

    public static readonly int EnhancementLevelCap = 100;
    public static readonly int EnhancementBaseEXP = 1000;
    public static readonly int EnhancementEXPPerLevel = 1000;

    #endregion

    /// <summary>
    /// 获取强化到下一级需要的经验
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public static int GetEnhancementRequiredEXP(int level)
    {
        return EnhancementBaseEXP + level * EnhancementEXPPerLevel;
    }

    /// <summary>
    /// 强化等级
    /// </summary>
    public int EnhancementLevel;
    public int MaxEnhancementLevel = 50;

    /// <summary>
    /// 强化经验
    /// </summary>
    public int EnhancementEXP;
    public int EnhancementRequiredEXP;

    /// <summary>
    /// 更新状态
    /// </summary>
    /// <param name="item"></param>
    public void AapplyLevelBonuses(Item item)
    {
        if (!item.IsWeapon()) return;

        EnhancementRequiredEXP = GetEnhancementRequiredEXP(EnhancementLevel);

        // 暴击
        item.crit += GetCritBonus();
        // 射弹速度
        item.shootSpeed *= GetShootSpeedMultiplier();

        // 名字
        item.ClearNameOverride();
        item.SetNameOverride(item.Name + $" lv.{EnhancementLevel}");
    }

    public int GetCritBonus() => (int)Math.Round(EnhancementLevel * 0.5f);
    public float GetManaCostBonus() => MathF.Round(EnhancementLevel * -0.25f / 100f, 2);

    public readonly float DamageMultiplierPerLevel = 1f / 100f;
    public float GetDamageMultiplier() => 1 + EnhancementLevel * DamageMultiplierPerLevel;

    public readonly float ShootSpeedPerLevel = 0.25f / 100f;
    public float GetShootSpeedMultiplier() => 1 + EnhancementLevel * ShootSpeedPerLevel;

    public readonly float UseSpeedPerLevel = 0.5f / 100f;
    public float GetUseSpeedMultiplier() => 1 + EnhancementLevel * UseSpeedPerLevel;

    public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage) => damage *= GetDamageMultiplier();
    public override float UseSpeedMultiplier(Item item, Player player) => GetUseSpeedMultiplier();

    public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult)
    {
        reduce += GetManaCostBonus();
    }

    // 不让重铸了
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
            tooltips.Insert(++itemNameIndex, new(Mod, nameof(EnhancementLevel), $"等级 {EnhancementLevel}/{100}")
            {
                OverrideColor = Color.DeepPink
            });

            // 强化经验 (等级满了就不显示经验值了)
            if (EnhancementLevel < 100)
            {
                tooltips.Insert(++itemNameIndex, new(Mod, nameof(EnhancementEXP), $"经验 {EnhancementEXP}/{EnhancementRequiredEXP}")
                {
                    OverrideColor = Color.HotPink
                });
            }

            // ==========2==========
            tooltips.Insert(++itemNameIndex, new(Mod, "-2-", "---------------"));

            // 伤害
            tooltips.Insert(++itemNameIndex, new(Mod, nameof(EnhancementEXP), $"伤害 {(GetDamageMultiplier() - 1) * 100:+0;-0;0}%")
            {
                OverrideColor = Color.OrangeRed
            });

            // 暴击
            tooltips.Insert(++itemNameIndex, new(Mod, nameof(EnhancementEXP), $"暴击 {GetCritBonus():+0;-0;0}%")
            {
                OverrideColor = Color.LightYellow
            });

            // 速度
            tooltips.Insert(++itemNameIndex, new(Mod, nameof(EnhancementEXP), $"速度 {(GetUseSpeedMultiplier() - 1) * 100:+0;-0;0}%")
            {
                OverrideColor = Color.YellowGreen
            });

            // 魔力消耗
            if (entity.mana > 0) // 消耗魔力的武器才会显示
            {
                tooltips.Insert(++itemNameIndex, new(Mod, nameof(EnhancementEXP), $"魔力消耗 {GetManaCostBonus() * 100f:+0;-0;0}%")
                {
                    OverrideColor = Color.BlueViolet
                });
            }

            // ==========3==========
            tooltips.Insert(++itemNameIndex, new TooltipLine(Mod, "-3-", "---------------"));
        }

        // 显示有什么 tooltips
        tooltips.Add(new(Mod, "contents", string.Join("\n", tooltips.Select(i => i.Name)))
        {
            OverrideColor = Color.Pink
        });
    }

    #region {Net Send & Receive} and {Save & Load Data}

    public override void NetSend(Item item, BinaryWriter writer)
    {
        writer.Write7BitEncodedInt(EnhancementLevel);
        writer.Write7BitEncodedInt(EnhancementEXP);
    }

    public override void NetReceive(Item item, BinaryReader reader)
    {
        EnhancementLevel = reader.Read7BitEncodedInt();
        EnhancementEXP = reader.Read7BitEncodedInt();

        AapplyLevelBonuses(item);
    }

    public override void SaveData(Item item, TagCompound tag)
    {
        tag[nameof(EnhancementLevel)] = EnhancementLevel;
        tag[nameof(EnhancementEXP)] = EnhancementEXP;
    }

    public override void LoadData(Item item, TagCompound tag)
    {
        if (tag.TryGet<int>(nameof(EnhancementLevel), out var enhancementLevelValue))
        {
            EnhancementLevel = enhancementLevelValue;
        }
        else { EnhancementLevel = 1; }

        if (tag.TryGet<int>(nameof(EnhancementEXP), out var enhancementEXPValue))
        {
            EnhancementEXP = enhancementEXPValue;
        }
        else { EnhancementEXP = 0; }

        AapplyLevelBonuses(item);
    }

    #endregion
}