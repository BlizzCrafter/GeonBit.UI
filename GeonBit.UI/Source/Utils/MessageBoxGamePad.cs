using GeonBit.UI.Entities;
using GeonBit.UI.Source.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using static GeonBit.UI.Utils.MessageBox;

namespace GeonBit.UI.Source.Utils
{
    /// <summary>
    /// This is a kind of wrapper for the built-in MessageBox-Util to support GamePad-Input.
    /// </summary>
    [System.Serializable]
    public class MessageBoxGamePad
    {
        /// <summary>
        /// Static ctor.
        /// </summary>
        static MessageBoxGamePad()
        {
            Entity.MakeSerializable(typeof(MessageBoxGamePad));
        }

        MessageBoxHandle _MessageBox;
        private int _CustomMessageBoxIndex = 0, _CustomMessageBoxCount = 0;
        private double _MessageBoxTimeOut = 0;

        private void NextMessageBoxOption()
        {
            _CustomMessageBoxIndex++;
            MessageBoxSelectOption();
        }

        private void PreviousMessageBoxOption()
        {
            _CustomMessageBoxIndex--;
            MessageBoxSelectOption();
        }

        /// <summary>
        /// The default ctr for this MessageBoxGamePad class.
        /// It sets the MessageBoxTimeOut value from the GamePadSetup class.
        /// If you want to change this value, do it in the GamePadSetup class and not here!
        /// </summary>
        public MessageBoxGamePad() { _MessageBoxTimeOut = GamePadSetup.MessageBoxTimeOut; }

        /// <summary>
        /// Show a message box with custom buttons and callbacks.
        /// </summary>
        /// <param name="header">Messagebox header.</param>
        /// <param name="message">Main text.</param>
        /// <param name="options">Msgbox response options.</param>
        /// <param name="extraEntities">Optional array of entities to add to msg box under the text and above the buttons.</param>
        /// <param name="size">Alternative size to use.</param>
        /// <param name="onDone">Optional callback to call when this msgbox closes.</param>
        /// <param name="parent">Parent to add message box to (if not defined will use root)</param>
        /// <returns>Message box handle.</returns>
        public void ShowMessageBox(
            string header, 
            string message, 
            MsgBoxOption[] options, 
            Entity[] extraEntities = null, 
            Vector2? size = null, 
            Action onDone = null, 
            Entity parent = null)
        {
            _CustomMessageBoxCount = options.Length;

            _MessageBox = ShowMsgBox(header, message, options, extraEntities, size, onDone, parent);

            MessageBoxSelectOption();
        }

        private void MessageBoxSelectOption()
        {
            if (_CustomMessageBoxIndex > _CustomMessageBoxCount - 1) _CustomMessageBoxIndex = 0;
            else if (_CustomMessageBoxIndex < 0) _CustomMessageBoxIndex = _CustomMessageBoxCount - 1;

            var msgOptionsPanel = _MessageBox.Panel.Children.Last();
            var msgOptionButtons = msgOptionsPanel.Children.ToList();

            msgOptionButtons.ForEach(x => x.FillColor = GamePadSetup.DefaultColor);
            msgOptionButtons[_CustomMessageBoxIndex].FillColor = GamePadSetup.SelectedColor;
        }

        private void MessageBoxConfirmOption()
        {
            var msgOptionsPanel = _MessageBox.Panel.Children.Last();
            var msgOptionButtons = msgOptionsPanel.Children.ToList();

            msgOptionButtons.ForEach(x => x.FillColor = GamePadSetup.DefaultColor);
            msgOptionButtons[_CustomMessageBoxIndex].OnClick?.Invoke(msgOptionButtons[_CustomMessageBoxIndex]);

            _CustomMessageBoxIndex = 0;
            _MessageBox = null;
        }

        /// <summary>
        /// Updates this MessageBoxGamePad object.
        /// This is mainly needed to enable GamePad-Input.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            if (_MessageBox != null)
            {
                if (UserInterface.Active.GamePadInputProvider.GamePadButtonPressed(Buttons.DPadRight))
                {
                    NextMessageBoxOption();
                }
                else if (UserInterface.Active.GamePadInputProvider.GamePadButtonPressed(Buttons.DPadLeft))
                {
                    PreviousMessageBoxOption();
                }

                if (_MessageBoxTimeOut < 0)
                {
                    if (UserInterface.Active.GamePadInputProvider.GamePadButtonClick(Buttons.A))
                    {
                        MessageBoxConfirmOption();
                    }
                }
                else _MessageBoxTimeOut -= gameTime.ElapsedGameTime.TotalMilliseconds;
            }
        }
    }
}
