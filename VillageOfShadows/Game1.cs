using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using VillageOfShadows.Core.World;

namespace VillageOfShadows
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private World _world = null!;
        private Texture2D _grass = null!;
        private Texture2D[] _trees = null!;
        private SpriteBatch _sb = null!;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            _world = new World(width: 120, height: 70, tileSize: 16, seed: 42);
            _world.SeedTrees(count: 120);
        }

        protected override void LoadContent()
        {
            _sb = new SpriteBatch(GraphicsDevice);

            _grass = TextureFactory.CreateGrass(GraphicsDevice, size: 16, seed: 1337);
            _trees = new[]
            {
    TextureFactory.CreateTreeStage(GraphicsDevice, TreeStage.Sapling),
    TextureFactory.CreateTreeStage(GraphicsDevice, TreeStage.Young),
    TextureFactory.CreateTreeStage(GraphicsDevice, TreeStage.Mature),
};

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _world.Update(dt);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _sb.Begin(samplerState: SamplerState.PointClamp); // pixel look

            // tiles
            for (int y = 0; y < _world.Height; y++)
                for (int x = 0; x < _world.Width; x++)
                {
                    var t = _world.Get(x, y);
                    var pos = new Vector2(x * _world.TileSize, y * _world.TileSize);
                    _sb.Draw(_grass, new Rectangle((int)pos.X, (int)pos.Y, _world.TileSize, _world.TileSize), Color.White);

                    if (t.HasTree)
                    {
                        var treeTex = _trees[(int)t.TreeStage];
                        // draw tree anchored to tile bottom-center
                        int drawX = x * _world.TileSize + (_world.TileSize / 2) - (treeTex.Width / 2);
                        int drawY = y * _world.TileSize + _world.TileSize - treeTex.Height;
                        _sb.Draw(treeTex, new Vector2(drawX, drawY), Color.White);
                    }
                }

            _sb.End();
        }
    }
}
