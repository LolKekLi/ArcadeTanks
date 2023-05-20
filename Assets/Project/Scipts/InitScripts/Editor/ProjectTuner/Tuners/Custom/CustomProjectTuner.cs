using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Project.Editor.Helpers;
using Project.Editor.Tuner.Custom.Settings;
using UnityEditor;
using UnityEngine;

namespace Project.Editor.Tuner.Custom
{
    public class ProjectSettingsTuner : SettingsTuner
    {
        private bool _isDebugEnabled = false;
        private bool _isInAppEnabled = false;
        private bool _isSayKitEnabled;

        private string _companyName = string.Empty;
        private string _productName = string.Empty;
        private string _packageName = string.Empty;

        private int _selectedStore = 0;

        private readonly Dictionary<int, CompanyType> TargetStoreTypes = new Dictionary<int, CompanyType>();

        private readonly Dictionary<CompanyType, string[]> ManifestRequiredAssets = new Dictionary<CompanyType, string[]>()
        {
            { CompanyType.None, null },
            { CompanyType.Vaveda, null },
        };

        protected override string ApplyButtonName
        {
            get => "Apply Settings";
        }

        public override void Init()
        {
            _companyName = UnityEditor.PlayerSettings.companyName;
            _productName = UnityEditor.PlayerSettings.productName;
            _packageName = Application.identifier;
            
            var stores = (CompanyType[])Enum.GetValues(typeof(CompanyType));
            
            for (int i = 0; i < stores.Length; i++)
            {
                TargetStoreTypes.Add(i, stores[i]);
            }
            
            _isDebugEnabled = DefineHelper.IsDefineEnabled(CustomProjectTunerSettings.DebugMenuDefine);
            _isInAppEnabled = DefineHelper.IsDefineEnabled(CustomProjectTunerSettings.InAppEnableDefine);
            DetectTargetStore(DefineHelper.GetPlatformDefines());

            if (IsVavedaCompanySelected())
            {
                _isSayKitEnabled = DefineHelper.IsDefineEnabled(CustomProjectTunerSettings.SayKitDefine);
            }
        }

        public override void DrawSettings()
        {
            _companyName = EditorGUILayout.TextField("Company Name:", _companyName);
            _productName = EditorGUILayout.TextField("Product Name:", _productName);
            _packageName = EditorGUILayout.TextField("Package Name:", _packageName);
            
            _selectedStore = EditorGUILayout.Popup("Target Store", _selectedStore,
                TargetStoreTypes.Values.Select(s => s.ToString()).ToArray());

            if (IsVavedaCompanySelected())
            {
                _isSayKitEnabled = EditorGUILayout.Toggle("SayKit Enabled: ", _isSayKitEnabled);
            }
 
            _isDebugEnabled = EditorGUILayout.Toggle("Debug Enabled: ", _isDebugEnabled);
            
            _isInAppEnabled = EditorGUILayout.Toggle("In App Enabled: ", _isInAppEnabled);

            base.DrawSettings();
        }

        protected override void ApplySettings()
        {
            UnityEditor.PlayerSettings.companyName = _companyName;
            UnityEditor.PlayerSettings.productName = _productName;
            
            UnityEditor.PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, _packageName);

            var targetStore = TargetStoreTypes[_selectedStore];
            
            SetupDefineSymbols(targetStore);
            VerifyManifest(targetStore);
        }

        private bool IsVavedaCompanySelected()
        {
            return TargetStoreTypes[_selectedStore] == CompanyType.Vaveda;
        }

        private void DetectTargetStore(string[] defines)
        {
            if (defines != null && defines.Length > 0)
            {
                var stores = (CompanyType[])Enum.GetValues(typeof(CompanyType));

                for (int i = 0; i < stores.Length; i++)
                {
                    if (defines.Contains(CompanyHelper.GetDefineByCompany(stores[i])))
                    {
                        _selectedStore = i;

                        break;
                    }
                }
            }
        }

        private void SetupDefineSymbols(CompanyType company)
        {
            var defines = DefineHelper.GetPlatformDefines().ToList();
            
            SetupCompanyDefine(ref defines, company);
            SetupDefine(ref defines, _isDebugEnabled, CustomProjectTunerSettings.DebugMenuDefine);
            SetupDefine(ref defines, _isSayKitEnabled, CustomProjectTunerSettings.SayKitDefine);
            SetupDefine(ref defines, _isInAppEnabled, CustomProjectTunerSettings.InAppEnableDefine);
            
            UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android,
                defines.ToArray());
        }

        private void SetupCompanyDefine(ref List<string> defines, CompanyType company)
        {
            var companyDefines = CompanyHelper.GetCompanyDefines().ToList();

            foreach (var define in companyDefines)
            {
                if (defines.Contains(define))
                {
                    defines.Remove(define);
                }
            }

            defines.Add(CompanyHelper.GetDefineByCompany(company));

            _isSayKitEnabled = _isSayKitEnabled && IsVavedaCompanySelected();
        }

        private void SetupDefine(ref List<string> defines, bool isEnabled, string define)
        {
            if (isEnabled)
            {
                if (!DefineHelper.IsDefineEnabled(define))
                {
                    defines.Add(define);
                }
            }
            else
            {
                defines.Remove(define);
            }
        }

        private void VerifyManifest(CompanyType company)
        {
            var requiredAssets = ManifestRequiredAssets[company];

            if (requiredAssets != null && requiredAssets.Length > 0)
            {
                var cachedRequiredAssets = requiredAssets.ToList(); 

                int removeIndex = Application.dataPath.IndexOf("Assets", StringComparison.Ordinal);
                var path = Application.dataPath.Remove(removeIndex);
                path += "Packages/manifest.json";
                
                if (File.Exists(path))
                {
                    using (StreamReader sr = File.OpenText(path))
                    {
                        string s = string.Empty;
                        
                        while ((s = sr.ReadLine()) != null)
                        {
                            for (int i = 0; i < requiredAssets.Length; i++)
                            {
                                if (s.Contains(requiredAssets[i]))
                                {
                                    cachedRequiredAssets.Remove(requiredAssets[i]);
                                }
                            }
                        }

                        if (cachedRequiredAssets.Count > 0)
                        {
                            Debug.LogException(new Exception($"Not added assets into manifest.json: "));

                            for (int i = 0; i < cachedRequiredAssets.Count; i++)
                            {
                                Debug.LogException(new Exception($"{cachedRequiredAssets[i]}"));
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogException(new Exception($"Not found manifest.json on path - {path}"));
                }
            }
        }
    }
}