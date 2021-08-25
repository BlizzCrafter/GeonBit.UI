using GeonBit.UI.Entities;
using GeonBit.UI.Source.Input;
using GeonBit.UI.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace GeonBit.UI.Source.Entities
{
    /// <summary>
    /// Special layouts for the root panel of a PanelGrid-System.
    /// At the end of this class you will find the corresponding methods.
    /// Add your own!
    /// </summary>
    public enum RootGridLayout
    {
        /// <summary>
        /// The Default RootGridLayout has a big center panel which is surrounded by smaller panels (bars, corners).
        /// </summary>
        Default,
        /// <summary>
        /// Same as the Default one, but this time the corners are very small which makes the bars bigger.
        /// </summary>
        SmallCorners,
        /// <summary>
        /// Same as SmallCorners but this one has also small vertical bars which makes the hole surroundings of the center panel very small.
        /// </summary>
        SmallCornersVerticals
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
    /// In this direction a Panel from the PanelGrid will be selected.
    /// </summary>
    public enum PanelDirection
    {
        /// <summary>
        /// No Panel-Direction.
        /// </summary>
        None,
        /// <summary>
        /// Right from the current selected Panel.
        /// </summary>
        Right,
        /// <summary>
        /// Left from the current selected Panel.
        /// </summary>
        Left,
        /// <summary>
        /// Up from the current selected Panel.
        /// </summary>
        Up,
        /// <summary>
        /// Down from the current selected Panel.
        /// </summary>
        Down
    }

    /// <summary>
    /// This PanelGrid has full GamePad support. It generates a grid-like system by a multitued of 3.
    /// Examples: 3x1, 3x2, 3x3 etc.
    /// Navigate through the grid by using the DPad for example.
    /// </summary>
    [System.Serializable]
    public class PanelGrid : Panel
    {
        /// <summary>
        /// Static ctor.
        /// </summary>
        static PanelGrid()
        {
            Entity.MakeSerializable(typeof(PanelGrid));
        }

        private SelectionMode _SelectionMode { get; set; } = SelectionMode.PanelRoot;
        private List<Entity> SelectableChildren = new List<Entity>();
        private Panel _SelectedPanel;

        private int _PanelCount = 3;
        private int _RowCount = 0, _CurrentRow = 1;
        private int _PanelIndex = 0, _ChildrenIndex = 0;

        private int _StartOfTheRow => (_CurrentRow * _Row) - _Row;
        private int _EndOfTheRow => (_CurrentRow * _Row) - 1;
        private int _EndOfTheGridRow => (_RowCount * _Row) - _Row;
        private const int _Row = 3;

        // Selectively locks this PanelGrid (and not all of its childrens too like with the default "Locked" property of an entity.
        private bool _LockPanelGrid = false;

        /// <summary>
        /// Get a panel from the underlying PanelGrid.
        /// </summary>
        public Panel GetGridPanel(Anchor anchor)
        {
            int i = 0;
            if (anchor == Anchor.TopLeft) i = 0;
            else if (anchor == Anchor.TopCenter) i = 1;
            else if (anchor == Anchor.TopRight) i = 2;
            else if (anchor == Anchor.CenterLeft) i = 3;
            else if (anchor == Anchor.Center) i = 4;
            else if (anchor == Anchor.CenterRight) i = 5;
            else if (anchor == Anchor.BottomLeft) i = 6;
            else if (anchor == Anchor.BottomCenter) i = 7;
            else if (anchor == Anchor.BottomRight) i = 8;
            else throw new Exceptions.InvalidValueException("The anchor parameter has an invalid value. Supported values are: TopLeft, TopCenter, TopRight, CenterLeft, Center, CenterRight, BottomLeft, BottomCenter, BottomRight.");

            if (Children != null && Children[i] != null)
            {
                return Children[i] as Panel;
            }
            else return null;
        }

        /// <summary>
        /// Selects this PanelGrid.
        /// </summary>
        public void SelectRoot()
        {
            Skin = PanelSkin.None;
            _SelectionMode = SelectionMode.PanelRoot;

            if (_SelectedPanel != null) _SelectedPanel.Skin = GamePadSetup.DefaultSkin;
        }

        /// <summary>
        /// Selects the first panel in this PanelGrid.
        /// </summary>
        public void SelectRootContent()
        {
            Skin = GamePadSetup.DefaultSkin;
            _SelectionMode = SelectionMode.Panel;

            ResetPanelSelection();
        }

        /// <summary>
        /// Selects the first panel content in the currently selected panel of this PanelGrid.
        /// </summary>
        public void SelectPanelContent()
        {
            _SelectionMode = SelectionMode.PanelContent;

            UpdateChildrenStateAll(SelectionState.Disabled);
            UpdateChildrenStateSelected();
        }

        /// <summary>
        /// Deselects the panel content of the currently selected panel of this PanelGrid.
        /// </summary>
        public void DeSelectPanelContent()
        {
            _SelectionMode = SelectionMode.Panel;

            UpdateChildrenStateAll(SelectionState.Enabled);
        }

        /// <summary>
        /// Create a 3x3 FullScreen GridPanel which acts as a centered root GridPanel for all other GridPanels you create.
        /// </summary>
        /// <param name="rootGridLayout">The layout of the GridPanel.</param>
        public PanelGrid(
            RootGridLayout rootGridLayout)
            : this(Vector2.Zero, Anchor.Center, null)
        {
            PanelOverflowBehavior = PanelOverflowBehavior.Overflow;

            CreatePanelGrid(9, new Vector2(0.33f, 0));

            if (rootGridLayout == RootGridLayout.Default) CreateDefaultLayout();
            else if (rootGridLayout == RootGridLayout.SmallCorners) CreateSmallCornersLayout();
            else if (rootGridLayout == RootGridLayout.SmallCornersVerticals) CreateSmallCornersVerticalsLayout();

            Padding = Vector2.Zero;
            SpaceAfter = Vector2.Zero;
            SpaceBefore = Vector2.Zero;

            Identifier = GamePadSetup.GetIdentifier(HierarchyIdentifier.RootGrid);

            SelectRootContent();
        }

        /// <summary>
        /// Create a "multitude of 3" GridPanel. E.g. 3, 6, 9 etc.
        /// </summary>
        /// <param name="size">The size of the whole GridPanel.</param>
        /// <param name="panelCount">The amount of panels inside the PanelGrid (multitude of 3!).</param>
        /// <param name="panelSize">The size of each panels.</param>
        /// <param name="panelAnchor">The anchor of the panels.</param>
        /// <param name="anchor">The anchor of the GridPanel.</param>
        /// <param name="offset">The offset of the GridPanel.</param>
        public PanelGrid(
            Vector2 size,
            int panelCount,
            Vector2 panelSize,
            Anchor panelAnchor,
            Anchor anchor = Anchor.Center,
            Vector2? offset = null)
            : this(size, anchor, offset)
        {
            if (panelCount % 3 != 0) throw new Exceptions.InvalidValueException("PanelCount must be a multitude of 3! E.g. 3, 6, 9 etc.");

            if (panelCount > 6)
            {
                PanelOverflowBehavior = PanelOverflowBehavior.VerticalScroll;
                Scrollbar.AdjustMaxAutomatically = false;
                Scrollbar.Visible = false;
            }

            CreatePanelGrid(panelCount, panelSize);

            Identifier = GamePadSetup.GetIdentifier(HierarchyIdentifier.PanelGrid);

            _LockPanelGrid = true;
        }
                
        private PanelGrid(
            Vector2 size,
            Anchor anchor = Anchor.Center,
            Vector2? offset = null)
            : base(size, GamePadSetup.DefaultSkin, anchor, offset)
        {
        }

        private void CreatePanelGrid(int panelCount, Vector2 panelSize)
        {
            _PanelCount = panelCount;
            _RowCount = _PanelCount / 3;

            PanelsGrid.GenerateColums(_PanelCount, this, panelSize, GamePadSetup.DefaultSkin);

            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i] is Panel)
                {
                    Children[i].Anchor = Anchor.AutoInline;
                    Children[i].Identifier = $"{GamePadSetup.GetIdentifier(HierarchyIdentifier.Panel)} #{i}";
                }
            }

            Skin = PanelSkin.None;
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
            if (Enabled && !_LockPanelGrid)
            {
                if (UserInterface.Active.GamePadInputProvider.GamePadButtonPressed(Buttons.Back))
                {
                    if (_SelectionMode == SelectionMode.Panel) ResetPanelSelection();
                }
                
                if (Identifier == GamePadSetup.GetIdentifier(HierarchyIdentifier.PanelGrid))
                {
                    if (UserInterface.Active.GamePadInputProvider.GamePadButtonPressed(Buttons.A))
                    {
                        if (_SelectionMode == SelectionMode.Panel)
                        {
                            _SelectedPanel.OnClick?.Invoke(_SelectedPanel);

                            _ChildrenIndex = 0;

                            SelectableChildren = 
                                _SelectedPanel
                                .Children
                                .Where(x => x.Identifier == GamePadSetup.GetIdentifier(HierarchyIdentifier.PanelContent)).ToList();

                            if (SelectableChildren != null && SelectableChildren.Count > 0)
                            {
                                SelectPanelContent();
                            }
                            else _SelectionMode = SelectionMode.Panel;
                        }
                        else if (_SelectionMode == SelectionMode.PanelContent)
                        {
                            SelectableChildren[_ChildrenIndex].OnClick?.Invoke(_SelectedPanel);

                            SelectableChildren[_ChildrenIndex].State = EntityState.MouseDown;
                        }
                        else if (_SelectionMode == SelectionMode.PanelRoot)
                        {
                            OnClick?.Invoke(this);

                            SelectRootContent();

                            FillColor = GamePadSetup.SelectedColor;
                        }
                    }
                    else if (UserInterface.Active.GamePadInputProvider.GamePadButtonPressed(Buttons.B))
                    {
                        _ChildrenIndex = 0;

                        if (_SelectionMode == SelectionMode.PanelContent)
                        {
                            _SelectedPanel.OnClick?.Invoke(_SelectedPanel);

                            DeSelectPanelContent();
                        }
                        else if (_SelectionMode == SelectionMode.Panel)
                        {
                            OnClick?.Invoke(this);

                            SelectRoot();

                            _LockPanelGrid = true;

                            PanelGrid rootGrid =
                                UserInterface.Active.Root.Find(GamePadSetup.GetIdentifier(HierarchyIdentifier.RootGrid), true) as PanelGrid;

                            if (rootGrid != null) rootGrid._LockPanelGrid = false;
                        }
                    }
                }
                else if(Identifier == GamePadSetup.GetIdentifier(HierarchyIdentifier.RootGrid))
                {
                    if (UserInterface.Active.GamePadInputProvider.GamePadButtonPressed(Buttons.A))
                    {
                        if (_SelectedPanel.Children != null && _SelectedPanel.Children.Count > 0)
                        {
                            _LockPanelGrid = true;

                            PanelGrid panelGrid =
                                        _SelectedPanel.Find(GamePadSetup.GetIdentifier(HierarchyIdentifier.PanelGrid), true) as PanelGrid;

                            if (panelGrid != null) panelGrid._LockPanelGrid = false;
                        }
                    }
                }

                if (UserInterface.Active.GamePadInputProvider.GamePadButtonPressed(Buttons.DPadRight))
                {
                    if (_SelectionMode == SelectionMode.Panel) SelectPanel(PanelDirection.Right);
                    else if (_SelectionMode == SelectionMode.PanelContent) SelectPanelContent(PanelDirection.Right);
                }
                else if (UserInterface.Active.GamePadInputProvider.GamePadButtonPressed(Buttons.DPadLeft))
                {
                    if (_SelectionMode == SelectionMode.Panel) SelectPanel(PanelDirection.Left);
                    else if (_SelectionMode == SelectionMode.PanelContent) SelectPanelContent(PanelDirection.Left);
                }
                else if (UserInterface.Active.GamePadInputProvider.GamePadButtonPressed(Buttons.DPadDown))
                {
                    if (_SelectionMode == SelectionMode.Panel) SelectPanel(PanelDirection.Down);
                    else if (_SelectionMode == SelectionMode.PanelContent) SelectPanelContent(PanelDirection.Down);
                }
                else if (UserInterface.Active.GamePadInputProvider.GamePadButtonPressed(Buttons.DPadUp))
                {
                    if (_SelectionMode == SelectionMode.Panel) SelectPanel(PanelDirection.Up);
                    else if (_SelectionMode == SelectionMode.PanelContent) SelectPanelContent(PanelDirection.Up);
                }

                if (UserInterface.Active.GamePadInputProvider.GamePadButtonClick(Buttons.A))
                {
                    if (_SelectionMode == SelectionMode.PanelContent) UpdateChildrenStateSelected();
                }
            }

            base.Update(ref targetEntity, ref dragTargetEntity, ref wasEventHandled, scrollVal);
        }

        /// <summary>
        /// Selects a panel from the PanelGrid.
        /// </summary>
        /// <param name="direction">The direction in which the index should try to shift.</param>
        public void SelectPanel(PanelDirection direction)
        {
            if (direction == PanelDirection.Right)
            {
                _PanelIndex++;

                if (_PanelIndex > _EndOfTheRow) _PanelIndex = _StartOfTheRow;
            }
            else if (direction == PanelDirection.Left)
            {
                _PanelIndex--;

                if (_PanelIndex < _StartOfTheRow) _PanelIndex = _EndOfTheRow;
            }
            else if (direction == PanelDirection.Down)
            {
                if (_RowCount > 1)
                {
                    if (_CurrentRow < _RowCount)
                    {
                        _PanelIndex += _Row;
                        _CurrentRow++;
                    }
                    else
                    {
                        _PanelIndex -= _StartOfTheRow;
                        _CurrentRow = 1;
                    }
                }
            }
            else if (direction == PanelDirection.Up)
            {
                if (_RowCount > 1)
                {
                    if (_CurrentRow > 1)
                    {
                        _PanelIndex -= _Row;
                        _CurrentRow--;
                    }
                    else
                    {
                        _PanelIndex += _EndOfTheGridRow;
                        _CurrentRow = _RowCount;
                    }
                }
            }

            UpdatePanelSelection(_PanelIndex, direction);
        }

        private void SelectPanelContent(PanelDirection direction)
        {
            if (direction == PanelDirection.Down || direction == PanelDirection.Right)
            {
                if (_ChildrenIndex < SelectableChildren.Count - 1) _ChildrenIndex++;
                else _ChildrenIndex = 0;
            }
            else if (direction == PanelDirection.Up || direction == PanelDirection.Left)
            {
                if (_ChildrenIndex > 0) _ChildrenIndex--;
                else _ChildrenIndex = SelectableChildren.Count - 1;
            }

            UpdateChildrenStateSelected();
        }

        private void UpdateChildrenStateSelected()
        {
            SelectableChildren.ForEach(
                x =>
                {
                    x.Enabled = true;
                    x.FillColor = GamePadSetup.DefaultColor;
                    x.State = EntityState.Default;
                });
            SelectableChildren[_ChildrenIndex].FillColor = GamePadSetup.SelectedColor;
            SelectableChildren[_ChildrenIndex].State = EntityState.MouseHover;
        }

        private void UpdateChildrenStateAll(SelectionState selectionState)
        {
            Children.OfType<Panel>().ToList().ForEach(
                            panel =>
                            {
                                panel.Children.Where(
                                    child => child.Identifier == GamePadSetup.GetIdentifier(HierarchyIdentifier.PanelContent)).ToList().ForEach(
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
        /// Resets the current panel selection to the first index.
        /// </summary>
        public void ResetPanelSelection()
        {
            _PanelIndex = 0;
            _CurrentRow = 1;
            UpdatePanelSelection();
        }

        /// <summary>
        /// Changes the skin of the selected panel and scrolls the view (GridPanel) along in case a scrollbar is available.
        /// </summary>
        /// <param name="panelIndex">The index of the selected panel.</param>
        /// <param name="direction">The direction of the panel selector.</param>
        private void UpdatePanelSelection(int panelIndex = 0, PanelDirection direction = PanelDirection.None)
        {
            if (panelIndex < 0 || panelIndex > Children.OfType<Panel>().Count() - 1) _PanelIndex = panelIndex = 0;

            if (Scrollbar != null)
            {
                if (Scrollbar.Max == 0)
                {
                    for (int i = 1; i < Children.OfType<Panel>().ToList().Count - 1; i++)
                    {
                        if (i % _Row == 0) Scrollbar.Max += (uint)_children[i].GetActualDestRect().Height;
                    }
                }

                if (direction == PanelDirection.Down)
                {
                    if (_CurrentRow == 1) Scrollbar.Value = 0;
                    else if (_CurrentRow > 2)
                    {
                        Scrollbar.Value += Children[_PanelIndex - _Row].GetActualDestRect().Bottom / 2;
                    }
                }
                else if (direction == PanelDirection.Up)
                {
                    if (Scrollbar.Value == 0 && _CurrentRow != 1)
                    {
                        Scrollbar.Value += Children[_PanelIndex - (_Row * 2)].GetActualDestRect().Bottom;
                    }
                    else if (_CurrentRow > 1)
                    {
                        Scrollbar.Value += Children[_PanelIndex - _Row].GetActualDestRect().Bottom;
                    }
                    else Scrollbar.Value = 0;
                }
                else if (direction == PanelDirection.None)
                {
                    Scrollbar.Value = 0;
                }
            }

            var panelChildren = Children.OfType<Panel>().ToList();

            panelChildren.ForEach(x => x.Skin = GamePadSetup.DefaultSkin);

            _SelectedPanel = panelChildren[panelIndex];
            _SelectedPanel.Skin = GamePadSetup.SelectedSkin;

            if (Identifier == GamePadSetup.GetIdentifier(HierarchyIdentifier.RootGrid))
            {
                Skin = PanelSkin.None;
            }

            // DEBUG
            // UserInterface.Active.MouseInputProvider.UpdateMousePosition(_SelectedPanel.GetActualDestRect().Center.ToVector2());            
        }

        #region PanelGrid Layouts

        private void CreateDefaultLayout()
        {
            Children[0].Size = new Vector2(0.1f, 0.1f);
            Children[0].Anchor = Anchor.TopLeft;

            Children[1].Size = new Vector2(0.8f, 0.1f);
            Children[1].Anchor = Anchor.TopCenter;

            Children[2].Size = new Vector2(0.1f, 0.1f);
            Children[2].Anchor = Anchor.TopRight;

            Children[3].Size = new Vector2(0.1f, 0.8f);
            Children[3].Anchor = Anchor.CenterLeft;

            Children[4].Size = new Vector2(0.8f, 0.8f);
            Children[4].Anchor = Anchor.Center;

            Children[5].Size = new Vector2(0.1f, 0.8f);
            Children[5].Anchor = Anchor.CenterRight;

            Children[6].Size = new Vector2(0.1f, 0.1f);
            Children[6].Anchor = Anchor.BottomLeft;

            Children[7].Size = new Vector2(0.8f, 0.1f);
            Children[7].Anchor = Anchor.BottomCenter;

            Children[8].Size = new Vector2(0.1f, 0.1f);
            Children[8].Anchor = Anchor.BottomRight;
        }

        private void CreateSmallCornersLayout()
        {
            Children[0].Size = new Vector2(0.05f, 0.1f);
            Children[0].Anchor = Anchor.TopLeft;

            Children[1].Size = new Vector2(0.9f, 0.1f);
            Children[1].Anchor = Anchor.TopCenter;

            Children[2].Size = new Vector2(0.05f, 0.1f);
            Children[2].Anchor = Anchor.TopRight;

            Children[3].Size = new Vector2(0.1f, 0.8f);
            Children[3].Anchor = Anchor.CenterLeft;

            Children[4].Size = new Vector2(0.8f, 0.8f);
            Children[4].Anchor = Anchor.Center;

            Children[5].Size = new Vector2(0.1f, 0.8f);
            Children[5].Anchor = Anchor.CenterRight;

            Children[6].Size = new Vector2(0.05f, 0.1f);
            Children[6].Anchor = Anchor.BottomLeft;

            Children[7].Size = new Vector2(0.9f, 0.1f);
            Children[7].Anchor = Anchor.BottomCenter;

            Children[8].Size = new Vector2(0.05f, 0.1f);
            Children[8].Anchor = Anchor.BottomRight;
        }

        private void CreateSmallCornersVerticalsLayout()
        {
            Children[0].Size = new Vector2(0.05f, 0.1f);
            Children[0].Anchor = Anchor.TopLeft;

            Children[1].Size = new Vector2(0.9f, 0.1f);
            Children[1].Anchor = Anchor.TopCenter;

            Children[2].Size = new Vector2(0.05f, 0.1f);
            Children[2].Anchor = Anchor.TopRight;

            Children[3].Size = new Vector2(0.05f, 0.8f);
            Children[3].Anchor = Anchor.CenterLeft;

            Children[4].Size = new Vector2(0.9f, 0.8f);
            Children[4].Anchor = Anchor.Center;

            Children[5].Size = new Vector2(0.05f, 0.8f);
            Children[5].Anchor = Anchor.CenterRight;

            Children[6].Size = new Vector2(0.05f, 0.1f);
            Children[6].Anchor = Anchor.BottomLeft;

            Children[7].Size = new Vector2(0.9f, 0.1f);
            Children[7].Anchor = Anchor.BottomCenter;

            Children[8].Size = new Vector2(0.05f, 0.1f);
            Children[8].Anchor = Anchor.BottomRight;
        }

        #endregion PanelGridLayouts
    }
}
