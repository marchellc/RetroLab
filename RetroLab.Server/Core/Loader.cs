﻿using Common.Logging;
using Common.Reflection;

using RetroLab.Server.Authentification;
using RetroLab.Server.Core;
using RetroLab.Server.Network;
using RetroLab.Server.Servers;

namespace RetroLab
{
    public static class Program
    {
        public static LogOutput Log;

        public static event Action OnExiting;

        public static async Task Main(string[] args)
        {
            Log = new LogOutput("RetroLab.Loader");
            Log.Setup();

            Log.Info($"Welcome! Initializing the central server application ..");

            if (!Args.TryParse(args))
            {
                Exit("failed to parse startup arguments");
                return;
            }

            Log.Info("Initializing directories ..");

            Paths.Load();

            Log.Info("Paths loaded!");

            Commands.Enable();
            AuthTokenLoader.Load();
            ServerListManager.Enable();

            Log.Info("Initializing the network system ..");

            NetworkHandler.Load();

            Log.Info("Finished loading!");

            await Task.Delay(-1);
        }

        public static void Exit(string reason)
        {
            Log?.Error($"Exiting! ({reason})");

            Task.Run(async () =>
            {
                await Task.Delay(3500);

                OnExiting.Call();

                await Task.Delay(1500);

                Log?.Dispose();
                Log = null;

                Environment.Exit(!string.IsNullOrWhiteSpace(reason) ? 1 : 0);
            });
        }
    }
}