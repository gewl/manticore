using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;

[CustomEditor(typeof(AutonomousMovementComponent))]
public class AutonomousMovementComponentEditor : OdinEditor {

    SerializedProperty movementBehaviors;
    int behaviorsListSize;
    AutonomousMovementComponent autonomousMovementComponent;

    protected override void OnEnable()
    {
        base.OnEnable();
        autonomousMovementComponent = (AutonomousMovementComponent)target;
        movementBehaviors = serializedObject.FindProperty("movementBehaviors");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        behaviorsListSize = movementBehaviors.arraySize;

        for (int i = 0; i < behaviorsListSize; i++)
        {
            SerializedProperty listElement = movementBehaviors.GetArrayElementAtIndex(i);
            AutonomousMovementComponent.MovementBehaviorTypes behavior = (AutonomousMovementComponent.MovementBehaviorTypes)listElement.enumValueIndex;
            HandleBehaviorInEditor(behavior);
        }
        serializedObject.ApplyModifiedProperties();
    }

    void HandleBehaviorInEditor(AutonomousMovementComponent.MovementBehaviorTypes behavior)
    {
        switch (behavior)
        {
            case AutonomousMovementComponent.MovementBehaviorTypes.Seek:
                SerializedProperty seekTarget = serializedObject.FindProperty("SeekTarget");
                EditorGUILayout.PropertyField(seekTarget);
                break;
            case AutonomousMovementComponent.MovementBehaviorTypes.Flee:
                SerializedProperty fleeTarget = serializedObject.FindProperty("FleeTarget");
                EditorGUILayout.PropertyField(fleeTarget);
                break;
            case AutonomousMovementComponent.MovementBehaviorTypes.Arrive:
                SerializedProperty arriveTarget = serializedObject.FindProperty("ArriveTarget");
                EditorGUILayout.PropertyField(arriveTarget);
                break;
            case AutonomousMovementComponent.MovementBehaviorTypes.Pursuit:
                SerializedProperty pursuitTarget = serializedObject.FindProperty("PursuitTarget");
                EditorGUILayout.PropertyField(pursuitTarget);
                break;
            case AutonomousMovementComponent.MovementBehaviorTypes.Evade:
                SerializedProperty evadeTarget = serializedObject.FindProperty("EvadeTarget");
                EditorGUILayout.PropertyField(evadeTarget);
                break;
            case AutonomousMovementComponent.MovementBehaviorTypes.Wander:
                SerializedProperty wanderDistance = serializedObject.FindProperty("wanderDistance");
                SerializedProperty wanderRadius = serializedObject.FindProperty("wanderRadius");
                SerializedProperty wanderJitter = serializedObject.FindProperty("wanderJitter");

                EditorGUILayout.PropertyField(wanderDistance);
                EditorGUILayout.PropertyField(wanderRadius);
                EditorGUILayout.PropertyField(wanderJitter);
                break;
            case AutonomousMovementComponent.MovementBehaviorTypes.ObstacleAvoidance:
                break;
            case AutonomousMovementComponent.MovementBehaviorTypes.WallAvoidance:
                break;
            case AutonomousMovementComponent.MovementBehaviorTypes.Interpose:
                SerializedProperty primaryTarget = serializedObject.FindProperty("PrimaryInterposeTarget");
                SerializedProperty secondaryTarget = serializedObject.FindProperty("SecondaryInterposeTarget");
                EditorGUILayout.PropertyField(primaryTarget);
                EditorGUILayout.PropertyField(secondaryTarget);
                break;
            case AutonomousMovementComponent.MovementBehaviorTypes.Hide:
                SerializedProperty hideTarget = serializedObject.FindProperty("HideTarget");
                EditorGUILayout.PropertyField(hideTarget);
                break;
            case AutonomousMovementComponent.MovementBehaviorTypes.PathFollowing:
                SerializedProperty pathNodes = serializedObject.FindProperty("PathNodes");
               EditorGUILayout.PropertyField(pathNodes);
                break;
            case AutonomousMovementComponent.MovementBehaviorTypes.OffsetPursuit:
                SerializedProperty offsetPursuitTarget = serializedObject.FindProperty("OffsetPursuitTarget");
                EditorGUILayout.PropertyField(offsetPursuitTarget);

                EditorGUI.BeginChangeCheck();
                Vector3 pursuitOffset = EditorGUILayout.Vector3Field("Pursuit Offset:", autonomousMovementComponent.PursuitOffset);
                if (EditorGUI.EndChangeCheck())
                {
                    autonomousMovementComponent.PursuitOffset = pursuitOffset;
                }
                break;
            case AutonomousMovementComponent.MovementBehaviorTypes.Separation:
                SerializedProperty separationRadius = serializedObject.FindProperty("SeparationRadius");
                EditorGUILayout.PropertyField(separationRadius);
                break;
            case AutonomousMovementComponent.MovementBehaviorTypes.Flocking:
                break;
            case AutonomousMovementComponent.MovementBehaviorTypes.Cohesion:
                break;
            case AutonomousMovementComponent.MovementBehaviorTypes.Alignment:
                SerializedProperty alignmentRadius = serializedObject.FindProperty("AlignmentRadius");
                EditorGUILayout.PropertyField(alignmentRadius);
                break;
            default:
                break;
        }
    }

}
