using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using VillageOfShadows.Core;
using VillageOfShadows.Core.Domain;
using VillageOfShadows.Core.Simulation;
using VillageOfShadows.Game.GameFlow;
using VillageOfShadows.Game.UI;

namespace VillageOfShadows.Game;

public sealed class Game1 : Microsoft.Xna.Framework.Game
{
    private GraphicsDeviceManager _graphics = null!;
    private SpriteBatch _spriteBatch = null!;
    private SpriteFont _font = null!;
    private Texture2D _pixel = null!;

    private GameManager _gm = null!;
    private MouseState _prevMouse;

    private SimpleButton _btnNext = null!;
    private SimpleButton _btnHidden = null!;
    private SimpleButton _btnCareful = null!;
    private SimpleButton _btnAggro = null!;
    private SimpleButton _btnResolve = null!;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();

        // Core wiring
        var cfg = new GameConfig();
        var rng = new RandomAdapter(seed: 12345);
        var sim = new SimulationService(cfg, rng);

        var village = CreateInitialVillage(cfg, rng);
        var vampire = new Vampire();

        _gm = new GameManager(sim, village, vampire);

        // Buttons
        _btnNext = new SimpleButton(new Rectangle(20, 20, 240, 40), "Advance Day to Night");
        _btnHidden = new SimpleButton(new Rectangle(20, 80, 240, 40), "Night: Stay Hidden");
        _btnCareful = new SimpleButton(new Rectangle(20, 130, 240, 40), "Night: Feed Carefully");
        _btnAggro = new SimpleButton(new Rectangle(20, 180, 240, 40), "Night: Feed Aggressively");
        _btnResolve = new SimpleButton(new Rectangle(20, 230, 240, 40), "Resolve Night");
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Add a SpriteFont named "DefaultFont" via MonoGame Content Pipeline
        _font = Content.Load<SpriteFont>("DefaultFont");

        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    protected override void Update(GameTime gameTime)
    {
        var mouse = Mouse.GetState();

        if (_gm.Phase == GamePhase.Day)
        {
            if (_btnNext.IsClicked(mouse, _prevMouse))
                _gm.AdvanceFromDayToNight();
        }
        else if (_gm.Phase == GamePhase.NightPlanning)
        {
            if (_btnHidden.IsClicked(mouse, _prevMouse))
                _gm.ChooseNightOrder(new NightOrder(NightOrderType.StayHidden));

            if (_btnCareful.IsClicked(mouse, _prevMouse))
                _gm.ChooseNightOrder(new NightOrder(NightOrderType.FeedCarefully));

            if (_btnAggro.IsClicked(mouse, _prevMouse))
                _gm.ChooseNightOrder(new NightOrder(NightOrderType.FeedAggressively));
        }
        else if (_gm.Phase == GamePhase.NightResolve)
        {
            if (_btnResolve.IsClicked(mouse, _prevMouse))
                _gm.ResolveNight();
        }
        else if (_gm.Phase == GamePhase.GameOver)
        {
            // MVP: ESC to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
        }

        _prevMouse = mouse;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        DrawHud();
        DrawButtons();
        DrawVillageDots();

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void DrawHud()
    {
        var h = _gm.Hud;

        int x = 300, y = 20, lh = 22;
        _spriteBatch.DrawString(_font, $"Day: {h.DayNumber}", new Vector2(x, y), Color.White); y += lh;
        _spriteBatch.DrawString(_font, $"Population: {h.Population}", new Vector2(x, y), Color.White); y += lh;
        _spriteBatch.DrawString(_font, $"Food: {h.Food}", new Vector2(x, y), Color.White); y += lh;
        _spriteBatch.DrawString(_font, $"Wood: {h.Wood}", new Vector2(x, y), Color.White); y += lh;
        _spriteBatch.DrawString(_font, $"Suspicion: {h.Suspicion}/100", new Vector2(x, y), Color.White); y += lh;
        _spriteBatch.DrawString(_font, $"Vampire Hunger: {h.Hunger}/100", new Vector2(x, y), Color.White); y += lh;

        y += 10;
        _spriteBatch.DrawString(_font, $"Phase: {_gm.Phase}", new Vector2(x, y), Color.White); y += lh;

        if (h.LastReportLines.Count > 0)
        {
            y += 10;
            _spriteBatch.DrawString(_font, "Last Night Report:", new Vector2(x, y), Color.White); y += lh;
            foreach (var line in h.LastReportLines.Take(8))
            {
                _spriteBatch.DrawString(_font, "- " + line, new Vector2(x, y), Color.White); y += lh;
            }
        }
    }

    private void DrawButtons()
    {
        if (_gm.Phase == GamePhase.Day)
        {
            _btnNext.Draw(_spriteBatch, _font, _pixel);
        }
        else if (_gm.Phase == GamePhase.NightPlanning)
        {
            _btnHidden.Draw(_spriteBatch, _font, _pixel);
            _btnCareful.Draw(_spriteBatch, _font, _pixel);
            _btnAggro.Draw(_spriteBatch, _font, _pixel);
        }
        else if (_gm.Phase == GamePhase.NightResolve)
        {
            _btnResolve.Draw(_spriteBatch, _font, _pixel);
        }
        else if (_gm.Phase == GamePhase.GameOver)
        {
            _spriteBatch.DrawString(_font, "GAME OVER (ESC to quit)", new Vector2(20, 300), Color.White);
        }
    }

    private void DrawVillageDots()
    {
        // very simple visualization
        foreach (var v in _gm.Village.Villagers.Where(v => v.IsAlive))
        {
            var r = new Rectangle(20 + v.X * 6, 380 + v.Y * 6, 4, 4);
            _spriteBatch.Draw(_pixel, r, Color.White);
        }

        // “vampire marker” (just a red square)
        _spriteBatch.Draw(_pixel, new Rectangle(20, 360, 8, 8), Color.Red);
        _spriteBatch.DrawString(_font, "Vampire", new Vector2(35, 355), Color.White);
    }

    private static Village CreateInitialVillage(GameConfig cfg, IRandom rng)
    {
        var v = new Village();
        v.Resources.Food = cfg.StartFood;
        v.Resources.Wood = cfg.StartWood;

        for (int i = 0; i < cfg.InitialVillagers; i++)
        {
            var villager = new Villager
            {
                AgeDays = rng.Next(18 * 365, 50 * 365),
                Profession = (Profession)rng.Next(0, 3),
                X = rng.Next(0, 60),
                Y = rng.Next(0, 20),
            };
            v.Villagers.Add(villager);
        }

        return v;
    }
}