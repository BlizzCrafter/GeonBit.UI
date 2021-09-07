using GeonBit.UI.Entities;
using GeonBit.UI.Source.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeonBit.UI.Source.Entities
{
    /// <summary>
    /// The orientation of the PanelBar.
    /// </summary>
    public enum Orientation
    {
        /// <summary>
        /// Horizontal is best suited for entities going from left to right. 
        /// </summary>
        Horizontal,
        /// <summary>
        /// Vertical is best suited for entities goning from top to bottom.
        /// </summary>
        Vertical
    }
    /// <summary>
    /// A PanelBar makes it easy to create a Panel with Entities in it.
    /// </summary>
    [System.Serializable]
    public class PanelBar : PanelGamePad
    {
        /// <summary>
        /// Static ctor.
        /// </summary>
        static PanelBar()
        {
            Entity.MakeSerializable(typeof(PanelBar));
        }

        /// <summary>
        /// Creates a basic PanelBar with a custom amount of entities.
        /// </summary>
        public PanelBar(Orientation orientation, params Entity[] entities)
            : base(new Vector2(0, 0))
        {
            int entitiesCount = entities.Length;

            Padding = new Vector2(6);

            for (int i = 0; i < entitiesCount; i++)
            {
                entities[i].Size = orientation == Orientation.Horizontal ? new Vector2(1f / entitiesCount, 0) : new Vector2(0, 1f / entitiesCount);
                entities[i].SpaceAfter = Vector2.Zero;
                entities[i].SpaceBefore = Vector2.Zero;
                AddChild(entities[i]);
            }

            InvisiblePanel = true;
        }
    }
}
