﻿using Bloxstrap.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

namespace Bloxstrap
{
    public class LaunchSettings
    {
        public LaunchFlag MenuFlag      { get; } = new("preferences,menu,settings");

        public LaunchFlag WatcherFlag   { get; } = new("watcher");

        public LaunchFlag QuietFlag     { get; } = new("quiet");

        public LaunchFlag UninstallFlag { get; } = new("uninstall");

        public LaunchFlag NoLaunchFlag  { get; } = new("nolaunch");

        public LaunchFlag UpgradeFlag   { get; } = new("upgrade");
        
        public LaunchFlag PlayerFlag    { get; } = new("player");
        
        public LaunchFlag StudioFlag    { get; } = new("studio");

        public LaunchMode RobloxLaunchMode { get; private set; } = LaunchMode.None;

        public string RobloxLaunchArgs { get; private set; } = "";

        /// <summary>
        /// Original launch arguments
        /// </summary>
        public string[] Args { get; private set; }

        private Dictionary<string, LaunchFlag> _flagMap = new();

        public LaunchSettings(string[] args)
        {
            const string LOG_IDENT = "LaunchSettings";

            Args = args;

            // build flag map
            foreach (var prop in this.GetType().GetProperties())
            {
                if (prop.PropertyType != typeof(LaunchFlag))
                    continue;

                if (prop.GetValue(this) is not LaunchFlag flag)
                    continue;

                foreach (string identifier in flag.Identifiers.Split(','))
                    _flagMap.Add(identifier, flag);
            }

            // parse
            for (int i = 0; i < Args.Length; i++)
            {
                string arg = Args[i];

                if (!arg.StartsWith('-'))
                    continue;

                string identifier = arg[1..];

                if (_flagMap[identifier] is not LaunchFlag flag)
                    continue;

                flag.Active = true;

                if (i < Args.Length - 1 && Args[i+1] is string nextArg && !nextArg.StartsWith('-'))
                {
                    flag.Data = nextArg;
                    App.Logger.WriteLine(LOG_IDENT, $"Identifier '{identifier}' is active with data");
                }
                else
                {
                    App.Logger.WriteLine(LOG_IDENT, $"Identifier '{identifier}' is active");
                }
            }

            if (PlayerFlag.Active)
                ParsePlayer(PlayerFlag.Data);
            else if (StudioFlag.Active)
                ParseStudio(StudioFlag.Data);
        }

        private void ParsePlayer(string? data)
        {
            RobloxLaunchMode = LaunchMode.Player;

            if (!String.IsNullOrEmpty(data))
                RobloxLaunchArgs = data;
        }

        private void ParseStudio(string? data)
        {
            RobloxLaunchMode = LaunchMode.Studio;

            // TODO: do this later
        }
    }
}
