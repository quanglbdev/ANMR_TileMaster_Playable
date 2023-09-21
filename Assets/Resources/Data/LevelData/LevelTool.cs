#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Doozy.Engine.Extensions;
using UnityEditor;
using UnityEngine;
using static System.String;
using Object = UnityEngine.Object;

public class LevelTool : EditorWindow
{
    [MenuItem("Tools/CopyLevel")]
    public static void ShowWindow()
    {
        GetWindow(typeof(LevelTool));
    }

    private GameLevelManager levelPrefab;
    private ObstacleDefinition _obstacleDefinition;

    private Object _targetFolder = null;
    private string _fileName;

    [Obsolete("Obsolete")]
    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        _obstacleDefinition =
            (ObstacleDefinition)EditorGUILayout.ObjectField(_obstacleDefinition, typeof(ObstacleDefinition));
        if (GUILayout.Button("Load ObstacleDefinition", EditorStyles.miniButtonRight, GUILayout.Width(150)))
        {
            _obstacleDefinition = LoadObstacleDefinition();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(30f);
        EditorGUILayout.BeginHorizontal();
        levelPrefab = (GameLevelManager)EditorGUILayout.ObjectField(levelPrefab, typeof(GameLevelManager));
        //_fileName = EditorGUILayout.TextField(_fileName);
        if (GUILayout.Button("Copy Level", EditorStyles.miniButtonRight, GUILayout.Width(150)))
        {
            CloneLevel();
        }

        // if (GUILayout.Button("CopyLevel", EditorStyles.miniButtonRight, GUILayout.Width(150)))
        // {
        //     CopyLevel(levelPrefab, _fileName, 1);
        // }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(30f);
        EditorGUILayout.BeginHorizontal();
        _targetFolder = EditorGUILayout.ObjectField(_targetFolder, typeof(Object));
        if (GUILayout.Button("Copy Folder", EditorStyles.miniButtonRight, GUILayout.Width(150)))
        {
            CloneByFolder();
        }
        
        EditorGUILayout.EndHorizontal();
    }

    private void CloneByFolder()
    {
        var levels = GetLevelManagers();
        for (var i = 0; i < levels.Count; i++)
        {
            CopyLevel(levels[i], $"Lv{i + 1}", i + 1);
        }
    }
    
    private void CloneLevel()
    {
        CopyLevel(levelPrefab, GetLevelName(levelPrefab.name), GetLevel(levelPrefab.name));
    }

    private string GetLevelName(string inputString)
    {
        // Initialize a string to store the numeric part of the inputString
        string numericPart = "";

        // Iterate through the characters in the inputString
        foreach (char c in inputString)
        {
            // Check if the character is a digit (0-9)
            if (char.IsDigit(c))
            {
                numericPart += c;
            }
        }

        // Attempt to parse the numeric part as an integer
        if (!IsNullOrEmpty(numericPart) && int.TryParse(numericPart, out int result))
        {
            Debug.Log("Extracted integer: " + $"Lv{result}");
            return $"Lv{result}";
        }

        Debug.LogError("Tên file level phải có số level ở sau cùng. VD: Level30");
        return Empty;
    }
    
    private int GetLevel(string inputString)
    {
        // Initialize a string to store the numeric part of the inputString
        string numericPart = "";

        // Iterate through the characters in the inputString
        foreach (char c in inputString)
        {
            // Check if the character is a digit (0-9)
            if (char.IsDigit(c))
            {
                numericPart += c;
            }
        }

        // Attempt to parse the numeric part as an integer
        if (!IsNullOrEmpty(numericPart) && int.TryParse(numericPart, out int result))
        {
            Debug.Log("Extracted integer: " + $"Lv{result}");
            return result;
        }

        Debug.LogError("Tên file level phải có số level ở sau cùng. VD: Level30");
        return -999;
    }

