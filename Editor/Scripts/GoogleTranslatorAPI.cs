using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Networking;

namespace LocalizationPackageExtensionsEditor
{
    public static class GoogleTranslatorAPI
    {
        static string APIKey => LocalizationPackageExtensionsProjectSettings.instance.googleAPIKey;

        public static void Request(Locale sourceLocale, Locale targetLocale, string text, Action<bool, string> result)
        {
            Request(sourceLocale.Identifier.Code, targetLocale.Identifier.Code, text, result);
        }

        public static void Request(string sourceLanguage, string targetLanguage, string text,
            Action<bool, string> result)
        {
            var formData = new List<IMultipartFormSection>
            {
                new MultipartFormDataSection("Content-Type", "application/json; charset=utf-8"),
                new MultipartFormDataSection("source", sourceLanguage),
                new MultipartFormDataSection("target", targetLanguage),
                new MultipartFormDataSection("format", "text"),
                new MultipartFormDataSection("q", text)
            };

            var uri = $"https://translation.googleapis.com/language/translate/v2?key={APIKey}";

            var webRequest = UnityWebRequest.Post(uri, formData);

            var async = webRequest.SendWebRequest();
            async.completed += operation =>
            {
                if (webRequest.result is UnityWebRequest.Result.ProtocolError or UnityWebRequest.Result.ConnectionError)
                {
                    Debug.LogError(webRequest.error);
                    result.Invoke(false, string.Empty);

                    return;
                }

                var data = JObject.Parse(webRequest.downloadHandler.text);
                var translatedText = data["data"]["translations"][0]["translatedText"].ToString();

                result.Invoke(true, translatedText);
            };
        }
    }
}