#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace WingmanInspector {

    public class WingmanContainer {

        public static GUIStyle BoldLabelStyle;
        public static float SearchBarHeight;
        public static WingmanPersistentData PersistentData;
        public static Texture TextureAtlas;
        public static Texture AllIcon;
        public static Texture XIcon;
        
        public static GUIStyle LeftToolBarGuiStyle;
        public static GUIContent CopyToolBarGuiContent;
        
        public static GUIStyle RightToolBarGuiStyle;
        public static GUIContent PasteToolBarGuiContent;

        private const string AllButtonName = "All";
        private const float DragThreshold = 12f;
        private const float MiniMapMargin = 4f;
        private const float SearchCompListSpace = 4f;
        private const float RowHeight = 25f;
        private const float InspectorScrollBarWidth = 12.666666667f;
        private const float ToolBarButtonWidth = 30f;

        private const string InspectorListClassName = "unity-inspector-editors-list";
        private const string InspectorScrolllassName = "unity-inspector-root-scrollview";
        private const string InspectorNoMultiEditClassName = "unity-inspector-no-multi-edit-warning";
        private const string MainWingmanName = "Wingman Main";
        private const string SearchResultsName = "SearchResults";
        
        private static Vector2 iconSize = new Vector2(12, 12);
        private static Vector2 toolBarIconSize = new Vector2(12, 12);
        
        public readonly EditorWindow InspectorWindow;
        
        private Object inspectingObject;
        private VisualElement editorListVisual;
        private IMGUIContainer miniMapGuiContainer;
        private IMGUIContainer pinnedHeaderContainer;
        private IMGUIContainer searchResultsGuiContainer;
        private IMGUIContainer pinnedDividerContainer;
        private ScrollView inspectorScrollView;

        private List<int> selectedCompIds;
        private List<int> validCompIds = new List<int>();
        private List<int> prevValidCompIds = new List<int>();
        private Dictionary<int, Component> compFromIndex = new Dictionary<int, Component>();
        private HashSet<string> noMultiEditVisualElements = new HashSet<string>();
        
        private Vector2 miniMapScrollPos;
        
        private int lastCompCount;
        private int lastRowCount;

        private enum AssetType { NotImportant, HierarchyGameObject,  HierarchyPrefab, HierarchyModel, ProjectPrefab }
        private AssetType inspectingAssetType;
        
        private List<ComponentSearchResults> searchResults = new List<ComponentSearchResults>();
        private const double TimeAfterLastKeyPressToSearch = 0.15;
        private double timeOfLastSearchUpdate;
        private bool performSearchFlag;
        
        private bool inspectorWasLocked;
        private PropertyInfo lockedPropertyInfo;
        
        private bool multiSelectModifier;
        private bool rangeSelectModifier;
        private int rangeModifierPivot;

        private const string DragAndDropKey = "WingmansDragAndDrop";
        private bool isDragging;
        private bool dragHandlerSet;
        private bool canStartDrag;
        private int dragId;
        private Vector2 initialDragMousePos;
        
        public WingmanContainer(EditorWindow window, Object obj) {
            InspectorWindow = window;
            lockedPropertyInfo = window.GetType().GetProperty("isLocked", BindingFlags.Public | BindingFlags.Instance);
            inspectorWasLocked = InspectorIsLocked();
            inspectorScrollView = (ScrollView)InspectorWindow.rootVisualElement.Q(null, InspectorScrolllassName);
            SetContainerSelectionToObject(obj);
        }

        public bool InspectorIsLocked() {
            return (bool)lockedPropertyInfo.GetValue(InspectorWindow);
        }

        public void RemoveGui() {
            if (!InspectingObjectIsValid()) return;

            if (ShowingWingmanGui()) {
                editorListVisual?.RemoveAt(MiniMapIndex());
            }

            if (ShowingSearchResults()) {
                editorListVisual?.RemoveAt(SearchResultsIndex());
            }
        }

        public void SetContainerSelectionToObject(Object obj) {
            inspectingObject = obj;
            
            if (!inspectingObject) {
                inspectingAssetType = AssetType.NotImportant;
                return;
            }
            
            // Figure out what type of asset we are inspecting
            {
                bool isAsset = AssetDatabase.Contains(inspectingObject);
                PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(inspectingObject);
                
                if (isAsset && prefabType is PrefabAssetType.Regular or PrefabAssetType.Variant) {
                    inspectingAssetType = AssetType.ProjectPrefab;
                }
                else if (!isAsset && prefabType is PrefabAssetType.Model) {
                    inspectingAssetType = AssetType.HierarchyModel;
                }
                else if (!isAsset && prefabType is PrefabAssetType.Regular or PrefabAssetType.Variant) {
                    inspectingAssetType = AssetType.HierarchyPrefab;
                }
                else if (!isAsset && prefabType is PrefabAssetType.NotAPrefab) {
                    inspectingAssetType = AssetType.HierarchyGameObject;
                }
                else {
                    inspectingAssetType = AssetType.NotImportant;
                }
            }
            
            searchResults.Clear();
            RefreshNoMultiInspectVisualsSet();
            PersistentData.AddDataForContainer(inspectingObject);
            selectedCompIds = PersistentData.SelectedCompIds(inspectingObject);
            
            if (HasTextInSearchField()) {
                PerformSearch();
                if (!HasSearchResults()) {
                    PersistentData.SetSearchString(inspectingObject, string.Empty);
                }
            }
        }

        public void Update() {
            if (!InspectingObjectIsValid()) return;

            if (Settings.TransOnlyDisable && OnlyHasTransform()) return;

            editorListVisual ??= InspectorWindow.rootVisualElement.Q(null, InspectorListClassName);
            
            if (editorListVisual == null) return;

            if (performSearchFlag && EditorApplication.timeSinceStartup - timeOfLastSearchUpdate > TimeAfterLastKeyPressToSearch) {
                PerformSearch();
                performSearchFlag = false;
                searchResultsGuiContainer?.MarkDirtyRepaint();
            }
            
            if (WasJustUnlocked() && Selection.activeObject != inspectingObject) {
                SetContainerSelectionToObject(Selection.activeObject); 
                UpdateComponentVisibility();
            }

            if (!ShowingWingmanGui() && editorListVisual.childCount > MiniMapIndex()) {
                float miniMapHeight = CalculateMiniMapHeight();
                
                miniMapGuiContainer = new IMGUIContainer();
                miniMapGuiContainer.name = MainWingmanName;
                miniMapGuiContainer.style.width = FullLength();
                miniMapGuiContainer.style.height = miniMapHeight;
                miniMapGuiContainer.style.minHeight = miniMapHeight; 
                miniMapGuiContainer.onGUIHandler = DrawWingmanGui;
                
                Margin(miniMapGuiContainer.style, MiniMapMargin);
                editorListVisual.Insert(MiniMapIndex(), miniMapGuiContainer);
                UpdateComponentVisibility();
            }

            bool showingSearchResults = ShowingSearchResults();
            
            if (!showingSearchResults && HasSearchResults() && editorListVisual.childCount > SearchResultsIndex()) {
                searchResultsGuiContainer = new IMGUIContainer();
                searchResultsGuiContainer.name = SearchResultsName;
                searchResultsGuiContainer.style.width = FullLength();
                searchResultsGuiContainer.style.height = FullLength(); 
                searchResultsGuiContainer.onGUIHandler = DrawSearchResultsGui;
                editorListVisual.Insert(SearchResultsIndex(), searchResultsGuiContainer);
            }
            
            if (showingSearchResults && !HasSearchResults()) {
                RemoveSearchGui();
                ToggleAllComonentVisibility(true);
            }
            
#if UNITY_2021
            Fix2021EditorMargins();
#endif
        }

        public void OnHierarchyGUI() {
            if (DragAndDrop.GetGenericData(DragAndDropKey) is not bool initiatedDrag || !initiatedDrag) return;

            if (Event.current.type == EventType.DragUpdated && !dragHandlerSet) {
                DragAndDrop.AddDropHandler(HierarchyDropHandler);
                dragHandlerSet = true;
                Event.current.Use();
            }

            if (Event.current.type == EventType.DragExited && dragHandlerSet) {
                DragAndDrop.RemoveDropHandler(HierarchyDropHandler);
                dragHandlerSet = false;
                Event.current.Use();
            }
        }

        private void DrawWingmanGui() {
            if (!InspectingObjectIsValid()) return;
            
            Rect reservedRect = GUILayoutUtility.GetRect(0f, 0f, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            bool showCopyPasteOnly = Settings.TransOnlyKeepCopyPaste && OnlyHasTransform();
            if (!Settings.HideToolbar || showCopyPasteOnly) {
                DrawToolBar(reservedRect, showCopyPasteOnly);
                reservedRect = ShiftRectStartVertically(reservedRect, SearchBarHeight + SearchCompListSpace);
            }

            List<Component> comps = GetAllVisibleComponents();
            float[] buttonWidths = GetButtonWidths(comps);
            
            int newCompCount = comps.Count;
            int newRowCount = GetRowCount(reservedRect.width, buttonWidths);
            
            // Create associated component data
            compFromIndex.Clear();
            validCompIds.Clear();
            for (int i = 0; i < comps.Count; i++) {
                compFromIndex.Add(i, comps[i]);
                validCompIds.Add(comps[i].GetInstanceID());
            }

            // Check for resizing the container
            bool resizeRequired = newCompCount != lastCompCount || newRowCount != lastRowCount;
            if (resizeRequired) {
                ResizeGuiContainer();
            }
            
            // Remove component from selection if it was removed from gameobject
            if (newCompCount < lastCompCount) {
                for (int i = selectedCompIds.Count - 1; i >= 0; i--) {
                    if (!validCompIds.Contains(selectedCompIds[i])) {
                        selectedCompIds.RemoveAt(i);
                    }
                }
            }
            
            bool compsGotAdjusted = newCompCount < lastCompCount || !CompareComponentIds(validCompIds, prevValidCompIds);
            
            // Set variables for next method call
            prevValidCompIds.Clear();
            foreach (int validCompId in validCompIds) {
                prevValidCompIds.Add(validCompId);
            }
            lastCompCount = newCompCount;
            lastRowCount = newRowCount;
            
            GetScrollViewDimensions(reservedRect, newRowCount, out Rect innerScrollRect, out Rect outerScrollRect);
            List<Rect> buttonPlacements = GetButtonPlacements(innerScrollRect, comps, buttonWidths);

            if (Event.current.type is EventType.MouseDown && Event.current.button is 1) {
                ShowContextMenu(comps, buttonPlacements);
                Event.current.Use(); // Eat event so right clicking doesn't toggle component
            }
            
            if (showCopyPasteOnly) return;

            // Update Modifiers
            EventModifiers modifiers = Event.current.modifiers;
            multiSelectModifier = modifiers.HasFlag(EventModifiers.Control);
            rangeSelectModifier = modifiers.HasFlag(EventModifiers.Shift);
            
            UpdateDragAndDrop();

            EditorGUI.BeginChangeCheck();
            DrawPreviewScrollView(buttonPlacements, comps, innerScrollRect, outerScrollRect);
            if (EditorGUI.EndChangeCheck() || compsGotAdjusted) {
                UpdateComponentVisibility();
            }
        }

        private void DrawPreviewScrollView(List<Rect> placementRects, List<Component> comps, Rect innerScrollRect, Rect outerScrollRect) {
            miniMapScrollPos = GUI.BeginScrollView(outerScrollRect, miniMapScrollPos, innerScrollRect, GUIStyle.none, GUIStyle.none);
            
            // Handle the All button
            { 
                const int allButtonId = -1;
                bool prevAllButtonToggle = AllIsSelected() && !HasTextInSearchField();
                Rect allButtonRect = placementRects[0];
                
                if (allButtonRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown) {
                    canStartDrag = true;
                    dragId = allButtonId;
                    ClearSearchOnComponentButtonPress();
                }
                
                bool draggingAll = dragId == allButtonId && !prevAllButtonToggle;

                if (DrawToggleButton(allButtonRect, AllIcon, AllButtonName, prevAllButtonToggle, draggingAll)) {
                    selectedCompIds.Clear();
                    rangeModifierPivot = 0;
                }
            }
            
            for (int i = 0; i < comps.Count; i++) {
                Component comp = comps[i];
                Rect buttonRect = placementRects[i + 1];
                int compId = comp.GetInstanceID();
                
                if (buttonRect.Contains(Event.current.mousePosition)) {
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
                        canStartDrag = true;
                        dragId = compId;
                    }
                }
                
                string compName = comp.GetType().Name;
                GUIContent content = EditorGUIUtility.ObjectContent(comp, comp.GetType());
                bool prevToggle = selectedCompIds.Contains(compId);
                bool draggingButton = compId == dragId && !prevToggle;
                
                bool toggled = DrawToggleButton(buttonRect, content.image, compName, prevToggle, draggingButton);
                
                if (toggled && !prevToggle) {
                    OnButtonToggleOn(i);
                    ClearSearchOnComponentButtonPress();
                }
                else if (!toggled && prevToggle) {
                    OnButtonToggleOff(i);
                    ClearSearchOnComponentButtonPress();
                }
            }
            
            GUI.EndScrollView();
        }

        private void GetScrollViewDimensions(Rect reservedRect, int rowCount, out Rect innerScrollRect, out Rect outerScrollRect) {
            innerScrollRect = new Rect(reservedRect) { height = rowCount * RowHeight };
            outerScrollRect = new Rect(reservedRect) { height = RowHeight * Settings.MaxNumberOfRows };
        }

        private List<Rect> GetButtonPlacements(Rect scrollViewRect, List<Component> comps, float[] buttonWidths) {
            List<Rect> placements = new List<Rect>(); 
            
            Rect placementRect = scrollViewRect;
            
            float usableWidth = scrollViewRect.width;
            if (!ShowingVerticalScrollBar()) {
                usableWidth -= InspectorScrollBarWidth;
            }
            
            Rect allButtonRect = new Rect(placementRect.position, new Vector2(buttonWidths[0], RowHeight));
            placements.Add(allButtonRect);
            
            float curWidth = usableWidth;
            curWidth -= buttonWidths[0];
            placementRect.position += new Vector2(buttonWidths[0], 0f);

            for (int i = 0; i < comps.Count; i++) {
                float buttonWidth = buttonWidths[i + 1];
                
                if (curWidth < buttonWidth) {
                    placementRect.position = new Vector2(scrollViewRect.position.x, placementRect.position.y + RowHeight);
                    curWidth = usableWidth;
                }
                curWidth -= buttonWidth;

                Rect buttonRect = new Rect(placementRect.position, new Vector2(buttonWidth, RowHeight));
                placements.Add(buttonRect);

                placementRect.position += new Vector2(buttonWidth, 0f);
            }

            return placements;
        }
        
        private void ClearSearchOnComponentButtonPress() {
            if (HasTextInSearchField()) {
                PersistentData.SetSearchString(inspectingObject, string.Empty);
                searchResults.Clear();
                GUI.changed = true;
                RemoveSearchGui();
                ToggleAllComonentVisibility(true);
            }
        }

        private bool DrawToggleButton(Rect placement, Texture icon, string label, bool toggled, bool beingDragged) {
            if (!toggled && isDragging && beingDragged) {
                toggled = true;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && placement.Contains(Event.current.mousePosition) && Event.current.button == 0) {
                toggled = !toggled;
            }

            int uniqueControlId = GUIUtility.GetControlID(FocusType.Passive);
            GUI.Toggle(placement, uniqueControlId, toggled, GUIContent.none, GUI.skin.button);
            
            Vector2 iconPos = new Vector2(placement.position.x + BoldLabelStyle.margin.right, 0f);
            Rect iconRect = CenterRectVertically(placement, new(iconPos, iconSize));
            GUI.DrawTexture(iconRect, icon);
            
            Vector2 labelSize = BoldLabelStyle.CalcSize(new GUIContent(label));
            Vector2 labelPos = new Vector2(iconRect.xMax, 0f);
            Rect labelRect = new Rect(labelPos, labelSize);
            labelRect = CenterRectVertically(placement, labelRect);
            GUI.Label(labelRect, label, BoldLabelStyle);

            return toggled;
        }
        
        private void OnButtonToggleOn(int compIndex) {
            int compId = ComponentIdFromIndex(compIndex);
            
            if (multiSelectModifier && !rangeSelectModifier) {
                rangeModifierPivot = compIndex;
                selectedCompIds.Add(compId);
                return;
            }
            
            if (rangeSelectModifier) {
                if (AllIsSelected()) {
                    rangeModifierPivot = compIndex;
                    selectedCompIds.Add(compId);
                    return;
                }
                
                AddRangeToSelected(compIndex);
                return;
            }

            selectedCompIds.Clear();
            selectedCompIds.Add(compId);
            rangeModifierPivot = compIndex;
        }
        
        private void OnButtonToggleOff(int compIndex) {
            int compId = ComponentIdFromIndex(compIndex);
            
            if (rangeSelectModifier && selectedCompIds.Count <= 1) return;
            
            if (!multiSelectModifier && !rangeSelectModifier && selectedCompIds.Count > 1) {
                selectedCompIds.Clear();
                selectedCompIds.Add(compId);
                rangeModifierPivot = compIndex;
                return;
            }
            
            if (rangeSelectModifier) {
                if (compIndex == rangeModifierPivot) {
                    selectedCompIds.Clear();
                    selectedCompIds.Add(compId);
                    return;
                }
                
                AddRangeToSelected(compIndex);

                if (compIndex < rangeModifierPivot) {
                    int islandMin = compIndex;
                    while (selectedCompIds.Contains(ComponentIdFromIndex(islandMin - 1))) {
                        islandMin -= 1;
                    }

                    for (int i = islandMin; i < compIndex; i++) {
                        selectedCompIds.Remove(ComponentIdFromIndex(i));
                    }
                }
                else {
                    int islandMax = compIndex;
                    while (selectedCompIds.Contains(ComponentIdFromIndex(islandMax + 1))) {
                        islandMax += 1;
                    }
                    
                    for (int i = compIndex + 1; i <= islandMax; i++) {
                        selectedCompIds.Remove(ComponentIdFromIndex(i));
                    }
                }
                
                return;
            }
            
            selectedCompIds.Remove(compId);
        }
        
        private void AddRangeToSelected(int compIndex) {
            (int min, int max) = rangeModifierPivot < compIndex ? (rangeModifierPivot, compIndex) : (compIndex, rangeModifierPivot);
            for (int i = min; i <= max; i++) {
                int id = ComponentIdFromIndex(i);
                if (!selectedCompIds.Contains(id)) {
                    selectedCompIds.Add(id);
                }
            }
        }
        
        private void DrawToolBar(Rect placementRect, bool showCopyPasteOnly) {
            placementRect.height = SearchBarHeight;
            
            float fullWidth = placementRect.width;
            float xStartPos = placementRect.position.x;
            
            if (!Settings.HideCopyPaste || showCopyPasteOnly) {
                if (DrawToolBarButton(placementRect, true)) {
                    CopySelectedToClipboard();
                }
                placementRect.position += new Vector2(ToolBarButtonWidth, 0f);
                if (DrawToolBarButton(placementRect, false)) {
                    PasteFromClipboard();
                }
                placementRect.position += new Vector2(ToolBarButtonWidth + MiniMapMargin, 0f);
            }
            
            if (showCopyPasteOnly) return;
            
            placementRect.width = fullWidth - (placementRect.position.x - xStartPos);

            const float crossSize = 11;
            const float crossDistFromEndOfSearch = 16;
            Rect crossPlacement = placementRect;
            crossPlacement.width = crossSize;
            crossPlacement.height = crossSize;
            crossPlacement.position = new Vector2(placementRect.xMax - crossDistFromEndOfSearch, placementRect.position.y);
            crossPlacement = CenterRectVertically(placementRect, crossPlacement);
            
            // Handle X input before drawing search field because it eats the input of overlayed elements
            string searchText = PersistentData.SearchString(inspectingObject);
            bool showX = searchText != string.Empty;
            bool pressedX = false;
            if (showX) {
                if (crossPlacement.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseUp) {
                    searchText = string.Empty;
                    searchResults.Clear();
                    pressedX = true;
                }
            }
            
            int prevSearchLen = searchText.Length;
            GUI.SetNextControlName("SearchField");
            searchText = GUI.TextField(placementRect, searchText, EditorStyles.toolbarSearchField);

            // Deselect any selected components when typing in search 
            if (!string.IsNullOrWhiteSpace(searchText)) {
                selectedCompIds.Clear();
            }
            
            // If we click outside of the search bar unfocus it
            if (pressedX || !placementRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown) {
                GUI.FocusControl(null);
                if (string.IsNullOrWhiteSpace(searchText)) {
                    searchText = string.Empty;
                }
            }

            // Draw X after search field so it shows on top
            if (showX) {
                Color prevColor = GUI.color;
                GUI.color = new Vector4(prevColor.r, prevColor.g, prevColor.b, 0.7f);
                GUI.Button(crossPlacement, XIcon, GUIStyle.none);
                GUI.color = prevColor;
            }
            
            if (prevSearchLen != searchText.Length) {
                performSearchFlag = true;
                timeOfLastSearchUpdate = EditorApplication.timeSinceStartup;
            }

            PersistentData.SetSearchString(inspectingObject, searchText);
        }
        
        private bool DrawToolBarButton(Rect placement, bool copy) {
            placement.width = ToolBarButtonWidth;
            
            bool pressed = GUI.Button(placement, copy ? CopyToolBarGuiContent : PasteToolBarGuiContent, copy ? LeftToolBarGuiStyle : RightToolBarGuiStyle);

            Rect iconRect = placement;
            iconRect.size = toolBarIconSize;
            iconRect = CenterRectVertically(placement, iconRect);
            iconRect = CenterRectHorizonally(placement, iconRect);

            if (EditorGUIUtility.isProSkin) {
                Rect uvRect = copy ? new Rect(0f, 0.5f, 0.5f, 0.5f) : new Rect(0f, 0f, 0.5f, 0.5f);
                GUI.DrawTextureWithTexCoords(iconRect, TextureAtlas, uvRect);
            }
            else {
                Rect uvRect = copy ? new Rect(0.5f, 0.5f, 0.5f, 0.5f) : new Rect(0.5f, 0f, 0.5f, 0.5f);
                GUI.DrawTextureWithTexCoords(iconRect, TextureAtlas, uvRect);
            }

            return pressed;
        }
        
        private void UpdateModifiers() {
            EventModifiers modifiers = Event.current.modifiers;
            multiSelectModifier = modifiers.HasFlag(EventModifiers.Control);
            rangeSelectModifier = modifiers.HasFlag(EventModifiers.Shift);
        }
        
        private List<Component> GetComponentsFromSelection() {
            if (!InspectingObjectIsValid()) {
                return null;
            }
            
            List<Component> allComps = GetAllVisibleComponents();
            
            if (AllIsSelected()) {
                return allComps;
            }
            
            List<Component> selComps = new List<Component>(selectedCompIds.Count);
            foreach (int compId in selectedCompIds) {
                selComps.Add(ComponentFromId(compId));
            }
            return selComps;
        }
        
        private class ComponentSearchResults {
            public Component Comp;
            public SerializedObject SerializedComponent;
            public List<SerializedProperty> Fields = new List<SerializedProperty>();
        }
        
        private void PerformSearch() {
            string searchText = PersistentData.SearchString(inspectingObject);
            if (string.IsNullOrWhiteSpace(searchText)) {
                searchResults.Clear();
                return;
            }

            List<Component> comps = GetAllVisibleComponents();
            if (comps == null) return;
            
            searchResults.Clear();
            
            foreach (Component comp in comps) {
                ComponentSearchResults results = null;
                SerializedObject serializedComponent = new SerializedObject(comp);
                List<SerializedProperty> fields = GetComponentFields(serializedComponent);
                
                if (fields == null) continue;
                
                foreach (SerializedProperty field in fields) {
                    if (FuzzyMatch(field.displayName, searchText)) {
                        searchResults ??= new List<ComponentSearchResults>();
                        results ??= new() {
                            Comp = comp, 
                            SerializedComponent = serializedComponent 
                        };
                        results.Fields.Add(field);
                    }
                }

                if (results != null) {
                    searchResults.Add(results);
                }
            }
        }
        
        private bool FuzzyMatch(string stringToSearch, string pattern) {
            const int adjacencyBonus = 5;      
            const int separatorBonus = 10;      
            const int camelBonus = 10;           

            const int leadingLetterPenalty = -3;  
            const int maxLeadingLetterPenalty = -9;
            const int unmatchedLetterPenalty = -1;

            int score = 0;
            int patternIdx = 0;
            int patternLength = pattern.Length;
            int strIdx = 0;
            int strLength = stringToSearch.Length;
            bool prevMatched = false;
            bool prevLower = false;
            bool prevSeparator = true;                   

            char? bestLetter = null;
            char? bestLower = null;
            int bestLetterScore = 0;

            while (strIdx != strLength) {
                char? patternChar = patternIdx != patternLength ? pattern[patternIdx] as char? : null;
                char strChar = stringToSearch[strIdx];

                char? patternLower = patternChar != null ? char.ToLower((char)patternChar) as char? : null;
                char strLower = char.ToLower(strChar);
                char strUpper = char.ToUpper(strChar);

                bool nextMatch = patternChar != null && patternLower == strLower;
                bool rematch = bestLetter != null && bestLower == strLower;

                bool advanced = nextMatch && bestLetter != null;
                bool patternRepeat = bestLetter != null && patternChar != null && bestLower == patternLower;
                if (advanced || patternRepeat) {
                    score += bestLetterScore;
                    bestLetter = null;
                    bestLower = null;
                    bestLetterScore = 0;
                }

                if (nextMatch || rematch) {
                    int newScore = 0;

                    if (patternIdx == 0) {
                        int penalty = Math.Max(strIdx * leadingLetterPenalty, maxLeadingLetterPenalty);
                        score += penalty;
                    }

                    if (prevMatched) {
                        newScore += adjacencyBonus;
                    }

                    if (prevSeparator) {
                        newScore += separatorBonus;
                    }

                    if (prevLower && strChar == strUpper && strLower != strUpper) {
                        newScore += camelBonus;
                    }

                    if (nextMatch) {
                        ++patternIdx;
                    }

                    if (newScore >= bestLetterScore) {
                        if (bestLetter != null) {
                            score += unmatchedLetterPenalty;
                        }

                        bestLetter = strChar;
                        bestLower = char.ToLower((char)bestLetter);
                        bestLetterScore = newScore;
                    }

                    prevMatched = true;
                }
                else {
                    score += unmatchedLetterPenalty;
                    prevMatched = false;
                }

                prevLower = strChar == strLower && strLower != strUpper;
                prevSeparator = strChar == '_' || strChar == ' ';

                ++strIdx;
            }

            if (bestLetter != null) {
                score += bestLetterScore;
            }

            const int idealScore = -10;
            return patternIdx == patternLength && score >= idealScore;
        }

        private DragAndDropVisualMode HierarchyDropHandler(int dropTargetInstanceID, HierarchyDropFlags dropMode, Transform parentForDraggedObjects, bool perform) {
            const int hierarchyId = -1314;
            
            bool copying = dropMode == HierarchyDropFlags.DropUpon && dropTargetInstanceID != hierarchyId;
            bool creating = dropTargetInstanceID == hierarchyId || dropMode == HierarchyDropFlags.DropBetween || dropMode == HierarchyDropFlags.None;

            DragAndDropVisualMode visualMode = DragAndDropVisualMode.None;
            if (copying) {
                visualMode = DragAndDropVisualMode.Copy;
            }
            else if (creating) {
                visualMode = DragAndDropVisualMode.Move;
            }

            if (!perform || (!copying && !creating)) {
                return visualMode;
            }
            
            List<Component> comps = GetComponentsFromSelection();
            if (comps == null) {
                return visualMode;
            }
            
            if (copying && EditorUtility.InstanceIDToObject(dropTargetInstanceID) is GameObject gameObject) {
                GroupUndoAction("Copy Components", () => gameObject.PasteComponents(comps));
                EditorApplication.delayCall += () => Selection.activeObject = gameObject;
                return visualMode;
            }
            
            GroupUndoAction("Create Object from Components", () => {
                GameObject newGameObject = new GameObject("GameObject");
                Undo.RegisterCreatedObjectUndo(newGameObject, string.Empty);
                newGameObject.PasteComponentsFromEmpty(comps);
                EditorApplication.delayCall += () => Selection.activeObject = newGameObject;
            });

            return visualMode;
        }

        private void GroupUndoAction(string undoName, Action action) {
            Undo.IncrementCurrentGroup();
            int curUndoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName(undoName);
            action.Invoke();
            Undo.CollapseUndoOperations(curUndoGroup);
        }
        
        private void UpdateDragAndDrop() {
            bool mouseDragEvent = Event.current.type == EventType.MouseDrag;

            if (!isDragging && canStartDrag && mouseDragEvent) {
                initialDragMousePos = Event.current.mousePosition;
                canStartDrag = false;
                return;
            }

            if (initialDragMousePos != Vector2.zero && mouseDragEvent && Vector2.Distance(initialDragMousePos, Event.current.mousePosition) >= DragThreshold) {
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.SetGenericData(DragAndDropKey, true);
                DragAndDrop.StartDrag(MainWingmanName);
                isDragging = true;
            }
            
            // DragExited is set when we drag out of the container or stop dragging inside it
            if (Event.current.type == EventType.DragExited) {
                canStartDrag = false;
                isDragging = false;
                initialDragMousePos = Vector2.zero;
                Event.current.Use();
            }
        }

        private void CheckForComponentListUpdate(out List<Component> comps, out bool orderOfCompsChanged) {
            comps = GetAllVisibleComponents();
            
            compFromIndex.Clear();
            validCompIds.Clear();
            for (int i = 0; i < comps.Count; i++) {
                compFromIndex.Add(i, comps[i]);
                validCompIds.Add(comps[i].GetInstanceID());
            }
            
            int newCompCount = comps.Count;
            if (newCompCount != lastCompCount) {
                ResizeGuiContainer();
            }

            orderOfCompsChanged = !CompareComponentIds(validCompIds, prevValidCompIds);
            
            if (newCompCount < lastCompCount) {
                for (int i = selectedCompIds.Count - 1; i >= 0; i--) {
                    if (!validCompIds.Contains(selectedCompIds[i])) {
                        selectedCompIds.RemoveAt(i);
                    }
                }
                orderOfCompsChanged = true;
            }
            
            prevValidCompIds.Clear();
            foreach (int validCompId in validCompIds) {
                prevValidCompIds.Add(validCompId);
            }
            
            lastCompCount = newCompCount;
        }

        private bool CompareComponentIds(List<int> list0, List<int> list1) {
            if (list0.Count != list1.Count) {
                return false;
            }

            for (int i = 0; i < list0.Count; i++) {
                if (list0[i] != list1[i]) {
                    return false;
                }
            }

            return true;
        }

        private void ResizeGuiContainer() {
            float height = CalculateMiniMapHeight();
            miniMapGuiContainer.style.height = height; 
            miniMapGuiContainer.style.minHeight = height; 
            miniMapGuiContainer.style.width = FullLength();
        }
        
        private void DrawSearchResultsGui() {
            if (!HasSearchResults() || !InspectingObjectIsValid()) return;

            ToggleAllComonentVisibility(false);
            
            foreach (ComponentSearchResults result in searchResults) {
                
                EditorGUILayout.InspectorTitlebar(true, result.Comp, false);
                
                EditorGUI.indentLevel++;
                foreach (SerializedProperty property in result.Fields) {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(property, true);
                    if (EditorGUI.EndChangeCheck()) {
                        result.SerializedComponent.ApplyModifiedProperties();
                    }
                }
                EditorGUI.indentLevel--;
                
                EditorGUILayout.Space();
            }
        }
        
        private void UpdateComponentVisibility() {
            int startIndex = ComponentStartIndex();
            int skipedCount = 0;
            
            for (int i = startIndex; i < editorListVisual.childCount; i++) {
                if (noMultiEditVisualElements.Contains(editorListVisual[i].name)) {
                    skipedCount++;
                    continue;
                }
                
                int compIndex = i - startIndex - skipedCount;
                if (compFromIndex.TryGetValue(compIndex, out Component comp)) {
                    bool showComp = selectedCompIds.Count <= 0 || selectedCompIds.Contains(comp.GetInstanceID());
                    editorListVisual[i].style.display = showComp ? DisplayStyle.Flex : DisplayStyle.None;
                }
            }
        }

        private void ToggleAllComonentVisibility(bool show) {
            int startIndex = ShowingSearchResults() ? SearchResultsIndex() + 1 : MiniMapIndex() + 1;
            for (int i = startIndex; i < editorListVisual.childCount; i++) {
                editorListVisual[i].style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        private bool ShowingWingmanGui() {
            int insertIndex = MiniMapIndex();

            if (insertIndex >= editorListVisual.childCount) {
                return false;
            }

            VisualElement potentialMiniMap = editorListVisual.hierarchy.ElementAt(insertIndex);
            return potentialMiniMap != null && potentialMiniMap.name == MainWingmanName;
        }

        private bool ShowingSearchResults() {
            int insertIndex = SearchResultsIndex();
            
            if (insertIndex >= editorListVisual.childCount) {
                return false;
            }
            
            VisualElement potentialSearchResults = editorListVisual.hierarchy.ElementAt(insertIndex);
            return potentialSearchResults != null && potentialSearchResults.name == SearchResultsName;
        }
        
        private bool HasSearchResults() {
            return searchResults != null && searchResults.Count > 0;
        }

        private bool OnlyHasTransform() {
#if UNITY_2022 || UNITY_2022_1_OR_NEWER
            return ((GameObject)inspectingObject).GetComponentCount() == 1;
#else
            return ((GameObject)inspectingObject).GetComponents<Component>().Length == 1;
#endif
        }

        private int GetRowCount(float rowWidth, float[] buttonWidths) {
            if (!ShowingVerticalScrollBar()) {
                rowWidth -= InspectorScrollBarWidth;
            }
            
            int rowCount = 1;
            float curWidth = rowWidth;

            foreach (float buttonWidth in buttonWidths) {
                if (curWidth < buttonWidth) {
                    curWidth = rowWidth;
                    rowCount++;
                }
                curWidth -= buttonWidth;
            }

            return rowCount;
        }

        private float[] GetButtonWidths(List<Component> comps) {
            float[] buttonWidths = new float[comps.Count + 1];
            buttonWidths[0] = GetButtonWidth(AllButtonName);
            for (int i = 1; i < buttonWidths.Length; i++) {
                buttonWidths[i] = GetButtonWidth(comps[i - 1].GetType().Name);
            }
            return buttonWidths;
        }
        
        private float GetButtonWidth(string text) {
            float totalPadding = BoldLabelStyle.margin.right * 2f;
            Vector2 guiSize = BoldLabelStyle.CalcSize(new GUIContent(text));
            return iconSize.x + guiSize.x + totalPadding;
        }
        
        private List<SerializedProperty> GetComponentFields(SerializedObject serializedComponent) {
            SerializedProperty iter = serializedComponent.GetIterator();

            if (iter == null || !iter.NextVisible(true)) {
                return null;
            }

            List<SerializedProperty> fields = new List<SerializedProperty>();
            
            do {
                fields.Add(iter.Copy());
            }
            while (iter.NextVisible(false));
            
            return fields;
        }
        
        private Rect CenterRectVertically(Rect parent, Rect child) {
            float yDiff = parent.height - child.height;
            float yPos = parent.position.y + (yDiff / 2f);
            child.position = new Vector2(child.position.x, yPos);
            return child;
        }

        private Rect CenterRectHorizonally(Rect parent, Rect child) {
            float xDiff = parent.width - child.width;
            float xPos = parent.position.x + (xDiff / 2f);
            child.position = new Vector2(xPos, child.position.y);
            return child;
        }
        
        private void Margin(IStyle style, float margin) {
            style.marginTop = margin;
            style.marginBottom = margin;
            style.marginLeft = margin;
            style.marginRight = margin;
        }
        
        private bool ShowingVerticalScrollBar() {
            return inspectorScrollView.verticalScroller.resolvedStyle.display == DisplayStyle.Flex;
        }
        
        private List<Component> GetAllVisibleComponents() {
            if (!InspectingObjectIsValid()) {
                return null;
            }

            GameObject selectedGameObject = inspectingObject as GameObject;
            
            if (Selection.gameObjects.Length == 1) {
                return GetAllVisibleComponents(selectedGameObject);
            }

            { // Get all visible components that each selected object shares
                List<Component> comps = GetAllVisibleComponents(selectedGameObject);

                if (InspectorIsLocked()) {
                    return comps;
                }

                foreach (GameObject otherGameObject in Selection.gameObjects) {
                    if (otherGameObject == selectedGameObject) continue;

                    List<Component> otherComps = GetAllVisibleComponents(otherGameObject);

                    for (int i = comps.Count - 1; i >= 0; i--) {
                        if (!ComponentListContainsType(otherComps, comps[i].GetType())) {
                            comps.RemoveAt(i);
                        }
                    }
                }
                
                return comps;
            }
        }
        
        private bool ComponentListContainsType(List<Component> list, Type componentType) {
            foreach (Component component in list) {
                if (component.GetType() == componentType) {
                    return true;
                }
            }
            return false;
        }

        private List<Component> GetAllVisibleComponents(GameObject gameObject) {
            Component[] comps = gameObject.GetComponents<Component>();
            List<Component> res = new List<Component>(comps.Length);
            foreach (Component comp in comps) {
                if (ComponentIsVisible(comp)) {
                    res.Add(comp);
                }
            }
            return res;
        }

        private bool ComponentIsVisible(Component comp) {
            // Comp can be null if the associated script cannot be loaded
            return comp && !comp.hideFlags.HasFlag(HideFlags.HideInInspector) && !ComponentIsOnBanList(comp);
        }

        private bool ComponentIsOnBanList(Component comp) {
            return comp is ParticleSystemRenderer;
        }

        private int ComponentIdFromIndex(int index) {
            return compFromIndex[index].GetInstanceID();
        }

        private Component ComponentFromId(int compId) {
            int index = 0;
            for (int i = 0; i < validCompIds.Count; i++) {
                if (validCompIds[i] == compId) {
                    index = i;
                }
            }
            return compFromIndex[index];
        }

        private bool AllIsSelected() {
            return selectedCompIds.Count == 0;
        }
        
        private bool WasJustUnlocked() {
            bool currentlyLocked = InspectorIsLocked();
            bool res = inspectorWasLocked && !currentlyLocked;
            inspectorWasLocked = currentlyLocked;
            return res;
        }

        private int MiniMapIndex() {
            return inspectingAssetType is AssetType.ProjectPrefab ? 2 : 1;
        }

        private int SearchResultsIndex() {
            return inspectingAssetType is AssetType.ProjectPrefab ? 3 : 2;
        }

        private int ComponentStartIndex() {
            return inspectingAssetType == AssetType.ProjectPrefab ? 3 : 2;
        }

        private void RemoveSearchGui() {
            if (ShowingSearchResults()) {
                editorListVisual.RemoveAt(SearchResultsIndex());
                searchResultsGuiContainer = null;
            }
        }

        private bool HasTextInSearchField() {
            return !string.IsNullOrWhiteSpace(PersistentData.SearchString(inspectingObject));
        }

        private float CalculateMiniMapHeight() {
            float searchBarAndPadding = SearchBarHeight + SearchCompListSpace;
            
            if (Settings.TransOnlyKeepCopyPaste && OnlyHasTransform()) {
                return SearchBarHeight;
            }
            
            float[] buttonWidths = GetButtonWidths(GetAllVisibleComponents());
            
            // Important! Use editor list width as container width as MiniMap.layout
            // is not always as up to date as it should be (if it were just created).
            // This prevents the container from flickering when changing objects.
            float guiContainerWidth = editorListVisual.layout.width - MiniMapMargin * 2f;
            float rowCount = Mathf.Clamp(GetRowCount(guiContainerWidth, buttonWidths), 1, Settings.MaxNumberOfRows);
            return (rowCount * RowHeight) + (Settings.HideToolbar ? 0f : searchBarAndPadding);
        }
        
        private StyleLength FullLength() {
            return new StyleLength(StyleKeyword.Auto);
        }
        
        private bool InspectingObjectIsValid() {
            return inspectingObject && inspectingObject is GameObject && inspectingAssetType is not AssetType.NotImportant;
        }
        
        // Add all visual elements to the noMultiEditVisualElements set so we know which components are not
        // being displayed in the inspector when multi-inspecting is occurring.
        // During multi-inspecting the editor list may have non-shared (hidden) components inserted as children 
        // that we need to skip over when updating component visibility to not throw off component indexing.
        // Any visual element after no-multi-edit warning tells us what is being hidden in the inspector.
        private void RefreshNoMultiInspectVisualsSet() {
            noMultiEditVisualElements.Clear();

            if (Selection.gameObjects.Length <= 1 || editorListVisual == null) return;
            
            int noMultiEditIndex = editorListVisual.childCount;

            for (int i = 0; i < editorListVisual.childCount; i++) {
                if (editorListVisual[i].ClassListContains(InspectorNoMultiEditClassName)) {
                    noMultiEditIndex = i;
                    break;
                }
            }
                
            for (int i = noMultiEditIndex + 1; i < editorListVisual.childCount; i++) {
                noMultiEditVisualElements.Add(editorListVisual[i].name);
            }
        }

        private void ShowContextMenu(List<Component> comps, List<Rect> buttonRects) {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Copy Selection"), false, CopySelectedToClipboard);
            menu.AddItem(new GUIContent("Paste Clipboard"), false, PasteFromClipboard);
            
            Component compUnderCursor = null;
            for (int i = 1; i < buttonRects.Count; i++) {
                if (buttonRects[i].Contains(Event.current.mousePosition + miniMapScrollPos)) {
                    compUnderCursor = comps[i - 1];
                    break;
                }
            }

            if (compUnderCursor) {
                menu.AddSeparator("");
                string compName = compUnderCursor.GetType().Name;
                
                // Copy component
                menu.AddItem(new GUIContent($"Copy { compName }"), false, () => {
                    PersistentData.Clipboard.CopyComponents(new() { compUnderCursor });
                });
                
                // Open component as script
                if (compUnderCursor is MonoBehaviour) {
                    menu.AddItem(new GUIContent($"Edit { compName } Script"), false, () => {
                        MonoScript script = MonoScript.FromMonoBehaviour(compUnderCursor as MonoBehaviour);
                        if (script) AssetDatabase.OpenAsset(script);
                    });
                }

                // Remove component
                if (compUnderCursor is not Transform) {
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent($"Remove { compName }"), false, () => {
                        RemoveComponentTypeFromSelection(compUnderCursor.GetType());
                    });
                }
            }
            
            menu.ShowAsContext();
        }

        private void RemoveComponentTypeFromSelection(Type compType) {
            GroupUndoAction("Remove Component", () => {
                foreach (GameObject gameObject in Selection.gameObjects) {
                    if (gameObject.TryGetComponent(compType, out Component component)) {
                        Undo.DestroyObjectImmediate(component);
                    }
                }
            });
        }

        private void CopySelectedToClipboard() {
            PersistentData.Clipboard.CopyComponents(GetComponentsFromSelection());
        }

        private void PasteFromClipboard() {
            if (InspectorIsLocked()) {
                (inspectingObject as GameObject).PasteComponents(PersistentData.Clipboard.Copies);
                return;
            }
            
            foreach (GameObject gameObject in Selection.gameObjects) {
                gameObject.PasteComponents(PersistentData.Clipboard.Copies);
            }
        }

        private Rect ShiftRectStartVertically(Rect rect, float length) { 
            rect.position += new Vector2(0f, length);
            rect.height -= length;
            return rect;
        }
        
        private void Fix2021EditorMargins() {
            bool ShowingTransform() {
                if (!InspectingObjectIsValid()) {
                    return false;
                }

                int compStartIndex = ComponentStartIndex();
                if (editorListVisual.childCount <= compStartIndex) {
                    return false;
                }
                
                return editorListVisual[compStartIndex].style.display ==  DisplayStyle.Flex;
            }
            
            if (ShowingTransform()) {
                const float transformHeaderMissingHeight = 7f;
                miniMapGuiContainer.style.marginTop = 0f;
                miniMapGuiContainer.style.marginBottom = transformHeaderMissingHeight + MiniMapMargin;
            }
            else {
                Margin(miniMapGuiContainer.style, MiniMapMargin);
                miniMapGuiContainer.style.marginTop = 0f;
            }
        }
    }
}
#endif