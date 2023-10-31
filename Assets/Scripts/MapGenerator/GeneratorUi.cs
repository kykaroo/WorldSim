using System;
using System.Collections.Generic;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace MapGenerator
{
    public class GeneratorUi : MonoBehaviour
    {
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
        [SerializeField] private Toggle autoUpdate;
        [SerializeField] private GameObject parametersPanel;
        [SerializeField] private TMP_Dropdown tileType;

        private GenerationConfig _config;
        private MapInfoController _mapInfoController;
        private bool _isAutoUpdate;
        private Dictionary<DrawMode, int> _drawModeIds;
        private bool _paramHasChanged;
        private bool _mapInfoIsClear;

        [Inject]
        public void Initialize(MapInfoController mapInfoController, GenerationConfig config)
        {
            _config = config;
            _mapInfoController = mapInfoController;
            
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
            var tileTypeList = new List<string>
            {
                "None",
                "Ocean",
                "Sand",
                "Grass",
                "Rocks",
                "Mountain",
                "Summit"
            };
            drawMode.ClearOptions();
            tileType.ClearOptions();
            drawMode.AddOptions(drawModeList);
            tileType.AddOptions(tileTypeList);

            parametersPanel.SetActive(false);

            InitializeValues();
            
            if (_isAutoUpdate)
            {
                AutoUpdateSubscriptions();
            }
            
            AddSubscriptions();
            
            GenerateMapPreview();
            GenerateMap();
        }

        private void InitializeValues()
        {
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
            _isAutoUpdate = _config.autoUpdate;
            autoUpdate.isOn = _config.autoUpdate;
        }

        private void AddSubscriptions()
        {
            viewParametersButton.onClick.AddListener(ToggleMenu);
            generateButton.onClick.AddListener(GenerateMap);
            generatePreviewButton.onClick.AddListener(GenerateMapPreview);
            drawMode.onValueChanged.AddListener(ChangeDrawMode);
            tileType.onValueChanged.AddListener(ChangeTileType);
            mapWidth.onValueChanged.AddListener(b => _config.mapWidth = int.Parse(b));
            mapHeight.onValueChanged.AddListener(b => _config.mapHeight = int.Parse(b));
            levelOfDetail.onValueChanged.AddListener(b => _config.levelOfDetail = int.Parse(b));
            noiseScale.onValueChanged.AddListener(b => _config.noiseScale = int.Parse(b));
            octaves.onValueChanged.AddListener(b => _config.octaves = int.Parse(b));
            persistence.onValueChanged.AddListener(b => _config.persistence = int.Parse(b));
            lacunarity.onValueChanged.AddListener(b => _config.lacunarity = int.Parse(b));
            seed.onValueChanged.AddListener(b => _config.seed = int.Parse(b));
            offsetX.onValueChanged.AddListener(b => _config.offset.x = int.Parse(b));
            offsetY.onValueChanged.AddListener(b => _config.offset.y = int.Parse(b));
            autoUpdate.onValueChanged.AddListener(ToggleAutoUpdate);
        }

        private void ChangeTileType(int arg0)
        {
            _config.tileToPlace = arg0 switch
            {
                0 => TileType.None,
                1 => TileType.Ocean,
                2 => TileType.Sand,
                3 => TileType.Grass,
                4 => TileType.Rocks,
                5 => TileType.Mountain,
                6 => TileType.Summit,
                _ => throw new ArgumentException()
            };
        }

        private void GenerateMapPreview()
        {
            if (!_paramHasChanged) return;

            _mapInfoController.CreateMapGraphic();
        }

        private void GenerateMap()
        {
            if (!_paramHasChanged) return;

            _mapInfoIsClear = false;
            _mapInfoController.CreateMapInfo();
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
            
            _mapInfoController.CreateMapGraphic();
        }
    }
}