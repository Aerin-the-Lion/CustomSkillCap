using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CustomSkillCap
{
    public  class SkillCapMod_Menu_NewGameCEO
    {
        //Menu_NewGameCEOのメニュー
        //ステータスバーの表示変更をする関数
        //メジャースキル:100をマックス、マイナースキル:50をマックス、という形で計算しているため、変える必要がある。
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Menu_NewGameCEO))]
        [HarmonyPatch("SetBalken")]
        static IEnumerable<CodeInstruction> SetBalkenArbeitsmarkt_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool found = false;
            var codes = new List<CodeInstruction>(instructions);
            int fillValueIndex = 20;
            int TalentMinorValueIndex = 62;
            int minorValueIndex = 70;

            for (int i = 0; i < codes.Count; i++)
            {
                if (!Main.CFG_IS_ENABLED.Value) { return codes.AsEnumerable(); }

                //上限値は1fのままでOK

                //CFG_MajorSkillCap : MajorSkillに対して、バーの大きさを調整
                if (i == fillValueIndex && codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 0.01f)
                {
                    // 1 / 500
                    codes[i].operand = 1 / Main.CFG_MajorSkillCap.Value;
                    found = true;
                }

                //CFG_TalentMinorSkillCap : MajorSkillに対して、バーの大きさを調整
                if (i == TalentMinorValueIndex && codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 0.6f)
                {
                    // 1 / 500
                    codes[i].operand = Main.CFG_TalentMinorSkillCap.Value / Main.CFG_MajorSkillCap.Value;
                    found = true;
                }

                //CFG_MinorSkillCap : MajorSkillに対して、バーの大きさを調整
                if (i == minorValueIndex && codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 0.5f)
                {
                    // 1 / 500
                    codes[i].operand = Main.CFG_MinorSkillCap.Value / Main.CFG_MajorSkillCap.Value;
                    found = true;
                }
            }
            if (found is false)
                throw new ArgumentException("Cannot find Explained Number!");
            return codes.AsEnumerable();
        }

        //Menu_NewGameCEOのメニュー
        //ステータスバーの表示を色の変更をする関数
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Menu_NewGameCEO))]
        [HarmonyPatch("GetValColor")]
        static IEnumerable<CodeInstruction> GetValColor_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool found = false;
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                if (!Main.CFG_IS_ENABLED.Value) { return codes.AsEnumerable(); }

                if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 30f)
                {
                    codes[i].operand = Main.CFG_MajorSkillCap.Value * 0.01f * 30f;
                    found = true;
                }

                if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 70f)
                {
                    codes[i].operand = Main.CFG_MajorSkillCap.Value * 0.01f * 70f;
                    found = true;
                }

            }
            if (found is false)
                throw new ArgumentException("Cannot find Explained Number!");
            return codes.AsEnumerable();
        }

        //Menu_NewGameCEOのメニュー
        //BUTTON_AddStatsの変更をする関数
        //メジャースキル:100をマックスという形で計算しているため、変える必要がある。
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Menu_NewGameCEO))]
        [HarmonyPatch("BUTTON_AddStats")]
        static IEnumerable<CodeInstruction> BUTTON_AddStats_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool found = false;
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count; i++)
            {
                //Debug.Log(i + "回目 : " + codes[i].opcode);
                if (!Main.CFG_IS_ENABLED.Value) { return codes.AsEnumerable(); }
                if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 100f)
                {
                    codes[i].operand = Main.CFG_MajorSkillCap.Value;
                    found = true;
                }
            }
            if (found is false)
                throw new ArgumentException("Cannot find Explained Number!");
            return codes.AsEnumerable();
        }

        //専門スキル以外のスキルキャップ開放 / あとは、サンドボックス関係
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Menu_NewGameCEO))]
        [HarmonyPatch("GetSkillCap")]
        static IEnumerable<CodeInstruction> GetSkillCap_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool found = false;
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                //Debug.Log(i + "回目 : " + codes[i].opcode);
                if (!Main.CFG_IS_ENABLED.Value) { return codes.AsEnumerable(); }

                //サンドボックスのスキルキャップ開放
                if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 100f)
                {
                    codes[i].operand = Main.CFG_MajorSkillCap.Value;
                    found = true;
                }

                //Talent Perk所有者　専門スキル以外のスキルキャップ開放
                if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 60f)
                {
                    codes[i].operand = Main.CFG_TalentMinorSkillCap.Value;
                    found = true;
                }
                //専門スキル以外のスキルキャップ開放
                if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 50f)
                {
                    codes[i].operand = Main.CFG_MinorSkillCap.Value;
                    found = true;
                }
            }
            if (found is false)
                throw new ArgumentException("Cannot find Explained Number!");
            return codes.AsEnumerable();
        }
    }
}
