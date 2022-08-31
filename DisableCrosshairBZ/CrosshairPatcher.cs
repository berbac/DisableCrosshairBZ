using HarmonyLib;
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
        internal static string[] showCrosshairWhilePointingAt = { "MapRoomFunctionality", "SeaTruckSleeperModule", "Jukebox" }; // special cases to show crosshair
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
        public static void OnUpdate_Postfix(GUIHand __instance)
        {
            GameObject activeTarget = null;
            bool isSittingOrSwimming = new[] { "Normal", "Sitting" }.Contains(Player.main.GetMode().ToString());

            if (Player.main == null)// || (!CrosshairMenu.Config.NoCrosshairOnFoot && crosshairIsOff)) // skip block if no Player.main instance exists
                return;

/*            if (!CrosshairMenu.Config.NoCrosshairOnFoot || crosshairIsOff)
            {
                targetNeedsCrosshair = false;
            }*/

            // getting techType for map room screen, jukebox etc.
            // check if CH needs to be enabled for interaction while on foot/swimming
            // try-catch needed -> throws error if no target in range
            try
            {
                Targeting.GetTarget(Player.main.gameObject, 10, out GameObject getTarget, out float _);
                CraftData.GetTechType(getTarget, out var _techType);
                techType = _techType.ToString();
            }
            catch (NullReferenceException)
            {
                techType = "";
            }
           
            activeTarget = __instance.GetActiveTarget();
            bool targetNeedsCrosshair = activeTarget || textHand || //(isSittingOrSwimming && activeTarget != null)
                (Player.main.IsInsideWalkable() && Array.Exists(showCrosshairWhilePointingAt, element => element == techType.Split('(')[0]));

/*
            File.AppendAllText("DisableCrosshair_Log.txt",
                "\n targetNeedsCrosshair: " + targetNeedsCrosshair +
                "\n GetActiveTarget: " + activeTarget +
                "\n techType: " + techType +
                "\n textHand: " + textHand +
                "\n____________________________________");*/

            if (crosshairIsOff)
            { 
                if //(targetNeedsCrosshair && (isSittingOrSwimming || Player.main.inExosuit) || // ) //!Player.main.inSeatruckPilotingChair && !Player.main.inExosuit)
                   (((!CrosshairMenu.Config.NoCrosshairOnFoot || targetNeedsCrosshair) && isSittingOrSwimming) ||
                   ((!CrosshairMenu.Config.NoCrosshairInPrawnSuit || targetNeedsCrosshair) && Player.main.inExosuit) ||
                   (!CrosshairMenu.Config.NoCrosshairInSeatruck && Player.main.inSeatruckPilotingChair))
                {
                    HandReticle.main.UnrequestCrosshairHide();
                    crosshairIsOff = false;
                    return;
                }

/*
                else if (CrosshairMenu.Config.NoCrosshairOnFoot && isSittingOrSwimming)
                    return;
                else if (targetNeedsCrosshair ||
                        (!CrosshairMenu.Config.NoCrosshairInPrawnSuit && Player.main.inExosuit) ||
                        (!CrosshairMenu.Config.NoCrosshairInSeatruck && Player.main.inSeatruckPilotingChair))
                {
                    HandReticle.main.UnrequestCrosshairHide();
                    crosshairIsOff = false;
                    return;
                }*/
                else return;
            }

            else // Crosshair is currently on
            {
                if //(targetNeedsCrosshair && (isSittingOrSwimming || Player.main.inExosuit) ||
                   ((CrosshairMenu.Config.NoCrosshairOnFoot && isSittingOrSwimming && !targetNeedsCrosshair) ||
                   (CrosshairMenu.Config.NoCrosshairInSeatruck && Player.main.inSeatruckPilotingChair) ||
                   (CrosshairMenu.Config.NoCrosshairInPrawnSuit && Player.main.inExosuit && !targetNeedsCrosshair))
                {
                    HandReticle.main.RequestCrosshairHide();
                    crosshairIsOff = true;
                    return;
                }
/*                if (targetNeedsCrosshair) // && isSittingOrSwimming) //Player.main.inSeatruckPilotingChair || Player.main.inExosuit)
                    return;*/

/*                else if (CrosshairMenu.Config.NoCrosshairOnFoot && isSittingOrSwimming)
                {
                    HandReticle.main.RequestCrosshairHide();
                    crosshairIsOff = true;
                    return;
                }*/
                else return;
            }
        }
    }
}
