using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;

namespace GeonBit.UI.Source.Entities
{
    /// <summary>
    /// This PanelGamePad contains a SelectList with gamepad support.
    /// </summary>
    public class SelectListPanel : PanelGamePad
    {
        private SelectListGamePad _SelectListGamePad;

        /// <summary>
        /// Creates the GamePad-Panel with the default skin and based on a regular Panel-Entity.
        /// </summary>
        /// <param name="size">Panel size.</param>
        /// <param name="anchor">Position anchor.</param>
        /// <param name="offset">Offset from anchor position.</param>
        /// <param name="lockFirstIndex">Locks the first entry. Useful for headers.</param>
        /// <param name="items">The items you want to add to the SelectList.</param>
        public SelectListPanel(
            Vector2 size, 
            Anchor anchor = Anchor.Center, 
            Vector2? offset = null, 
            bool lockFirstIndex = true, 
            params string[] items)
            : base(size, anchor, offset)
        {
            AddChild(_SelectListGamePad = new SelectListGamePad(lockFirstIndex, items) 
            { 
                Identifier = GetIdentifier(HierarchyIdentifier.PanelContent) 
            });
        }

        /// <summary>
        /// Select an item from the SelectList by using the SelectedIndex.
        /// </summary>
        protected override void SelectContent(PanelDirection direction)
        {
            if (direction == PanelDirection.Down)
            {
                if (_SelectListGamePad.SelectedIndex < _SelectListGamePad.Count - 1)
                {
                    _SelectListGamePad.SelectedIndex++;
                }
                else _SelectListGamePad.SelectedIndex = 0;
            }
            else if (direction == PanelDirection.Up)
            {
                if (_SelectListGamePad.SelectedIndex > 0)
                {
                    _SelectListGamePad.SelectedIndex--;
                }
                else _SelectListGamePad.SelectedIndex = _SelectListGamePad.Count - 1;
            }
        }
    }
}
