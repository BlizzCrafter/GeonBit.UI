using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;

namespace GeonBit.UI.Source.Input
{
    /// <summary>
    /// Use the GamePadSetup class to define some basic rules for your GamePad supported GeonBit.UI GUI.
    /// </summary>
    public static class GamePadSetup
    {
        /// <summary>
        /// Default function we use to generate tooltip text entities.
        /// </summary>
        /// <param name="source">Source entity.</param>
        /// <returns>Entity to use for tooltip text.</returns>
        public static Entity GenerateGamePadTooltipFunc(Entity source)
        {
            // no tooltip text? return null
            if (source.ToolTipText == null) return null;

            // create tooltip paragraph
            var tooltip = new Paragraph(source.ToolTipText, size: new Vector2(500, -1));
            tooltip.BackgroundColor = Color.Black;

            // add callback to update tooltip position
            tooltip.BeforeDraw += (Entity ent) =>
            {
                // get dest rect and calculate tooltip position based on size and mouse position
                var destRect = tooltip.GetActualDestRect();
                var sourceVec = new Vector2(
                    source.GetActualDestRect().X + source.GetActualDestRect().Width / 2, 
                    source.GetActualDestRect().Y + source.GetActualDestRect().Height);
                var position = sourceVec + source.Padding + new Vector2(-destRect.Width / 2, -destRect.Height - 20);

                // make sure tooltip is not out of screen boundaries
                var screenBounds = UserInterface.Active.Root.GetActualDestRect();
                if (position.Y < screenBounds.Top) position.Y = screenBounds.Top;
                if (position.Y > screenBounds.Bottom - destRect.Height) position.Y = screenBounds.Bottom - destRect.Height;
                if (position.X < screenBounds.Left) position.X = screenBounds.Left;
                if (position.X > screenBounds.Right - destRect.Width) position.X = screenBounds.Right - destRect.Width;

                // update tooltip position
                tooltip.SetAnchorAndOffset(Anchor.TopLeft, position / UserInterface.Active.GlobalScale);
            };
            tooltip.CalcTextActualRectWithWrap();
            tooltip.BeforeDraw(tooltip);

            // return tooltip object
            return tooltip;
        }

        /// <summary>
        /// The default Color of an entity.
        /// </summary>
        public static Color DefaultColor = Color.White;
        /// <summary>
        /// The selected Color of an entity.
        /// </summary>
        public static Color SelectedColor = Color.LightPink;

        /// <summary>
        /// Xbox Button_A color.
        /// </summary>
        public static Color ButtonA = new Color(60, 219, 78);
        /// <summary>
        /// Xbox Button_X color.
        /// </summary>
        public static Color ButtonX = new Color(64, 204, 208);
        /// <summary>
        /// Xbox Button_Y color.
        /// </summary>
        public static Color ButtonY = new Color(236, 219, 51);
        /// <summary>
        /// Xbox Button_B color.
        /// </summary>
        public static Color ButtonB = new Color(208, 66, 66);

        /// <summary>
        /// The default Skin of an entity.
        /// </summary>
        public static PanelSkin DefaultSkin = PanelSkin.Simple;
        /// <summary>
        /// The selected Skin of an entity.
        /// </summary>
        public static PanelSkin SelectedSkin = PanelSkin.Fancy;

        /// <summary>
        /// How long does it take before a MessageBox should be confirmable?
        /// Don't make it to short to avoid that the MessageBox would close immediately.
        /// </summary>
        public static double MessageBoxTimeOut = 500;

        /// <summary>
        /// How long does it take before a clicked panel content should call the OnClick(); event?
        /// Don't make it to short to avoid that the clicked panel content triggers right after a panel was selected.
        /// </summary>
        public static double PanelContentClickedTimeOut = 100;

        /// <summary>
        /// Initialize the GamePadSetup class.
        /// </summary>
        public static void Initialize()
        {
            UserInterface.Active.MouseInputProvider.UpdateMousePosition(Vector2.Zero);
            UserInterface.Active.GenerateTooltipFunc = GenerateGamePadTooltipFunc;

            RichParagraphStyleInstruction.AddInstruction("BUTTON_A", new RichParagraphStyleInstruction(ButtonA, FontStyle.Bold, 2));
            RichParagraphStyleInstruction.AddInstruction("BUTTON_X", new RichParagraphStyleInstruction(ButtonX, FontStyle.Bold, 2));
            RichParagraphStyleInstruction.AddInstruction("BUTTON_Y", new RichParagraphStyleInstruction(ButtonY, FontStyle.Bold, 2));
            RichParagraphStyleInstruction.AddInstruction("BUTTON_B", new RichParagraphStyleInstruction(ButtonB, FontStyle.Bold, 2));
            RichParagraphStyleInstruction.AddInstruction("DPAD", new RichParagraphStyleInstruction(Color.Black, FontStyle.Bold, 1, Color.WhiteSmoke));
        }
    }
}
