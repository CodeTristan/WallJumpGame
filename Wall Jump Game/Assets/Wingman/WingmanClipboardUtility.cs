#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace WingmanInspector {

    public static class WingmanClipboardUtility {

        private static readonly HashSet<Type> singleInstanceComponents = new HashSet<Type> {
            typeof(Transform), typeof(RectTransform), typeof(Rigidbody), typeof(Rigidbody2D),
            typeof(Collider), typeof(Collider2D), typeof(CharacterController), typeof(Animator),
            typeof(Animation), typeof(Camera), typeof(AudioSource), typeof(AudioListener),
            typeof(Light), typeof(MeshFilter), typeof(MeshRenderer), typeof(SkinnedMeshRenderer),
            typeof(SpriteRenderer), typeof(Canvas), typeof(CanvasRenderer), typeof(NavMeshAgent), 
            typeof(NavMeshObstacle)
        };

        public static void PasteComponentsFromEmpty(this GameObject gameObject, List<Component> comps) {
            foreach (Component refComp in comps) {
                Type refCompType = refComp.GetType();

                if (refCompType == typeof(Transform)) {
                    gameObject.GetComponent<Transform>().CopyFields(refComp);
                    continue;
                }

                if (CanOnlyHaveOneInstance(refCompType) && gameObject.TryGetComponent(refCompType, out Component _)) {
                    continue;
                }

                Component comp = Undo.AddComponent(gameObject, refCompType);
                comp.CopyFields(refComp);
            }
        }

        public static void PasteComponents(this GameObject gameObject, List<Component> comps) {
            foreach (Component refComp in comps) {
                gameObject.PasteComponent(refComp.GetType(), new SerializedObject(refComp));
            }
        }
        
        public static void PasteComponents(this GameObject gameObject, List<WingmanComponentCopy> compCopies) {
            foreach (WingmanComponentCopy compCopy in compCopies) {
                gameObject.PasteComponent(compCopy.ComponentType, compCopy.SerializedObject);
            }
        }

        private static void PasteComponent(this GameObject gameObject, Type compType, SerializedObject serReference) {
            bool hasComponent = gameObject.TryGetComponent(compType, out Component existingComp);
            
            if (hasComponent) {
                existingComp.CopyFields(serReference);
                return;
            }
            
            Undo.AddComponent(gameObject, compType).CopyFields(serReference);
        }

        private static bool CanOnlyHaveOneInstance(Type compType) {
            if (singleInstanceComponents.Contains(compType)) {
                return true;
            }
            return compType.GetCustomAttribute<DisallowMultipleComponent>() != null;
        }
        
        private static void CopyFields(this Component target, SerializedObject serReference) {
            SerializedObject serTarget = new SerializedObject(target);
            
            SerializedProperty property = serReference.GetIterator();
            if (property.NextVisible(true)) {
                do {
                    serTarget.CopyFromSerializedProperty(property);
                }
                while (property.NextVisible(false));
            }

            serTarget.ApplyModifiedProperties();
        }

        private static void CopyFields(this Component target, Component reference) {
            target.CopyFields(new SerializedObject(reference));
        }
        
    }

}

#endif