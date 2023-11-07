using System;
using System.Collections.Generic;
using Ai;
using Data;
using PlayerControllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace MapGenerator
{
    public class GeneratorUi : MonoBehaviour
    {
        [SerializeField] private GameObject mesh;
        [SerializeField] private Button generateButton;
        [SerializeField] private Button generatePreviewButton;
        [SerializeField] private Button viewParametersButton;
        [SerializeField] private TMP_Dropdown drawMode;
        [SerializeField] private TMP_InputField mapWidth;
        [SerializeField] private TMP_InputField mapHeight;
        [SerializeField] private TMP_InputField levelOfDetail;
        [SerializeField] private TMP_InputField noiseScale;
        [SerializeField] private TMP_InputField octaves;
        [SerializeField] private TMP_InputField persistence;
        [SerializeField] private TMP_InputField lacunarity;
        [SerializeField] private TMP_InputField seed;
        [SerializeField] private TMP_InputField offsetX;
        [SerializeField] private TMP_InputField offsetY;
        [SerializeField] private TMP_InputField buildTime;
        [SerializeField] private Toggle autoUpdate;
        [SerializeField] private Toggle instantBuild;
        [SerializeField] private GameObject parametersPanel;
        [SerializeField] private TMP_Dropdown worldTileType;
        [SerializeField] private TMP_Dropdown buildMode;
        [SerializeField] private TMP_Dropdown constructionTileType;
        [SerializeField] private TMP_Dropdown floorTileType;
        [SerializeField] private GameObject tileInfoPanel;
        [SerializeField] private TextMeshProUGUI tileCoordsText;
        [SerializeField] private TextMeshProUGUI tileWorldTypeText;
        [SerializeField] private TextMeshProUGUI tileConstructionTypeText;
        [SerializeField] private TextMeshProUGUI tileFloorTypeText;
        [SerializeField] private TextMeshProUGUI tileWalkSpeedText;
        [SerializeField] private TextMeshProUGUI turnCounterText;
        [SerializeField] private TextMeshProUGUI characterText;

        private GenerationConfig _config;
        private MapInfoController _mapInfoController;
        private MouseController _mouseController;
        private TurnManager _turnManager;
        private bool _isAutoUpdate;
        private Dictionary<DrawMode, int> _drawModeIds;
        private bool _paramHasChanged;
        private bool _mapInfoIsClear;
        private int _turnCounter;

        [Inject]
        public void Initialize(MapInfoController mapInfoController, GenerationConfig config,
            MouseController mouseController, TurnManager turnManager)
        {
            _config = config;
            _mapInfoController = mapInfoController;
            _mouseController = mouseController;
            _turnManager = turnManager;
            
            tileInfoPanel.SetActive(false);
            _mouseController.OnSelectedTileChanged += UpdateTileInfo;
            _turnManager.OnTurnTrigger += RaiseTurnCounter;

            InitializeValues();
            
            if (_isAutoUpdate)
            {
                AutoUpdateSubscriptions();
            }

            _mouseController.IsInstantBuild = config.instantBuild;
            
            AddSubscriptions();
            
            GenerateMapPreview();
            GenerateMap();
        }

        private void RaiseTurnCounter()
        {
            _turnCounter += 1;
            turnCounterText.text = $"Turn: {_turnCounter}";
        }

        private void UpdateTileInfo(Tile tile)
        {
            if (!_mapInfoController.GenerationComplete) return;
            
            if (tile == null)
            {
                tileInfoPanel.SetActive(false);
                return;
            }

            tileInfoPanel.SetActive(true);
            tileCoordsText.text = $"X: {tile.X}, Y: {tile.Y}";
            tileWorldTypeText.text = $"Tile world type: {tile.Type}";
            tileConstructionTypeText.text = $"Tile building type: {(tile.Building == null ? "None" : tile.Building.Type)}";
            tileFloorTypeText.text = $"Tile floor type: {(tile.Floor == null ? "None" : tile.Floor.Type)}";
            tileWalkSpeedText.text = $"Tile walk speed multiplier: {tile.WalkSpeedMultiplier}";
            characterText.text = $"Character: {(tile.Pawn == null ? "None" : tile.Pawn)}";
        }

        private void InitializeValues()
        {
            _drawModeIds = new()
            {
                { DrawMode.NoiseMap, 0 },
                { DrawMode.ColorMap, 1 }
            };

            _paramHasChanged = true;
            gameObject.SetActive(true);

            var drawModeList = new List<string>
            {
                "Noise map",
                "Color map"
            };
            
            var worldTileTypeList = new List<string>
            {
                "None",
                "Ocean",
                "Sand",
                "Grass",
                "Rocks",
                "Mountain",
                "Summit"
            };
            
            var buildModeList = new List<string>
            {
                "World",
                "Building",
                "Floor",
                "Character"
            };
            
            var constructionTileTypeList = new List<string>
            {
                "None",
                "Wall",
                "Statue"
            };
            
            var floorTileTypeList = new List<string>
            {
                "None",
                "Wood"
            };
            
            drawMode.ClearOptions();
            worldTileType.ClearOptions();
            buildMode.ClearOptions();
            constructionTileType.ClearOptions();
            floorTileType.ClearOptions();
            
            drawMode.AddOptions(drawModeList);
            worldTileType.AddOptions(worldTileTypeList);
            buildMode.AddOptions(buildModeList);
            constructionTileType.AddOptions(constructionTileTypeList);
            floorTileType.AddOptions(floorTileTypeList);
            
            parametersPanel.SetActive(false);
            
            _drawModeIds.TryGetValue(_config.drawMode,out var drawModeId);
            
            drawMode.value = drawModeId;
            mapWidth.text = _config.mapWidth.ToString();
            mapHeight.text = _config.mapHeight.ToString();
            levelOfDetail.text = _config.levelOfDetail.ToString();
            noiseScale.text = _config.noiseScale.ToString();
            octaves.text = _config.octaves.ToString();
            persistence.text = _config.persistence.ToString();
            lacunarity.text = _config.lacunarity.ToString();
            seed.text = _config.seed.ToString();
            offsetX.text = _config.offset.y.ToString();
            offsetY.text = _config.offset.y.ToString();
            buildTime.text = _config.buildTime.ToString();
            _isAutoUpdate = _config.autoUpdate;
            autoUpdate.isOn = _config.autoUpdate;
            instantBuild.isOn = _config.autoUpdate;
        }

        private void AddSubscriptions()
        {
            viewParametersButton.onClick.AddListener(ToggleMenu);
            generateButton.onClick.AddListener(GenerateMap);
            generatePreviewButton.onClick.AddListener(GenerateMapPreview);
            drawMode.onValueChanged.AddListener(ChangeDrawMode);
            worldTileType.onValueChanged.AddListener(ChangeWorldTileType);
            buildMode.onValueChanged.AddListener(ChangeBuildMode);
            constructionTileType.onValueChanged.AddListener(ChangeConstructionTileType);
            floorTileType.onValueChanged.AddListener(ChangeFloorTileType);
            mapWidth.onValueChanged.AddListener(width => _config.mapWidth = int.Parse(width));
            mapHeight.onValueChanged.AddListener(height => _config.mapHeight = int.Parse(height));
            levelOfDetail.onValueChanged.AddListener(detailLevel => _config.levelOfDetail = int.Parse(detailLevel));
            noiseScale.onValueChanged.AddListener(scale => _config.noiseScale = int.Parse(scale));
            octaves.onValueChanged.AddListener(octave => _config.octaves = int.Parse(octave));
            persistence.onValueChanged.AddListener(persis => _config.persistence = int.Parse(persis));
            lacunarity.onValueChanged.AddListener(lacun => _config.lacunarity = int.Parse(lacun));
            seed.onValueChanged.AddListener(generationSeed => _config.seed = int.Parse(generationSeed));
            offsetX.onValueChanged.AddListener(xOffset => _config.offset.x = int.Parse(xOffset));
            offsetY.onValueChanged.AddListener(yOffset => _config.offset.y = int.Parse(yOffset));
            buildTime.onValueChanged.AddListener(timeToBuild => _config.buildTime = int.Parse(timeToBuild));
            autoUpdate.onValueChanged.AddListener(ToggleAutoUpdate);
            instantBuild.onValueChanged.AddListener(ToggleInstantBuild);
        }

        private void ChangeFloorTileType(int arg0)
        {
            _config.floorTileToPlace = arg0 switch
            {
                0 => FloorTileType.None,
                1 => FloorTileType.Wood,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void ToggleInstantBuild(bool arg0)
        {
            _mouseController.IsInstantBuild = arg0;
        }

        private void ChangeBuildMode(int arg0)
        {
            _config.buildMode = arg0 switch
            {
                0 => BuildMode.World,
                1 => BuildMode.Building,
                2 => BuildMode.Floor,
                3 => BuildMode.Character,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void ChangeConstructionTileType(int arg0)
        {
            _config.buildingsTileToPlace = arg0 switch
            {
                0 => BuildingsTileType.None,
                1 => BuildingsTileType.Wall,
                2 => BuildingsTileType.Statue,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            _mouseController.ConstructionTileTypeChange();
        }

        private void ChangeWorldTileType(int arg0)
        {
            _config.worldTileToPlace = arg0 switch
            {
                0 => WorldTileType.None,
                1 => WorldTileType.Water,
                2 => WorldTileType.Sand,
                3 => WorldTileType.Grass,
                4 => WorldTileType.Rocks,
                5 => WorldTileType.Mountain,
                6 => WorldTileType.Summit,
                _ => throw new ArgumentException()
            };
        }

        private void GenerateMapPreview()
        {
            if (!_paramHasChanged) return;
            if(!_mapInfoIsClear) return;

            mesh.SetActive(true);
            _mapInfoController.CreateMapGraphic();
        }

        private void GenerateMap()
        {
            if (!_paramHasChanged) return;

            _mapInfoIsClear = false;
            _mapInfoController.CreateMapInfo();
            mesh.SetActive(false);
        }

        private void ToggleMenu()
        {
            parametersPanel.SetActive(!parametersPanel.activeSelf);
        }

        private void ToggleAutoUpdate(bool arg)
        {
            _isAutoUpdate = arg;
            
            AutoUpdateSubscriptions();
        }

        private void ChangeDrawMode(int drawModeId)
        {
            _config.drawMode = drawModeId switch
            {
                0 => DrawMode.NoiseMap,
                1 => DrawMode.ColorMap,
                _ => throw new ArgumentException()
            };
        }

        private void AutoUpdateSubscriptions()
        {
            drawMode.onValueChanged.AddListener(OnParamsChanged);
            mapWidth.onValueChanged.AddListener(OnParamsChanged);
            mapHeight.onValueChanged.AddListener(OnParamsChanged);
            levelOfDetail.onValueChanged.AddListener(OnParamsChanged);
            noiseScale.onValueChanged.AddListener(OnParamsChanged);
            octaves.onValueChanged.AddListener(OnParamsChanged);
            persistence.onValueChanged.AddListener(OnParamsChanged);
            lacunarity.onValueChanged.AddListener(OnParamsChanged);
            seed.onValueChanged.AddListener(OnParamsChanged);
            offsetX.onValueChanged.AddListener(OnParamsChanged);
            offsetY.onValueChanged.AddListener(OnParamsChanged);
        }

        private void OnParamsChanged(int fgh)
        {
            if (!_isAutoUpdate)
            {
                _paramHasChanged = true;
                return;
            }

            if (!_mapInfoIsClear)
            {
                _mapInfoIsClear = true;
                _mapInfoController.ClearMap();
            }
            
            mesh.SetActive(true);
            _mapInfoController.CreateMapGraphic();
        }
        
        private void OnParamsChanged(string fgh)
        {
            if (!_isAutoUpdate)
            {
                _paramHasChanged = true;
                return;
            }
            
            if (!_mapInfoIsClear)
            {
                _mapInfoIsClear = true;
                _mapInfoController.ClearMap();
            }
            
            mesh.SetActive(true);
            _mapInfoController.CreateMapGraphic();
        }
    }
}