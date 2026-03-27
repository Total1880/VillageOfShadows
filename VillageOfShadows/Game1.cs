using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using VillageOfShadows.Core.Config;
using VillageOfShadows.Core.Entities;
using VillageOfShadows.Core.Entities.Components;
using VillageOfShadows.Core.Simulation;
using VillageOfShadows.Core.Utils;
using VillageOfShadows.Core.World;
using VillageOfShadows.Game.Rendering;
using VillageOfShadows.Game.State;
using VillageOfShadows.Game.Systems;
using VillageOfShadows.Game.UI;

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
        private KeyboardState _prevKeyboard;
        private SpriteFont _uiFont = null!;

        private readonly BuildState _buildState = new();
        private readonly PlacementSystem _placementSystem = new();
        private BuildMenuRenderer _buildMenuRenderer = null!;
        private PlacementPreviewRenderer _placementPreviewRenderer = null!;

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

            // Font
            _uiFont = Content.Load<SpriteFont>("DefaultFont"); // pas naam aan naar jouw spritefont asset

            // Textures
            var grass = TextureFactory.CreateGrass(GraphicsDevice, size: 16, seed: 1337);

            IEntityRenderer entityRenderer = new EntityRenderer(
                new PineTreeRenderer(_pixel),
                new OakTreeRenderer(_pixel),
                new AppleTreeRenderer(_pixel),
                new VillagerRenderer(_pixel),
                new StockpileRenderer(_pixel)
            );

            _worldRenderer = new WorldRenderer(grass, entityRenderer);
            _buildMenuRenderer = new BuildMenuRenderer(_pixel, _uiFont);
            _placementPreviewRenderer = new PlacementPreviewRenderer(_pixel);
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

            // World + placement preview in world space
            _sb.Begin(
                samplerState: SamplerState.PointClamp,
                transformMatrix: _camera.GetViewMatrix());

            _worldRenderer.Draw(_sb, _world);
            var mouseWorld = ScreenToWorld(Mouse.GetState().Position);

            _placementPreviewRenderer.Draw(_sb, _world, _buildState, mouseWorld);

            _sb.End();

            // UI in screen space
            _sb.Begin(samplerState: SamplerState.PointClamp);

            _buildMenuRenderer.Draw(_sb, _buildState);

            _sb.End();

            base.Draw(gameTime);
        }

        private void HandleCamera(float dt)
        {
            var kb = Keyboard.GetState();
            var ms = Mouse.GetState();

            float panSpeed = 600f;
            Vector2 move = Vector2.Zero;

            if (kb.IsKeyDown(Keys.Z) || kb.IsKeyDown(Keys.Up)) move.Y -= 1;
            if (kb.IsKeyDown(Keys.S) || kb.IsKeyDown(Keys.Down)) move.Y += 1;
            if (kb.IsKeyDown(Keys.Q) || kb.IsKeyDown(Keys.Left)) move.X -= 1;
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
            var keyboard = Keyboard.GetState();

            if (Pressed(keyboard, _prevKeyboard, Keys.Escape))
            {
                if (_buildState.IsPlacing)
                    _buildState.CancelPlacement();
                else if (_buildState.IsBuildMenuOpen)
                    _buildState.IsBuildMenuOpen = false;
                else
                    Exit();
            }

            HandleBuildMenuInput(keyboard);
            HandlePlacementInput(mouse);

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
            _prevKeyboard = keyboard;
        }

        private void HandleBuildMenuInput(KeyboardState keyboard)
        {
            if (Pressed(keyboard, _prevKeyboard, Keys.B))
            {
                _buildState.IsBuildMenuOpen = !_buildState.IsBuildMenuOpen;
            }

            if (!_buildState.IsBuildMenuOpen)
                return;

            if (Pressed(keyboard, _prevKeyboard, Keys.D1))
            {
                _buildState.StartPlacement(BuildType.Stockpile);
            }
        }

        private void HandlePlacementInput(MouseState mouse)
        {
            if (!_buildState.IsPlacing)
                return;

            Point worldPoint = ScreenToWorld(mouse.Position);

            int tileX = worldPoint.X / _world.Config.TileSize;
            int tileY = worldPoint.Y / _world.Config.TileSize;

            if (LeftClicked(mouse, _prevMouse))
            {
                bool placed = _placementSystem.TryPlace(_world, _buildState.SelectedBuild, tileX, tileY);
                if (placed)
                {
                    _buildState.CancelPlacement();
                }
            }

            if (RightClicked(mouse, _prevMouse))
            {
                _buildState.CancelPlacement();
            }
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

        private static bool Pressed(KeyboardState current, KeyboardState previous, Keys key)
    => current.IsKeyDown(key) && !previous.IsKeyDown(key);

        private static bool LeftClicked(MouseState current, MouseState previous)
            => current.LeftButton == ButtonState.Pressed && previous.LeftButton == ButtonState.Released;

        private static bool RightClicked(MouseState current, MouseState previous)
            => current.RightButton == ButtonState.Pressed && previous.RightButton == ButtonState.Released;

        private void DrawBuildMenu(SpriteBatch sb, SpriteFont font)
        {
            if (!_buildState.IsBuildMenuOpen)
                return;

            sb.Draw(_pixel, new Rectangle(20, 20, 220, 100), Color.Black * 0.7f);
            sb.DrawString(font, "Build Menu", new Vector2(30, 30), Color.White);
            sb.DrawString(font, "[1] Stockpile", new Vector2(30, 60), Color.White);
        }

        private void DrawPlacementPreview(SpriteBatch sb)
        {
            if (!_buildState.IsPlacing)
                return;

            Point worldPoint = ScreenToWorld(Mouse.GetState().Position);

            int ts = _world.Config.TileSize;
            int tileX = worldPoint.X / ts;
            int tileY = worldPoint.Y / ts;

            if (!_world.InBounds(tileX, tileY))
                return;

            bool canPlace = _world.GetTile(tileX, tileY).IsWalkable
                && !_world.GetTileEntitiesOnTile(tileX, tileY).Any(e => e is Building);

            Color color = canPlace ? Color.Lime * 0.4f : Color.Red * 0.4f;

            sb.Draw(_pixel, new Rectangle(tileX * ts, tileY * ts, ts, ts), color);
        }
    }
}