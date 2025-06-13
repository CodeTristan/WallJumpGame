#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace WingmanInspector {

    public class Settings : EditorWindow {

        public static Action OnSettingsChanged;
        
        public static bool HideToolbar { get; private set; }
        public static bool HideCopyPaste { get; private set; }
        public static bool TransOnlyDisable { get; private set; }
        public static bool TransOnlyKeepCopyPaste { get; private set; }
        public static int MaxNumberOfRows { get; private set; } = 3;

        private const string HideToolbarKey = "WingmanHideToolBar"; 
        private const string HideCopyPasteKey = "WingmanHideCopyPaste"; 
        private const string TransOnlyDisableKey = "WingmanTransformDisable"; 
        private const string TransOnlyKeepCopyPasteKey = "WingmanTransformKeepCopyPaste"; 
        private const string MaxNumberOfRowsKey = "WingmanNumberOfRows";

        private const string MaxRowsSettingName = "Max number of component rows"; 
        private const string HideToolbarSettingName = "Hide toolbar"; 
        private const string HideCpSettingName = "Hide copy & paste buttons"; 
        private const string HideWingmanSettingName = "Hide Wingman entirely";
        private const string OnlyCpSettingName = "Only show copy & paste buttons";

        public static void Load() {
            HideToolbar = EditorPrefs.GetBool(HideToolbarKey, false);
            HideCopyPaste = EditorPrefs.GetBool(HideCopyPasteKey, false);
            TransOnlyDisable = EditorPrefs.GetBool(TransOnlyDisableKey, false);
            TransOnlyKeepCopyPaste = EditorPrefs.GetBool(TransOnlyKeepCopyPasteKey, false);
            MaxNumberOfRows = EditorPrefs.GetInt(MaxNumberOfRowsKey, 3);
        }
        
        public static void ShowWindow() {
            Settings window = GetWindow<Settings>("Wingman Settings");
            window.ShowUtility();
        }

        private void OnGUI() {
            EditorGUI.BeginChangeCheck();
            
            GUILayout.Label("Display", EditorStyles.largeLabel);
            {
                float previousLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 200;
                MaxNumberOfRows = EditorGUILayout.IntSlider(MaxRowsSettingName, MaxNumberOfRows, 1, 10);
                EditorGUIUtility.labelWidth = previousLabelWidth;
                
                HideToolbar = GUILayout.Toggle(HideToolbar, HideToolbarSettingName);
                
                EditorGUI.BeginDisabledGroup(HideToolbar);
                HideCopyPaste = GUILayout.Toggle(HideToolbar ? false : HideCopyPaste, HideCpSettingName);
                EditorGUI.EndDisabledGroup();
                
                if (HideToolbar || HideCopyPaste) {
                    GUILayout.Space(10);
                    EditorGUILayout.HelpBox("Copy & paste is still available via the context menu", MessageType.Info);
                }
            }
            
            GUILayout.Space(10);
            
            GUILayout.Label("When Transform Only", EditorStyles.largeLabel);
            {
                TransOnlyDisable = GUILayout.Toggle(TransOnlyDisable, HideWingmanSettingName);
                
                EditorGUI.BeginDisabledGroup(TransOnlyDisable);
                TransOnlyKeepCopyPaste = GUILayout.Toggle(TransOnlyDisable ? false : TransOnlyKeepCopyPaste, OnlyCpSettingName);
                EditorGUI.EndDisabledGroup();
                
                if (TransOnlyKeepCopyPaste) {
                    GUILayout.Space(10);
                    EditorGUILayout.HelpBox($"'{OnlyCpSettingName}' overrides '{HideCpSettingName}' when active.", MessageType.Info);
                }
            }
            
            if (EditorGUI.EndChangeCheck()) {
                EditorPrefs.SetBool(HideToolbarKey, HideToolbar);
                EditorPrefs.SetBool(HideCopyPasteKey, HideCopyPaste);
                EditorPrefs.SetBool(TransOnlyDisableKey, TransOnlyDisable);
                EditorPrefs.SetBool(TransOnlyKeepCopyPasteKey, TransOnlyKeepCopyPaste);
                EditorPrefs.SetInt(MaxNumberOfRowsKey, MaxNumberOfRows);
                OnSettingsChanged?.Invoke();
            }
        }

    }
    
}

#endif