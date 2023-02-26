using HarmonyLib;
using Il2Cpp;

namespace CheatMenu;

[HarmonyPatch(typeof(FirstPersonCharacter), "DoFallDamage")]
public static class DisableFallDamage
{
    public static bool Prefix()
    {
        return !CheatMenu.IsGodModeEnabled;
    }
}