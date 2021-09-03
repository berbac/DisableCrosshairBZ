using HarmonyLib;
using System;
using System.IO;
using UnityEngine;


namespace DisableCrosshairSZ
{
    [HarmonyPatch(typeof(MainCameraControl), "Update")]
    public static class CrosshairPatcher
    {
        private static bool NoCrosshairInSeatruck => CrosshairOptions.NoCrosshairInSeatruck;
        private static bool _crosshairOff;

        [HarmonyPrefix]
        public static bool Prefix()
        {
            File.AppendAllText("Seatruck_Seat_Event.txt", "Acting..." + Environment.NewLine);
            if (NoCrosshairInSeatruck && Player.main.inSeatruckPilotingChair)
            {
                File.AppendAllText("Seatruck_Seat_Event.txt", "Sitting in seatruck, crosshair off" + Environment.NewLine);
                HandReticle.main.RequestCrosshairHide();
                _crosshairOff = true;
                return false;
            }

            else if (NoCrosshairInSeatruck && !_crosshairOff)
            {
                File.AppendAllText("Seatruck_Seat_Event.txt", "Sitting in seatruck, crosshair on" + Environment.NewLine);
                HandReticle.main.UnrequestCrosshairHide();
                _crosshairOff = false;
                return false;
            }
            return true;
        }

    }
}
