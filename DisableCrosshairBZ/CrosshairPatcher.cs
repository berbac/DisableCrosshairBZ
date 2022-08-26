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
        internal static string[] showCrosshairInHere = { "MapRoomFunctionality", "SeaTruckSleeperModule", "Jukebox" };

        //public static TargetType guiHand;
/*        [HarmonyPatch(typeof(InteractionVolume), "GetTargetType")]
        [HarmonyPrefix]
        public static void SetGrabMode_Prefix(EcoTarget __instance)
        {
            guiHand = __instance.type;
            //File.AppendAllText("DisableCrosshairBZLog2.txt", "\n     (I): GUIHand.grabMode val : " + ___grabMode);
        }*/

        [HarmonyPatch(typeof(GUIHand), "OnUpdate")]
        [HarmonyPostfix]
        public static void OnUpdate_Prefix(GUIHand __instance, GameObject ___activeTarget)
        {
            if (Player.main == null) // skip this if no Player.main instance exists
                return;
            

            // check if CH needs to be enabled for interaction
            Targeting.GetTarget(Player.mainObject, 10, out GameObject getTarget, out float _);
            CraftData.GetTechType(getTarget, out var techType);
            var targetNeedsCrosshair = __instance.GetActiveTarget() || Array.Exists(showCrosshairInHere, element => element == techType.ToString().Split('(')[0]);

            File.AppendAllText("DisableCrosshairBZLog.txt", "\n Aiming at : " + techType.ToString().Split('(')[0]);
            if (_crosshairOff)
            {
                if ((targetNeedsCrosshair && !Player.main.inSeatruckPilotingChair && !Player.main.inExosuit))// || crosshairHasText)
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
                if ((targetNeedsCrosshair && !(Player.main.inSeatruckPilotingChair || Player.main.inExosuit)))// || crosshairHasText)
                    return;

                else if (CrosshairMenu.Config.NoCrosshairOnFoot && Player.main.currentMountedVehicle == null)
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
        }
    }
}
