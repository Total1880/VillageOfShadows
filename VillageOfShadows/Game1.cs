using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using VillageOfShadows.Core.Simulation;
using VillageOfShadows.Core.Utils;
using VillageOfShadows.Core.World;
using VillageOfShadows.Game.Rendering;

namespace VillageOfShadows
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private World _world = null!;
        private Texture2D _grass = null!;
        private Texture2D[] _trees = null!;
        private SpriteBatch _sb = null!;
        private Camera2D _camera = null!;
        private WorldSimulation _sim = null!;
        private IRandom _rng = null!;
        private int _prevScroll;
        private WorldRenderer _worldRenderer = null!;
        private EntityRenderer _entityRenderer = null!;
        private Texture2D _pixel = null!;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Core
            var config = new VillageOfShadows.Core.Config.WorldConfig { TileSize = 16 };
            _world = new VillageOfShadows.Core.World.World(width: 120, height: 70, config);

            // Seed trees (zoals je al deed, of via system)
            _world.Get(20, 20).HasTree = true; // of maak een seed helper

            // Entities in Core
            _world.Villagers.Add(new VillageOfShadows.Core.Entities.Villager(_world.TileCenter(10, 10)));
            _world.Villagers.Add(new VillageOfShadows.Core.Entities.Villager(_world.TileCenter(14, 12)));

            // Simulation pipeline
            _rng = new VillageOfShadows.Core.Utils.RandomAdapter(seed: 777);
            _sim = new VillageOfShadows.Core.Simulation.WorldSimulation(_rng)
                .AddSystem(new VillageOfShadows.Core.Simulation.TreeGrowthSystem())
                .AddSystem(new VillageOfShadows.Core.Simulation.VillagerWanderSystem());

            // Camera
            _camera = new Camera2D { MinZoom = 0.5f, MaxZoom = 4f };
            _prevScroll = Mouse.GetState().ScrollWheelValue;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _sb = new SpriteBatch(GraphicsDevice);

            // 1x1 pixel (als je die nog niet had)
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });

            // textures
            var grass = TextureFactory.CreateGrass(GraphicsDevice, size: 16, seed: 1337);
            var trees = new[]
            {
    TextureFactory.CreateTreeStage(GraphicsDevice, TreeStage.Sapling),
    TextureFactory.CreateTreeStage(GraphicsDevice, TreeStage.Young),
    TextureFactory.CreateTreeStage(GraphicsDevice, TreeStage.Mature),
};

            // renderers
            _worldRenderer = new WorldRenderer(grass, trees);
            _entityRenderer = new EntityRenderer(_pixel);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            HandleCamera(dt);
            _sim.Update(_world, dt);

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _sb.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.GetViewMatrix());
            _worldRenderer.Draw(_sb, _world);
            _entityRenderer.DrawVillagers(_sb, _world);
            _sb.End();

            base.Draw(gameTime);
        }

        private void HandleCamera(float dt)
        {
            var kb = Keyboard.GetState();
            var ms = Mouse.GetState();

            float panSpeed = 600f; // world pixels per second
            Vector2 move = Vector2.Zero;

            // --- Movement ---
            if (kb.IsKeyDown(Keys.W) || kb.IsKeyDown(Keys.Up)) move.Y -= 1;
            if (kb.IsKeyDown(Keys.S) || kb.IsKeyDown(Keys.Down)) move.Y += 1;
            if (kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.Left)) move.X -= 1;
            if (kb.IsKeyDown(Keys.D) || kb.IsKeyDown(Keys.Right)) move.X += 1;

            if (move != Vector2.Zero)
            {
                move.Normalize();

                // Divide by zoom so pan feels consistent
                _camera.Move(move * (panSpeed * dt) / _camera.Zoom);
            }

            // --- Zoom ---
            int scroll = ms.ScrollWheelValue;
            int delta = scroll - _prevScroll;
            _prevScroll = scroll;

            if (delta != 0)
            {
                float zoomFactor = delta > 0 ? 1.10f : 0.90f;

                _camera.ZoomAtScreenPoint(
                    zoomFactor,
                    ms.Position.ToVector2(),
                    GraphicsDevice);
            }

            // --- Clamp inside world ---
            int worldPixelW = _world.Width * _world.Config.TileSize;
            int worldPixelH = _world.Height * _world.Config.TileSize;

            _camera.ClampToWorld(worldPixelW, worldPixelH, GraphicsDevice);
        }
    }
}
