using Data;
using MapGenerator;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;
using Tile = MapGenerator.Tile;

namespace DependencyInjection
{
    public class MapGeneratorInstaller : MonoInstaller
    {
        [SerializeField] private GenerationConfig generationConfig;
        [SerializeField] private new Renderer renderer;
        [SerializeField] private GeneratorUi generatorUi;
        [SerializeField] private CameraConfig cameraConfig;
        [SerializeField] private GameObject highLight;
        [SerializeField] private Camera camera;
        [SerializeField] private Tilemap tilemap;
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<MapGraphicGenerator>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MapInfoGenerator>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<Noise>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<TextureGenerator>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MapInfoController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<WorldData>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MapDisplay>().AsSingle().WithArguments(renderer).NonLazy();
            Container.BindInterfacesAndSelfTo<CameraController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<WorldController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<Tilemap>().FromInstance(tilemap).AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MouseController>().AsSingle().WithArguments(highLight).NonLazy();
            
            Container.BindInterfacesAndSelfTo<GenerationConfig>().FromNewScriptableObject(generationConfig).AsSingle();
            Container.BindInterfacesAndSelfTo<GeneratorUi>().FromInstance(generatorUi).AsSingle();
            Container.BindInterfacesAndSelfTo<CameraConfig>().FromNewScriptableObject(cameraConfig).AsSingle();
            Container.Bind<Camera>().FromMethod((b) => Instantiate(b.Container.Resolve<CameraConfig>().camera)).AsSingle();
        }
    }
}