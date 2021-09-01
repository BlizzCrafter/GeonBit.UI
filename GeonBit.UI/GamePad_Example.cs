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
            UserInterface.Active.ShowCursor = false;
            //UserInterface.Active.DebugDraw = true;

            spriteBatch = new SpriteBatch(GraphicsDevice);

            int _ScreenWidth = graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            int _ScreenHeight = graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            graphics.PreferredBackBufferWidth = (int)_ScreenWidth / 2;
            graphics.PreferredBackBufferHeight = (int)_ScreenHeight / 2;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            UserInterface.Active.GlobalScale = 0.5f;

            GamePadSetup.Initialize();

            InitGamePadExample();
        }

        private void InitGamePadExample()
        {
            _RootGridPanel = new PanelGrid(RootGridLayout.SmallCornersVerticals, Anchor.TopRight);
            UserInterface.Active.AddEntity(_RootGridPanel);

            PanelGrid panelGrid = new PanelGrid(new Vector2(0, 0), 24, new Vector2(0.33f, -1), Anchor.AutoInline) { Name = "Panel Grid" };
            {                
                for (int i = 0; i < panelGrid.Children.Count; i++)
                {
                    if (panelGrid.Children[i] is Panel)
                    {
                        // add header
                        panelGrid.Children[i].AddChild(new Header($"Panel - #{i}"));

                        // add some buttons
                        panelGrid.Children[i].AddChild(new ButtonGamePad("Hello World", ButtonSkin.Default) { ToolTipText = $"This is Test-Tooltip #{i}", OnClick = SayHelloWorldClicked, Identifier = PanelGamePad.GetIdentifier(HierarchyIdentifier.PanelContent) });
                        panelGrid.Children[i].AddChild(new ButtonGamePad("Toggle", ButtonSkin.Default) { ToggleMode = true, Identifier = PanelGamePad.GetIdentifier(HierarchyIdentifier.PanelContent) });
                    }
                }
            }

            PanelGrid selectListGrid = new PanelGrid(new Vector2(0, 0), 6, new Vector2(0.33f, -1), Anchor.AutoInline) { Name = "Select List" };
            {
                for (int i = 0; i < selectListGrid.Children.Count; i++)
                {
                    if (selectListGrid.Children[i] is Panel)
                    {
                        // add header
                        selectListGrid.Children[i].AddChild(new Header($"Panel - #{i}"));
                    }
                }
            }

            _RootGridPanel.GetGridPanel(Anchor.Center).AddChild(
                new PanelTabsGamePad(TabLocation.Top, 
                    panelGrid, 
                    selectListGrid));
            _RootGridPanel.GetGridPanel(Anchor.TopCenter).AddChild(
                new PanelBar(
                    new ButtonGamePad("Button-1", anchor: Anchor.AutoInline) { ToolTipText = "This is Test-Tooltip #1", Identifier = PanelGamePad.GetIdentifier(HierarchyIdentifier.PanelContent) },
                    new ButtonGamePad("Non-Selectable", anchor: Anchor.AutoInline),
                    new ButtonGamePad("Button-3", anchor: Anchor.AutoInline) { ToolTipText = "This is Test-Tooltip #3", Identifier = PanelGamePad.GetIdentifier(HierarchyIdentifier.PanelContent) },
                    new ButtonGamePad("Button-4", anchor: Anchor.AutoInline) { ToolTipText = "This is Test-Tooltip #4", Identifier = PanelGamePad.GetIdentifier(HierarchyIdentifier.PanelContent) },
                    new ButtonGamePad("Toggle", ButtonSkin.Alternative, anchor: Anchor.AutoInline) { ToggleMode = true, Identifier = PanelGamePad.GetIdentifier(HierarchyIdentifier.PanelContent) }
                    ));
            _RootGridPanel.GetGridPanel(Anchor.TopRight).AddChild(
                new PanelBar(
                        new Icon(IconType.Heart, Anchor.Center) { MinSize = new Vector2(0, 0) } 
                    ));
            _RootGridPanel.GetGridPanel(Anchor.BottomCenter).AddChild(
                new PanelBar(
                        new RichParagraph(@"{{BUTTON_A}}A{{DEFAULT}}: Accept {{BUTTON_B}}B{{DEFAULT}}: Back {{MONO}}DPad{{DEFAULT}}: Select {{MONO}}LB|RB{{DEFAULT}}: Switch", 
                        Anchor.Center, scale: 2f) 
                        { AlignToCenter = true, WrapWords = false, BreakWordsIfMust = false, AddHyphenWhenBreakWord = false })
                    );

            base.Initialize();
        }

        private void SayHelloWorldClicked(Entity entity)
        {
            _RootGridPanel.Enabled = false;

            _MessageBox = new MessageBoxGamePad();
            _MessageBox.ShowMessageBox(
                $"Message from {PanelGamePad.GetIdentifier(entity.Parent.Identifier)}",
                "Hello, GamePad-User!",
                MessageBoxOptions());
        }

        private MsgBoxOption[] MessageBoxOptions()
        {
            return new MsgBoxOption[]
            {
                new MsgBoxOption("Yes", MsgBoxClicked),
                new MsgBoxOption("No", MsgBoxClicked),
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
