using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;

namespace GeonBit.UI.Source.Entities
{
    /// <summary>
    /// This PanelGamePad contains a DropDown with gamepad support.
    /// </summary>
    public class DropDownPanel : PanelGamePad
    {
        /// <summary>
        /// The currently selected drop down reference.
        /// </summary>
        public DropDownGamePad SelectedDropDown { get; internal set; }

        /// <summary>
        /// Creates the GamePad-Panel with the default skin and based on a regular Panel-Entity.
        /// </summary>
        /// <param name="size">Panel size.</param>
        /// <param name="anchor">Position anchor.</param>
        /// <param name="offset">Offset from anchor position.</param>
        /// <param name="entities">The items you want to add to the SelectList.</param>
        public DropDownPanel(
            Vector2 size,
            Anchor anchor = Anchor.Center,
            Vector2? offset = null,
            params Entity[] entities)
            : base(size, anchor, offset)
        {
            Padding = new Vector2(6);

            for (int i = 0; i < entities.Length; i++)
            {
                if (string.IsNullOrEmpty(entities[i].Identifier))
                {
                    entities[i].Identifier = GetIdentifier(HierarchyIdentifier.PanelContent);
                }

                entities[i].Size = new Vector2(0, 1f / entities.Length);
                entities[i].Padding = Vector2.Zero;
                entities[i].SpaceAfter = Vector2.Zero;
                entities[i].SpaceBefore = Vector2.Zero;

                AddChild(entities[i]);
            }

            InvisiblePanel = true;
        }

        /// <summary>
        /// Select an item from the DropDown by using the SelectedIndex.
        /// </summary>
        protected override void SelectContent(PanelDirection direction)
        {
            if (direction == PanelDirection.Down)
            {
                if (SelectedDropDown != null)
                {
                    SelectedDropDown.SelectNextItem();
                }
                else base.SelectContent(direction);
            }
            else if (direction == PanelDirection.Up)
            {
                if (SelectedDropDown != null)
                {
                    SelectedDropDown.SelectPreviousItem();
                }
                else base.SelectContent(direction);
            }
        }
    }
}
