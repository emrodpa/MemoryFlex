using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayLib_Phone
{
    public partial class Sequence
    {
        public Guid Id { get; protected set; }

        public List<Guid> FigureIDList { get; protected set; }

        /// <summary>
        /// elapsed between displayed figures in the sequence
        /// (in milliseconds)
        /// </summary>
        public int Interval { get; protected set; }

        public PlayLevels Level { get; protected set; }

        protected Sequence()
        {
            Id = Guid.NewGuid();

            FigureIDList = new List<Guid>();

        }  // of parameterless constructor

        public Sequence(List<Guid> p_FigureIDList, int p_Interval, PlayLevels p_Level)
            : this()
        {
            foreach (Guid figureID in p_FigureIDList)
            {
                if (!FigureIDList.Exists(fid => fid.Equals(figureID)))
                    FigureIDList.Add(figureID);
                else
                    throw new Exception(string.Format("Figure {0} already in sequence", figureID));

            }  // of loop over Figures

            Interval = p_Interval;
            Level = p_Level;

        }  // of overloaded constructor

    }  // of class Sequence

}  // of namespace PlayLib_Phone
