using HarmonyLib;
using Il2Cpp;

namespace CheatMenu;

[HarmonyPatch(typeof(PlayerStats), "OnSwingWeapon")]
public class DisableWeaponStamina
{
    public static bool Prefix(ref float staminaCost)
    {
        if (CheatMenu.IsGodModeEnabled)
        {
            staminaCost = 0;
        }

        return true;
    }
}