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

        PanelGrid _RootGridPanel, _GridPanel;
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
            //int topPanelHeight = 40;
            //Panel topPanel = new Panel(new Vector2(0, topPanelHeight + 2), GamePadSetup.DefaultSkin, Anchor.TopCenter);
            //topPanel.Padding = Vector2.Zero;
            //UserInterface.Active.AddEntity(topPanel);

            //for (int i = 0; i < 3; i++)
            //{
            //    Button button = new Button($"Button #{i}", ButtonSkin.Default, Anchor.AutoInline, new Vector2(0.33f, topPanelHeight - 2));
            //    button.Offset = new Vector2(5f, 2.5f);
            //    topPanel.AddChild(button);
            //}

            //int leftPanelWidth = 65;
            //Panel leftPanel = new Panel(new Vector2(leftPanelWidth + 2, 0.8f), GamePadSetup.DefaultSkin, Anchor.CenterLeft);
            //leftPanel.Padding = Vector2.Zero;
            //UserInterface.Active.AddEntity(leftPanel);

            //for (int i = 0; i < 9; i++)
            //{
            //    Button button = new Button($"#{i}", ButtonSkin.Default, Anchor.AutoInline, new Vector2(0, leftPanelWidth - 2));
            //    button.Offset = new Vector2(2.5f, 5f);
            //    leftPanel.AddChild(button);
            //}

            //int rightPanelWidth = 65;
            //Panel rightPanel = new Panel(new Vector2(rightPanelWidth + 2, 0.8f), GamePadSetup.DefaultSkin, Anchor.CenterRight);
            //rightPanel.Padding = Vector2.Zero;
            //UserInterface.Active.AddEntity(rightPanel);

            //for (int i = 0; i < 9; i++)
            //{
            //    Button button = new Button($"#{i}", ButtonSkin.Default, Anchor.AutoInline, new Vector2(0, rightPanelWidth - 2));
            //    button.Offset = new Vector2(2.5f, 5f);
            //    rightPanel.AddChild(button);
            //}

            //int bottomPanelHeight = 40;
            //Panel bottomPanel = new Panel(new Vector2(0, bottomPanelHeight + 2), GamePadSetup.DefaultSkin, Anchor.BottomCenter);
            //bottomPanel.Padding = Vector2.Zero;
            //UserInterface.Active.AddEntity(bottomPanel);

            //for (int i = 0; i < 3; i++)
            //{
            //    Button button = new Button($"Button #{i}", ButtonSkin.Default, Anchor.AutoInline, new Vector2(0.33f, bottomPanelHeight - 2));
            //    button.Offset = new Vector2(5f, 2.5f);
            //    bottomPanel.AddChild(button);
            //}

            _RootGridPanel = new PanelGrid(RootGridLayout.SmallCornersVerticals);
            UserInterface.Active.AddEntity(_RootGridPanel);

            _GridPanel = new PanelGrid(new Vector2(0, 0), 24, new Vector2(0.33f, -1), Anchor.AutoInline);
            {
                for (int i = 0; i < _GridPanel.Children.Count; i++)
                {
                    if (_GridPanel.Children[i] is Panel)
                    {
                        // add header
                        _GridPanel.Children[i].AddChild(new Header($"Panel - #{i}"));

                        // add some buttons
                        _GridPanel.Children[i].AddChild(new Button("Say Hello", ButtonSkin.Default) { OnClick = SayHelloClicked, Identifier = GamePadSetup.GetIdentifier(HierarchyIdentifier.PanelContent) });
                        _GridPanel.Children[i].AddChild(new Button("Say Cya", ButtonSkin.Default) { OnClick = SayCyaClicked, Identifier = GamePadSetup.GetIdentifier(HierarchyIdentifier.PanelContent) });
                    }
                }
            }
            _RootGridPanel.GetCenterPanel().AddChild(_GridPanel);

            UserInterface.Active.MouseInputProvider.UpdateMousePosition(Vector2.Zero);

            //UserInterface.Active.DebugDraw = true;

            base.Initialize();
        }

        private void SayHelloClicked(Entity entity)
        {
            _RootGridPanel.Enabled = false;

            _MessageBox = new MessageBoxGamePad();
            _MessageBox.ShowMessageBox(
                $"Message from {GamePadSetup.GetIdentifier(entity.Identifier)}",
                "Hello, GamePad-User!",
                MessageBoxOptions("Hello"));
        }

        private void SayCyaClicked(Entity entity)
        {
            _RootGridPanel.Enabled = false;

            _MessageBox = new MessageBoxGamePad();
            _MessageBox.ShowMessageBox(
                $"Message from {GamePadSetup.GetIdentifier(entity.Identifier)}",
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
