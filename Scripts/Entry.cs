using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;

namespace fufu.Scripts;

[ModInitializer("Init")]
public class Entry
{
    // 🔥 在这里加上你的模组ID（必须加）
    public const string ModId = "fufu";

    public static void Init()
    {
        var harmony = new Harmony(ModId);
        harmony.PatchAll();
        
        ScriptManagerBridge.LookupScriptsInAssembly(typeof(Entry).Assembly);
        Log.Debug("Mod initialized!");
    }
}