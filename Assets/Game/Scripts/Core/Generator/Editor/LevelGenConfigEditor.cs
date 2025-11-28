using UnityEditor;
using UnityEngine;
using Game.Scripts.Core.Generator;
using Game.Scripts.Runtime.LightAndShadow;

// Assuming Subsystem is accessible for the runtime button

[CustomEditor(typeof(LevelGenConfig))]
public class LevelGenConfigEditor : Editor
{
    // Properties for organization
    private SerializedProperty _seed;
    private SerializedProperty _branchChance;
    private SerializedProperty _singleDoorChance;
    private SerializedProperty _roomCount;
    private SerializedProperty _roomSizeMin;
    private SerializedProperty _roomSizeMax;
    private SerializedProperty _spacing;
    private SerializedProperty _doorHeight;

    private SerializedProperty _floorMaterial;
    private SerializedProperty _initialFloorColor;
    private SerializedProperty _wallMaterial;
    private SerializedProperty _initialWallColor;
    private SerializedProperty _wallHeight;
    private SerializedProperty _wallThickness;
    private SerializedProperty _doorWidth;

    private SerializedProperty _doorPrefab;
    private SerializedProperty _bakeNavMesh;

    private void OnEnable()
    {
        // Cache the SerializedProperties using the backing field names
        _seed = serializedObject.FindProperty("<Seed>k__BackingField");
        _branchChance = serializedObject.FindProperty("<BranchChance>k__BackingField");
        _singleDoorChance = serializedObject.FindProperty("<SingleDoorChance>k__BackingField");
        _roomCount = serializedObject.FindProperty("<RoomCount>k__BackingField");
        _roomSizeMin = serializedObject.FindProperty("<RoomSizeMin>k__BackingField");
        _roomSizeMax = serializedObject.FindProperty("<RoomSizeMax>k__BackingField");
        _spacing = serializedObject.FindProperty("<Spacing>k__BackingField");
        _doorHeight = serializedObject.FindProperty("<DoorHeight>k__BackingField");

        _floorMaterial = serializedObject.FindProperty("<FloorMaterial>k__BackingField");
        _initialFloorColor = serializedObject.FindProperty("<InitialFloorColor>k__BackingField");
        _wallMaterial = serializedObject.FindProperty("<WallMaterial>k__BackingField");
        _initialWallColor = serializedObject.FindProperty("<InitialWallColor>k__BackingField");
        _wallHeight = serializedObject.FindProperty("<WallHeight>k__BackingField");
        _wallThickness = serializedObject.FindProperty("<WallThickness>k__BackingField");
        _doorWidth = serializedObject.FindProperty("<DoorWidth>k__BackingField");

        _doorPrefab = serializedObject.FindProperty("<DoorPrefab>k__BackingField");
        _bakeNavMesh = serializedObject.FindProperty("<BakeNavMesh>k__BackingField");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("🗺️ Generation Settings", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(_seed);
        if (GUILayout.Button("Random Seed", GUILayout.Width(100)))
        {
            _seed.intValue = Random.Range(1, 100000);
        }

        EditorGUILayout.EndHorizontal();

        _branchChance.floatValue = EditorGUILayout.Slider(
            new GUIContent("Branch Chance"),
            _branchChance.floatValue,
            0.0f,
            1.0f
        );
        _singleDoorChance.floatValue = EditorGUILayout.Slider(
            new GUIContent("Single Door Chance"),
            _singleDoorChance.floatValue,
            0.0f,
            1.0f
        );

        EditorGUILayout.PropertyField(_roomCount);
        EditorGUILayout.PropertyField(_roomSizeMin, new GUIContent("Room Size Min (X/Z)"));
        EditorGUILayout.PropertyField(_roomSizeMax, new GUIContent("Room Size Max (X/Z)"));
        EditorGUILayout.PropertyField(_spacing, new GUIContent("Room Spacing"));

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("🧱 Visuals & Geometry", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(_floorMaterial, new GUIContent("Floor Material"));
        EditorGUILayout.PropertyField(_initialFloorColor, new GUIContent("Initial Floor Color"));

        EditorGUILayout.PropertyField(_wallMaterial, new GUIContent("Wall Material"));
        EditorGUILayout.PropertyField(_initialWallColor, new GUIContent("Initial Wall Color"));
        EditorGUILayout.PropertyField(_wallHeight, new GUIContent("Wall Height"));
        EditorGUILayout.PropertyField(_wallThickness, new GUIContent("Wall Thickness"));

        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(_doorWidth, new GUIContent("Door Width"));
        EditorGUILayout.PropertyField(_doorHeight, new GUIContent("Door Height"));

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("📦 Prefabs & AI Navigation", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(_doorPrefab, new GUIContent("Door Prefab"));

        EditorGUILayout.PropertyField(_bakeNavMesh, new GUIContent("Bake NavMesh Per Room"));
        if (_bakeNavMesh.boolValue)
        {
            EditorGUILayout.HelpBox(
                "Enabling per-room baking requires correct Agent Radius settings in the Navigation window (Window > AI > Navigation) for proper sizing.",
                MessageType.Info
            );
        }

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space(20);

       
        LevelGenConfig config = (LevelGenConfig)target;

        using (new EditorGUI.DisabledScope(!Application.isPlaying))
        {
            if (GUILayout.Button("GENERATE LEVEL (Runtime)"))
            {
                Subsystem.Get<LightAndShadowSubsystem>().Clear();
                LevelGenerator LG = Subsystem.Get<LevelGenerator>();
                if (LG != null)
                {
                    LG.SetConfig(config);
                    LG.Generate();
                }
                else
                {
                    Debug.LogError(
                        "Cannot find LevelGenerator in the scene. Please ensure one is attached to a GameObject.");
                }
            }
        }

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox(
                "You must be in Play Mode to generate the level via this button. This ensures all runtime systems (like Subsystem) are initialized.",
                MessageType.Info);
        }
    }
}