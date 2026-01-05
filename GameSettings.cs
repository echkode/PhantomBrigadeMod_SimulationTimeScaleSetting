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
            implementations[keySimulationTurnEaseInOut] = SimulationTurnEaseInOut;

            var sim = DataShortcuts.sim;
            timeScaleEaseOutTime = sim.timeScaleEaseOutTime;
            timeScaleMinEaseOut = sim.timeScaleMinEaseOut;
            timeScaleEaseOutSpeed = sim.timeScaleEaseOutSpeed;
            timeScaleEaseInSpeed = sim.timeScaleEaseInSpeed;
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

        static void SimulationTurnEaseInOut(DataContainerGameSetting definition, string valueRaw)
        {
            var sim = DataShortcuts.sim;
            if (!SettingUtility.TryParseBool(valueRaw))
            {
                sim.timeScaleEaseOutTime = -1f;
                sim.timeScaleMinEaseOut = -1f;
                sim.timeScaleEaseOutSpeed = 1f;
                sim.timeScaleEaseInSpeed = 10f;
                return;
            }
            sim.timeScaleEaseOutTime = timeScaleEaseOutTime;
            sim.timeScaleMinEaseOut = timeScaleMinEaseOut;
            sim.timeScaleEaseOutSpeed = timeScaleEaseOutSpeed;
            sim.timeScaleEaseInSpeed = timeScaleEaseInSpeed;
        }

        // This is the key of the entry in the game settings config database.
        public const string keySimulationFullSpeedScaleFactor = "game_combat_simulation_time_scale_full";
        public const string keySimulationSlowSpeedScaleFactor = "game_combat_simulation_time_scale_slow";
        const string keySimulationTurnEaseInOut = "game_combat_simulation_turn_ease_inout";

        static float timeScaleEaseOutTime;
        static float timeScaleMinEaseOut;
        static float timeScaleEaseOutSpeed;
        static float timeScaleEaseInSpeed;
    }
}
