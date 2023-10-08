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


// ILコードのIndexで処理箇所の指定しているので、別のMod、もしくはメインゲーム内でなにかしらの変更があった場合、
// 動作しなくなるおそれがあります。
namespace CustomSkillCap
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInProcess("Mad Games Tycoon 2.exe")]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGuid = "me.Aerin_the_Lion.Mad_Games_Tycoon_2.plugins.CustomSkillCap";
        public const string PluginName = "Custom Skill Cap";
        public const string PluginVersion = "1.0.0.0";

        public static ConfigEntry<bool> CFG_IS_ENABLED { get; private set; }
        public static ConfigEntry<float> CFG_MajorSkillCap { get; private set; }
        public static ConfigEntry<float> CFG_MinorSkillCap { get; private set; }
        public static ConfigEntry<float> CFG_TalentMinorSkillCap { get; private set; }
        public void LoadConfig()
        {
            string textIsEnable = "0. MOD Settings";
            string SkillSet = "1. SkillCap Settings";

            CFG_IS_ENABLED = Config.Bind<bool>(textIsEnable, "Activate the MOD", true, "If you need to enable the mod, toggle it to 'Enabled'");

            CFG_MajorSkillCap = Config.Bind<float>(SkillSet, "Major Skill Cap", 500f, "");
            CFG_MinorSkillCap = Config.Bind<float>(SkillSet, "Minor Skill Cap", 350f, "");
            CFG_TalentMinorSkillCap = Config.Bind<float>(SkillSet, "Talent Perk + Minor Skill Cap", 450f, "");

            Config.SettingChanged += delegate (object sender, SettingChangedEventArgs args){};
        }

        void Awake()
        {
            LoadConfig();
            Harmony.CreateAndPatchAll(typeof(SkillCapMod));

        }

        //void Update()
        //{
            //UpdateCount++;
            //Debug.Log("Update Count : " + UpdateCount);
        //}
    }


}
