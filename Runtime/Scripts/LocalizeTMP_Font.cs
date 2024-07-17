using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

namespace LocalizationPackageExtensions
{
    [Serializable]
    public class LocalizedFont : LocalizedAsset<TMP_FontAsset> { }

    [AddComponentMenu("Localization Extensions/Localize TMP_Font")]
    public class LocalizeTMP_Font : LocalizedAssetBehaviour<TMP_FontAsset, LocalizedFont>
    {
        public TMP_Text tmp;

        void Reset()
        {
            tmp = GetComponent<TMP_Text>();
        }

        protected override void UpdateAsset(TMP_FontAsset localizedFont)
        {
            tmp.font = localizedFont;
        }
    }
}