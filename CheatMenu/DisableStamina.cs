using HarmonyLib;
using Il2Cpp;

namespace CheatMenu;

[HarmonyPatch(typeof(PlayerStats), "OnSwingWeapon")]
public class DisableStamina
{
    public static bool Prefix(ref float staminaCost, ref float strengthGain)
    {
        if (CheatMenu.IsGodModeEnabled)
        {
            staminaCost = 0;
            strengthGain = 100;
        }

        return !CheatMenu.IsShown;
    }
}