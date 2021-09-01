using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GeonBit.UI.Source.Entities
{
    /// <summary>
    /// Defines the location of the PanelTab (buttons panel).
    /// </summary>
    public enum TabLocation
    {
        /// <summary>
        /// Buttons-Panel will be at the top.
        /// </summary>
        Top,
        /// <summary>
        /// Buttons-Panel will be at the bottom.
        /// </summary>
        Bottom,
        /// <summary>
        /// Buttons-Panel will be invisible.
        /// </summary>
        Invisible
    }

    /// <summary>
    /// A graphical panel or form you can create and add entities to.
    /// Used to group together entities with common logic.
    /// </summary>
    [System.Serializable]
    public class PanelTabsGamePad : PanelTabs, IEntityGamePad
    {
        /// <summary>
        /// Static ctor.
        /// </summary>
        static PanelTabsGamePad()
        {
            Entity.MakeSerializable(typeof(PanelTabsGamePad));
        }

        private TabLocation _TabLocation = TabLocation.Top;
        private Panel _InternalRootPanel, _PanelsPanel, _ButtonsPanel;

        private int _PanelIndex = 0;
        private int _PanelTabCount;

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

        #endregion Events

        /// <summary>
        /// Select the next tab in the panel tab.
        /// </summary>
        public void NextTab()
        {
            if (_PanelIndex + 1 > _PanelsPanel.Children.Count - 1) _PanelIndex = 0;
            else _PanelIndex++;

            SelectTabIndex(_PanelsPanel);
        }

        /// <summary>
        /// Select the previous tab in the panel tab.
        /// </summary>
        public void PreviousTab()
        {
            if (_PanelIndex - 1 < 0) _PanelIndex = _PanelsPanel.Children.Count - 1;
            else _PanelIndex--;

            SelectTabIndex(_PanelsPanel);
        }

        private void SelectTabIndex(Entity panelsPanel)
        {
            var tabData = panelsPanel.Children[_PanelIndex].AttachedData as TabData;
            string nextPanelName = tabData.name;

            SelectTab(nextPanelName);
        }

        /// <summary>
        /// Add a new tab to the panel tabs.
        /// </summary>
        /// <param name="name">Tab name (also what will appear on the panel button).</param>
        /// <param name="host">The panel which should added to the tab.</param>
        public void AddTab(string name, Panel host)
        {
            var newTab = AddTab(name, PanelSkin.Simple);
            newTab.panel.Padding = Vector2.Zero;
            newTab.panel.AddChild(host);
        }

        /// <summary>
        /// Create the panel tabs with gamepad support.
        /// </summary>
        public PanelTabsGamePad(TabLocation tabLocation, params PanelGamePad[] panelGamePadTabs)
            : base(PanelSkin.Simple)
        {
            _TabLocation = tabLocation;
            _PanelTabCount = panelGamePadTabs.Length;

            for (int i = 0; i < _PanelTabCount; i++)
            {
                AddTab(panelGamePadTabs[i].Name, panelGamePadTabs[i]);
            }

            _InternalRootPanel = Find<Panel>("_internalRoot", true, false);
            _ButtonsPanel = Find<Panel>("_buttonsPanel", true, false);
            _PanelsPanel = Find<Panel>("_panelsPanel", true, false);

            if (_TabLocation == TabLocation.Bottom) _PanelsPanel.SendToBack();
            else if (_TabLocation == TabLocation.Invisible) _ButtonsPanel.Visible = false;

            //We are doing clicking and event handling customly.
            ClickThrough = true;
        }

        /// <summary>
        /// Called every frame to update the children of this entity.
        /// </summary>
        /// <param name="targetEntity">The deepest child entity with highest priority that we point on and can be interacted with.</param>
        /// <param name="dragTargetEntity">The deepest child dragable entity with highest priority that we point on and can be drag if mouse down.</param>
        /// <param name="wasEventHandled">Set to true if current event was already handled by a deeper child.</param>
        /// <param name="scrollVal">Combined scrolling value (panels with scrollbar etc) of all parents.</param>
        protected override void UpdateChildren(ref Entity targetEntity, ref Entity dragTargetEntity, ref bool wasEventHandled, Point scrollVal)
        {
            if (Enabled)
            {
                if (UserInterface.Active.GamePadInputProvider.GamePadButtonPressed(Buttons.LeftShoulder))
                {
                    PreviousTab();
                }
                else if (UserInterface.Active.GamePadInputProvider.GamePadButtonPressed(Buttons.RightShoulder))
                {
                    NextTab();
                }
            }

            base.UpdateChildren(ref targetEntity, ref dragTargetEntity, ref wasEventHandled, scrollVal);
        }

        /// <summary>
        /// Draw the entity.
        /// </summary>
        /// <param name="spriteBatch">Sprite batch to draw on.</param>
        /// <param name="phase">The phase we are currently drawing.</param>
        protected override void DrawEntity(SpriteBatch spriteBatch, DrawPhase phase)
        {
            base.DrawEntity(spriteBatch, phase);

            if (_TabLocation == TabLocation.Bottom)
            {
                if (_ButtonsPanel != null && _ButtonsPanel.Children.Count > 0)
                {
                    _PanelsPanel.Offset = Vector2.Zero;

                    _ButtonsPanel.Offset = new Vector2(
                        0,
                        (_InternalRootPanel.GetActualDestRect().Height / UserInterface.Active.GlobalScale) -
                        _ButtonsPanel.Children[0].GetActualDestRect().Height / UserInterface.Active.GlobalScale);
                }
            }
            else if (_TabLocation == TabLocation.Invisible)
            {
                _PanelsPanel.Offset = Vector2.Zero;
                _ButtonsPanel.Offset = Vector2.Zero;
            }
        }
    }
}
