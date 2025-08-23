using System;
using UnityEditor;
using UnityEngine;

namespace Character_Controller.Runtime.Ladders
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Ladder))]
    public class LadderEditor : Editor
    {
        private SerializedObject _so;
        private SerializedProperty _spriteRenderer;
        private SerializedProperty _trigger;
        private SerializedProperty _height;

        private void OnEnable()
        {
            _so = serializedObject;
            _spriteRenderer = _so.FindProperty("spriteRenderer");
            _trigger = _so.FindProperty("trigger");
            _height = _so.FindProperty("height");
        }

        public override void OnInspectorGUI()
        {
            _so.Update();
            EditorGUILayout.PropertyField(_spriteRenderer);
            EditorGUILayout.PropertyField(_trigger);
            EditorGUILayout.PropertyField(_height);

            GUILayout.Space(20);
            if (GUILayout.Button("Update Height"))
            {
                var ladder = target as Ladder;
                ladder.UpdateHeight();
            }
            
            _so.ApplyModifiedProperties();
        }
    }
}