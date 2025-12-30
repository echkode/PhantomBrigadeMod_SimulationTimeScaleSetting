// Copyright (c) 2025 EchKode
// SPDX-License-Identifier: BSD-3-Clause

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;
using UnityEngine;

using PhantomBrigade.Data;

namespace EchKode.PBMods.SimulationTimeScaleSetting
{
    [HarmonyPatch]
    static class Patch
    {
        [HarmonyPatch(typeof(SettingUtility), nameof(SettingUtility.LoadData))]
        [HarmonyPostfix]
        static void Su_LoadDataPostfix()
        {
            var selections = SettingUtility.GetSelections();
            var slow = DataMultiLinkerGameSetting.data[GameSettings.keySimulationSlowSpeedScaleFactor];
            slow.valueMax = selections[GameSettings.keySimulationFullSpeedScaleFactor];
        }

        [HarmonyPatch(typeof(CIViewPauseOptions), nameof(CIViewPauseOptions.OnSettingSlider))]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Civpo_OnSettingSliderTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            // Replace the hard-coded format string with a custom one for the new settings to preserve the necessary precision.

            var callMatch = new CodeMatch(CodeInstruction.Call(typeof(CIViewPauseOptions), "OnValueChanged"));
            var loadStrMatch = new CodeMatch(OpCodes.Ldstr);
            var call = CodeInstruction.Call(typeof(Patch), nameof(Patch.ValueToString));

            var cm = new CodeMatcher(instructions, generator);
            cm.End()
                .MatchStartBackwards(callMatch)
                .Advance(-1);
            var loadEntry = cm.Instruction.Clone();
            cm.End()
                .MatchStartBackwards(loadStrMatch)
                .Advance(-1)
                .InsertAndAdvance(loadEntry)
                .SetOpcodeAndAdvance(OpCodes.Ldloc_S)
                .RemoveInstruction()
                .SetInstruction(call);

            return cm.InstructionEnumeration();
        }

        [HarmonyPatch(typeof(CIViewPauseOptions), nameof(CIViewPauseOptions.OnSettingSlider))]
        [HarmonyPostfix]
        static void Civpo_OnSettingSliderPostfix(CIViewPauseOptions __instance, string settingKey, float valueFromBar)
        {
            if (settingKey != GameSettings.keySimulationFullSpeedScaleFactor)
            {
                return;
            }
            var slow = DataMultiLinkerGameSetting.data[GameSettings.keySimulationSlowSpeedScaleFactor];
            slow.valueMax = string.Format("{0:F4}", Mathf.Ceil(valueFromBar * 10000f) / 10000f);
            var helpers = Traverse.Create(__instance).Field<Dictionary<string, CIHelperSetting>>("settingInstances").Value;
            var selections = SettingUtility.GetSelections();
            var valueStr = selections[GameSettings.keySimulationSlowSpeedScaleFactor];
            var updateSetting = SettingUtility.TryParseFloat(valueStr, out var slowValue) && valueFromBar < slowValue;
            var hasHelper = helpers.TryGetValue(GameSettings.keySimulationSlowSpeedScaleFactor, out var helper);
            if (hasHelper)
            {
                __instance.SetupHelper(slow, helper, valueStr);
            }
            if (updateSetting)
            {
                __instance.OnSettingSlider(GameSettings.keySimulationSlowSpeedScaleFactor, valueFromBar);
            }
            else if (hasHelper)
            {
                __instance.RedrawValue(GameSettings.keySimulationSlowSpeedScaleFactor, helper);
            }
        }

        public static string ValueToString(DataContainerGameSetting entry, float value)
        {
            var fmt = "F2";
            if (entry.key == GameSettings.keySimulationFullSpeedScaleFactor
                || entry.key == GameSettings.keySimulationSlowSpeedScaleFactor)
            {
                fmt = entry.valueFormat;
            }
            return value.ToString(fmt);
        }
    }
}
