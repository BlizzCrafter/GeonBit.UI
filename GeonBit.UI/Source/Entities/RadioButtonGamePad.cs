using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeonBit.UI.Source.Entities
{
    /// <summary>
    /// A Radio Button entity is like a checkbox (label with a box next to it that can be checked / unchecked) with the exception that whenever a radio button is checked, all its siblings are unchecked automatically.
    /// </summary>
    /// <remarks>
    /// Radio Buttons only affect their direct siblings, so if you need multiple groups of radio buttons on the same panel you can use internal panels to group them together.
    /// </remarks>
    [System.Serializable]
    public class RadioButtonGamePad : RadioButton, IEntityGamePad
    {
        /// <summary>
        /// Static ctor.
        /// </summary>
        static RadioButtonGamePad()
        {
            Entity.MakeSerializable(typeof(RadioButtonGamePad));
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
        }
        /// <summary>
        /// Calls the DoOnMouseLeave() event internally.
        /// </summary>
        public void TriggerOnDeSelect()
        {
            DoOnMouseLeave();
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

        /// <summary>
        /// Create the radio button.
        /// </summary>
        /// <param name="text">Radio button label text.</param>
        /// <param name="anchor">Position anchor.</param>
        /// <param name="size">Radio button size.</param>
        /// <param name="offset">Offset from anchor position.</param>
        /// <param name="isChecked">If true, radio button will be created as checked.</param>
        public RadioButtonGamePad(string text, Anchor anchor = Anchor.Auto, Vector2? size = null, Vector2? offset = null, bool isChecked = false) :
            base(text, anchor, size, offset, isChecked)
        {
            //We are doing clicking and event handling customly.
            ClickThrough = true;
        }

        /// <summary>
        /// Create radiobutton without text.
        /// </summary>
        public RadioButtonGamePad() : this(string.Empty)
        {
        }
    }
}
