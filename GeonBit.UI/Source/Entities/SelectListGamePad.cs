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
            if (!HasSelectedValue) SelectNextItem();
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

        /// <summary>
        /// Calls the OnMouseWheelScroll() event internally.
        /// </summary>
        public void TriggerOnScroll(PanelDirection direction, bool thumbstickEvent)
        {
            if (thumbstickEvent)
            {
                int scrollDirection = 0;

                if (direction == PanelDirection.Up) scrollDirection = -1;
                else if (direction == PanelDirection.Down) scrollDirection = 1;

                ScrollPosition = ScrollPosition + scrollDirection;
            }
        }

        /// <summary>
        /// Triggers when the layout of the RootGrid changed.
        /// </summary>
        public void TriggerOnLayoutChange(PanelGrid rootGrid)
        {
        }

        #endregion Events

        /// <summary>
        /// Select the next item in the SelectList.
        /// </summary>
        public void SelectNextItem()
        {
            int selection = SelectedIndex;
            selection = NextItemCheck(selection);

            while (LockedItems.ContainsKey(selection) && LockedItems[selection])
            {
                selection = NextItemCheck(selection);
            }

            SelectedIndex = selection;
            ScrollToSelected();
        }
        private int NextItemCheck(int selection)
        {
            selection++;
            return MaxCountCheck(selection);
        }
        private int MaxCountCheck(int value)
        {
            if (value > Count - 1) return 0;
            else return value;
        }

        /// <summary>
        /// Select the previous item in the SelectList.
        /// </summary>
        public void SelectPreviousItem()
        {
            int selection = SelectedIndex;
            selection = PreviousItemCheck(selection);

            while (LockedItems.ContainsKey(selection) && LockedItems[selection])
            {
                selection = PreviousItemCheck(selection);
            }

            SelectedIndex = selection;
            ScrollToSelected();
        }
        private int PreviousItemCheck(int selection)
        {
            selection--;
            return MinCountCheck(selection);
        }
        private int MinCountCheck(int value)
        {
            if (value < 0) return Count - 1;
            else return value;
        }

        /// <summary>
        /// Create the select list with gamepad support.
        /// </summary>
        /// <param name="items">The items you want to add to the SelectList.</param>
        public SelectListGamePad(params string[] items)
            : base(Vector2.Zero, Anchor.Auto, Vector2.Zero, PanelSkin.ListBackground)
        {
            Identifier = "__SelectListGamePad";

            for (int i = 0; i < items.Length; i++)
            {
                AddItem(items[i]);
            }
        }
    }
}
