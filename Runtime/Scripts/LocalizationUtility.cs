using TMPro;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace LocalizationPackageExtensions
{
    public static class LocalizationUtility
    {
        /// <summary>
        /// Find a localized string from a table and apply it directly to a TMP component.
        /// This method operates asynchronously.
        /// </summary>
        public static void ApplyLocalizedString(TableReference tableReference, TableEntryReference entryReference, TMP_Text tmp)
        {
            tmp.text = null;
            var async = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(tableReference, entryReference);
            async.Completed += handle => tmp.text = handle.Result;
        }
    }
}