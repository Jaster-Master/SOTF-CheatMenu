/*using HarmonyLib;
using Il2CppSons.Ai.Vail;

namespace CheatMenu;

[HarmonyPatch(typeof(VailActor), "ReceivedDamage")]
public class EnableInstantKill
{
    public static bool Prefix(ref float amount)
    {
        if (CheatMenu.IsInstantKillEnabled)
        {
            amount = int.MaxValue;
        }

        return true;
    }
}*/