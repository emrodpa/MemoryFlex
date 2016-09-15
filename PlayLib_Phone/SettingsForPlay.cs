using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using Windows.Data.Xml;
using System.Xml;
using System.Xml.Linq;
//using Windows.Data.Xml.Dom;
using Windows.Storage;

namespace PlayLib_Phone
{
    /// <summary>
    /// holder of several settings
    /// for a given to PlayLevel value
    /// </summary>
    public partial class SettingsForPlay
    {
        public PlayLevels PlayLevel { get; protected set; }

        public int NumberFiguresInSequence { get; protected set; }

        /// <summary>
        /// in milliseconds
        /// </summary>
        public int IntervalBetweenFigures { get; protected set; }

        public Goal oGoal { get; protected set; }

        public int NumberOfRows { get; protected set; }
        public int NumberOfColumns { get; protected set; }

        public int GridWidth { get; set; }
        public int GridHeight { get; set; }

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

        public SettingsForPlay(PlayLevels p_PlayLevel, int p_NumberFiguresInSequence, int p_IntervalBetweenFigures, int p_MaxPLayChances, int p_MaximunPlayTimeInSeconds,
                               int p_NumberOfRows, int p_NumberOfColumns, int p_GridWidth, int p_GridHeight
                              )
        {
            PlayLevel = p_PlayLevel;
            NumberFiguresInSequence = p_NumberFiguresInSequence;
            IntervalBetweenFigures = p_IntervalBetweenFigures;
            oGoal = new Goal(p_MaxPLayChances, p_MaximunPlayTimeInSeconds);
            GridWidth = p_GridWidth;
            GridHeight = p_GridHeight;

            // do some adjustment for device position and/or resolution
            // by keeping product NumOfRows * NumberOfColums
            // for now just a swap of columns and row numbers for portrait orientations
            if (GridWidth >= GridHeight)
            {
                NumberOfRows = p_NumberOfRows;
                NumberOfColumns = p_NumberOfColumns;

            }
            else
            {
                NumberOfRows = p_NumberOfColumns;
                NumberOfColumns = p_NumberOfRows;

                /*
                // limit number of columns to 4 when width is too thin
                if (GridWidth < 660)
                {
                    NumberOfColumns = SplitIntoIntegerProduct(p_NumberOfRows * p_NumberOfColumns, new List<int>() { 4,3,5,6,7 });
                    NumberOfRows = (int) Math.Round(1.0 * p_NumberOfRows * p_NumberOfColumns / NumberOfColumns);

                }  // of if width is too thin
                */

            }  // of if/else is landscape orientation

        }  // of SettingsForPlay()

        public int SplitIntoIntegerProduct(int p_TotalNumber, List<int> p_AvailableNumbers)
        {
            int l_ColumnNumber = -1;

            int l_nCounter = -1;

            while (l_nCounter < p_AvailableNumbers.Count)
            {
                l_nCounter++;

                l_ColumnNumber = p_AvailableNumbers[l_nCounter];

                if (p_TotalNumber % l_ColumnNumber == 0)
                {
                    break;
                }  // of if is a multiple

            }  // of while

            return l_ColumnNumber;

        }  // of FindSuitableNumberOfColumns()

        public static List<SettingsForPlay> GetListFromFileAsync(string l_ConfigFilename, int p_GridWidth, int p_GridHeight)
        {
            List<SettingsForPlay> l_SettingsForPlayList = new List<SettingsForPlay>();

            XDocument l_XDocument = XDocument.Load(l_ConfigFilename);

            var l_XmlElemPlayLevels = from elem in l_XDocument.Descendants("playlevel")
                                      where 1 == 1
                                      select elem;

            SettingsForPlay l_oSettingsForPlay;
            foreach (var elem in l_XmlElemPlayLevels)
            {
                var l_XmlElemLevel = elem.Element("levelid");
                PlayLevels l_PlayLevel = (PlayLevels)Int32.Parse(l_XmlElemLevel.Value);

                var l_XmlElemNumberFiguresInSequence = elem.Element("NumberFiguresInSequence");
                int l_NumberFiguresInSequence = Int32.Parse(l_XmlElemNumberFiguresInSequence.Value);

                var l_XmlElemIntervalBetweenFigures = elem.Element("IntervalBetweenFigures");
                int l_IntervalBetweenFigures = Int32.Parse(l_XmlElemIntervalBetweenFigures.Value);

                var l_XmlElemNumberOfRows = elem.Element("NumberOfRows");
                int l_NumberOfRows = Int32.Parse(l_XmlElemNumberOfRows.Value);

                var l_XmlElemNumberOfColumns = elem.Element("NumberOfColumns");
                int l_NumberOfColumns = Int32.Parse(l_XmlElemNumberOfColumns.Value);

                var l_XmlElemMaxPLayChances = elem.Element("MaxPLayChances");
                int l_MaxPLayChances = Int32.Parse(l_XmlElemMaxPLayChances.Value);

                var l_XmlElemMaximunPlayTimeInSeconds = elem.Element("MaximumPlayTimeInSeconds");
                int l_MaximunPlayTimeInSeconds = Int32.Parse(l_XmlElemMaximunPlayTimeInSeconds.Value);

                l_oSettingsForPlay = new SettingsForPlay(l_PlayLevel, l_NumberFiguresInSequence, l_IntervalBetweenFigures, l_MaxPLayChances, l_MaximunPlayTimeInSeconds, l_NumberOfRows, l_NumberOfColumns, p_GridWidth, p_GridHeight);

                l_SettingsForPlayList.Add(l_oSettingsForPlay);

                l_oSettingsForPlay = null;

            }  // of loop over playlevel nodes

            return l_SettingsForPlayList;

        }  // of  GetListFromFileAsync()

    }   // of class SettingsForPlay

}  // of namespace PlayLib_Phone
