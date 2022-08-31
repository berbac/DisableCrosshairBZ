﻿using HarmonyLib;
using System;
using System.IO;
using UnityEngine;
using System.Linq;
//TODO: Need crosshair in Energy Room

namespace DisableCrosshairBZ
{
    [HarmonyPatch]
    public static class CrosshairPatcher
    {
        internal static bool crosshairIsOff;
        internal static string[] showCrosshairWhilePointingAt = { "MapRoomFunctionality(Clone)", "SeaTruckSleeperModule(Clone)", "Jukebox(Clone)" }; // special cases to show crosshair
        internal static string techType;
        internal static bool textHand;

        [HarmonyPatch(typeof(HandReticle), "UpdateText")]
        [HarmonyPrefix]
        public static void SetTextRaw_Prefix(string ___textHand, string ___textHandSubscript)
        {
            textHand = !string.IsNullOrEmpty(___textHand + ___textHandSubscript);
        }

        [HarmonyPatch(typeof(GUIHand), "OnUpdate")]
        [HarmonyPostfix]
        public static void OnUpdate_Postfix(GUIHand __instance, GUIHand.GrabMode ___grabMode)
        {
            if (Player.main == null) // skip block if no Player.main instance exists
                return;

            // getting techType for map room screen, jukebox etc.
            // check if CH needs to be enabled for interaction while on foot/swimming
            // try-catch needed -> throws error if no target in range

            try
            {
                Targeting.GetTarget(Player.main.gameObject, 10, out GameObject getTarget, out float _);
                CraftData.GetTechType(getTarget, out var _techType);
                techType = _techType.name;        
            }
            catch (NullReferenceException)
            {
                techType = null;
            }

            Player.Mode playerMode = Player.main.GetMode();
            bool isNormalOrSitting = playerMode == Player.Mode.Normal || playerMode == Player.Mode.Sitting;
            //GameObject activeTarget = null;
            //activeTarget = __instance.GetActiveTarget();
            bool targetNeedsCrosshair = (__instance.GetActiveTarget() && playerMode == Player.Mode.Normal) || textHand || 
                (Player.main.IsInsideWalkable() && Array.Exists(showCrosshairWhilePointingAt, element => element == techType));

/*            File.AppendAllText("DisableCrosshair_Log.txt",
                "\n targetNeedsCrosshair: " + targetNeedsCrosshair +
                //"\n GetActiveTarget: " + activeTarget +
                "\n textHand: " + textHand +
                "\n playerMode: " + playerMode +
                "\n grabMode: " + ___grabMode +
                "\n____________________________________");*/

            if (crosshairIsOff)
            { 
                if (((!CrosshairMenu.Config.NoCrosshairOnFoot || targetNeedsCrosshair) && isNormalOrSitting) || 
                   ((!CrosshairMenu.Config.NoCrosshairInPrawnSuit || targetNeedsCrosshair) && Player.main.inExosuit) ||
                   (!CrosshairMenu.Config.NoCrosshairInSeatruck && Player.main.inSeatruckPilotingChair))
                {
                    HandReticle.main.UnrequestCrosshairHide();
                    crosshairIsOff = false;
                    return;
                }
                else return;
            }

            else // Crosshair is currently on
            {
                if ((CrosshairMenu.Config.NoCrosshairOnFoot && isNormalOrSitting && !targetNeedsCrosshair) ||
                   (CrosshairMenu.Config.NoCrosshairInSeatruck && Player.main.inSeatruckPilotingChair) ||
                   (CrosshairMenu.Config.NoCrosshairInPrawnSuit && Player.main.inExosuit && !targetNeedsCrosshair))
                {
                    HandReticle.main.RequestCrosshairHide();
                    crosshairIsOff = true;
                    return;
                }
                else return;
            }
        }
    }
}
