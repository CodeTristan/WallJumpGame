#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace WingmanInspector {

    public class WingmanPersistentData : ScriptableObject {

        public readonly WingmanClipboard Clipboard = new WingmanClipboard();

        [SerializeField] private List<int> indexLookUp = new List<int>();
        [SerializeField] private List<string> searchFields = new List<string>();
        [SerializeField] private List<SelectionData> selectedCompIds = new List<SelectionData>();
        
        [Serializable]
        private class SelectionData {
            public List<int> selectionList = new List<int>();
        }

        public List<int> SelectedCompIds(Object obj) {
            if (GetObjectIndex(obj, out int index)) {
                return selectedCompIds[index].selectionList;
            }
            return null;
        } 
        
        public string SearchString(Object obj) {
            if (GetObjectIndex(obj, out int index)) {
                return searchFields[index];
            }
            return string.Empty;
        }

        public void SetSearchString(Object obj, string str) {
            if (GetObjectIndex(obj, out int index)) {
                searchFields[index] = str;
            }
        }

        public void AddDataForContainer(Object obj) {
            int id = obj.GetInstanceID();

            // BinarySerach returns index if found in list, or negative bitwise compliment of index if not found
            int index = indexLookUp.BinarySearch(id);
            if (index >= 0) return;
            
            index = ~index; // Turn negative bitwise compliment into insertion index 
            indexLookUp.Insert(index, id); 
            selectedCompIds.Insert(index, new SelectionData());
            searchFields.Insert(index, string.Empty);
        }

        public void ClearAllData() {
            indexLookUp.Clear();
            selectedCompIds.Clear();
            searchFields.Clear();
            AssetDatabase.SaveAssetIfDirty(this);
        }
        
        private bool GetObjectIndex(Object obj, out int index) {
            index = indexLookUp.BinarySearch(obj.GetInstanceID());
            return index >= 0;
        }
        
        [CustomEditor(typeof(WingmanPersistentData))]
        private class Editor : UnityEditor.Editor {
            
            public override void OnInspectorGUI() {
                GUIStyle labelStyle = new(EditorStyles.label);
                labelStyle.wordWrap = true;
                
                EditorGUILayout.LabelField(
                    $"Stores persistent data for {nameof(Wingman)} like selected components and search strings.\n\n" +
                    "This data clears every time the editor is restarted.\n\n" +
                    "This file can be safely ignored by version control.", 
                    labelStyle 
                );
            }
            
        }

    }

}
#endif