﻿using HarmonyLib;
using System.Collections.Generic;

namespace CultOfTheLambMods;

internal static class TwitchJunk
{
    [HarmonyPatch(typeof(GameManager))]
    public static class GameManagerPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch("Awake")]
        public static void Postfix()
        {
            var availableTwitchTotemDecorations = DataManager.GetAvailableTwitchTotemDecorations();
            var availableTwitchTotemSkins = DataManager.GetAvailableTwitchTotemSkins();

            var totemDecorations = new List<StructureBrain.TYPES>(6)
            {
                StructureBrain.TYPES.DECORATION_TWITCH_FLAG_CROWN,
                StructureBrain.TYPES.DECORATION_TWITCH_MUSHROOM_BAG,
                StructureBrain.TYPES.DECORATION_TWITCH_ROSE_BUSH,
                StructureBrain.TYPES.DECORATION_TWITCH_STONE_FLAG,
                StructureBrain.TYPES.DECORATION_TWITCH_STONE_STATUE,
                StructureBrain.TYPES.DECORATION_TWITCH_WOODEN_GUARDIAN
            };

            var twitchSkins = new List<string>(2) { "TwitchCat", "TwitchMouse" };

            if (Plugin.UnlockTwitchStuff.Value)
            {
                if (DataManager.TwitchTotemRewardAvailable())
                {
                    foreach (var availableTwitchTotemDecoration in availableTwitchTotemDecorations)
                        StructuresData.CompleteResearch(availableTwitchTotemDecoration);
                    foreach (var availableTwitchTotemDecoration in availableTwitchTotemDecorations)
                        StructuresData.SetRevealed(availableTwitchTotemDecoration);
                    foreach (var availableTwitchTotemSkin in availableTwitchTotemSkins)
                        DataManager.SetFollowerSkinUnlocked(availableTwitchTotemSkin);
                }
            }
            else
            {
                foreach (var type in totemDecorations)
                {
                    if (StructuresData.GetUnlocked(type))
                    {
                        DataManager.Instance.UnlockedStructures.Remove(type);
                    }

                    if (DataManager.Instance.RevealedStructures.Contains(type))
                    {
                        DataManager.Instance.RevealedStructures.Remove(type);
                    }
                }

                foreach (var skin in twitchSkins)
                {
                    if (DataManager.Instance.FollowerSkinsUnlocked.Contains(skin))
                    {
                        DataManager.Instance.FollowerSkinsUnlocked.Remove(skin);
                    }

                    if (DataManager.Instance.NewFollowerSkins.Contains(skin))
                    {
                        DataManager.Instance.NewFollowerSkins.Remove(skin);
                    }
                }
            }
        }

        [HarmonyPostfix]
        //[HarmonyPatch(nameof(GameManager.AuthenticateCultistDLC))] -- pay the $6
        [HarmonyPatch(nameof(GameManager.AuthenticatePrePurchaseDLC))]
        [HarmonyPatch(nameof(GameManager.AuthenticatePlushBonusDLC))]
        [HarmonyPatch(nameof(GameManager.AuthenticateTwitchDrop1))]
        [HarmonyPatch(nameof(GameManager.AuthenticateTwitchDrop2))]
        [HarmonyPatch(nameof(GameManager.AuthenticateTwitchDrop3))]
        public static void Postfix(ref bool __result)
        {
            if (Plugin.UnlockTwitchStuff.Value)
            {
                __result = true;
            }
        }
    }
}