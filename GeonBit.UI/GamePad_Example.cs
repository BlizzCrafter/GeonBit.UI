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
            _RootGridPanel = new PanelGrid(RootGridLayout.SmallCorners, Anchor.TopRight) { Name = "Root Grid" };
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
                        panelGrid.Children[i].AddChild(new ButtonGamePad("Hello World") { ToolTipText = $"This is Test-Tooltip #{i}", OnClick = SayHelloWorldClicked });
                        panelGrid.Children[i].AddChild(new ButtonGamePad("Toggle") { ToggleMode = true });
                    }
                }
            }

            SelectListPanel selectListPanel = new SelectListPanel(new Vector2(0, 0), anchor: Anchor.TopCenter,
                headline: new RichParagraph(System.String.Format(
                    "{0,-8} {1,-8} {2,-8} {3,-8} {4,-8} {5,-8} {6,-8}", "{{LIST_TITLE}}",
                    "Name", "Class", "Level", "Weapon", "Test2", "Test3"), Anchor.TopLeft, scale: 1.6f)
                {
                    WrapWords = false,
                    BreakWordsIfMust = false,
                    AddHyphenWhenBreakWord = false
                },
                items: new string[]
                {
                    System.String.Format("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10}", "Joe", "Mage", "5", "Stuff", "Test2", "Test3"),
                    System.String.Format("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10}", "Ron", "Monk", "7", "Club", "Test2", "Test3"),
                    System.String.Format("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10}", "Alex", "Rogue", "3", "Daggers", "Test2", "Test3"),
                    System.String.Format("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10}", "Jim", "Paladin", "7", "Sword", "Test2", "Test3"),
                    System.String.Format("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10}", "Abe", "Cleric", "8", "Wand", "Test2", "Test3"),
                    System.String.Format("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10}", "James", "Warlock", "20", "Axe", "Test2", "Test3"),
                    System.String.Format("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10}", "Bob", "Bard", "1", "Knife", "Test2", "Test3"),
                    System.String.Format("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10}", "Frank", "Mage", "5", "Wand", "Test2", "Test3"),
                    System.String.Format("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10}", "Luis", "Monk", "7", "Daggers", "Test2", "Test3"),
                    System.String.Format("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10}", "Babo", "Rogue", "3", "Claws", "Test2", "Test3"),
                    System.String.Format("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10}", "Karin", "Paladin", "7", "Mace", "Test2", "Test3"),
                    System.String.Format("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10}", "Lexter", "Cleric", "8", "Spellbook", "Test2", "Test3"),
                    System.String.Format("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10}", "Lumi", "Warlock", "20", "Longsword", "Test2", "Test3"),
                    System.String.Format("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10}", "Jax", "Bard", "1", "Harp", "Test2", "Test3"),
                    System.String.Format("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10}", "Joli", "Mage", "5", "Spellbook", "Test2", "Test3"),
                    System.String.Format("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10}", "Keno", "Monk", "7", "Longstuff", "Test2", "Test3"),
                    System.String.Format("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10}", "Tom", "Rogue", "3", "Fists", "Test2", "Test3"),
                    System.String.Format("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10}", "Rex", "Paladin", "7", "Shield", "Test2", "Test3"),
                    System.String.Format("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10}", "Timbo", "Cleric", "8", "Potions", "Test2", "Test3"),
                    System.String.Format("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10}", "Alix", "Warlock", "20", "Spear", "Test2", "Test3"),
                    System.String.Format("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10}", "Rambo", "Bard", "1", "Mandolin", "Test2", "Test3")
                })
            { Name = "Select List" };
            selectListPanel.SelectListGamePad.ItemsScale = 1.3f;

            _RootGridPanel.GetGridPanel(Anchor.Center).AddChild(
                new PanelTabsGamePad(TabLocation.Top,
                    panelGrid,
                    selectListPanel));

            _RootGridPanel.GetGridPanel(Anchor.TopCenter).AddChild(
                new PanelBar(Orientation.Horizontal,
                    new ButtonGamePad("Button-1", anchor: Anchor.AutoInline) { ToolTipText = "This is Test-Tooltip #1" },
                    new ButtonGamePad("Non-Selectable", HierarchyIdentifier.None, anchor: Anchor.AutoInline) { Enabled = false },
                    new ButtonGamePad("Button-3", anchor: Anchor.AutoInline) { ToolTipText = "This is Test-Tooltip #3" },
                    new ButtonGamePad("Button-4", anchor: Anchor.AutoInline) { ToolTipText = "This is Test-Tooltip #4" },
                    new ButtonGamePad("Toggle", skin: ButtonSkin.Alternative, anchor: Anchor.AutoInline) { ToggleMode = true }
                    ));

            _RootGridPanel.GetGridPanel(Anchor.TopRight).AddChild(
                new PanelBar(Orientation.Horizontal,
                        new Icon(IconType.Heart, Anchor.Center) { MinSize = new Vector2(0, 0) } 
                    ));

            _RootGridPanel.GetGridPanel(Anchor.CenterRight)
                .AddChild(new DropDownPanel(new Vector2(0, 0),
                entities: new Entity[]
                {
                    new DropDownGamePad("Mage", "Monk", "Rogue", "Paladin", "Cleric", "Warlock", "Bard") { DefaultText = "Class" },
                    new DropDownGamePad("< 5", "< 10", "< 25", "< 50", "< 100") { DefaultText = "Level" },
                    new DropDownGamePad("Stuff", "Club", "Daggers", "Sword", "Wand", "Claws", "Mace", "Spellbook") { DefaultText = "Weapon" },
                    new ButtonGamePad("Button-1"),
                    new SliderGamePad(0, 100, Vector2.Zero),
                    new ButtonGamePad("Button-2"),
                    new ButtonGamePad("Non-Selectable", HierarchyIdentifier.None) { Enabled = false }
                }) { Name = "Filter Panel" })
                .OnClick = (e) => 
                {
                    //Demonstrates the possibility of switching the RootGridLayout during runtime.
                    if (_RootGridPanel.IsRootGridLayout)
                    {
                        _RootGridPanel.SetGridLayout(RootGridLayout.SmallCornersWideVerticals);
                    }
                    else _RootGridPanel.RevertGridLayout();
                };

            _RootGridPanel.GetGridPanel(Anchor.BottomCenter).AddChild(
                new PanelBar(Orientation.Horizontal,
                        new RichParagraph(@"{{BUTTON_A}}A{{DEFAULT}}: Accept {{BUTTON_B}}B{{DEFAULT}}: Back {{MONO}}DPad{{DEFAULT}}: Select {{MONO}}LB|RB{{DEFAULT}}: Switch",
                        Anchor.Center, scale: 2f)
                        { AlignToCenter = true, WrapWords = false, BreakWordsIfMust = false, AddHyphenWhenBreakWord = false })
                    );

            //Hide all empty Panels and make them unselectable by gamepad.
            _RootGridPanel.HideEmptyPanels();

            //The CenterRight GridPanel is not visible by default (panelGrid with index 0 is selected first).
            _RootGridPanel.GetGridPanel(Anchor.CenterRight).Visible = false;

            //The CenterRight GridPanel should only be visible when the SelectListPanel is visible.
            selectListPanel.OnVisiblityChange = (e) =>
            {
                Panel filterPanel = _RootGridPanel.GetGridPanel(Anchor.CenterRight);
                filterPanel.Visible = e.Visible;

                if (!e.Visible && _RootGridPanel.SelectedPanel == filterPanel)
                {
                    _RootGridPanel.ResetPanelSelection();
                }
            };
            panelGrid.OnVisiblityChange = (e) =>
            {
                _RootGridPanel.GetGridPanel(Anchor.CenterRight).Visible = e.Visible;
            };

            base.Initialize();
        }

        private void SayHelloWorldClicked(Entity entity)
        {
            _RootGridPanel.Enabled = false;

            _MessageBox = new MessageBoxGamePad();
            _MessageBox.ShowMessageBox(
                $"{PanelGamePad.GetIdentifier(entity.Parent.Identifier)}",
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
