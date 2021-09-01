using GeonBit.UI.Entities;
using GeonBit.UI.Source.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace GeonBit.UI.Source.Entities
{
    /// <summary>
    /// The HierachyIdentifier shows us in which hierachy we are currently standing inside a GamePad-Panel.
    /// </summary>
    public enum HierarchyIdentifier
    {
        /// <summary>
        /// The very first PanelGrid of our system.
        /// </summary>
        RootGrid,
        /// <summary>
        /// A PanelGrid gets hosted in on of the Panels of a RootGrid.
        /// </summary>
        PanelGrid,
        /// <summary>
        /// A Panel is a selectable element inside a PanelGrid.
        /// </summary>
        Panel,
        /// <summary>
        /// The PanelContents are selectables inside a Panel.
        /// </summary>
        PanelContent
    }

    /// <summary>
    /// The SelectionState determines if an entity should get enabled or disabled.
    /// </summary>
    public enum SelectionState
    {
        /// <summary>
        /// Disable the entity.
        /// </summary>
        Disabled,
        /// <summary>
        /// Enable the entity.
        /// </summary>
        Enabled
    }

    /// <summary>
    /// The SelectionMode defines which panel type is currently selected in a PanelGrid.
    /// </summary>
    public enum SelectionMode
    {
        /// <summary>
        /// None is selected.
        /// </summary>
        None,
        /// <summary>
        /// A PanelRoot is selected.
        /// </summary>
        PanelRoot,
        /// <summary>
        /// A Panel is selected.
        /// </summary>
        Panel,
        /// <summary>
        /// The content of a Panel is selected.
        /// </summary>
        PanelContent
    }

    /// <summary>
    /// Inherit from this class to enable basic gamepad support in your custom entities.
    /// </summary>
    public abstract class PanelGamePad : Panel
    {
        /// <summary>
        /// The last entity we have clicked.
        /// </summary>
        public static Entity ClickedPanelContent { get; set; }

        /// <summary>
        /// The currently selected Entity.
        /// </summary>
        public static Entity SelectedPanelContent { get; set; }

        /// <summary>
        /// Our current PanelIndex (selection).
        /// </summary>
        public int PanelIndex { get; protected set; }
        /// <summary>
        /// This will be the used index when resetting the Panel-Selection.
        /// </summary>
        public int DefaultPanelIndex { get; protected set; }
        /// <summary>
        /// Our current ChildrenIndex (selection).
        /// </summary>
        public int ChildrenIndex { get; protected set; }
        /// <summary>
        /// This is how many panels we are having in the Children list.
        /// </summary>
        public int PanelCount { get; protected set; } = 3;

        /// <summary>
        /// The SelectionMode defines in which hierarchy we are currently standing with our gamepad.
        /// </summary>
        protected SelectionMode SelectionMode { get; set; } = SelectionMode.PanelRoot;

        /// <summary>
        /// Highlighting a Panel happens in the RootGrid.
        /// An entity like the PanelBar gets completly streched above such a Panel per default, which makes the highlight invisible.
        /// Setting InvisiblePanel to true ensure visibility in this case, because then the corresponding Panel will be invisible instead.
        /// </summary>
        protected bool InvisiblePanel 
        {
            get { return _InvisiblePanel; }
            set
            {
                _InvisiblePanel = value;
                if (value) FillColor = new Color();
            }
        }
        private bool _InvisiblePanel;

        /// <summary>
        /// Selectively locks this PanelGrid (and not all of its childrens too like with the default "Locked" property of an entity.
        /// </summary>
        protected bool LockPanelGrid { get; set; } = false;

        /// <summary>
        /// The currently selected Panel.
        /// </summary>
        protected Panel SelectedPanel { get; set; }

        private List<Entity> _SelectableChildren = new List<Entity>();
        private bool _PanelContentClicked;
        private double _PanelContentClickedTimeOut;

        /// <summary>
        /// Get a GamePad-Panel implementation related identifier.
        /// </summary>
        /// <param name="identifier">A base identifier.</param>
        /// <returns>The real identifier.</returns>
        public static string GetIdentifier(HierarchyIdentifier identifier)
        {
            return $"__{identifier}";
        }

        /// <summary>
        /// Get a trimmed GamePad-Panel implementation related identifier.
        /// </summary>
        /// <param name="identifier">A real identifier.</param>
        /// <returns>The base identifier.</returns>
        public static string GetIdentifier(string identifier)
        {
            return identifier.TrimStart('_');
        }

        #region Selection-Modes

        /// <summary>
        /// Selects the PanelRoot.
        /// </summary>
        protected void RootMode(bool comingFromRootGrid, bool raiseEvents = true)
        {
            if (raiseEvents) OnClick?.Invoke(this);

            //We are coming not from the RootGrid which means we are just coming from a RootPanel (from inner -> to outer).
            if (!comingFromRootGrid)
            {
                //The RootPanel should have no Skin attached (we won't overlap with a panel from the RootGrid).
                Skin = PanelSkin.None;

                //NextMod: from Panel -> to PanelRoot selection.
                SelectionMode = SelectionMode.PanelRoot;

                //Deselect the SelectedPanel if available (DefaultSkin, DefaultColor, NoColor).
                SelectedPanel.Skin = GamePadSetup.DefaultSkin;
                if (InvisiblePanel) SelectedPanel.FillColor = new Color();
                else SelectedPanel.FillColor = GamePadSetup.DefaultColor;

                //Lock the PanelRoot (preperation for RootGrid(outer)-Selection).
                LockPanelGrid = true;

                //Get the RootGrid.
                PanelGrid rootGrid = UserInterface.Active.Root.Find(GetIdentifier(HierarchyIdentifier.RootGrid), true) as PanelGrid;

                //Unlock the RootGrid (now we are leaving this Panel and coming to the outer selection [RootGrid]).
                rootGrid.LockPanelGrid = false;
            }
            //We are coming from the RootGrid which means we are trying to go deeper (from outer -> to inner).
            else
            {
                //Do we have any selectable children here?
                if (SelectedPanel.Children != null && SelectedPanel.Children.Count > 0)
                {
                    //Yes, so we are locking the RootGrid.
                    LockPanelGrid = true;

                    //Get the PanelGrid.
                    PanelGamePad panelGamePad =
                                SelectedPanel.Find(GetIdentifier(HierarchyIdentifier.PanelGrid), true) as PanelGamePad;

                    //Unlock the PanelGrid (now we are leaving the RootGrid and coming to the inner selection [PanelGrid].
                    panelGamePad.LockPanelGrid = false;
                }
            }
            
            //Deselect PanelContent.
            SelectedPanelContent = null;
        }

        /// <summary>
        /// Selects the first Panel from the PanelRoot.
        /// </summary>
        protected void PanelModeIn(bool raiseEvents = true)
        {
            if (raiseEvents) OnClick?.Invoke(this);

            //Deselect the RootPanel (DefaultSkin).
            Skin = GamePadSetup.DefaultSkin;

            //Give the RootPanel a selected color (shows the end-user that the RootPanel is currently active).
            FillColor = GamePadSetup.SelectedColor;

            //NextMode: from RootPanel -> to Panel selection.
            SelectionMode = SelectionMode.Panel;

            //PanelSelection is the default one again (index = 0).
            ResetPanelSelection();
        }

        /// <summary>
        /// Deselects the current panel content and enable all children of the PanelGrid again.
        /// </summary>
        protected void PanelModeOut(bool raiseEvents = true)
        {
            if (raiseEvents) OnClick?.Invoke(this);

            if (SelectedPanel == this)
            {
                //There is no Panel-Layer, so we are switching directly to RootMode.
                RootMode(false);
            }
            else
            {
                //NextMod: From PanelContent -> to Panel selection.
                SelectionMode = SelectionMode.Panel;
            }

            //Enable all children from the whole panel again.
            SetChildrenEnabled(SelectionState.Enabled);

            //Deselect PanelContent.
            SelectedPanelContent = null;
        }

        /// <summary>
        /// Selects the first panel content in the currently selected panel of this PanelGrid.
        /// </summary>
        protected void PanelContentMode(bool raiseEvents = true)
        {
            if (raiseEvents) SelectedPanel.OnClick?.Invoke(SelectedPanel);

            //Create a list of selectable children.
            _SelectableChildren =
                SelectedPanel
                .Children
                .Where(x => x.Identifier == GetIdentifier(HierarchyIdentifier.PanelContent)).ToList();

            //Check if we found any selectable children. If yes change selection mode to PanelContent.
            if (_SelectableChildren != null && _SelectableChildren.Count > 0)
            {
                //Reset ChildrenIndex to start child selection from the first index again.
                ChildrenIndex = 0;

                //NextMod: from Panel -> to PanelContent selection.
                SelectionMode = SelectionMode.PanelContent;

                //Disable all children from the whole panel.
                SetChildrenEnabled(SelectionState.Disabled);
                //Select the child at the current ChildIndex.
                SelectCurrentChildIndex();
            }
        }

        #endregion Selection-Modes

        #region Selection-Actions

        /// <summary>
        /// Resets the PanelSelection to index = 0.
        /// </summary>
        public virtual void ResetPanelSelection()
        {
            PanelIndex = DefaultPanelIndex;
            UpdatePanelSelection(PanelDirection.None);
        }

        /// <summary>
        /// Selects a panel from the PanelGrid.
        /// </summary>
        /// <param name="direction">The direction in which the index should try to shift.</param>
        protected virtual void SelectPanel(PanelDirection direction) 
        {
            ClickedPanelContent = null;

            if (PanelIndex < 0 || PanelIndex > Children.OfType<Panel>().Count() - 1) PanelIndex = 0;

            UpdatePanelSelection(direction);
        }

        /// <summary>
        /// Select the Panel at the current PanelIndex.
        /// </summary>
        private void SelectdPanelIndex()
        {
            //Cheking if the Children containing Panels.
            var panelChildren = Children.OfType<Panel>().ToList();
            if (panelChildren != null && panelChildren.Count > 0)
            {
                //Get the selected panel.
                SelectedPanel = panelChildren[PanelIndex];

                //Reset Skin for all Children.
                panelChildren.ForEach(x => x.Skin = GamePadSetup.DefaultSkin);

                //Set the Skin of the selected panel to "SelectedSkin".
                SelectedPanel.Skin = GamePadSetup.SelectedSkin;
            }
            else
            {
                //If Children containing no Panels, this Panel will be selected instead.
                SelectedPanel = this;

                //Tint all Children in default colors.
                SelectedPanel.Children.ToList().ForEach(x => x.FillColor = GamePadSetup.DefaultColor);

                //Setting the Skin of this Panel is not needed.
                //SelectedPanel.Skin = GamePadSetup.DefaultSkin;

                //Directly switch to PanelContentMode, because we have no Panel-Selection available.
                PanelContentMode();
            }

            //If we are coming from the RootGrid, remove the Skin.
            if (Identifier == GetIdentifier(HierarchyIdentifier.RootGrid))
            {
                Skin = PanelSkin.None;
            }
        }

        /// <summary>
        /// This get called after a panel selection happened.
        /// Updating the selected panel, the skin, the colors etc.
        /// </summary>
        /// <param name="direction">Where do we came from?</param>
        protected virtual void UpdatePanelSelection(PanelDirection direction = PanelDirection.None)
        {
            SelectdPanelIndex();
        }

        /// <summary>
        /// Set the ChildIndex based on the PanelDirection and select the child.
        /// </summary>
        /// <param name="direction">The panel direction we want to shift to.</param>
        protected void SetAndSelectCurrentChild(PanelDirection direction)
        {
            if (direction == PanelDirection.Down || direction == PanelDirection.Right)
            {
                if (ChildrenIndex < _SelectableChildren.Count - 1) ChildrenIndex++;
                else ChildrenIndex = 0;
            }
            else if (direction == PanelDirection.Up || direction == PanelDirection.Left)
            {
                if (ChildrenIndex > 0) ChildrenIndex--;
                else ChildrenIndex = _SelectableChildren.Count - 1;
            }

            SelectCurrentChildIndex();
        }

        /// <summary>
        /// Select the child at the current ChildrenIndex.
        /// </summary>
        private void SelectCurrentChildIndex(bool raiseEvents = true)
        {
            //Enable all the children from the currently selected panel.
            _SelectableChildren.ForEach(
                x =>
                {
                    x.Enabled = true;
                    x.FillColor = GamePadSetup.DefaultColor;
                    x.State = EntityState.Default;
                });

            //Add selected color and state to the currently selected child.
            _SelectableChildren[ChildrenIndex].FillColor = GamePadSetup.SelectedColor;
            _SelectableChildren[ChildrenIndex].State = EntityState.MouseHover;

            if (raiseEvents)
            {
                if (SelectedPanelContent != null)
                {
                    if (SelectedPanelContent is IEntityGamePad)
                    {
                        //Deselect old SelectedPanelContent.
                        ((IEntityGamePad)SelectedPanelContent).TriggerOnDeSelect();
                    }
                }
            }

            //Set new SelectedPanelContent.
            SelectedPanelContent = _SelectableChildren[ChildrenIndex];

            if (raiseEvents)
            {
                if (SelectedPanelContent is IEntityGamePad)
                {
                    //Select new SelectedPanelContent.
                    ((IEntityGamePad)SelectedPanelContent).TriggerOnSelect();
                }
            }
        }

        #endregion Selection-Actions

        /// <summary>
        /// Click on the currently selected child (PanelContent).
        /// </summary>
        protected void ClickPanelContent(bool raiseClickEvent = true)
        {
            _PanelContentClicked = true;
            ClickedPanelContent = _SelectableChildren[ChildrenIndex];
            _SelectableChildren[ChildrenIndex].State = EntityState.MouseDown;
        }

        /// <summary>
        /// Enable or Disable all children at once.
        /// </summary>
        /// <param name="selectionState">The wanted selection state.</param>
        private void SetChildrenEnabled(SelectionState selectionState)
        {
            Children.ToList().ForEach(
                            entity =>
                            {
                                if (entity.Identifier == GetIdentifier(HierarchyIdentifier.PanelContent))
                                {
                                    if (selectionState == SelectionState.Enabled)
                                    {
                                        entity.Enabled = true;
                                        entity.FillColor = GamePadSetup.DefaultColor;
                                        entity.State = EntityState.Default;
                                    }
                                    else if (selectionState == SelectionState.Disabled)
                                    {
                                        entity.Enabled = false;
                                        entity.State = EntityState.Default;
                                    }
                                }
                
                                entity.Children.Where(
                                    child => child.Identifier == GetIdentifier(HierarchyIdentifier.PanelContent)).ToList().ForEach(
                                    selectable =>
                                    {
                                        if (selectionState == SelectionState.Enabled)
                                        {
                                            selectable.Enabled = true;
                                            selectable.FillColor = GamePadSetup.DefaultColor;
                                            selectable.State = EntityState.Default;
                                        }
                                        else if (selectionState == SelectionState.Disabled)
                                        {
                                            selectable.Enabled = false;
                                            selectable.State = EntityState.Default;
                                        }
                                    });
                            });
        }

        /// <summary>
        /// Creates the GamePad-Panel with the default skin and based on a regular Panel-Entity.
        /// </summary>
        /// <param name="size">Panel size.</param>
        /// <param name="anchor">Position anchor.</param>
        /// <param name="offset">Offset from anchor position.</param>
        protected PanelGamePad(
               Vector2 size,
               Anchor anchor = Anchor.Center,
               Vector2? offset = null)
               : base(size, GamePadSetup.DefaultSkin, anchor, offset)
        {
            //Fires the PanelContent.OnClick event after the timeout.
            //Define the timeout with the GamePadSetup class - not here!
            _PanelContentClickedTimeOut = GamePadSetup.PanelContentClickedTimeOut;

            //PanelGrids are locked by default.
            //They get selectivly unlocked when the user navigates through the grid to avoid double inputs.
            LockPanelGrid = true;
            
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
            if (Enabled && !LockPanelGrid)
            {
                if (Identifier != GetIdentifier(HierarchyIdentifier.RootGrid))
                {
                    if (UserInterface.Active.GamePadInputProvider.GamePadButtonPressed(Buttons.A))
                    {
                        if (SelectionMode == SelectionMode.PanelRoot) PanelModeIn();
                        else if (SelectionMode == SelectionMode.Panel) PanelContentMode();
                        else if (SelectionMode == SelectionMode.PanelContent) ClickPanelContent();
                    }
                    else if (UserInterface.Active.GamePadInputProvider.GamePadButtonClick(Buttons.B))
                    {
                        if (SelectionMode == SelectionMode.PanelContent) PanelModeOut();
                        else if (SelectionMode == SelectionMode.Panel) RootMode(false);
                    }
                }
                else if (Identifier == GetIdentifier(HierarchyIdentifier.RootGrid))
                {
                    if (UserInterface.Active.GamePadInputProvider.GamePadButtonPressed(Buttons.A))
                    {
                        if (SelectedPanel.Children != null && SelectedPanel.Children.Count > 0) RootMode(true);
                    }
                }

                if (UserInterface.Active.GamePadInputProvider.GamePadButtonPressed(Buttons.DPadRight))
                {
                    if (SelectionMode == SelectionMode.Panel) SelectPanel(PanelDirection.Right);
                    else if (SelectionMode == SelectionMode.PanelContent) SetAndSelectCurrentChild(PanelDirection.Right);
                }
                else if (UserInterface.Active.GamePadInputProvider.GamePadButtonPressed(Buttons.DPadLeft))
                {
                    if (SelectionMode == SelectionMode.Panel) SelectPanel(PanelDirection.Left);
                    else if (SelectionMode == SelectionMode.PanelContent) SetAndSelectCurrentChild(PanelDirection.Left);
                }
                else if (UserInterface.Active.GamePadInputProvider.GamePadButtonPressed(Buttons.DPadDown))
                {
                    if (SelectionMode == SelectionMode.Panel) SelectPanel(PanelDirection.Down);
                    else if (SelectionMode == SelectionMode.PanelContent) SetAndSelectCurrentChild(PanelDirection.Down);
                }
                else if (UserInterface.Active.GamePadInputProvider.GamePadButtonPressed(Buttons.DPadUp))
                {
                    if (SelectionMode == SelectionMode.Panel) SelectPanel(PanelDirection.Up);
                    else if (SelectionMode == SelectionMode.PanelContent) SetAndSelectCurrentChild(PanelDirection.Up);
                }

                if (UserInterface.Active.GamePadInputProvider.GamePadButtonPressed(Buttons.Back))
                {
                    if (SelectionMode == SelectionMode.Panel) ResetPanelSelection();
                }

                if (SelectedPanelContent != null)
                {
                    targetEntity = SelectedPanelContent;
                    UserInterface.Active.ActiveEntity = SelectedPanelContent;

                    if (SelectedPanelContent is IEntityGamePad)
                    {
                        if (UserInterface.Active.GamePadInputProvider.GamePadButtonDown(Buttons.A))
                        {
                            ((IEntityGamePad)SelectedPanelContent).TriggerDoWhileButtonDown();
                        }
                        else ((IEntityGamePad)SelectedPanelContent).TriggerDoWhileHover();

                        if (UserInterface.Active.GamePadInputProvider.GamePadButtonPressed(Buttons.A))
                        {
                            ((IEntityGamePad)SelectedPanelContent).TriggerOnButtonDown();
                        }
                        else if (UserInterface.Active.GamePadInputProvider.GamePadButtonReleased(Buttons.A))
                        {
                            ((IEntityGamePad)SelectedPanelContent).TriggerOnButtonReleased();
                        }
                    }
                }

                if (_PanelContentClicked)
                {
                    if (_PanelContentClickedTimeOut < 0)
                    {
                        _PanelContentClicked = false;

                        if (ClickedPanelContent is IEntityGamePad)
                        {
                            ((IEntityGamePad)ClickedPanelContent).TriggerOnButtonClick();
                        }
                        else ClickedPanelContent.OnClick?.Invoke(SelectedPanel);

                        _PanelContentClickedTimeOut = GamePadSetup.PanelContentClickedTimeOut;

                        SelectCurrentChildIndex(false);
                    }
                    else _PanelContentClickedTimeOut -= UserInterface.Active.CurrGameTime.ElapsedGameTime.TotalMilliseconds;
                }
            }

            base.Update(ref targetEntity, ref dragTargetEntity, ref wasEventHandled, scrollVal);
        }
    }
}
