using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace winappMemoryFlexer
{
    public partial class GlobalHolder
    {
        public int NumberOfRows {get; protected set;}
        public int NumberOfColumns { get; protected set; }

        public int GridWidth { get; protected set; }
        public int GridHeight { get; protected set; }

        public int DeltaHeight
        {
            get
            {
                return GridHeight / NumberOfRows;
            }
        }

        public int DeltaWidth
        {
            get
            {
                return GridWidth / NumberOfColumns;
            }
        }

        protected GlobalHolder()
        {
            //m_ImageNamePositionList = new List<ImageNamePosition>();

        }  // of constructor

        public GlobalHolder(int p_NumberOfRows, int p_NumberOfColumns, int p_GridWidth, int p_GridHeight)
            : this()
        {
            NumberOfRows = p_NumberOfRows;
            NumberOfColumns = p_NumberOfColumns;
            GridWidth = p_GridWidth;
            GridHeight = p_GridHeight;
                    
        }  // of overloaded constructor

    }  // of class GlobalHolder

}  // of namespace winappMemoryFlexer  
