using GeonBit.UI.Entities;
using GeonBit.UI.Source.Entities;
using GeonBit.UI.Source.Input;
using GeonBit.UI.Source.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using static GeonBit.UI.Utils.MessageBox;

namespace GeonBit.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class GamePad_Example : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        PanelGrid _RootGridPanel;
        MessageBoxGamePad _MessageBox;

        /// <summary>
        /// Create the game instance.
        /// </summary>
        public GamePad_Example()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.IsBorderless = true;
        }

        /// <summary>
        /// Initialize the main application.
        /// </summary>
        protected override void Initialize()
        {
            UserInterface.Initialize(Content, BuiltinThemes.hd);
            UserInterface.Active.UseRenderTarget = true;
            UserInterface.Active.IncludeCursorInRenderTarget = false;
            //UserInterface.Active.ShowCursor = false;

            spriteBatch = new SpriteBatch(GraphicsDevice);

            int _ScreenWidth = graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            int _ScreenHeight = graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            graphics.PreferredBackBufferWidth = (int)_ScreenWidth / 2;
            graphics.PreferredBackBufferHeight = (int)_ScreenHeight / 2;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            UserInterface.Active.GlobalScale = 0.5f;

            InitGamePadExample();
        }

        private void InitGamePadExample()
        {
            _RootGridPanel = new PanelGrid(RootGridLayout.SmallCornersVerticals);
            UserInterface.Active.AddEntity(_RootGridPanel);

            PanelGrid panelGrid = new PanelGrid(new Vector2(0, 0), 24, new Vector2(0.33f, -1), Anchor.AutoInline);
            {
                for (int i = 0; i < panelGrid.Children.Count; i++)
                {
                    if (panelGrid.Children[i] is Panel)
                    {
                        // add header
                        panelGrid.Children[i].AddChild(new Header($"Panel - #{i}"));

                        // add some buttons
                        panelGrid.Children[i].AddChild(new Button("Say Hello", ButtonSkin.Default) { OnClick = SayHelloClicked, Identifier = PanelGamePad.GetIdentifier(HierarchyIdentifier.PanelContent) });
                        panelGrid.Children[i].AddChild(new Button("Say Cya", ButtonSkin.Default) { OnClick = SayCyaClicked, Identifier = PanelGamePad.GetIdentifier(HierarchyIdentifier.PanelContent) });
                    }
                }
            }

            _RootGridPanel.GetGridPanel(Anchor.Center).AddChild(panelGrid);
            _RootGridPanel.GetGridPanel(Anchor.TopCenter).AddChild(
                new PanelBar(
                    new Button("Test1"),
                    new Button("Test2"),
                    new Button("Test3"),
                    new Button("Test4"),
                    new Button("Test5")
                    ));

            UserInterface.Active.MouseInputProvider.UpdateMousePosition(Vector2.Zero);

            //UserInterface.Active.DebugDraw = true;

            base.Initialize();
        }

        private void SayHelloClicked(Entity entity)
        {
            _RootGridPanel.Enabled = false;

            _MessageBox = new MessageBoxGamePad();
            _MessageBox.ShowMessageBox(
                $"Message from {PanelGamePad.GetIdentifier(entity.Identifier)}",
                "Hello, GamePad-User!",
                MessageBoxOptions("Hello"));
        }

        private void SayCyaClicked(Entity entity)
        {
            _RootGridPanel.Enabled = false;

            _MessageBox = new MessageBoxGamePad();
            _MessageBox.ShowMessageBox(
                $"Message from {PanelGamePad.GetIdentifier(entity.Identifier)}",
                "Cya, GamePad-User!",
                MessageBoxOptions("Cya"));
        }

        private MsgBoxOption[] MessageBoxOptions(string custom)
        {
            return new MsgBoxOption[]
            {
                new MsgBoxOption("OK", MsgBoxClicked),
                new MsgBoxOption(custom, MsgBoxClicked),
                new MsgBoxOption("Cancel", MsgBoxClicked)
            };
        }
        private bool MsgBoxClicked()
        {
            _RootGridPanel.Enabled = true;

            return true;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (!IsActive)
                return;

            // exit on escape
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            UserInterface.Active.Update(gameTime);
            if (_MessageBox != null) _MessageBox.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            UserInterface.Active.Draw(spriteBatch);

            GraphicsDevice.Clear(Color.CornflowerBlue);

            UserInterface.Active.DrawMainRenderTarget(spriteBatch);

            base.Draw(gameTime);
        }
    }
}
