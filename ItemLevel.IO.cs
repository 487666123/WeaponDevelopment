using System.IO;
using Terraria.ModLoader.IO;

namespace WeaponDevelopment;

public partial class ItemLevel
{
    public void NetSend(BinaryWriter writer)
    {
        writer.Write7BitEncodedInt(StarRating);
        writer.Write7BitEncodedInt(LevelCap);
        writer.Write7BitEncodedInt(Level);
        writer.Write7BitEncodedInt(Exp);
    }

    public void NetReceive(BinaryReader reader)
    {
        StarRating = reader.Read7BitEncodedInt();
        LevelCap = reader.Read7BitEncodedInt();
        Level = reader.Read7BitEncodedInt();
        Exp = reader.Read7BitEncodedInt();
    }

    public TagCompound SerializeData()
    {
        return new TagCompound
        {
            [nameof(StarRating)] = StarRating,
            [nameof(LevelCap)] = LevelCap,
            [nameof(Level)] = Level,
            [nameof(Exp)] = Exp
        };
    }

    public static ItemLevel Load(TagCompound tag)
    {
        var itemLevel = new ItemLevel();

        if (tag.TryGet<int>(nameof(StarRating), out var StarRatingValue))
        {
            itemLevel.StarRating = StarRatingValue;
        }

        if (tag.TryGet<int>(nameof(LevelCap), out var LevelCapValue))
        {
            itemLevel.LevelCap = LevelCapValue;
        }

        if (tag.TryGet<int>(nameof(Level), out var levelValue))
        {
            itemLevel.Level = levelValue;
        }

        if (tag.TryGet<int>(nameof(Exp), out var ExpValue))
        {
            itemLevel.Exp = ExpValue;
        }

        return itemLevel;
    }
}