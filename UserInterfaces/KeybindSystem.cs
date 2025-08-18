using Terraria.GameInput;
using Terraria.ModLoader;

namespace WeaponDevelopment.UserInterfaces;

public class KeybindSystem : ModSystem
{
    public static ModKeybind ControlPanel { get; private set; }

    public override void Load()
    {
        ControlPanel = KeybindLoader.RegisterKeybind(Mod, nameof(ControlPanel), "OemPipe");
    }

    public override void Unload()
    {
        ControlPanel = null;
    }
}

public class KeybindPlayer : ModPlayer
{
    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        if (KeybindSystem.ControlPanel.JustPressed)
        {
            EnhancementUI.Instance.Enabled = !EnhancementUI.Instance.Enabled;
        }
    }
}