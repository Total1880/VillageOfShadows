using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using VillageOfShadows.Core.Config;
using VillageOfShadows.Core.Entities;
using VillageOfShadows.Core.Simulation;
using VillageOfShadows.Core.Utils;
using VillageOfShadows.Core.World;
using VillageOfShadows.Game.Rendering;

namespace VillageOfShadows
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private readonly GraphicsDeviceManager _graphics;

        private World _world = null!;
        private SpriteBatch _sb = null!;
        private Camera2D _camera = null!;
        private WorldSimulation _sim = null!;
        private IRandom _rng = null!;
        private int _prevScroll;
        private MouseState _prevMouse;

        private WorldRenderer _worldRenderer = null!;
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
            var config = new WorldConfig { TileSize = 16 };
            _world = new World(width: 120, height: 70, config);
            _rng = new RandomAdapter();

            // Seed trees (tile entity)
            for (int i = 0; i < 10; i++)
            {


                _world.TryPlaceTileEntity(new PineTree(), _rng.Next(5, 30), _rng.Next(5, 30));
                _world.TryPlaceTileEntity(new OakTree(), _rng.Next(5, 30), _rng.Next(5, 30));
                _world.TryPlaceTileEntity(new AppleTree(), _rng.Next(5, 30), _rng.Next(5, 30));
            }

            // Seed villagers (list entities)
            _world.TrySpawnActor(new Villager(), _rng.Next(5, 30), _rng.Next(5, 30));
            _world.TrySpawnActor(new Villager(), _rng.Next(5, 30), _rng.Next(5, 30));


            // Simulation pipeline
            _sim = new WorldSimulation(_rng)
                .AddSystem(new TreeGrowthSystem())
                .AddSystem(new VillagerWanderSystem())
                .AddSystem(new VillagerJobSystem())
                .AddSystem(new ChopTreeSystem());

            // Camera
            _camera = new Camera2D { MinZoom = 0.5f, MaxZoom = 4f };
            _prevScroll = Mouse.GetState().ScrollWheelValue;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _sb = new SpriteBatch(GraphicsDevice);

            // 1x1 pixel
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });

            // Textures
            var grass = TextureFactory.CreateGrass(GraphicsDevice, size: 16, seed: 1337);

            // Entity rendering pipeline (dispatcher)
            IEntityRenderer entityRenderer = new EntityRenderer(
                new PineTreeRenderer(_pixel),
                new OakTreeRenderer(_pixel),
                new AppleTreeRenderer(_pixel),
                new VillagerRenderer(_pixel)
            );

            // World renderer tekent tiles + tile-entities via entityRenderer
            _worldRenderer = new WorldRenderer(grass, entityRenderer);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            HandleCamera(dt);
            HandleInput();
            _sim.Update(_world, dt);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _sb.Begin(
                samplerState: SamplerState.PointClamp,
                transformMatrix: _camera.GetViewMatrix());

            _worldRenderer.Draw(_sb, _world);

            _sb.End();

            base.Draw(gameTime);
        }

        private void HandleCamera(float dt)
        {
            var kb = Keyboard.GetState();
            var ms = Mouse.GetState();

            float panSpeed = 600f;
            Vector2 move = Vector2.Zero;

            if (kb.IsKeyDown(Keys.W) || kb.IsKeyDown(Keys.Up)) move.Y -= 1;
            if (kb.IsKeyDown(Keys.S) || kb.IsKeyDown(Keys.Down)) move.Y += 1;
            if (kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.Left)) move.X -= 1;
            if (kb.IsKeyDown(Keys.D) || kb.IsKeyDown(Keys.Right)) move.X += 1;

            if (move != Vector2.Zero)
            {
                move.Normalize();
                _camera.Move(move * (panSpeed * dt) / _camera.Zoom);
            }

            int scroll = ms.ScrollWheelValue;
            int delta = scroll - _prevScroll;
            _prevScroll = scroll;

            if (delta != 0)
            {
                float zoomFactor = delta > 0 ? 1.10f : 0.90f;
                _camera.ZoomAtScreenPoint(zoomFactor, ms.Position.ToVector2(), GraphicsDevice);
            }

            int worldPixelW = _world.Width * _world.Config.TileSize;
            int worldPixelH = _world.Height * _world.Config.TileSize;
            _camera.ClampToWorld(worldPixelW, worldPixelH, GraphicsDevice);
        }

        private void HandleInput()
        {
            var mouse = Mouse.GetState();

            bool leftClicked =
                mouse.LeftButton == ButtonState.Pressed &&
                _prevMouse.LeftButton == ButtonState.Released;

            if (leftClicked)
            {
                Point worldPoint = ScreenToWorld(mouse.Position);

                int tx = worldPoint.X / _world.Config.TileSize;
                int ty = worldPoint.Y / _world.Config.TileSize;

                if (_world.InBounds(tx, ty))
                {
                    foreach (var entity in _world.GetTileEntitiesOnTile(tx, ty))
                    {
                        if (entity is Tree tree)
                        {
                            MarkTreeForChop(tree);
                            break;
                        }
                    }
                }
            }

            _prevMouse = mouse;
        }

        private void MarkTreeForChop(Tree tree)
        {
            if (tree.MarkedForChop)
                return;

            bool alreadyHasJob = _world.GetJobs<ChopTreeJob>()
                .Any(j => j.TreeId == tree.EntityId && !j.IsCompleted);

            if (alreadyHasJob)
                return;

            tree.MarkedForChop = true;
            _world.AddJob(new ChopTreeJob(tree.EntityId));
        }

        private Point ScreenToWorld(Point screenPoint)
        {
            var inverse = Matrix.Invert(_camera.GetTransform());
            Vector2 world = Vector2.Transform(screenPoint.ToVector2(), inverse);
            return world.ToPoint();
        }
    }
}