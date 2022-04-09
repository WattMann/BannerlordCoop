﻿using System;
using System.Collections.Generic;
using System.Net;
using Coop.Mod.Persistence;
using Network.Infrastructure;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace Coop.Mod.DebugUtil
{
    public static class CLICommands
    {
        private const string sGroupName = "coop";
        private const string sTestGroupName = "test";

        private static DebugUI m_DebugUI;

        [CommandLineFunctionality.CommandLineArgumentFunction("info", sGroupName)]
        public static string DumpInfo(List<string> parameters)
        {
            string sMessage = "";
            sMessage += CoopServer.Instance + Environment.NewLine;
            sMessage += Environment.NewLine + "*** Client ***" + Environment.NewLine;
            sMessage += CoopClient.Instance + Environment.NewLine;
            return sMessage;
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("show_debug_ui", sGroupName)]
        public static string ToggleDebugUI(List<string> parameters)
        {
            if (m_DebugUI == null)
            {
                m_DebugUI = new DebugUI();
                Main.Instance.Updateables.Add(m_DebugUI);
            }

            m_DebugUI.Visible = !m_DebugUI.Visible;

            return "";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("start_local_server", sGroupName)]
        public static string StartServer(List<string> parameters)
        {
            if (CoopServer.Instance.StartServer() == null)
            {
                ServerConfiguration config = CoopServer.Instance.Current.ActiveConfig;
                CoopClient.Instance.Connect(config.NetworkConfiguration.LanAddress, config.NetworkConfiguration.LanPort);
                return CoopServer.Instance.ToString();
            }

            return null;
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("connect_to", sGroupName)]
        public static string ConnectTo(List<string> parameters)
        {
            if (parameters.Count != 2 ||
                !IPAddress.TryParse(parameters[0], out IPAddress ip) ||
                !int.TryParse(parameters[1], out int iPort))
            {
                return $"Usage: \"{sGroupName}.connect_to [IP] [Port]\"." +
                       Environment.NewLine +
                       $"\tExample: \"{sGroupName}.connect_to 127.0.0.1 4201\".";
            }

            CoopClient.Instance.Connect(ip, iPort);
            return "Client connection request sent.";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("disconnect", sGroupName)]
        public static string Disconnect(List<string> parameters)
        {
            CoopClient.Instance.Disconnect();
            return "Client disconnection request sent.";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("help", sGroupName)]
        public static string Help(List<string> parameters)
        {
            return "Coop commands:\n" +
                   "\tcoop.record <filename>\tStart record movements of all parties.\n" +
                   "\tcoop.play <filename>\tPlayback recorded movements of main hero party.\n" +
                   "\tcoop.stop\t\tStop record or playback.";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("record", sGroupName)]
        public static string Record(List<string> parameters)
        {
            if (parameters.Count < 1)
            {
                return Help(null);
            }

            return Replay.StartRecord(parameters[0]);
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("play", sGroupName)]
        public static string Play(List<string> parameters)
        {
            if (parameters.Count < 1)
            {
                return Help(null);
            }

            return Replay.Playback(parameters[0]);
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("stop", sGroupName)]
        public static string Stop(List<string> parameters)
        {
            return Replay.Stop();
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("spawn_party", sTestGroupName)]
        public static string Spawn(List<string> parameters)
        {
            if (parameters.Count != 0 && parameters.Count != 1)
            {
                return $"Usage: \"{sTestGroupName}.spawn_party [spawnDistanceFromMainParty]";
            }
            
            float spawnDistance = 5;
            if(parameters.Count == 1)
            {
                if(!float.TryParse(parameters[0], out float spawnDistanceArg))
                {
                    return $"Usage: \"{sTestGroupName}.spawn_party [spawnDistanceFromMainParty]. Provided argument '{parameters[0]}' is not a valid spawnDistance.";
                }
                spawnDistance = spawnDistanceArg;
            }

            MobileParty party = PartySpawnHelper.SpawnTestersNearby(Campaign.Current.MainParty, spawnDistance);
            return $"Spawned {party}.";
        }
    }
}
