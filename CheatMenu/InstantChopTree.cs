using HarmonyLib;
using Il2CppSons.Gameplay.TreeCutting;
using MelonLoader;
using UnityEngine;

namespace CheatMenu;

[HarmonyPatch(typeof(TreeCutBlock), "OnTriggerEnter")]
public class InstantChopTree
{
    public static bool Prefix(TreeCutBlock __instance, Collider other)
    {
        if (CheatMenu.IsInstantChopTreeEnabled)
        {
            // TODO: direction could be incorrect
            var direction = __instance.transform.position - other.transform.position;
            __instance._cutManager.InstantCutForceFall(direction);
        }

        return true;
    }
}