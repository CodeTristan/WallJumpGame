#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

namespace WingmanInspector {

    public class WingmanComponentCopy {

        public SerializedObject SerializedObject;
        public Type ComponentType;

        public WingmanComponentCopy(Component component) {
            SerializedObject = new(component);
            ComponentType = component.GetType();
        }
        
    }
    
}

#endif