    private void CopyLevel(GameLevelManager levelManager, string fileName, int level)
    {
        if (levelManager == null || _obstacleDefinition == null || fileName == Empty || fileName == null ||
            fileName == "" || level == -999)
        {
            Debug.LogError("levelManager or obstacleDefinition or fileName is NULL");
            return;
        }

        level %= 10;
        if (level > 5)
        {
            level -= 5;
        }

        var levelDifficulty = (level) switch
        {
            1 => Config.LEVEL_DIFFICULTY.EASY,
            2 => Config.LEVEL_DIFFICULTY.EASY,
            3 => Config.LEVEL_DIFFICULTY.MEDIUM,
            4 => Config.LEVEL_DIFFICULTY.MEDIUM,
            5 => Config.LEVEL_DIFFICULTY.HARD,
            _ => Config.LEVEL_DIFFICULTY.EASY
        };
        var levelDefinition = CreateInstance<LevelDefinition>();
        var listTileMaps = levelManager.listTileMaps;
        levelDefinition.difficulty = levelDifficulty;
        levelDefinition.dataAmountOnMap = levelManager.dataAmountOnMap == 0 ? 6 : levelManager.dataAmountOnMap;
        levelDefinition.secondsRequired = levelManager.secondsRequired;
        levelDefinition.dataTileInFloor.Clear();
        var indexOnMap = 0;
        for (var i = 0; i < listTileMaps.Count; i++)
        {
            var itemsData = new List<ItemTileData>();
            for (var y = listTileMaps[i].cellBounds.yMin; y < listTileMaps[i].cellBounds.yMax; y++)
            {
                for (var x = listTileMaps[i].cellBounds.xMin; x < listTileMaps[i].cellBounds.xMax; x++)
                {
                    if (listTileMaps[i].HasTile(new Vector3Int(x, y, 0)))
                    {
                        indexOnMap++;
                        ItemTileData itemTileData = new ItemTileData(i, indexOnMap, new Vector2Int(x, y));
                        var sprite = listTileMaps[i].GetTile<UnityEngine.Tilemaps.Tile>(new Vector3Int(x, y, 0)).sprite;
                        var obstacleType = _obstacleDefinition.FindObstacle(sprite);
                        itemTileData.obstacleType = obstacleType;
                        itemsData.Add(itemTileData);
                    }
                }
            }

            levelDefinition.dataTileInFloor.Add(new DataTileInFloor(i, itemsData));
        }

        if (AssetDatabase.LoadAssetAtPath($"Assets/Resources/Data/LevelData/{fileName}.asset",
                typeof(LevelDefinition))) ;
        AssetDatabase.DeleteAsset($"Assets/Resources/Data/LevelData/{fileName}.asset");
        AssetDatabase.CreateAsset(levelDefinition, $"Assets/Resources/Data/LevelData/{fileName}.asset");
        EditorUtility.SetDirty(levelDefinition);
        AssetDatabase.SaveAssets();

        AssetDatabase.Refresh();

        //DestroyImmediate(levelDefinition);
    }

    private List<GameLevelManager> GetLevelManagers()
    {
        List<GameLevelManager> levelPrefabs = new();
        var path = AssetDatabase.GetAssetPath(_targetFolder);
        var guids = AssetDatabase.FindAssets("t:prefab", new string[] { path });
        foreach (var guid in guids)
        {
            var _prefabPath = AssetDatabase.GUIDToAssetPath(guid);
            var level = AssetDatabase.LoadAssetAtPath<GameLevelManager>(_prefabPath);
            levelPrefabs.Add(level);
        }

        Debug.Log(levelPrefabs.Count);

        return levelPrefabs;
    }


    private ObstacleDefinition LoadObstacleDefinition()
    {
        var guids = AssetDatabase.FindAssets("t:ObstacleDefinition");

        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var obstacleDefinition = AssetDatabase.LoadAssetAtPath<ObstacleDefinition>(path);

            if (obstacleDefinition != null)
                return obstacleDefinition;
        }

        return null;
    }
}
#endif