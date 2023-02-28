using HarmonyLib;
using Il2Cpp;
using Il2CppSons.Ai.Vail;
using Il2CppSons.Weapon;

namespace CheatMenu;

[HarmonyPatch(typeof(VailActor), "ReceivedDamage")]
public class DisableSwingWeapon
{
    public static bool Prefix(ref float amount)
    {
        if (CheatMenu.IsInstantKillEnabled)
        {
            amount = int.MaxValue;
        }

        return true;
    }
}