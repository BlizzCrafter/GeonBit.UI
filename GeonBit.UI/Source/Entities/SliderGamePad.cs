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
    /// It's a regular slider from GeonBit.UI but with gamepad support.
    /// </summary>
    [System.Serializable]
    public class SliderGamePad : Slider, IEntityGamePad
    {
        /// <summary>
        /// Static ctor.
        /// </summary>
        static SliderGamePad()
        {
            Entity.MakeSerializable(typeof(SliderGamePad));
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
            //DoWhileMouseDown();
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
            //DoOnMouseReleased();
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
            int scrollDirection = 0;

            if (direction == PanelDirection.Left) scrollDirection = -1;
            else if (direction == PanelDirection.Right) scrollDirection = 1;

            Value = _value + scrollDirection * GetStepSize();
        }

        #endregion Events

        /// <summary>
        /// Create the slider with gamepad support.
        /// </summary>
        /// <param name="min">Min value (inclusive).</param>
        /// <param name="max">Max value (inclusive).</param>
        /// <param name="size">Slider size.</param>
        /// <param name="skin">Slider skin (texture).</param>
        /// <param name="anchor">Position anchor.</param>
        /// <param name="offset">Offset from anchor position.</param>
        public SliderGamePad(uint min, uint max, Vector2 size, SliderSkin skin = SliderSkin.Default, Anchor anchor = Anchor.Auto, Vector2? offset = null) 
            : base(min, max, size, skin, anchor, offset)
        {

        }
    }
}
