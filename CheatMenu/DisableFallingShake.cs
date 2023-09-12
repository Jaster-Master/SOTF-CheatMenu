using HarmonyLib;
using Il2Cpp;

namespace CheatMenu;

[HarmonyPatch(typeof(camFollowHead), "startFallingShake")]
public class DisableFallingShake
{
    public static bool Prefix()
    {
        return !CheatMenu.IsGodModeEnabled;
    }
}