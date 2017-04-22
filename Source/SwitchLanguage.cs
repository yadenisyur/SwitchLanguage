/*
 * Switch Language
 * Copyright © 2017, Arne Peirs (Olympic1)
 * 
 * Kerbal Space Program is Copyright © 2011-2017 Squad. See http://kerbalspaceprogram.com/.
 * This project is in no way associated with nor endorsed by Squad.
 * 
 * Licensed under the terms of the MIT License.
 * See https://opensource.org/licenses/MIT for full details.
 */

using System.IO;
using KSP.Localization;
using UnityEngine;

namespace SwitchLanguage
{
    //[KSPAddon(KSPAddon.Startup.PSystemSpawn, true)]
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class SwitchLanguage : MonoBehaviour
    {
        private static SwitchLanguage instance;
        private static PopupDialog menu;
        private static readonly string filePath = KSPUtil.ApplicationRootPath + "/GameData/SwitchLanguage/settings.cfg";
        //private static readonly string cachePath = KSPUtil.ApplicationRootPath + "/GameData/ModuleManager.ConfigCache";

        private void Start()
        {
            DontDestroyOnLoad(this);
            Init();
        }

        private static SwitchLanguage Init()
        {
            if (instance != null)
                return instance;
            instance = new SwitchLanguage();

            string languageFromFile = GetLanguageFromFile();

            //// Is there a ModuleManager cache
            //if (File.Exists(cachePath))
            //{
            //    string languageFromCache = GetLanguageFromCache();
            //    if (languageFromFile != languageFromCache)
            //    {
            //        Debug.Log("[SL] Language was changed by Module Manager");
            //        languageFromFile = languageFromCache;
            //    }
            //}

            Debug.Log("[SL] Switching language to: " + languageFromFile);
            Localization.instance.SwitchToLanguage(languageFromFile);
            return instance;
        }

        private static string GetLanguageFromFile()
        {
            string language = Localization.instance.CurrentLanguage;
            if (File.Exists(filePath))
            {
                foreach (string readAllLine in File.ReadAllLines(filePath))
                {
                    if (readAllLine.Contains("language") && readAllLine.Contains("="))
                    {
                        string[] strArray = readAllLine.Split('=');
                        if (strArray.Length == 2)
                            language = strArray[1].Trim();
                    }
                }
            }
            else
            {
                Dialog("SwitchLanguageMsg", "Switch Language",
                    $"Could not find settings file! Setting language to {Localization.instance.CurrentLanguage}.\n\rPlease download SwitchLanguage again!");
            }
            return language;
        }

        //private static string GetLanguageFromCache()
        //{
        //    string language = Localization.instance.CurrentLanguage;
        //    foreach (ConfigNode node in ConfigNode.Load(cachePath).GetNodes("UrlConfig"))
        //    {
        //        if (node.GetValue("name").Contains("SwitchLanguage"))
        //        {
        //            if (node.HasNode("SwitchLanguage"))
        //            {
        //                ConfigNode settings = node.GetNode("SwitchLanguage");
        //                language = settings.HasValue("language") ? settings.GetValue("language").Trim() : language;
        //            }
        //        }
        //    }
        //    return language;
        //}

        private static void Dialog(string name, string title, string message)
        {
            Debug.Log($"[SL] {message}");

            if (menu == null)
            {
                menu = PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), name, title,
                    message, "Ok", true, HighLogic.UISkin);
            }
            else
            {
                menu.Dismiss();
                menu = null;
            }
        }
    }
}
