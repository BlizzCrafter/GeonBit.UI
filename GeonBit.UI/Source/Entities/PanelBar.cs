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
        public PanelBar(params Entity[] entities)
            : base(new Vector2(0, 0))
        {
            int entitiesCount = entities.Length;

            Padding = new Vector2(8);

            for (int i = 0; i < entitiesCount; i++)
            {
                entities[i].Size = new Vector2(1f / entitiesCount, 0);
                AddChild(entities[i]);
            }

            Identifier = GetIdentifier(HierarchyIdentifier.PanelGrid);

            InvisiblePanel = true;
        }
    }
}
