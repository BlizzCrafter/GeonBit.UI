using GeonBit.UI.Entities;
using GeonBit.UI.Source.Input;
using Microsoft.Xna.Framework;

namespace GeonBit.UI.Source.Entities
{
    /// <summary>
    /// DropDown is just like a list, but it only shows the currently selected value unless clicked on (the list is
    /// only revealed while interacted with).
    /// </summary>
    [System.Serializable]
    public class DropDownGamePad : DropDown, IEntityGamePad
    {
        /// <summary>
        /// Static ctor.
        /// </summary>
        static DropDownGamePad()
        {
            Entity.MakeSerializable(typeof(DropDownGamePad));
        }

        #region Events

        /// <summary>
        /// Calls the DoOnClick() event internally.
        /// </summary>
        public void TriggerOnButtonClick()
        {
            DoOnClick();

            if (!_Selected)
            {
                _Selected = true;
                ArrowDownImage.Visible = true;
                SetSelectedDropDownInParent(this); //Select this DropDown.
            }
            else
            {
                _Selected = false;
                ArrowDownImage.Visible = false;
                SetSelectedDropDownInParent(null); //Deselect this DropDown.
            }

            //Initial selection. This is happening automatically without user input.
            if (!HasSelectedValue) SelectNextItem();
        }
        /// <summary>
        /// Calls the DoOnMouseEnter() event internally.
        /// </summary>
        public void TriggerOnSelect()
        {
            DoOnMouseEnter();

            SelectedTextPanel.FillColor = GamePadSetup.SelectedColor;
        }
        /// <summary>
        /// Calls the DoOnMouseLeave() event internally.
        /// </summary>
        public void TriggerOnDeSelect()
        {
            DoOnMouseLeave();

            SelectedTextPanel.FillColor = GamePadSetup.DefaultColor;
            ArrowDownImage.Visible = false;

            SetSelectedDropDownInParent(null);
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
        public void TriggerOnScroll(PanelDirection direction)
        {
            DoOnMouseWheelScroll();
        }

        #endregion Events

        private bool _Selected;

        /// <summary>
        /// Select the next item in the SelectList.
        /// </summary>
        public void SelectNextItem()
        {
            int selection = SelectedIndex;
            selection = NextItemCheck(selection);

            while (SelectList.LockedItems.ContainsKey(selection) && SelectList.LockedItems[selection])
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

            while (SelectList.LockedItems.ContainsKey(selection) && SelectList.LockedItems[selection])
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

        private void SetSelectedDropDownInParent(DropDownGamePad dropDownGamePad)
        {
            if (Parent is DropDownPanel)
            {
                ((DropDownPanel)Parent).SelectedDropDown = dropDownGamePad;
            }
        }

        /// <summary>
        /// Create the DropDown list with gamepad support.
        /// </summary>
        /// <param name="items">The items you want to add to the DropDown.</param>
        public DropDownGamePad(params string[] items) :
            base(Vector2.Zero, Anchor.Auto, Vector2.Zero, PanelSkin.ListBackground, null, false)
        {
            Identifier = PanelGamePad.GetIdentifier(HierarchyIdentifier.PanelContent);

            for (int i = 0; i < items.Length; i++)
            {
                AddItem(items[i]);
            }
        }
    }
}
