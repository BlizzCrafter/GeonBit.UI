using GeonBit.UI.Entities;
using GeonBit.UI.Source.Input;
using GeonBit.UI.Utils;
using Microsoft.Xna.Framework;
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
        SmallCornersVerticals,
        /// <summary>
        /// Same as SmallCorners but this one has also wide vertical bars which makes the center panel a bit smaller.
        /// </summary>
        SmallCornersWideVerticals,
        /// <summary>
        /// Same as SmallCornersWideVerticals but this one has even wider vertical bars which makes the center panel even smaller.
        /// </summary>
        SmallCornersUltraWideVerticals,
        /// <summary>
        /// Same as SmallCornersUltraWideVerticals but this one has even wider horizontal bars which makes the center panel even smaller.
        /// </summary>
        SmallCornersUltraWideVerticalsHorizontals
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
    public class PanelGrid : PanelGamePad
    {
        /// <summary>
        /// Static ctor.
        /// </summary>
        static PanelGrid()
        {
            Entity.MakeSerializable(typeof(PanelGrid));
        }

        /// <summary>
        /// Set the grid layout to the specified value.
        /// </summary>
        public void SetGridLayout(RootGridLayout rootGridLayout)
        {
            _NewRootGridLayout = rootGridLayout;

            if (_NewRootGridLayout == RootGridLayout.Default) CreateDefaultLayout();
            else if (_NewRootGridLayout == RootGridLayout.SmallCorners) CreateSmallCornersLayout();
            else if (_NewRootGridLayout == RootGridLayout.SmallCornersVerticals) CreateSmallCornersVerticalsLayout();
            else if (_NewRootGridLayout == RootGridLayout.SmallCornersWideVerticals) CreateSmallCornersWideVerticalsLayout();
            else if (_NewRootGridLayout == RootGridLayout.SmallCornersUltraWideVerticals) CreateSmallCornersUltraWideVerticalsLayout();
            else if (_NewRootGridLayout == RootGridLayout.SmallCornersUltraWideVerticalsHorizontals) CreateSmallCornersUltraWideVerticalsHorizontalsLayout();

            Children.ToList().ForEach(
                    rootGridPanel => rootGridPanel.Children.ToList().ForEach(
                    panelGrid => panelGrid.Children.OfType<IEntityGamePad>().ToList().ForEach(
                    IEntityGamePad => IEntityGamePad.TriggerOnLayoutChange(this))));
        }
        /// <summary>
        /// Revert the grid layout to the initially one.
        /// </summary>
        public void RevertGridLayout() => SetGridLayout(RootGridLayout);
        /// <summary>
        /// True if the RootGridLayout is the initially one. False if it was changed.
        /// </summary>
        public bool IsRootGridLayout => RootGridLayout == _NewRootGridLayout;
        /// <summary>
        /// The initial grid layout.
        /// </summary>
        public RootGridLayout RootGridLayout { get; private set; }
        private RootGridLayout _NewRootGridLayout;

        /// <summary>
        /// Calling this makes the PanelSelection available.
        /// It should be only called on the RootGrid and after all UserInterface elements are fully generated.
        /// </summary>
        public override void StartPanelSelection()
        {
            base.StartPanelSelection();

            HideEmptyPanels();
        }

        /// <summary>
        /// This hides all empty panels in the RootGrid.
        /// </summary>
        private void HideEmptyPanels()
        {
            foreach (Panel panel in Children.OfType<Panel>().ToList())
            {
                if (panel.Children.Count == 0) panel.Visible = false;
            }
        }

        /// <summary>
        /// This will be the used index when resetting the Panel-Selection.
        /// </summary>
        public override void SetDefaultPanelIndex(Anchor anchor)
        {
            int defaultIndex = GetGridPanelIndex(anchor);

            DefaultPanelIndex = defaultIndex == -1 ? 0 : defaultIndex;

            base.SetDefaultPanelIndex(anchor);
        }

        /// <summary>
        /// Get a Panel from the underlying PanelGrid.
        /// </summary>
        public Panel GetGridPanel(Anchor anchor)
        {
            int i = ConvertAnchorToIndex(anchor);

            if (Children != null && Children[i] != null)
            {
                return Children[i] as Panel;
            }
            else return null;
        }
        /// <summary>
        /// Get a Panel-Index from the underlying PanelGrid.
        /// </summary>
        public int GetGridPanelIndex(Anchor anchor)
        {
            int i = ConvertAnchorToIndex(anchor);

            if (Children != null && Children[i] != null)
            {
                return i;
            }
            else return -1;
        }
        private int ConvertAnchorToIndex(Anchor anchor)
        {
            if (anchor == Anchor.TopLeft) return 0;
            else if (anchor == Anchor.TopCenter) return 1;
            else if (anchor == Anchor.TopRight) return 2;
            else if (anchor == Anchor.CenterLeft) return 3;
            else if (anchor == Anchor.Center) return 4;
            else if (anchor == Anchor.CenterRight) return 5;
            else if (anchor == Anchor.BottomLeft) return 6;
            else if (anchor == Anchor.BottomCenter) return 7;
            else if (anchor == Anchor.BottomRight) return 8;
            else throw new Exceptions.InvalidValueException("The anchor parameter has an invalid value. Supported values are: TopLeft, TopCenter, TopRight, CenterLeft, Center, CenterRight, BottomLeft, BottomCenter, BottomRight.");
        }

        private int _CurrentRow => (PanelIndex / _Row) + 1;
        private int _StartOfTheRow => (_CurrentRow * _Row) - _Row;
        private int _EndOfTheRow => (_CurrentRow * _Row) - 1;
        private int _EndOfTheGridRow => (_RowCount * _Row) - _Row;
        private int _RowCount = 0;
        private const int _Row = 3;

        /// <summary>
        /// Create a 3x3 FullScreen GridPanel which acts as a centered root GridPanel for all other GridPanels you create.
        /// </summary>
        /// <param name="selectionDimension">Flat (2D) or Deep (3D) selection.</param>
        /// <param name="rootGridLayout">The layout of the GridPanel.</param>
        /// <param name="defaultPanelSelection">The Panel under this Anchor will be the target for resetting the Panel-Selection.</param>
        public PanelGrid(
            SelectionDimension selectionDimension,
            RootGridLayout rootGridLayout,
            Anchor defaultPanelSelection = Anchor.TopLeft)
            : this(Vector2.Zero, Anchor.Center, null)
        {
            //This is the RootGrid.
            RootGrid = this;

            //Identifiy it.
            Identifier = GetIdentifier(HierarchyIdentifier.RootGrid);

            PanelOverflowBehavior = PanelOverflowBehavior.Overflow;
            SelectionDimension = selectionDimension;

            CreatePanelGrid(9, new Vector2(0.33f, 0));
            {
                //Need to set these properties after panel grid creation.
                Padding = Vector2.Zero;
                SpaceAfter = Vector2.Zero;
                SpaceBefore = Vector2.Zero;
            }
            SetGridLayout(RootGridLayout = rootGridLayout);
            SetDefaultPanelIndex(defaultPanelSelection);
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

            Padding = new Vector2(16, 12);

            CreatePanelGrid(panelCount, panelSize);
        }
                
        private PanelGrid(
            Vector2 size,
            Anchor anchor = Anchor.Center,
            Vector2? offset = null)
            : base(size, anchor, offset)
        {
        }

        private void CreatePanelGrid(int panelCount, Vector2 panelSize)
        {
            PanelCount = panelCount;
            _RowCount = PanelCount / 3;

            PanelsGrid.GenerateColums(PanelCount, this, panelSize, GamePadSetup.DefaultSkin);

            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i] is Panel)
                {
                    Children[i].ClickThrough = true;
                    Children[i].Anchor = Anchor.AutoInline;
                    Children[i].Identifier = $"{Identifier}{GetIdentifier(HierarchyIdentifier.Panel)}#{i}";
                }
            }

            Skin = PanelSkin.None;
        }

        /// <summary>
        /// Selects a panel from the PanelGrid.
        /// </summary>
        /// <param name="direction">The direction in which the index should try to shift.</param>
        protected override void SelectPanel(PanelDirection direction)
        {
            if (direction == PanelDirection.Right)
            {
                if (PanelIndex + 1 > _EndOfTheRow) PanelIndex = _StartOfTheRow;
                else PanelIndex++;
            }
            else if (direction == PanelDirection.Left)
            {
                if (PanelIndex - 1 < _StartOfTheRow) PanelIndex = _EndOfTheRow;
                else PanelIndex--;
            }
            else if (direction == PanelDirection.Down)
            {
                if (_RowCount > 1)
                {
                    if (_CurrentRow < _RowCount) PanelIndex += _Row;
                    else PanelIndex -= _StartOfTheRow;
                }
            }
            else if (direction == PanelDirection.Up)
            {
                if (_RowCount > 1)
                {
                    if (_CurrentRow > 1) PanelIndex -= _Row;
                    else PanelIndex += _EndOfTheGridRow;
                }
            }

            base.SelectPanel(direction);
        }

        /// <summary>
        /// Changes the skin of the selected panel and scrolls the view (GridPanel) along in case a scrollbar is available.
        /// </summary>
        /// <param name="direction">The direction of the panel selector.</param>
        protected override void UpdatePanelSelection(PanelDirection direction = PanelDirection.None)
        {
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
                        Scrollbar.Value += Children[PanelIndex - _Row].GetActualDestRect().Bottom / 2;
                    }
                }
                else if (direction == PanelDirection.Up)
                {
                    if (Scrollbar.Value == 0 && _CurrentRow != 1)
                    {
                        Scrollbar.Value += Children[PanelIndex - (_Row * 2)].GetActualDestRect().Bottom;
                    }
                    else if (_CurrentRow > 1)
                    {
                        Scrollbar.Value += Children[PanelIndex - _Row].GetActualDestRect().Bottom;
                    }
                    else Scrollbar.Value = 0;
                }
                else if (direction == PanelDirection.None)
                {
                    Scrollbar.Value = 0;
                }
            }

            base.UpdatePanelSelection(direction);
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

        private void CreateSmallCornersWideVerticalsLayout()
        {
            Children[0].Size = new Vector2(0.05f, 0.1f);
            Children[0].Anchor = Anchor.TopLeft;

            Children[1].Size = new Vector2(0.9f, 0.1f);
            Children[1].Anchor = Anchor.TopCenter;

            Children[2].Size = new Vector2(0.05f, 0.1f);
            Children[2].Anchor = Anchor.TopRight;

            Children[3].Size = new Vector2(0.15f, 0.8f);
            Children[3].Anchor = Anchor.CenterLeft;

            Children[4].Size = new Vector2(0.7f, 0.8f);
            Children[4].Anchor = Anchor.Center;

            Children[5].Size = new Vector2(0.15f, 0.8f);
            Children[5].Anchor = Anchor.CenterRight;

            Children[6].Size = new Vector2(0.05f, 0.1f);
            Children[6].Anchor = Anchor.BottomLeft;

            Children[7].Size = new Vector2(0.9f, 0.1f);
            Children[7].Anchor = Anchor.BottomCenter;

            Children[8].Size = new Vector2(0.05f, 0.1f);
            Children[8].Anchor = Anchor.BottomRight;
        }

        private void CreateSmallCornersUltraWideVerticalsLayout()
        {
            Children[0].Size = new Vector2(0.05f, 0.1f);
            Children[0].Anchor = Anchor.TopLeft;

            Children[1].Size = new Vector2(0.9f, 0.1f);
            Children[1].Anchor = Anchor.TopCenter;

            Children[2].Size = new Vector2(0.05f, 0.1f);
            Children[2].Anchor = Anchor.TopRight;

            Children[3].Size = new Vector2(0.25f, 0.8f);
            Children[3].Anchor = Anchor.CenterLeft;

            Children[4].Size = new Vector2(0.5f, 0.8f);
            Children[4].Anchor = Anchor.Center;

            Children[5].Size = new Vector2(0.25f, 0.8f);
            Children[5].Anchor = Anchor.CenterRight;

            Children[6].Size = new Vector2(0.05f, 0.1f);
            Children[6].Anchor = Anchor.BottomLeft;

            Children[7].Size = new Vector2(0.9f, 0.1f);
            Children[7].Anchor = Anchor.BottomCenter;

            Children[8].Size = new Vector2(0.05f, 0.1f);
            Children[8].Anchor = Anchor.BottomRight;
        }

        private void CreateSmallCornersUltraWideVerticalsHorizontalsLayout()
        {
            Children[0].Size = new Vector2(0.05f, 0.1f);
            Children[0].Anchor = Anchor.TopLeft;

            Children[1].Size = new Vector2(0.9f, 0.25f);
            Children[1].Anchor = Anchor.TopCenter;

            Children[2].Size = new Vector2(0.05f, 0.1f);
            Children[2].Anchor = Anchor.TopRight;

            Children[3].Size = new Vector2(0.25f, 0.8f);
            Children[3].Anchor = Anchor.CenterLeft;

            Children[4].Size = new Vector2(0.5f, 0.5f);
            Children[4].Anchor = Anchor.Center;

            Children[5].Size = new Vector2(0.25f, 0.8f);
            Children[5].Anchor = Anchor.CenterRight;

            Children[6].Size = new Vector2(0.05f, 0.1f);
            Children[6].Anchor = Anchor.BottomLeft;

            Children[7].Size = new Vector2(0.9f, 0.25f);
            Children[7].Anchor = Anchor.BottomCenter;

            Children[8].Size = new Vector2(0.05f, 0.1f);
            Children[8].Anchor = Anchor.BottomRight;
        }

        #endregion PanelGridLayouts
    }
}
