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
        private SerializedProperty _topTrigger;
        private SerializedProperty _bottomTrigger;
        
        private SerializedProperty _height;
        private SerializedProperty _topTriggerOffset;
        private SerializedProperty _bottomTriggerOffset;

        private void OnEnable()
        {
            _so = serializedObject;
            _spriteRenderer = _so.FindProperty("spriteRenderer");
            _trigger = _so.FindProperty("trigger");
            _topTrigger = _so.FindProperty("topTrigger");
            _bottomTrigger = _so.FindProperty("bottomTrigger");
            _height = _so.FindProperty("height");
            _topTriggerOffset = _so.FindProperty("topTriggerOffset");
            _bottomTriggerOffset = _so.FindProperty("bottomTriggerOffset");
        }

        public override void OnInspectorGUI()
        {
            _so.Update();
            EditorGUILayout.PropertyField(_spriteRenderer);
            EditorGUILayout.PropertyField(_trigger);
            EditorGUILayout.PropertyField(_topTrigger);
            EditorGUILayout.PropertyField(_bottomTrigger);
            
            EditorGUILayout.PropertyField(_height);
            GUILayout.Space(10);
            if (GUILayout.Button("Update Height"))
            {
                var ladder = target as Ladder;
                ladder.UpdateHeight();
            }
            
            GUILayout.Space(15);
            EditorGUILayout.PropertyField(_topTriggerOffset);
            EditorGUILayout.PropertyField(_bottomTriggerOffset);
            GUILayout.Space(10);
            if (GUILayout.Button("Update Triggers Offset"))
            {
                var ladder = target as Ladder;
                ladder.UpdateTriggersOffset();
            }
            
            _so.ApplyModifiedProperties();
        }
    }
}