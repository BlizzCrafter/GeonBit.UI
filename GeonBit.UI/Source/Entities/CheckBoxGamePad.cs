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
    /// A checkbox entity, eg a label with a square you can mark as checked or uncheck.
    /// Holds a boolean value.
    /// </summary>
    [System.Serializable]
    public class CheckBoxGamePad : CheckBox, IEntityGamePad
    {
        /// <summary>
        /// Static ctor.
        /// </summary>
        static CheckBoxGamePad()
        {
            Entity.MakeSerializable(typeof(CheckBoxGamePad));
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
        public void TriggerOnScroll(PanelDirection direction, bool thumbstickEvent)
        {
            DoOnMouseWheelScroll();
        }

        #endregion Events

        /// <summary>
        /// Create a new checkbox entity.
        /// </summary>
        /// <param name="text">CheckBox label text.</param>
        /// <param name="anchor">Position anchor.</param>
        /// <param name="size">CheckBox size.</param>
        /// <param name="offset">Offset from anchor position.</param>
        /// <param name="isChecked">If true, this checkbox will be created as 'checked'.</param>
        public CheckBoxGamePad(string text, Anchor anchor = Anchor.Auto, Vector2? size = null, Vector2? offset = null, bool isChecked = false) 
            : base(text, anchor, size, offset, isChecked)
        {
            //We are doing clicking and event handling customly.
            ClickThrough = true;
        }
    }
}
