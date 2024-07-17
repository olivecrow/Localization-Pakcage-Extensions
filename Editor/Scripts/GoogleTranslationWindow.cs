#if LOCALIZATION_PACKAGE_INSTALLED
using System;
using UnityEditor;
using UnityEngine.Localization;

namespace LocalizationPackageExtensionsEditor
{
    public class GoogleTranslationWindow : TranslationWindowBase
    {
        protected override string path_icon => "Assets/Localization Package Extensions/Editor/Icons/Google Translator Icon.png";

        [MenuItem("Window/Asset Management/Localization Package Extensions/Google Translation")]
        static void OpenWindow()
        {
            var wnd = GetWindow<GoogleTranslationWindow>();
            wnd.Show();
        }


        protected override void Open()
        {
            OpenWindow();
        }

        protected override void CreateSpecificGUI()
        {

        }

        protected override void RequestTranslation(Locale targetLocale, string text, Action<bool, string> result)
        {
            GoogleTranslatorAPI.Request(baseLocale, targetLocale, text, result);
        }
    }
}

#endif