using HarmonyLib;
using QModManager.API.ModLoading;

namespace DisableCrosshairBZ
{
    [QModCore]
    public static class Loader
    {
        [QModPatch]
        public static void Initialize()
        {
            Harmony.CreateAndPatchAll(typeof(CrosshairPatcher), "com.berbac.subnauticabz.disablecrosshair.mod");
            //Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "DisableCrosshairBZ");
            CrosshairMenu.Patch();
        }

    }
}
