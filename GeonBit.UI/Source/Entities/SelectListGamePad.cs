using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;

namespace GeonBit.UI.Source.Entities
{
    /// <summary>
    /// List of items (strings) user can scroll and pick from by using a gamepad.
    /// </summary>
    public class SelectListGamePad : SelectList, IEntityGamePad
    {
        /// <summary>
        /// Static ctor.
        /// </summary>
        static SelectListGamePad()
        {
            Entity.MakeSerializable(typeof(SelectListGamePad));
        }

        #region Events

        /// <summary>
        /// Calls the DoOnClick() event internally.
        /// </summary>
        public void TriggerOnButtonClick()
        {
            DoOnClick();
        }
        /// <summary>
        /// Calls the DoOnMouseEnter() event internally.
        /// </summary>
        public void TriggerOnSelect()
        {
            DoOnMouseEnter();

            //Initial selection. This is happening automatically without user input.
            if (SelectedIndex == -1) SelectedIndex = 0;
        }
        /// <summary>
        /// Calls the DoOnMouseLeave() event internally.
        /// </summary>
        public void TriggerOnDeSelect()
        {
            DoOnMouseLeave();

            //Revert selection.
            SelectedIndex = -1;
        }
        /// <summary>
        /// Calls the OnMouseDown() event internally.
        /// </summary>
        public void TriggerOnButtonDown()
        {
            DoOnMouseDown();
        }
        /// <summary>
        /// Calls the DoWhileMouseDown() event internally.
        /// </summary>
        public void TriggerDoWhileButtonDown()
        {
            DoWhileMouseDown();
        }
        /// <summary>
        /// Calls the DoWhileMouseHover() event internally.
        /// </summary>
        public void TriggerDoWhileHover()
        {
            DoWhileMouseHover();
        }
        /// <summary>
        /// Calls the DoOnMouseReleased() event internally.
        /// </summary>
        public void TriggerOnButtonReleased()
        {
            DoOnMouseReleased();
        }
        /// <summary>
        /// Calls the DoOnValueChange() event internally.
        /// </summary>
        public void TriggerOnValueChanged()
        {
            DoOnValueChange();
        }

        #endregion Events

        /// <summary>
        /// Create the select list with gamepad support.
        /// </summary>
        /// <param name="lockFirstIndex">Locks the first entry. Useful for headers in table like lists.</param>
        /// <param name="items">The items you want to add to the SelectList.</param>
        public SelectListGamePad(bool lockFirstIndex = true, params string[] items)
            : base(Vector2.Zero, Anchor.Auto, Vector2.Zero, PanelSkin.ListBackground)
        {
            LockedItems[0] = lockFirstIndex;

            for (int i = 0; i < items.Length; i++)
            {
                AddItem(items[i]);
            }
        }
    }
}
