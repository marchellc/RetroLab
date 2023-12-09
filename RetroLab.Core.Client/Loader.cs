using Common.Logging;

using HarmonyLib;

using System;

namespace RetroLab
{
    public static class Loader
    {
        public static LogOutput Log;
        public static Harmony Harmony;

        public static void Load()
        {
            LogOutput.Common.AddLogger(new Logger(true));

            Log = new LogOutput("RetroLab.Loader");
            Log.Setup();

            Utils.Log = new LogOutput("RetroLab.Utils");
            Utils.Log.Setup();

            Log.Info("Hello! Initializing Harmony ..");

            try
            {
                Harmony = new Harmony("com.retrolab.client");
                Harmony.PatchAll();
            }
            catch (Exception ex)
            {
                Log.Error($"Harmony failed to initialize!\n{ex}");
                return;
            }

            Log.Info("Loading config ..");

            try
            {
                Config.Load();
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to load the config file!\n{ex}");
                return;
            }

            Log.Info("Config loaded!");
        }
    }
}