using HarmonyLib;

using UnityEngine.SceneManagement;
using UnityEngine;

namespace RetroLab.Patches
{
    [HarmonyPatch(typeof(DebugInfoLoader), nameof(DebugInfoLoader.OnEnable))]
    public static class OnEnablePatch
    {
        public static bool Prefix(DebugInfoLoader __instance)
        {
            __instance.Gpu.text = SystemInfo.graphicsDeviceName;
            __instance.Cpu.text = SystemInfo.processorType;
            __instance.GraphicApi.text = SystemInfo.graphicsDeviceType + " " + SystemInfo.graphicsDeviceVersion;

            __instance.GpuMemory.text = "VRAM: " + SystemInfo.graphicsMemorySize + "MB";
            __instance.ShaderLevel.text = "ShaderLevel " + SystemInfo.graphicsShaderLevel.ToString().Insert(1, ".");
            __instance.Fullscreen.text = "Fullscreen: " + ResolutionManager.Fullscreen;
            __instance.Resolution.text = $"{Screen.width}x{Screen.height}@{Application.targetFrameRate}";
            __instance.CpuThreadsAndFrequency.text = $"Threads: {SystemInfo.processorCount} @ {SystemInfo.processorFrequency} MHz";
            __instance.Ram.text = "RAM: " + SystemInfo.systemMemorySize + "MB";
            __instance.Audio.text = "Audio Supported: " + SystemInfo.supportsAudio;
            __instance.Os.text = SystemInfo.operatingSystem.Replace("  ", " ");
            __instance.Steam.text = $"Discord: {(DiscordClient.IsReady ? $"{DiscordClient.Name} ({DiscordClient.Id})" : "not initialized")}";
            __instance.UnityVersion.text = "Unity " + Application.unityVersion;
            __instance.GameVersion.text = "Version: " + CustomNetworkManager.CompatibleVersions[0];

            var text = Application.buildGUID;

            if (string.IsNullOrEmpty(text))
                text = "Unity Editor";

            __instance.Build.text = "Build: " + text;           
            __instance.CentralServerText.text = __instance._centralserver = $"Central Server: {(CentralClient.IsConnected ? CentralClient.Client.Peer.Target.ToString() : "Not Connected!")}";
            __instance.GameLanguage.text = "Language:" + PlayerPrefs.GetString("translation_path", "English (default)");
            __instance.GameScene.text = "Scene: " + SceneManager.GetActiveScene().name;

            __instance.Errors.text = string.Concat(new object[]
            {
                "Asserts: ",
                    DebugScreenController.Asserts,
                " Errors: ",
                    DebugScreenController.Errors,
                " Exceptions: ",
                    DebugScreenController.Exceptions
            });

            return false;
        }
    }

    [HarmonyPatch(typeof(DebugInfoLoader), nameof(DebugInfoLoader.FixedUpdate))]
    public static class FixedUpdatePatch
    {
        public static bool Prefix(DebugInfoLoader __instance)
        {
            if (CentralClient.Client is null || !CentralClient.IsConnected)
                __instance.CentralServerText.text = __instance._centralserver = $"Central Server: not connected";
            else
                __instance.CentralServerText.text = __instance._centralserver = $"Central Server: {CentralClient.Client.Peer.Target}";

            __instance.Steam.text = $"Discord: {(DiscordClient.IsReady ? $"{DiscordClient.Name} ({DiscordClient.Id})" : "not initialized")}";

            return false;
        }
    }
}
