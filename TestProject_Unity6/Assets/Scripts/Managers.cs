using System.Runtime.Serialization;
using UnityEngine;

public class Managers : MonoBehaviour
{
    public static Managers Instance;

    public EffectManager _effect;
    public CameraMover _cameraMover;
    public DroppedItemManager _dropItem;
    public MonsterManager _monsterManager;
    public UIManager _uiManager;
    public GameDataManager _gameDataManager;
    public TableData _tableData;
    public FileManager _fileManager;
    public MapManager _mapManager;

    public static EffectManager effect { get { return Instance?._effect; } }
    public static CameraMover CameraMover { get { return Instance?._cameraMover; } }
    public static DroppedItemManager DropItem { get { return Instance?._dropItem; } }
    public static MonsterManager MonsterManager { get { return Instance?._monsterManager; } }
    public static UIManager UIManager { get { return Instance?._uiManager; } }
    public static GameDataManager GameData { get { return Instance?._gameDataManager; } }
    public static TableData TableData { get { return Instance?._tableData; } }
    public static FileManager File { get { return Instance?._fileManager; } }
    public static MapManager Map { get { return Instance?._mapManager; } }

    private void Awake()
    {
        Instance = this;
    }
}
