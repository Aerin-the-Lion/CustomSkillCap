using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using HarmonyLib;
using UnityEngine.UI;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Reflection.Emit;

namespace CustomSkillCap
{
    internal class SkillCapMod : MonoBehaviour
    {
        //スキルキャップ開放
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(characterScript))]
        [HarmonyPatch("GetSkillCap_Skill")]
        static IEnumerable<CodeInstruction> GetSkillCap_Skill_Transpiler(IEnumerable<CodeInstruction> instructions)
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
                    //Debug.Log("-------------------------------------");
                    //Debug.Log("GetSkillCap_Skill : Index[" + i + "] is DISABLED!");
                    //Debug.Log("-------------------------------------");
                    found = true;
                }
            }
            if (found is false)
                throw new ArgumentException("Cannot find Explained Number!");
            return codes.AsEnumerable();
        }

        //専門スキル以外のスキルキャップ開放 / あとは、サンドボックス関係
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(characterScript))]
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
                    //Debug.Log("-------------------------------------");
                    //Debug.Log("GetSkillCap_サンドボックスのスキルキャップ開放 : Index[" + i + "] is DISABLED!");
                    //Debug.Log("-------------------------------------");
                    found = true;
                }

                //Talent Perk所有者　専門スキル以外のスキルキャップ開放
                if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 60f)
                {
                    codes[i].operand = Main.CFG_TalentMinorSkillCap.Value;
                    //Debug.Log("-------------------------------------");
                    //Debug.Log("GetSkillCap_Talent Perk所有者 : Index[" + i + "] is DISABLED!");
                    //Debug.Log("-------------------------------------");
                    found = true;
                }
                //専門スキル以外のスキルキャップ開放
                if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 50f)
                {
                    codes[i].operand = Main.CFG_MinorSkillCap.Value;
                    //Debug.Log("-------------------------------------");
                    //Debug.Log("GetSkillCap_専門スキル以外のスキルキャップ開放 : Index[" + i + "] is DISABLED!");
                    //Debug.Log("-------------------------------------");
                    found = true;
                }
            }
            if (found is false)
                throw new ArgumentException("Cannot find Explained Number!");
            return codes.AsEnumerable();
        }

        //学んでいる際のスキルキャップ開放
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(characterScript))]
        [HarmonyPatch("Learn")]
        static IEnumerable<CodeInstruction> Learn_Transpiler(IEnumerable<CodeInstruction> instructions)
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
                    //Debug.Log("-------------------------------------");
                    //Debug.Log("Learn : Index[" + i + "] is DISABLED!");
                    //Debug.Log("-------------------------------------");
                    found = true;
                }
            }
            if (found is false)
                throw new ArgumentException("Cannot find Explained Number!");
            return codes.AsEnumerable();
        }

        //ピックアップ時の従業員のステータスバーの表示を計算する関数
        //メジャースキル:100をマックス、マイナースキル:50をマックス、という形で計算しているため、変える必要がある。
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(GUI_Main))]
        [HarmonyPatch("SetBalkenEmployee")]
        static IEnumerable<CodeInstruction> SetBalkenEmployee_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool found = false;
            var codes = new List<CodeInstruction>(instructions);
            int fillValueIndex = 20;
            int TalentMinorValueIndex = 62;
            int minorValueIndex = 70;

            for (int i = 0; i < codes.Count; i++)
            {
                if (!Main.CFG_IS_ENABLED.Value) { return codes.AsEnumerable(); }

                if (i == fillValueIndex && codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 0.01f)
                {
                    // 1 / 500
                    codes[i].operand = 1 / Main.CFG_MajorSkillCap.Value;
                    //Debug.Log("-------------------------------------");
                    //Debug.Log("SetBalkenEmployee_スキルのステータスバーを調整 : Index[" + i + "] is DISABLED!");
                    //Debug.Log("-------------------------------------");
                    found = true;
                }

                if (i == TalentMinorValueIndex && codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 0.6f)
                {
                    // 1 / 500
                    codes[i].operand = Main.CFG_TalentMinorSkillCap.Value / Main.CFG_MajorSkillCap.Value;
                    //Debug.Log("-------------------------------------");
                    //Debug.Log("SetBalkenEmployee : Index[" + i + "] is DISABLED!");
                    //Debug.Log("-------------------------------------");
                    found = true;
                }

                if (i == minorValueIndex && codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 0.5f)
                {
                    // 1 / 500
                    codes[i].operand = Main.CFG_MinorSkillCap.Value / Main.CFG_MajorSkillCap.Value;
                    //Debug.Log("-------------------------------------");
                    //Debug.Log("SetBalkenEmployee : Index[" + i + "] is DISABLED!");
                    //Debug.Log("-------------------------------------");
                    found = true;
                }
            }
            if (found is false)
                throw new ArgumentException("Cannot find Explained Number!");
            return codes.AsEnumerable();
        }

        //人材マーケットでの従業員のステータスバーの表示を計算する関数
        //メジャースキル:100をマックス、マイナースキル:50をマックス、という形で計算しているため、変える必要がある。
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(GUI_Main))]
        [HarmonyPatch("SetBalkenArbeitsmarkt")]
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

                if (i == fillValueIndex && codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 0.01f)
                {
                    // 1 / 500
                    codes[i].operand = 1 / Main.CFG_MajorSkillCap.Value;
                    //Debug.Log("-------------------------------------");
                    //Debug.Log("SetBalkenArbeitsmarkt_人材マーケットでのスキルのステータスバーを調整 : Index[" + i + "] is DISABLED!");
                    //Debug.Log("-------------------------------------");
                    found = true;
                }

                //CFG_TalentMinorSkillCap : MajorSkillに対して、バーの大きさを調整
                if (i == TalentMinorValueIndex && codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 0.6f)
                {
                    // 1 / 500
                    codes[i].operand =  Main.CFG_TalentMinorSkillCap.Value / Main.CFG_MajorSkillCap.Value;
                    //Debug.Log("-------------------------------------");
                    //Debug.Log("SetBalkenArbeitsmarkt_人材マーケットでのスキルのステータスバーを調整 : Index[" + i + "] is DISABLED!");
                    //Debug.Log("-------------------------------------");
                    found = true;
                }

                //CFG_MinorSkillCap : MajorSkillに対して、バーの大きさを調整
                if (i == minorValueIndex && codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 0.5f)
                {
                    // 1 / 500
                    codes[i].operand =  Main.CFG_MinorSkillCap.Value / Main.CFG_MajorSkillCap.Value;
                    //Debug.Log("-------------------------------------");
                    //Debug.Log("SetBalkenArbeitsmarkt_人材マーケットでのスキルのステータスバーを調整 : Index[" + i + "] is DISABLED!");
                    //Debug.Log("-------------------------------------");
                    found = true;
                }
            }
            if (found is false)
                throw new ArgumentException("Cannot find Explained Number!");
            return codes.AsEnumerable();
        }

        //ステータスバーの表示を色の変更をする関数
        //メジャースキル:100をマックス、マイナースキル:50をマックス、という形で計算しているため、変える必要がある。
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(GUI_Main))]
        [HarmonyPatch("GetValColorEmployee")]
        static IEnumerable<CodeInstruction> GetValColorEmployee_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool found = false;
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                if (!Main.CFG_IS_ENABLED.Value) { return codes.AsEnumerable(); }

                if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 30f)
                {
                    codes[i].operand = Main.CFG_MajorSkillCap.Value * 0.01f * 30f;
                    //Debug.Log("-------------------------------------");
                    //Debug.Log("GetValColorEmployee : Index[" + i + "] is DISABLED!");
                    //Debug.Log("-------------------------------------");
                    found = true;
                }

                if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 70f)
                {
                    codes[i].operand = Main.CFG_MajorSkillCap.Value * 0.01f * 70f;
                    //Debug.Log("-------------------------------------");
                    //Debug.Log("GetValColorEmployee : Index[" + i + "] is DISABLED!");
                    //Debug.Log("-------------------------------------");
                    found = true;
                }

            }
            if (found is false)
                throw new ArgumentException("Cannot find Explained Number!");
            return codes.AsEnumerable();
        }

        //従業員の概要メニュー
        //ステータスバーの表示変更をする関数
        //メジャースキル:100をマックス、マイナースキル:50をマックス、という形で計算しているため、変える必要がある。
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Menu_MitarbeiterUebersicht))]
        [HarmonyPatch("SetBalken")]
        static IEnumerable<CodeInstruction> SetBalken_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool found = false;
            var codes = new List<CodeInstruction>(instructions);
            int fillValueIndex = 20;

            for (int i = 0; i < codes.Count; i++)
            {
                if (i == fillValueIndex && !Main.CFG_IS_ENABLED.Value) { return codes.AsEnumerable(); }

                if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 0.01f)
                {
                    codes[i].operand = 1 / Main.CFG_MajorSkillCap.Value;
                    //Debug.Log("-------------------------------------");
                    //Debug.Log("Menu_MitarbeiterUebersicht.SetBalken : Index[" + i + "] is DISABLED!");
                    //Debug.Log("-------------------------------------");
                    found = true;
                }

            }
            if (found is false)
                throw new ArgumentException("Cannot find Explained Number!");
            return codes.AsEnumerable();
        }

        //従業員の概要メニュー
        //ステータスバーの表示を色の変更をする関数
        //メジャースキル:100をマックス、マイナースキル:50をマックス、という形で計算しているため、変える必要がある。
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Menu_MitarbeiterUebersicht))]
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
                    //Debug.Log("-------------------------------------");
                    //Debug.Log("Menu_MitarbeiterUebersicht.GetValColor : Index[" + i + "] is DISABLED!");
                    //Debug.Log("-------------------------------------");
                    found = true;
                }

                if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 70f)
                {
                    codes[i].operand = Main.CFG_MajorSkillCap.Value * 0.01f * 70f;
                    //Debug.Log("-------------------------------------");
                    //Debug.Log("Menu_MitarbeiterUebersicht.GetValColor : Index[" + i + "] is DISABLED!");
                    //Debug.Log("-------------------------------------");
                    found = true;
                }

            }
            if (found is false)
                throw new ArgumentException("Cannot find Explained Number!");
            return codes.AsEnumerable();
        }


        //従業員管理画面の個人ステータスバー
        //ステータスバーの表示変更をする関数
        //メジャースキル:100をマックス、マイナースキル:50をマックス、という形で計算しているため、変える必要がある。
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Item_Personal_InRoom))]
        [HarmonyPatch("SetData")]
        static IEnumerable<CodeInstruction> Item_Personal_InRoom_SetData_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool found = false;
            var codes = new List<CodeInstruction>(instructions);
            int fillValueIndex = 46;
            int getValColorArgIndex = 55;

            for (int i = 0; i < codes.Count; i++)
            {
                var instruction = codes[i];
                if (!Main.CFG_IS_ENABLED.Value) { yield return instruction; }

                //数値部分を調整
                if (i == fillValueIndex && codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 0.01f)
                {
                    codes[i].operand = 1 / Main.CFG_MajorSkillCap.Value;    //operandだけだったらyield returnは必要ない
                    //Debug.Log("-------------------------------------");
                    //Debug.Log("Item_Personal_InRoom.SetData : Index[" + i + "] is DISABLED!");
                    //Debug.Log("-------------------------------------");
                    found = true;
                }

                //バーの色の判定を調整
                if (i == getValColorArgIndex && codes[i].opcode == OpCodes.Ldarg_2)
                {
                    yield return instruction;
                    yield return new CodeInstruction(OpCodes.Ldc_R4, (1 / Main.CFG_MajorSkillCap.Value));
                    yield return new CodeInstruction(OpCodes.Mul);

                    yield return new CodeInstruction(OpCodes.Ldc_R4, 100f);
                    yield return new CodeInstruction(OpCodes.Mul);
                    //Debug.Log("-------------------------------------");
                    //Debug.Log("Item_Personal_InRoom.SetData : Index[" + i + "] is DISABLED!");
                    //Debug.Log("-------------------------------------");
                    found = true;
                }
                else
                {
                    yield return instruction;
                }
            }
            if (found is false)
                throw new ArgumentException("Cannot find Explained Number!");
        }

        //従業員管理画面の個人ステータスバー
        //ステータスバーの表示変更をする関数
        //メジャースキル:100をマックス、マイナースキル:50をマックス、という形で計算しているため、変える必要がある。
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Item_Arbeitsmarkt))]
        [HarmonyPatch("SetData")]
        static IEnumerable<CodeInstruction> Item_Arbeitsmarkt_SetData_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool found = false;
            var codes = new List<CodeInstruction>(instructions);
            int fillValueIndex = 55;
            int getValColorArgIndex = 64;

            for (int i = 0; i < codes.Count; i++)
            {
                var instruction = codes[i];
                if (!Main.CFG_IS_ENABLED.Value) { yield return instruction; }

                //数値部分を調整
                if (i == fillValueIndex && codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 0.01f)
                {
                    codes[i].operand = 1 / Main.CFG_MajorSkillCap.Value;    //operandだけだったらyield returnは必要ない
                    Debug.Log("-------------------------------------");
                    Debug.Log("Item_Arbeitsmarkt.SetData : Index[" + i + "] is DISABLED!");
                    Debug.Log("-------------------------------------");
                    found = true;
                }

                //バーの色の判定を調整
                if (i == getValColorArgIndex && codes[i].opcode == OpCodes.Ldarg_2)
                {
                    yield return instruction;
                    yield return new CodeInstruction(OpCodes.Ldc_R4, (1 / Main.CFG_MajorSkillCap.Value));
                    yield return new CodeInstruction(OpCodes.Mul);

                    yield return new CodeInstruction(OpCodes.Ldc_R4, 100f);
                    yield return new CodeInstruction(OpCodes.Mul);
                    Debug.Log("-------------------------------------");
                    Debug.Log("Item_Arbeitsmarkt.SetData : Index[" + i + "] is DISABLED!");
                    Debug.Log("-------------------------------------");
                    found = true;
                }
                else
                {
                    yield return instruction;
                }
            }
            if (found is false)
                throw new ArgumentException("Cannot find Explained Number!");
        }

    }
}
