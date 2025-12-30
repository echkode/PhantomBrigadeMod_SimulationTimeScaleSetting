// Copyright (c) 2025 EchKode
// SPDX-License-Identifier: BSD-3-Clause

using System.Collections.Generic;

using HarmonyLib;

using PhantomBrigade.Data;

namespace EchKode.PBMods.SimulationTimeScaleSetting
{
    public static class GameSettings
    {
        public static void AddDelegates()
        {
            var implementationsFieldInfo = AccessTools.DeclaredField(typeof(SettingUtility), "implementations");
            if (implementationsFieldInfo == null)
            {
                return;
            }
            var initializedFieldInfo = AccessTools.DeclaredField(typeof(SettingUtility), "initialized");
            if (initializedFieldInfo == null)
            {
                return;
            }
            if (!(bool)initializedFieldInfo.GetValue(null))
            {
                SettingUtility.Initialize();
            }

            var implementations = (Dictionary<string, SettingImplementationDelegate>)implementationsFieldInfo.GetValue(null);
            if (implementations == null)
            {
                return;
            }
            implementations[keySimulationFullSpeedScaleFactor] = SimulationTimeScaleFull;
            implementations[keySimulationSlowSpeedScaleFactor] = SimulationTimeScaleSlow;
        }

        static void SimulationTimeScaleFull(DataContainerGameSetting definition, string valueRaw)
        {
            if (!SettingUtility.TryParseFloat(valueRaw, out var scale))
            {
                return;
            }
            DataShortcuts.sim.timeScaleMain = scale;
        }

        static void SimulationTimeScaleSlow(DataContainerGameSetting definition, string valueRaw)
        {
            if (!SettingUtility.TryParseFloat(valueRaw, out var scale))
            {
                return;
            }
            DataShortcuts.sim.timeScaleSlow = scale;
        }

        // This is the key of the entry in the game settings config database.
        public const string keySimulationFullSpeedScaleFactor = "game_combat_simulation_time_scale_full";
        public const string keySimulationSlowSpeedScaleFactor = "game_combat_simulation_time_scale_slow";
    }
}
