using HarmonyLib;
using System;
using System.IO;
using UnityEngine;

namespace DisableCrosshairBZ
{
    [HarmonyPatch]
    public static class CrosshairPatcher
    {
        internal static bool ShouldHideCrosshair;
        internal static string[] showCrosshairWhilePointingAt = { "MapRoomFunctionality(Clone)", "SeaTruckSleeperModule(Clone)", "Jukebox(Clone)" }; // special cases to show crosshair
        internal static string techType;
        internal static bool textHand;
        internal static int hideCount;
        //internal static int logTime = DateTime.Now.Second;
        //internal static int hideCountOld;

        [HarmonyPatch(typeof(HandReticle), "UpdateText")]
        [HarmonyPrefix]
        public static void SetTextRaw_Prefix(string ___textHand, string ___textHandSubscript, ref int ___hideCount)
        {
            textHand = !string.IsNullOrEmpty(___textHand + ___textHandSubscript);
            hideCount = ___hideCount;
        }

        [HarmonyPatch(typeof(GUIHand), "OnUpdate")]
        [HarmonyPostfix]
        public static void OnUpdate_Postfix(GUIHand __instance)
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
            bool targetNeedsCrosshair = (__instance.GetActiveTarget() && playerMode == Player.Mode.Normal && !Player.main.cinematicModeActive) || 
                textHand || 
                (Player.main.IsInsideWalkable() && Array.Exists(showCrosshairWhilePointingAt, element => element == techType));

            //if (DateTime.Now.Second != logTime)
            //{
            //File.AppendAllText("DisableCrosshair_Log.txt",
            //    "\n targetNeedsCrosshair: " + targetNeedsCrosshair +
            //    "\n GetActiveTarget: " + __instance.GetActiveTarget() +
            //    "\n textHand: " + textHand +
            //    "\n playerMode: " + playerMode +
            //    //"\n grabMode: " + ___grabMode +
            //    "\n hideCount: " + hideCount +
            //    "\n crosshairIsOff: " + crosshairIsOff +
            //    "\n targetNeedsCrosshair: " + targetNeedsCrosshair +
            //    "\n current crosshair type: " + HandReticle.main.CurrentIconType + 
            //    "\n" + DateTime.Now +
            //    "\n____________________________________");
            //    logTime = DateTime.Now.Second;
            //}
            if (hideCount > 0)
            { 
                if (((!CrosshairOptions.NoCrosshairOnFoot || targetNeedsCrosshair) && isNormalOrSitting) || 
                   ((!CrosshairOptions.NoCrosshairInPrawnSuit || targetNeedsCrosshair) && Player.main.inExosuit) ||
                   (!CrosshairOptions.NoCrosshairInSeatruck && Player.main.inSeatruckPilotingChair))
                {
                    //HandReticle.main.UnrequestCrosshairHide();
                    //if (hideCount >= hideCountOld) // for compatibility with SlotExtenderZero
                    //{
                    //    HandReticle.main.UnrequestCrosshairHide();
                    //}
                    //hideCountOld = hideCount;
                    ShouldHideCrosshair = false;
                    return;
                }
                else return;
            }

            else // Crosshair is currently on
            {
                if ((CrosshairOptions.NoCrosshairOnFoot && isNormalOrSitting && !targetNeedsCrosshair) ||
                   (CrosshairOptions.NoCrosshairInSeatruck && Player.main.inSeatruckPilotingChair) ||
                   (CrosshairOptions.NoCrosshairInPrawnSuit && Player.main.inExosuit && !targetNeedsCrosshair))
                {
                    //HandReticle.main.RequestCrosshairHide();
                    //if (hideCount <= hideCountOld) // for compatibility with SlotExtenderZero
                    //{
                    //    HandReticle.main.RequestCrosshairHide();
                    //}
                    //hideCountOld = hideCount;
                    ShouldHideCrosshair = true;
                    return;
                }
                else return;
            }
        }
        [HarmonyPatch(typeof(HandReticle), nameof(HandReticle.UpdateText))]
        [HarmonyPostfix]
        public static void SetHideCount(ref int ___hideCount)
        {
            if (ShouldHideCrosshair)
            {
                ___hideCount = 1;
            }
            else
            {
                ___hideCount = 0;
            }
        }
    }
}
