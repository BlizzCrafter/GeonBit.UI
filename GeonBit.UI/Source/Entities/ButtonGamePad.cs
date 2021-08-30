using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;

namespace GeonBit.UI.Source.Entities
{
    /// <summary>
    /// A clickable gamepad supported button with label on it.
    /// </summary>
    [System.Serializable]
    public class ButtonGamePad : Button
    {
        /// <summary>
        /// Static ctor.
        /// </summary>
        static ButtonGamePad()
        {
            Entity.MakeSerializable(typeof(ButtonGamePad));
        }

        /// <summary>
        /// Create the gamepad supported button.
        /// </summary>
        /// <param name="text">Text for the button label.</param>
        /// <param name="skin">Button skin (texture to use).</param>
        /// <param name="anchor">Position anchor.</param>
        /// <param name="size">Button size (if not defined will use default size).</param>
        /// <param name="offset">Offset from anchor position.</param>
        public ButtonGamePad(
            string text, 
            ButtonSkin skin = ButtonSkin.Default, 
            Anchor anchor = Anchor.Auto, 
            Vector2? size = null,
            Vector2? offset = null)
            : base(text, skin, anchor, size, offset)
        {
            //We are doing clicking and event handling customly.
            ClickThrough = true;
        }

        /// <summary>
        /// Called every frame to update entity state and call events.
        /// </summary>
        /// <param name="targetEntity">The deepest child entity with highest priority that we point on and can be interacted with.</param>
        /// <param name="dragTargetEntity">The deepest child dragable entity with highest priority that we point on and can be drag if mouse down.</param>
        /// <param name="wasEventHandled">Set to true if current event was already handled by a deeper child.</param>
        /// <param name="scrollVal">Combined scrolling value (panels with scrollbar etc) of all parents.</param>
        public override void Update(ref Entity targetEntity, ref Entity dragTargetEntity, ref bool wasEventHandled, Point scrollVal)
        {
            if (Enabled)
            {
                if (PanelGamePad.ClickedPanelContent == this)
                {
                    if (UserInterface.Active.GamePadInputProvider.GamePadButtonPressed())
                    {
                        DoOnMouseDown();
                    }
                    else if (UserInterface.Active.GamePadInputProvider.GamePadButtonReleased())
                    {
                        DoOnMouseReleased();
                    }

                    if (UserInterface.Active.GamePadInputProvider.GamePadButtonClick())
                    {
                        DoOnClick();
                    }
                }
            }

            base.Update(ref targetEntity, ref dragTargetEntity, ref wasEventHandled, scrollVal);
        }
    }
}
