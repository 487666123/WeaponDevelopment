using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WeaponDevelopment.Items;

namespace WeaponDevelopment;

public partial class ItemLevel : TagSerializable
{
    public static readonly Func<TagCompound, ItemLevel> DESERIALIZER = Load;

    #region static

    public static readonly int StarRatingCap = 5;

    public static readonly int BaseLevelCap = 50;
    public static readonly int MaxLevelCap = 100;

    public static readonly int BaseExp = 1000;
    public static readonly int ExpPerLevel = 1000;

    // 大小
    public static readonly float ItemScalePerLevel = 0.25f / 100f;
    // 伤害
    public static readonly float DamageMultiplierPerLevel = 1f / 100f;
    // 暴击
    public static readonly float CritPerLevel = 0.5f;
    // 使用速度
    public static readonly float UseSpeedPerLevel = 0.5f / 100f;
    // 弹幕速度
    public static readonly float ShootSpeedPerLevel = 0.5f / 100f;
    // 魔力消耗
    public static readonly float ManaCostPerLevel = -0.5f / 100f;

    #endregion

    public int StarRating { get; private set; } = 1;
    public float StarRatingMultiplier => 0.4f + StarRating * 0.12f;

    public int Level { get; private set; } = 0;
    public int LevelCap { get; private set; } = BaseLevelCap;

    public int Exp { get; private set; }
    public int ExpCap => BaseExp + Level * ExpPerLevel;

    /// <summary> 大小 </summary>
    public float ItemScaleBonus => Level * ItemScalePerLevel * StarRatingMultiplier;

    public float BaseDamage => Level * 0.05f * StarRatingMultiplier;

    /// <summary> 伤害 </summary>
    public float DamageMultiplier => 1 + Level * DamageMultiplierPerLevel * StarRatingMultiplier;

    /// <summary> 暴击 </summary>
    public int CritBonus => (int)Math.Round(Level * CritPerLevel * StarRatingMultiplier);

    /// <summary> 使用速度 </summary>
    public float UseSpeedMultiplier => 1 + Level * UseSpeedPerLevel * StarRatingMultiplier;

    /// <summary> 弹幕速度 </summary>
    public float ShootSpeedMultiplier => 1 + Level * ShootSpeedPerLevel * StarRatingMultiplier;

    /// <summary> 魔力消耗 </summary>
    public float ManaCostBonus => Level * ManaCostPerLevel * StarRatingMultiplier;

    /// <summary> 应用加成 </summary>
    public void ApplyLevelBonuses(Item item)
    {
        if (!item.IsWeapon()) return;

        // 暴击
        if (item.DamageType != DamageClass.Summon)
            item.crit += CritBonus;

        // 名字
        item.ClearNameOverride();
        item.SetNameOverride(item.Name + $" lv.{Level}");
    }

    /// <summary>
    /// 强化
    /// </summary>
    public void Enhance(Item material)
    {
        StarRating = 5;
        LevelCap = 100;

        if (material is null || !CanuUpgrade || material.ModItem is not EnhancementStone enhancementStone) return;

        var totalExp = enhancementStone.TotalExp;
        var required = ExpCap - Exp;

        while (CanuUpgrade)
        {
            if (totalExp >= required)
            {
                Level += 1;
                totalExp -= required;
                required = ExpCap - Exp;
            }
            else
            {
                Exp += totalExp;
                totalExp = 0;
                break;
            }
        }

        material.stack = totalExp / enhancementStone.Exp;
    }

    public bool CanuUpgrade => Level < LevelCap;
}