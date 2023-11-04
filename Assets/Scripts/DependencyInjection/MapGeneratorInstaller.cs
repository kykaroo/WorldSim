using Data;
using MapGenerator;
using PlayerControllers;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

namespace DependencyInjection
{
    public class MapGeneratorInstaller : MonoInstaller
    {
        [SerializeField] private GenerationConfig generationConfig;
        [SerializeField] private new Renderer renderer;
        [SerializeField] private GeneratorUi generatorUi;
        [SerializeField] private CameraConfig cameraConfig;
        [SerializeField] private Tilemap worldTilemap;
        [SerializeField] private Tilemap highLightTilemap;
        [SerializeField] private Tilemap constructionTilemap;
        [SerializeField] private Sprite tileSprite;
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<MapGraphicGenerator>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MapInfoGenerator>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<Noise>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<TextureGenerator>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MapInfoController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<WorldData>().AsSingle().WithArguments(worldTilemap, constructionTilemap).NonLazy();
            Container.BindInterfacesAndSelfTo<MapDisplay>().AsSingle().WithArguments(renderer).NonLazy();
            Container.BindInterfacesAndSelfTo<CameraController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<WorldController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MouseController>().AsSingle().WithArguments(highLightTilemap, tileSprite).NonLazy();

            Container.BindInterfacesAndSelfTo<GenerationConfig>().FromNewScriptableObject(generationConfig).AsSingle();
            Container.BindInterfacesAndSelfTo<GeneratorUi>().FromInstance(generatorUi).AsSingle();
            Container.BindInterfacesAndSelfTo<CameraConfig>().FromNewScriptableObject(cameraConfig).AsSingle();
            Container.Bind<Camera>().FromMethod((b) => Instantiate(b.Container.Resolve<CameraConfig>().camera)).AsSingle();
        }
    }
}