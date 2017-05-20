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
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class SwitchLanguage : MonoBehaviour
    {
        private static SwitchLanguage instance;
        private static PopupDialog menu;
        private static readonly string filePath = KSPUtil.ApplicationRootPath + "/GameData/SwitchLanguage/Settings.cfg";

        private void Start()
        {
            Init();
        }

        private static SwitchLanguage Init()
        {
            if (instance != null)
                return instance;
            instance = new SwitchLanguage();

            string languageFromFile = GetLanguageFromFile();

            Debug.Log("[SL] Switching language to " + languageFromFile);
            Localizer.SwitchToLanguage(languageFromFile);
            return instance;
        }

        private static string GetLanguageFromFile()
        {
            string language = Localizer.CurrentLanguage;
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
                    $"Could not find the settings file, creating one...\r\nChange the language in the file and restart KSP.\r\n\r\nSetting language to {Localizer.CurrentLanguage}.");

                CreateFile();
            }
            return language;
        }

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

        private static void CreateFile()
        {
            ConfigNode settings = new ConfigNode();
            ConfigNode node = settings.AddNode("SwitchLanguage");
            node.AddValue("language", Localizer.CurrentLanguage);
            settings.Save(filePath);
        }
    }
}
