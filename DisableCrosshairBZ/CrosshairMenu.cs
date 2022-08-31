using HarmonyLib;
using SMLHelper.V2.Json;

namespace DisableCrosshairBZ
{
    public class CrosshairOptions : ConfigFile
    { // these are the default values
        public bool NoCrosshairInSeatruck = false; 
        public bool NoCrosshairInPrawnSuit = false;
        public bool NoCrosshairOnFoot = false;
    }

    public static class CrosshairMenu
    {
        public static CrosshairOptions Config { get; } = new CrosshairOptions();
        private static Harmony _harmony;

        [HarmonyPatch(typeof(uGUI_OptionsPanel), "Update")]
        public static void Patch()
        {
            Config.Load(); // load crosshair config from config.json
            _harmony = new Harmony("com.berbac.subnauticabz.disablecrosshair.mod");
            _harmony.Patch(AccessTools.Method(typeof(uGUI_OptionsPanel), "AddGeneralTab", null, null), null, new HarmonyMethod(typeof(CrosshairMenu).GetMethod("AddGerneralTab_Postfix")), null);
            _harmony.Patch(AccessTools.Method(typeof(GameSettings), "SerializeSettings", null, null), null, new HarmonyMethod(typeof(CrosshairMenu).GetMethod("SerializeSettings_Postfix")), null);
        }

        public static void AddGerneralTab_Postfix(uGUI_OptionsPanel __instance)
        {
            __instance.AddHeading(0, "Hide Crosshair");
            __instance.AddToggleOption(0, "While Piloting Seatruck", Config.NoCrosshairInSeatruck, (bool v) => Config.NoCrosshairInSeatruck = v);
            __instance.AddToggleOption(0, "While in Prawn Suit", Config.NoCrosshairInPrawnSuit, (bool v) => Config.NoCrosshairInPrawnSuit = v);
            __instance.AddToggleOption(0, "While Walking/Swimming", Config.NoCrosshairOnFoot, (bool v) => Config.NoCrosshairOnFoot = v);
        }

        public static void SerializeSettings_Postfix(GameSettings.ISerializer serializer)
        {
            Config.Save(); // save crosshair config to config.json
        }
    }
}
