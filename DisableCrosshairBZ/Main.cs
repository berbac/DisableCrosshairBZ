using HarmonyLib;
using QModManager.API.ModLoading;
using System.Reflection;

namespace DisableCrosshairBZ
{
    [QModCore]
    public static class Loader
    {
        [QModPatch]
        public static void Initialize()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "DisableCrosshairBZ");
            CrosshairMenu.Patch();
        }

    }
}
