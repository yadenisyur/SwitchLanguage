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
using TMPro;
using UnityEngine;

namespace SwitchLanguage
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class SwitchLanguage : MonoBehaviour
    {
        private static SwitchLanguage instance;
        private static ConfigNode config;
        private static PopupDialog menu;
        private static bool error;
        private static readonly string filePath = $"{KSPUtil.ApplicationRootPath}GameData/SwitchLanguage/Settings.cfg";

        public void Awake()
        {
            if (File.Exists(filePath))
            {
                Init();
            }
            else
            {
                error = true;
                CreateFile();
            }
        }

        public void Start()
        {
            if (error)
                Dialog("SwitchLanguageMsg", "Switch Language",
                    $"Could not find the settings file, creating one...\r\nChange the language in the file and restart KSP.\r\n\r\nSetting language to {Localizer.CurrentLanguage}.");
        }

        private static SwitchLanguage Init()
        {
            if (instance != null)
                return instance;
            instance = new SwitchLanguage();

            if (config == null)
                config = ConfigNode.Load(filePath).GetNode("SwitchLanguage");

            string language = GetLanguageFromFile();
            bool useFont = bool.Parse(config.GetValue("useFont"));

            if (useFont)
            {
                string fontName = config.GetValue("font");
                FontLoader loader = FindObjectOfType<FontLoader>();
                TMP_FontAsset font = loader.LoadedFonts.Find(t => t.name.Equals(fontName));
                loader.AddGameSubFont(language, false, font);

                Debug.Log($"[SwitchLanguage] Setting language to {language}");
                Localizer.SwitchToLanguage(language);

                Debug.Log($"[SwitchLanguage] Setting font to {fontName}");
                loader.SwitchFontLanguage(language);
            }
            else
            {
                Debug.Log($"[SwitchLanguage] Setting language to {language}");
                Localizer.SwitchToLanguage(language);
            }

            return instance;
        }

        private static string GetLanguageFromFile()
        {
            string language = Localizer.CurrentLanguage;
            foreach (string readAllLine in File.ReadAllLines(filePath))
            {
                if (readAllLine.Contains("language") && readAllLine.Contains("="))
                {
                    string[] strArray = readAllLine.Split('=');
                    if (strArray.Length == 2)
                        language = strArray[1].Trim();
                }
            }
            return language;
        }

        private static void Dialog(string name, string title, string message)
        {
            Debug.Log($"[SwitchLanguage] {message}");

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
            node.AddValue("useFont", false);
            node.AddValue("font", string.Empty);
            settings.Save(filePath);
        }
    }
}
