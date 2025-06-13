#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace WingmanInspector {

    public static class Wingman {

        private static List<WingmanContainer> containers = new List<WingmanContainer>();
        private static GUIStyle boldLabelStyle;
        private static WingmanPersistentData persistentData;
        
        private static Type inspectorWindowType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
        private static FieldInfo allInspectorsFieldInfo = inspectorWindowType.GetField("m_AllInspectors", BindingFlags.NonPublic | BindingFlags.Static);

        private const string PersistentName = "WingmanPersistentData";
        
        private static bool assetIsDisabled;
        private const string DisableMenuPath = "Tools/Wingman/Disable Wingman";
        private const string SettingsMenuPath = "Tools/Wingman/Settings";
        private const string AssetIsDisabledEditorPref = "Wingman Enable State";
        
        [InitializeOnLoadMethod] 
        private static void Init() {
            assetIsDisabled = EditorPrefs.GetBool(AssetIsDisabledEditorPref, false);
            if (assetIsDisabled) return;
            
            EditorApplication.delayCall += InitAsset;
        }

        [MenuItem(DisableMenuPath)]
        private static void DisableAsset() {
            assetIsDisabled = !assetIsDisabled;
            EditorPrefs.SetBool(AssetIsDisabledEditorPref, assetIsDisabled);

            if (assetIsDisabled) {
                DeInitAsset();
            }
            else {
                InitAsset();
            }
        }
        
        [MenuItem(DisableMenuPath, true)]
        private static bool DisableAssetValidate() {
            Menu.SetChecked(DisableMenuPath, assetIsDisabled);
            return true;
        }

        [MenuItem(SettingsMenuPath)]
        private static void OpenSettings() {
            Settings.ShowWindow(); 
        }

        private static void InitAsset() {
            try {
                boldLabelStyle = new GUIStyle(EditorStyles.boldLabel);
                boldLabelStyle.fontSize = 10;
                
                CheckForPersistent();
                SubscribeToCallbacks();
                
                float searchFieldHeight = EditorStyles.toolbarSearchField.fixedHeight;
                WingmanContainer.BoldLabelStyle = boldLabelStyle;
                WingmanContainer.SearchBarHeight = searchFieldHeight;
                
                WingmanContainer.RightToolBarGuiStyle ??= new GUIStyle(EditorStyles.miniButtonRight) { fixedHeight = searchFieldHeight };
                WingmanContainer.LeftToolBarGuiStyle  ??= new GUIStyle(EditorStyles.miniButtonLeft)  { fixedHeight = searchFieldHeight };
                
                WingmanContainer.CopyToolBarGuiContent  ??= new GUIContent(string.Empty, "Copy selected components to clipboard");
                WingmanContainer.PasteToolBarGuiContent ??= new GUIContent(string.Empty, "Paste clipboard components");

                WingmanContainer.TextureAtlas ??= AssetDatabase.LoadAssetAtPath<Texture>($"{GetAssetLocation()}/WingmanIcons.png");
                WingmanContainer.XIcon        ??= EditorGUIUtility.IconContent("CrossIcon").image;
                
                // Don't optionally assign if null because we need to switch icon when editor theme changes
                WingmanContainer.AllIcon = EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_GridLayoutGroup Icon" : "GridLayoutGroup Icon").image;
                Settings.Load();
            }
            catch {
                EditorApplication.delayCall += InitAsset;
            }
        }

        private static void DeInitAsset() {
            UnSubscribeToCallbacks();
            foreach (WingmanContainer container in containers) {
                container.RemoveGui();
            }
            containers.Clear();
        }

        private static void SubscribeToCallbacks() {
            EditorApplication.update -= RefreshInspectorWindows;
            EditorApplication.update += RefreshInspectorWindows;
            
            EditorApplication.update -= Update;
            EditorApplication.update += Update;
            
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyGUI;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
            
            Selection.selectionChanged -= OnSelectionChanged;
            Selection.selectionChanged += OnSelectionChanged;
            
            EditorApplication.quitting -= OnQuit;
            EditorApplication.quitting += OnQuit;

            Settings.OnSettingsChanged += OnSettingsChanged;
        }

        private static void UnSubscribeToCallbacks() {
            EditorApplication.update -= RefreshInspectorWindows;
            EditorApplication.update -= Update;
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyGUI;
            Selection.selectionChanged -= OnSelectionChanged;
            EditorApplication.quitting -= OnQuit;
            Settings.OnSettingsChanged -= OnSettingsChanged;
        }
        
        private static void RefreshInspectorWindows() {
            IList windows = (IList)allInspectorsFieldInfo.GetValue(inspectorWindowType);
            
            if (windows == null || windows.Count <= 0) {
                containers.Clear();
                return;
            }
            
            // Add new window as a container to the list
            foreach (EditorWindow inspectorWindow in windows) {
                if (!InspectorHasContainer(inspectorWindow)) {
                    containers.Add(new WingmanContainer(inspectorWindow, Selection.activeObject));
                }
            }
            
            // Remove any containers whose windows were closed
            for (int i = containers.Count - 1; i >= 0; i--) {
                if (!containers[i].InspectorWindow) {
                    containers.RemoveAt(i);
                }
            }
        }

        private static bool InspectorHasContainer(EditorWindow inspector) {
            foreach (WingmanContainer container in containers) {
                if (container.InspectorWindow.GetInstanceID() == inspector.GetInstanceID()) {
                    return true;
                }
            }
            return false;
        }
        
        private static void OnSelectionChanged() {
            foreach (WingmanContainer container in containers) {
                if (!container.InspectorIsLocked()) {
                    container.SetContainerSelectionToObject(Selection.activeObject);
                }
                container.Update();
            }
        }

        private static void Update() {
            CheckForPersistent();
            foreach (WingmanContainer container in containers) {
                container.Update();
            }
        }

        private static void OnHierarchyGUI(int instanceID, Rect selectionRect) {
            foreach (WingmanContainer container in containers) {
                container.OnHierarchyGUI();
            }
        }

        private static void CheckForPersistent() {
            if (persistentData) return;
            
            string path = GetAssetLocation();
            string persistentPath = $"{path}/{PersistentName}.asset";

            persistentData = AssetDatabase.LoadAssetAtPath<WingmanPersistentData>(persistentPath);
            
            if (!persistentData) {
                persistentData = ScriptableObject.CreateInstance<WingmanPersistentData>();
                persistentData.name = PersistentName;
                AssetDatabase.CreateAsset(persistentData, persistentPath);
                AssetDatabase.SaveAssets();
            }
            
            WingmanContainer.PersistentData = persistentData;
        }
        
        private static string GetAssetLocation() {
            string[] assetIds = AssetDatabase.FindAssets($"{nameof(Wingman)}");
            foreach (string assetId in assetIds) {
                string filePath = AssetDatabase.GUIDToAssetPath(assetId);
                string fileName = Path.GetFileName(filePath);
                if (fileName == $"{nameof(Wingman)}.cs") {
                    return Path.GetDirectoryName(filePath);
                }
            }
            return string.Empty;
        }

        private static void OnQuit() {
            persistentData?.ClearAllData();
        }

        private static void OnSettingsChanged() {
            foreach (WingmanContainer container in containers) {
                container.RemoveGui();
                container.Update();
                container.InspectorWindow.Repaint();
            }
        }

    }
    
}

#endif