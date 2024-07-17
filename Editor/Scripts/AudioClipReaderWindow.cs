using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using LocalizationPackageExtensions;
using UnityEditor;
using UnityEditor.Localization;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace LocalizationPackageExtensionsEditor
{
    internal enum LocaleStyle
    {
        Code,
        Name,
        DisplayName,
        NativeName,
        TwoLetterISOLanguageName,
        ThreeLetterISOLanguageName,
        ThreeLetterWindowsLanguageName
    }
    internal class AudioClipReaderWindow : EditorWindow
    {
        MaskField targetLocaleField;
        TextField format;
        EnumField targetField;
        EnumField localeStyleField;
        Button readAndApplyButton;

        AudioPairTable audioPairTable;
        StringTableCollection stringTableCollection;
        ReadOnlyCollection<Locale> allLocales;
        
        internal static void Open(AudioPairTable audioPairTable)
        {
            var wnd = GetWindow<AudioClipReaderWindow>(true);
            wnd.minSize = new Vector2(400, 500);
            wnd.maxSize = new Vector2(400, 500);
            wnd.ShowUtility();
            wnd.audioPairTable = audioPairTable;
            wnd.stringTableCollection = LocalizationEditorSettings.GetStringTableCollection(audioPairTable.tableCollectionName);
        }

        void OnLostFocus()
        {
            Close();
        }

        void OnEnable()
        {
            allLocales = LocalizationEditorSettings.GetLocales();
        }

        void CreateGUI()
        {
            var formatInfo = new HelpBox("There are 4 keywords.\n\n" +
                                  "1. {key}    : the key of entry.\n" +
                                  "2. {id}       : the id of entry.\n" +
                                  "3. {locale} : the locale of this table.\n" +
                                  "4. {table}  : the name of this table.\n\n" +
                                  "All keywords must be lowercase.", HelpBoxMessageType.Info);
            formatInfo.Q<Label>().style.fontSize = 12;
            rootVisualElement.Add(formatInfo);
            
            format = new TextField("Format");
            format.value = "{key}_{locale}";
            rootVisualElement.Add(format);

            localeStyleField = new EnumField("Locale Name Style", LocaleStyle.Code);
            localeStyleField.tooltip =
                "ex) ko (Locale without Region)\n\n" +
                "Code: ko\n" +
                "Name: ko\n" +
                "DisplayName: Korean\n" +
                "NativeName: 한국어\n" +
                "TwoLatterISOLanguage: ko\n" +
                "ThreeLatterISOLanguage: kor\n" +
                "ThreeLatterWindowsLanguage: KOR\n\n\n" 
                +
                "ex) ko-KR (Locale with Region)\n\n" +
                "Code: ko-KR\n" +
                "Name: ko-KR\n" +
                "DisplayName: Korean (South Korea)\n" +
                "NativeName: 한국어 (대한민국)\n" +
                "TwoLatterISOLanguage: ko\n" +
                "ThreeLatterISOLanguage: kor\n" +
                "ThreeLatterWindowsLanguage: KOR\n\n";;
            rootVisualElement.Add(localeStyleField);
            
            format.RegisterValueChangedCallback(on_format_changed);

            targetField = new EnumField("Target Cells", TargetCell.All);
            rootVisualElement.Add(targetField);


            targetLocaleField = new MaskField("Target Locales", allLocales.Select(x => x.LocaleName).ToList(), -1);
            rootVisualElement.Add(targetLocaleField);
            
            rootVisualElement.Add(new VisualElement(){style = { height = 20}});

            readAndApplyButton = new Button(ReadAndApply);
            readAndApplyButton.text = "Read and Apply";
            readAndApplyButton.style.height = 30;
            rootVisualElement.Add(readAndApplyButton);
            
            return;
            void on_format_changed(ChangeEvent<string> evt)
            {
                localeStyleField.SetEnabled(evt.newValue.Contains("{locale}"));
            }
        }

        void ReadAndApply()
        {
            var targetFolder = EditorUtility.OpenFolderPanel("Select Folder", "Assets", string.Empty);
            if(string.IsNullOrWhiteSpace(targetFolder)) return;

            var relative = targetFolder.Substring(Application.dataPath.Length - "Assets".Length);
            var clips = new HashSet<AudioClip>();
            var guids = AssetDatabase.FindAssets("t:AudioClip", new[] { relative });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                clips.Add(clip);
                Debug.Log($"find clips: {clip.name}");
            }

            var targetLocales = Mask
                .GetSelectedElements(allLocales, targetLocaleField.value)
                .Select(x => x.Identifier).ToList();
            foreach (var table in stringTableCollection.StringTables
                         .Where(x => targetLocales.Contains(x.LocaleIdentifier)))
            {
                foreach (var entry in table.SharedData.Entries)
                {
                    switch ((TargetCell)targetField.value)
                    {
                        case TargetCell.Empty:
                            if(audioPairTable.HasClip(table.TableCollectionName, table.LocaleIdentifier)) continue;
                            break;
                        case TargetCell.Filled:
                            if(!audioPairTable.HasClip(table.TableCollectionName, table.LocaleIdentifier)) continue;
                            break;
                        case TargetCell.All:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    
                    var formatName = format.value
                        .Replace("{key}", entry.Key)
                        .Replace("{id}", entry.Id.ToString());

                    if (format.value.Contains("{locale}"))
                    {
                        switch ((LocaleStyle)localeStyleField.value)
                        {
                            case LocaleStyle.Code:
                                formatName = formatName.Replace("{locale}", table.LocaleIdentifier.Code);
                                break;
                            case LocaleStyle.Name:
                                formatName = formatName.Replace("{locale}", table.LocaleIdentifier.CultureInfo.Name);
                                break;
                            case LocaleStyle.DisplayName:
                                formatName = formatName.Replace("{locale}", table.LocaleIdentifier.CultureInfo.DisplayName);
                                break;
                            case LocaleStyle.NativeName:
                                formatName = formatName.Replace("{locale}", table.LocaleIdentifier.CultureInfo.NativeName);
                                break;
                            case LocaleStyle.TwoLetterISOLanguageName:
                                formatName = formatName.Replace("{locale}", table.LocaleIdentifier.CultureInfo.TwoLetterISOLanguageName);
                                break;
                            case LocaleStyle.ThreeLetterISOLanguageName:
                                formatName = formatName.Replace("{locale}", table.LocaleIdentifier.CultureInfo.TwoLetterISOLanguageName);
                                break;
                            case LocaleStyle.ThreeLetterWindowsLanguageName:
                                formatName = formatName.Replace("{locale}", table.LocaleIdentifier.CultureInfo.ThreeLetterWindowsLanguageName);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    var clip = clips.FirstOrDefault(x => x.name == formatName);
                    if (clip != null)
                    {
                        Debug.Log($"set clip: {table.LocaleIdentifier} | {entry.Key} => {clip.name}");
                        var audioPair = new AudioStringPair(entry.Key, entry.Id, table.LocaleIdentifier, clip);
                        audioPairTable.AddPair(audioPair);
                        EditorUtility.SetDirty(audioPairTable);
                    }
                }
            }
            AssetDatabase.SaveAssetIfDirty(audioPairTable);
            if (HasOpenInstances<AudioPairTableWindow>())
            {
                GetWindow<AudioPairTableWindow>().RefreshTableGUI();
            }
            Close();
        }
    }
}