using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;

namespace GeonBit.UI.Source.Entities
{
    /// <summary>
    /// This PanelGamePad contains a SelectList with gamepad support.
    /// </summary>
    public class SelectListPanel : PanelGamePad
    {
        /// <summary>
        /// The SelectListGamePad-Reference.
        /// </summary>
        public SelectListGamePad SelectListGamePad { get; private set; }

        /// <summary>
        /// Creates the GamePad-Panel with the default skin and based on a regular Panel-Entity.
        /// </summary>
        /// <param name="size">Panel size.</param>
        /// <param name="headline">The headline for the SelectList.</param>
        /// <param name="anchor">Position anchor.</param>
        /// <param name="offset">Offset from anchor position.</param>
        /// <param name="items">The items you want to add to the SelectList.</param>
        public SelectListPanel(
            Vector2 size,
            RichParagraph headline,
            Anchor anchor = Anchor.Center, 
            Vector2? offset = null, 
            params string[] items)
            : base(size, anchor, offset)
        {
            AddChild(SelectListGamePad = new SelectListGamePad(items) 
            { 
                Identifier = GetIdentifier(HierarchyIdentifier.PanelContent),
                Padding = new Vector2(Padding.X, Padding.Y + SpaceAfter.Y + headline.GetCharacterActualSize().Y)
            });

            AddChild(headline);
        }

        /// <summary>
        /// Select an item from the SelectList by using the SelectedIndex.
        /// </summary>
        protected override void SelectContent(PanelDirection direction)
        {
            if (direction == PanelDirection.Down)
            {
                SelectListGamePad.SelectNextItem();
            }
            else if (direction == PanelDirection.Up)
            {
                SelectListGamePad.SelectPreviousItem();
            }
        }
    }
}
