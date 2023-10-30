﻿using Data;
using MapGenerator;
using UnityEngine;
using Zenject;

namespace DependencyInjection
{
    public class MapGeneratorInstaller : MonoInstaller
    {
        [SerializeField] private GenerationConfig generationConfig;
        [SerializeField] private new Renderer renderer;
        [SerializeField] private GeneratorUi generatorUi;
        [SerializeField] private Camera Camera;
        [SerializeField] private TileData tilePrefab;
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<MapPreviewGenerator>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MapGenerator.MapGenerator>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<Noise>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<TextureGenerator>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MapDisplay>().AsSingle().WithArguments(renderer).NonLazy();
            Container.BindInterfacesAndSelfTo<CameraController>().AsSingle().WithArguments(Camera).NonLazy();
            
            Container.BindInterfacesAndSelfTo<GenerationConfig>().FromInstance(generationConfig).AsSingle();
            Container.BindInterfacesAndSelfTo<GeneratorUi>().FromInstance(generatorUi).AsSingle();
            Container.BindInterfacesAndSelfTo<TileData>().FromInstance(tilePrefab).AsSingle();
        }
    }
}