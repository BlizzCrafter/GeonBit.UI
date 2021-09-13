using GeonBit.UI.Entities;
using GeonBit.UI.Source.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace GeonBit.UI.Source.Entities
{
    /// <summary>
    /// The SelectionDimension defines the depths of the selection in a PanelGrid.
    /// </summary>
    public enum SelectionDimension
    {
        /// <summary>
        /// Selection is possible in a 2D-way. You CAN NOT select deep panels and its contents.
        /// </summary>
        Flat,
        /// <summary>
        /// Selection is possible in a 3D-way. You CAN select deep panels and its contents.
        /// </summary>
        Deep
    }

    /// <summary>
    /// The HierachyIdentifier shows us in which hierachy we are currently standing inside a GamePad-Panel.
    /// </summary>
    public enum HierarchyIdentifier
    {
        /// <summary>
        /// Not available in GamePad-Selection.
        /// </summary>
        None,
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
        /// The name of this Panel. Useful to name panel tabs as well.
        /// </summary>
        public string Name { get; set; } = "Default";

        /// <summary>
        /// Get the SelectionDimension (2D or 3D) of this PanelGamePad.
        /// </summary>
        public SelectionDimension SelectionDimension { get; internal set; }

        /// <summary>
        /// Get the RootGrid-Reference.
        /// </summary>
        public static PanelGamePad RootGrid { get; set; }

        /// <summary>
        /// The last entity we have clicked.
        /// </summary>
        public static Entity ClickedPanelContent { get; set; }

        /// <summary>
        /// The currently selected Entity.
        /// </summary>
        public static Entity SelectedPanelContent { get; set; }

        /// <summary>
        /// The currently selected Panel.
        /// </summary>
        public Panel SelectedPanel { get; private set; }

        /// <summary>
        /// Our current PanelIndex (selection).
        /// </summary>
        public int PanelIndex { get; protected set; }
        private int _OldPanelIndex;
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
        public SelectionMode SelectionMode { get; private set; } = SelectionMode.PanelRoot;

        /// <summary>
        /// True if this PanelGamePad is currently in the most outer SelectionMode. False if it is in deep SelectionMode.
        /// </summary>
        public bool IsOuterSelection
        {
            get
            {
                if (RootGrid.SelectionDimension == SelectionDimension.Flat) return true;
                else if (SelectedPanel == null) return true;
                else
                {
                    if (SelectedPanel is PanelGamePad)
                    {
                        if (((PanelGamePad)SelectedPanel).SelectionMode == SelectionMode.PanelRoot) return true;
                    }
                    else
                    {
                        var selectedPanelGrid = FindSelectedPanelGamePad(SelectedPanel);
                        if (selectedPanelGrid != null && selectedPanelGrid is PanelGamePad)
                        {
                            if (selectedPanelGrid.SelectionMode == SelectionMode.PanelRoot) return true;
                        }
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Selectively locks this PanelGrid (and not all of its childrens too like with the default "Locked" property of an entity).
        /// </summary>
        public bool PanelGridLocked { get; private set; }

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

        private bool _PanelContentClicked;
        private double _PanelContentClickedTimeOut;
        private string _SelectedIdentifier;
        private List<Entity> _SelectableChildren = new List<Entity>();

        private PanelGamePad FindSelectedPanelGamePad(Panel searchPanel)
        {
            PanelTabsGamePad panelTabs = searchPanel.Children.ToList().Find(x => x is PanelTabsGamePad) as PanelTabsGamePad;
            if (panelTabs != null)
            {
                return panelTabs.Find($"{GetIdentifier(HierarchyIdentifier.PanelGrid)}:{panelTabs.PanelIndex}", true, false) as PanelGamePad;
            }
            else
            {
                return searchPanel.Find(GetIdentifier(HierarchyIdentifier.PanelGrid), true, false) as PanelGamePad;
            }
        }

        #region Selection-Modes

        /// <summary>
        /// Calling this makes the PanelSelection available.
        /// It should be only called on the RootGrid and after all UserInterface elements are fully generated.
        /// </summary>
        public virtual void StartPanelSelection()
        {
            if (Identifier != GetIdentifier(HierarchyIdentifier.RootGrid))
            {
                throw new Exceptions.InvalidValueException("You can only start the PanelSelection from the RootGrid!");
            }

            Visible = true;

            if (SelectionDimension == SelectionDimension.Flat)
            {
                //Ensures first selection on startup before entering the PanelContentMode.
                SelectedPanel = FindSelectedPanelGamePad(Children[DefaultPanelIndex] as Panel);

                //Enter the PanelMode of the SelectedPanel.
                ((PanelGamePad)SelectedPanel).PanelModeIn(false);

                //Unlock the SelectedPanel.
                ((PanelGamePad)SelectedPanel).PanelGridLocked = false;

                //Lock the RootGrid.
                PanelGridLocked = true;
            }
            else
            {
                PanelGridLocked = false;

                PanelModeIn(false);
            }
        }

        /// <summary>
        /// Selects the PanelRoot.
        /// </summary>
        private void RootMode(bool comingFromRootGrid)
        {
            //We are coming not from the RootGrid which means we are just coming from a RootPanel (from inner -> to outer).
            if (!comingFromRootGrid)
            {
                //The RootPanel should have no Skin attached (we won't overlap with a panel from the RootGrid).
                Skin = PanelSkin.None;

                //NextMod: from Panel -> to PanelRoot selection.
                SelectionMode = SelectionMode.PanelRoot;

                //Deselect the SelectedPanel if available (DefaultSkin, DefaultColor, NoColor).
                DeselectSelectedPanel();

                //Select the selected root panel by using the _SelectedIdentifier - previously set by coming from the PanelGrid.
                Panel selectedRootPanel = RootGrid.Find(_SelectedIdentifier, true) as Panel;
                if (selectedRootPanel != null)
                {
                    selectedRootPanel.Skin = GamePadSetup.SelectedSkin;
                }

                //Lock the PanelRoot (preperation for RootGrid(outer)-Selection).
                PanelGridLocked = true;

                //Unlock the RootGrid (now we are leaving this Panel and coming to the outer selection [RootGrid]).
                RootGrid.PanelGridLocked = false;
            }
            //We are coming from the RootGrid which means we are trying to go deeper (from outer -> to inner).
            else
            {
                //Do we have any selectable children here?
                if (SelectedPanel.Children != null && SelectedPanel.Children.Count > 0)
                {
                    //Yes, so we are locking the RootGrid.
                    PanelGridLocked = true;

                    //Deselect the SelectedPanel if available (DefaultSkin, DefaultColor, NoColor).
                    DeselectSelectedPanel();

                    PanelGamePad selectedPanelGamePad = FindSelectedPanelGamePad(SelectedPanel);
                    if (selectedPanelGamePad != null)
                    {
                        //Set the _SelectedIdentifier in the PanelGrid so that we can find it when coming from the RootGrid.
                        selectedPanelGamePad._SelectedIdentifier = SelectedPanel.Identifier;

                        //Unlock the PanelGrid (now we are leaving the RootGrid and coming to the inner selection [PanelGrid].
                        selectedPanelGamePad.PanelGridLocked = false;
                    }
                }
            }

            //Deselect PanelContent.
            if (SelectedPanelContent is IEntityGamePad)
            {
                ((IEntityGamePad)SelectedPanelContent).TriggerOnDeSelect();
            }
            SelectedPanelContent = null;
        }

        /// <summary>
        /// Selects the first Panel from the PanelRoot.
        /// </summary>
        private void PanelModeIn(bool raiseEvents = true)
        {
            if (raiseEvents) OnClick?.Invoke(this);

            //Deselect the RootPanel (DefaultSkin).
            Skin = GamePadSetup.DefaultSkin;

            //Give the RootPanel a selected color (shows the end-user that the RootPanel is currently active).
            FillColor = GamePadSetup.SelectedColor;

            //Deselect the SelectedPanel from the RootGrid (alternatively).
            //Panel selectedRootPanel = UserInterface.Active.Root.Find(_SelectedIdentifier, true) as Panel;
            //if (selectedRootPanel != null)
            //{
            //    selectedRootPanel.FillColor = GamePadSetup.SelectedColor;
            //}

            //NextMode: from RootPanel -> to Panel selection.
            SelectionMode = SelectionMode.Panel;

            //PanelSelection is the default one again (index = 0).
            ResetPanelSelection();
        }

        /// <summary>
        /// Deselects the current panel content and enable all children of the PanelGrid again.
        /// </summary>
        private void PanelModeOut(bool raiseEvents = true)
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
        private void PanelContentMode()
        {
            //NextMod: from Panel -> to PanelContent selection.
            SelectionMode = SelectionMode.PanelContent;

            //Create a list of selectable children.
            _SelectableChildren =
                SelectedPanel
                .Children
                .Where(x => x.Identifier == GetIdentifier(HierarchyIdentifier.PanelContent)).ToList();

            //Reset ChildrenIndex to start child selection from the first index again.
            ChildrenIndex = 0;

            //Disable all children from the whole panel.
            SetChildrenEnabled(SelectionState.Disabled);
            //Select the child at the current ChildIndex.
            SelectCurrentChildIndex();
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
        /// Reverts the PanelSelection to the old one. For example when PanelSelection is blocked (e.g. invisible panels).
        /// Use this in your custom PanelGamePad class to correct values like the _CurrentRow in a PanelGrid.
        /// </summary>
        public virtual void RevertOldPanelSelection()
        {
            PanelIndex = _OldPanelIndex;
        }

        /// <summary>
        /// This will be the used index when resetting the Panel-Selection.
        /// </summary>
        public virtual void SetDefaultPanelIndex(Anchor anchor)
        {
            _OldPanelIndex = DefaultPanelIndex;
        }

        /// <summary>
        /// Deselects the SelectedPanel by setting the DefaultSkin and the corresponding FillColor.
        /// </summary>
        private void DeselectSelectedPanel()
        {
            if (SelectedPanel != null)
            {
                SelectedPanel.Skin = GamePadSetup.DefaultSkin;
                if (InvisiblePanel) SelectedPanel.FillColor = new Color();
                else SelectedPanel.FillColor = GamePadSetup.DefaultColor;
            }
        }

        /// <summary>
        /// Selects a panel from the PanelGrid.
        /// </summary>
        /// <param name="direction">The direction in which the index should try to shift.</param>
        protected virtual void SelectPanel(PanelDirection direction)
        {
            ClickedPanelContent = null;

            var panelChildren = Children.OfType<Panel>().ToList();

            if (PanelIndex < 0 || PanelIndex > panelChildren.Count - 1) PanelIndex = 0;
            if (panelChildren.Count > 0 && !panelChildren[PanelIndex].Visible) RevertOldPanelSelection();

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
                //Reset Skin for all Children.
                panelChildren.ForEach(x => x.Skin = GamePadSetup.DefaultSkin);

                //Get the selected panel.
                SelectedPanel = panelChildren[PanelIndex];

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

            _OldPanelIndex = PanelIndex;
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
        /// Set the ChildIndex based on the PanelDirection and select the content.
        /// </summary>
        /// <param name="direction">The panel direction we want to shift to.</param>
        protected virtual void SelectContent(PanelDirection direction)
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
            if (_SelectableChildren != null && _SelectableChildren.Count > 0)
            {
                //Enable all the children from the currently selected panel.
                _SelectableChildren.ForEach(
                    x =>
                    {
                        //We don't tint entities like SelectLists.
                        if (x is SelectListPanel == false &&
                            x is SelectListGamePad == false && 
                            x is DropDownPanel == false &&
                            x is DropDownGamePad == false)
                        {
                            x.Enabled = true;
                            x.FillColor = GamePadSetup.DefaultColor;
                            x.State = EntityState.Default;

                            //Add selected color and state to the currently selected child.
                            _SelectableChildren[ChildrenIndex].FillColor = GamePadSetup.SelectedColor;
                            _SelectableChildren[ChildrenIndex].State = EntityState.MouseHover;
                        }
                    });

                //Deselect old SelectedPanelContent.
                SelectedPanelContentTrigger(false, raiseEvents);

                //Set new SelectedPanelContent.
                SelectedPanelContent = _SelectableChildren[ChildrenIndex];

                //Select new SelectedPanelContent.
                SelectedPanelContentTrigger(true, raiseEvents);
            }
        }

        private void SelectedPanelContentTrigger(bool select, bool raiseEvents)
        {
            if (raiseEvents)
            {
                if (SelectedPanelContent != null && SelectedPanelContent is IEntityGamePad)
                {
                    if (select) ((IEntityGamePad)SelectedPanelContent).TriggerOnSelect();
                    else ((IEntityGamePad)SelectedPanelContent).TriggerOnDeSelect();
                }
            }
        }

        #endregion Selection-Actions

        /// <summary>
        /// Click on the currently selected child (PanelContent).
        /// </summary>
        private void ClickPanelContent()
        {
            if (_SelectableChildren != null && _SelectableChildren.Count > 0)
            {
                _PanelContentClicked = true;
                ClickedPanelContent = _SelectableChildren[ChildrenIndex];
                _SelectableChildren[ChildrenIndex].State = EntityState.MouseDown;
            }
        }

        /// <summary>
        /// Enable or Disable all children at once.
        /// </summary>
        /// <param name="selectionState">The wanted selection state.</param>
        private void SetChildrenEnabled(SelectionState selectionState)
        {
            //We don't tint or enable/disable panels like DropDownPanel and SelectListPanel here.
            //They are doing it's own coloring- and enable/disable- techniques.
            if (this is DropDownPanel == false && this is SelectListPanel == false)
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
    }

        /// <summary>
        /// Creates the GamePad-Panel with the default skin and based on a regular Panel-Entity.
        /// </summary>
        /// <param name="size">Panel size.</param>
        /// <param name="anchor">Position anchor.</param>
        /// <param name="offset">Offset from anchor position.</param>
        protected PanelGamePad(Vector2 size, Anchor anchor = Anchor.Center, Vector2? offset = null) 
            : base(size, GamePadSetup.DefaultSkin, anchor, offset)
        {
            //A PanelGamePad is a PanelGrid by default.
            Identifier = GetIdentifier(HierarchyIdentifier.PanelGrid);

            //Fires the PanelContent.OnClick event after the timeout.
            //Define the timeout with the GamePadSetup class - not here!
            _PanelContentClickedTimeOut = GamePadSetup.PanelContentClickedTimeOut;

            //PanelGrids are locked by default.
            //They get selectivly unlocked when the user navigates through the grid to avoid double inputs.
            PanelGridLocked = true;
            
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
            if (Enabled && !PanelGridLocked)
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
                        if (RootGrid.SelectionDimension == SelectionDimension.Deep)
                        {
                            if (SelectionMode == SelectionMode.PanelContent) PanelModeOut();
                            else if (SelectionMode == SelectionMode.Panel) RootMode(false);
                        }
                    }
                }
                else if (Identifier == GetIdentifier(HierarchyIdentifier.RootGrid))
                {
                    if (RootGrid.SelectionDimension == SelectionDimension.Deep)
                    {
                        if (UserInterface.Active.GamePadInputProvider.GamePadButtonPressed(Buttons.A))
                        {
                            if (SelectedPanel.Children != null && SelectedPanel.Children.Count > 0) RootMode(true);
                        }
                    }
                }

                if (UserInterface.Active.GamePadInputProvider.GamePadButtonPressed(Buttons.DPadRight))
                {
                    if (SelectionMode == SelectionMode.Panel) SelectPanel(PanelDirection.Right);
                    else if (SelectionMode == SelectionMode.PanelContent) SelectContent(PanelDirection.Right);
                }
                else if (UserInterface.Active.GamePadInputProvider.GamePadButtonPressed(Buttons.DPadLeft))
                {
                    if (SelectionMode == SelectionMode.Panel) SelectPanel(PanelDirection.Left);
                    else if (SelectionMode == SelectionMode.PanelContent) SelectContent(PanelDirection.Left);
                }
                else if (UserInterface.Active.GamePadInputProvider.GamePadButtonPressed(Buttons.DPadDown))
                {
                    if (SelectionMode == SelectionMode.Panel) SelectPanel(PanelDirection.Down);
                    else if (SelectionMode == SelectionMode.PanelContent) SelectContent(PanelDirection.Down);
                }
                else if (UserInterface.Active.GamePadInputProvider.GamePadButtonPressed(Buttons.DPadUp))
                {
                    if (SelectionMode == SelectionMode.Panel) SelectPanel(PanelDirection.Up);
                    else if (SelectionMode == SelectionMode.PanelContent) SelectContent(PanelDirection.Up);
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

                        if (UserInterface.Active.GamePadInputProvider.GamePadButtonDown(Buttons.DPadDown))
                        {
                            ((IEntityGamePad)SelectedPanelContent).TriggerOnScroll(PanelDirection.Down, false);
                        }
                        else if (UserInterface.Active.GamePadInputProvider.GamePadButtonDown(Buttons.DPadUp))
                        {
                            ((IEntityGamePad)SelectedPanelContent).TriggerOnScroll(PanelDirection.Up, false);
                        }
                        else if (UserInterface.Active.GamePadInputProvider.GamePadButtonDown(Buttons.DPadLeft))
                        {
                            ((IEntityGamePad)SelectedPanelContent).TriggerOnScroll(PanelDirection.Left, false);
                        }
                        else if (UserInterface.Active.GamePadInputProvider.GamePadButtonDown(Buttons.DPadRight))
                        {
                            ((IEntityGamePad)SelectedPanelContent).TriggerOnScroll(PanelDirection.Right, false);
                        }
                        else if (UserInterface.Active.GamePadInputProvider.GamePadButtonDown(Buttons.LeftThumbstickDown) ||
                                UserInterface.Active.GamePadInputProvider.GamePadButtonDown(Buttons.RightThumbstickDown))
                        {
                            ((IEntityGamePad)SelectedPanelContent).TriggerOnScroll(PanelDirection.Down, true);
                        }
                        else if (UserInterface.Active.GamePadInputProvider.GamePadButtonDown(Buttons.LeftThumbstickUp) ||
                                UserInterface.Active.GamePadInputProvider.GamePadButtonDown(Buttons.RightThumbstickUp))
                        {
                            ((IEntityGamePad)SelectedPanelContent).TriggerOnScroll(PanelDirection.Up, true);
                        }
                        else if (UserInterface.Active.GamePadInputProvider.GamePadButtonDown(Buttons.LeftThumbstickLeft) ||
                                UserInterface.Active.GamePadInputProvider.GamePadButtonDown(Buttons.RightThumbstickLeft))
                        {
                            ((IEntityGamePad)SelectedPanelContent).TriggerOnScroll(PanelDirection.Left, true);
                        }
                        else if (UserInterface.Active.GamePadInputProvider.GamePadButtonDown(Buttons.LeftThumbstickRight) ||
                                UserInterface.Active.GamePadInputProvider.GamePadButtonDown(Buttons.RightThumbstickRight))
                        {
                            ((IEntityGamePad)SelectedPanelContent).TriggerOnScroll(PanelDirection.Right, true);
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

        /// <summary>
        /// Returns either the Identifier or the base type.
        /// Easier identification when debugging.
        /// </summary>
        public override string ToString()
        {
            return string.IsNullOrEmpty(Identifier) ? base.ToString() : Identifier;
        }
    }
}
