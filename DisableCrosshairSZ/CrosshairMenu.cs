using System.Reflection;
using HarmonyLib;

//namespace DisableCrosshairSZ
public static class CrosshairMenu
{
        private static Harmony _harmony;
        private static int _tabIndex;

        public static void Patch()
        {
            _harmony = new Harmony("com.berbac.subnautica.disablecrosshair.mod");
            _harmony.Patch(AccessTools.Method(typeof(uGUI_OptionsPanel), "AddGeneralTab", null, null), null, new HarmonyMethod(typeof(CrosshairMenu).GetMethod("AddGerneralTab_Postfix")), null);
            _harmony.Patch(AccessTools.Method(typeof(uGUI_TabbedControlsPanel), "AddTab", null, null), null, new HarmonyMethod(typeof(CrosshairMenu).GetMethod("AddTab_Postfix")), null);
            _harmony.Patch(AccessTools.Method(typeof(GameSettings), "SerializeCrosshaairSettings", null, null), null, new HarmonyMethod(typeof(CrosshairMenu).GetMethod("SerializeCrosshaairSettings_Postfix")), null);
    }


    public static void AddGerneralTab_Postfix(uGUI_OptionsPanel __instance)
    {
        __instance.AddHeading(_tabIndex, "Crosshair");
        __instance.AddToggleOption(_tabIndex, "No crosshair while piloting seatruck", CrosshairOptions.NoCrosshairInSeatruck, (bool v) => CrosshairOptions.NoCrosshairInSeatruck = v);
    }

    public static void AddTab_Postfix(int __result, string label)
    {
        if (label.Equals("General"))
            _tabIndex = __result;
    }
    public static void SerializeCrosshaairSettings_Postfix(GameSettings.ISerializer serializer)
    {
        CrosshairOptions.NoCrosshairInSeatruck = serializer.Serialize("NoCrosshairInSeatruck", CrosshairOptions.NoCrosshairInSeatruck);
    }
}
