using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace DisableCrosshairBZ
{
    [HarmonyPatch]
    public static class CrosshairPatcher
    {
        internal static bool _crosshairOff;
        internal static string[] showCrosshairWhilePointingAt = { "MapRoomFunctionality", "SeaTruckSleeperModule", "Jukebox" };

        [HarmonyPatch(typeof(GUIHand), "OnUpdate")]
        [HarmonyPostfix]
        public static void OnUpdate_Postfix(GUIHand __instance)
        {
            if (Player.main == null) // skip block if no Player.main instance exists
                return;

            // check if CH needs to be enabled for interaction
            // try-catch needed -> throws error if no target in range
            string techType;
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

            var activeTarget = __instance.GetActiveTarget();
            bool targetNeedsCrosshair = activeTarget ||
                (Array.Exists(showCrosshairWhilePointingAt, element => element == techType.Split('(')[0]) && Player.main.IsInsideWalkable());

            //File.AppendAllText("DisableCrosshairBZLog.txt", "\n Block läuft!\n_______________________________");

            if (_crosshairOff)
            {
                if (targetNeedsCrosshair && !Player.main.inSeatruckPilotingChair && !Player.main.inExosuit)
                {
                    HandReticle.main.UnrequestCrosshairHide();
                    _crosshairOff = false;
                    return;
                }

                else if (CrosshairMenu.Config.NoCrosshairOnFoot && Player.main.currentMountedVehicle == null)
                    return;

                else if ((!Player.main.inExosuit && !Player.main.inSeatruckPilotingChair) ||
                    (!CrosshairMenu.Config.NoCrosshairInPrawnSuit && Player.main.inExosuit) ||
                    (!CrosshairMenu.Config.NoCrosshairInSeatruck && Player.main.inSeatruckPilotingChair))
                {
                    HandReticle.main.UnrequestCrosshairHide();
                    _crosshairOff = false;
                    return;
                }
                else return;
            }

            else //(!_crosshairOff)
            {
                if (!targetNeedsCrosshair || Player.main.inSeatruckPilotingChair || Player.main.inExosuit)
                {
                    if (CrosshairMenu.Config.NoCrosshairOnFoot && Player.main.currentMountedVehicle == null)
                    {
                        HandReticle.main.RequestCrosshairHide();
                        _crosshairOff = true;
                        return;
                    }

                    else if ((CrosshairMenu.Config.NoCrosshairInSeatruck && Player.main.inSeatruckPilotingChair) ||
                            (CrosshairMenu.Config.NoCrosshairInPrawnSuit && Player.main.inExosuit))
                    {
                        HandReticle.main.RequestCrosshairHide();
                        _crosshairOff = true;
                    }
                }
                else return;
            }
        }
    }
}
