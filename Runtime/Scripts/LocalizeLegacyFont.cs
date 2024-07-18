using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

namespace LocalizationPackageExtensions
{
    [Serializable]
    public class LocalizedLegacyFont : LocalizedAsset<Font> { }

    [AddComponentMenu("Localization Extensions/Localize Legacy Font")]
    public class LocalizeLegacyFont : LocalizedAssetBehaviour<Font, LocalizedLegacyFont>
    {
        public Text text;
        public TextMesh textMesh;

        void Reset()
        {
            text = GetComponent<Text>();
            textMesh = GetComponent<TextMesh>();
        }

        protected override void UpdateAsset(Font localizedFont)
        {
            if (text)text.font = localizedFont;
            if (textMesh) textMesh.font = localizedFont;
        }
    }
}