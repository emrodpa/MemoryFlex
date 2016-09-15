using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Storage;
using System.Security.Cryptography;

namespace PlayLib_Phone
{
    public enum PlayLevels { Beginner = 0, Intermediate, Advance }

    /// <summary>
    /// TODO: inherit from IError
    /// </summary>
    public partial class FactorySequences
    {
        // Generate a random number.
        Random random = new Random();

        protected List<Figure> m_FigureList;

        public FactoryFigures FacFigures { get; protected set; }

        public SettingsForPlay oSettingsForPlay { get; protected set; }

        public List<Figure> FigureList
        {
            get
            {
                return m_FigureList;
            }
        }

        /// <summary>
        /// signals that the factory is ready
        /// </summary>
        public bool IsReady { get; protected set; }

        public FactorySequences(FactoryFigures p_FacFigures, SettingsForPlay p_SettingsForPlay)
        {
            IsReady = false;

            FacFigures = p_FacFigures;

            oSettingsForPlay = p_SettingsForPlay;

        }  // of parameterless constructor

        public FactorySequences(StorageFolder p_TargetFolder, SettingsForPlay p_SettingsForPlay)
            : this(new FactoryFigures(p_TargetFolder), p_SettingsForPlay)
        {
            IsReady = false;
        }  // of parameterless constructor

        public async Task<Sequence> CreateSequenceAsync()
        {
            Sequence l_Sequence = null;

            if (!IsReady)
                throw new Exception("Factory not ready");

            // select figures randomly
            List<Guid> l_FigureIDListForSequence = new List<Guid>();

            FacFigures.ResetPreviousFigureFilenameList();

            int l_Mod;
            while (l_FigureIDListForSequence.Count < oSettingsForPlay.NumberFiguresInSequence)
            {
                l_Mod = (Int32)GenerateRndNumber(m_FigureList.Count - 1);
                if (l_FigureIDListForSequence.Count(fid => fid.Equals(m_FigureList[l_Mod].Id)) > 0)
                    continue;
                l_FigureIDListForSequence.Add(m_FigureList[l_Mod].Id);
                l_Mod = -1;

            }  // of loop over positions available in grid

            l_Sequence = new Sequence(l_FigureIDListForSequence, oSettingsForPlay.IntervalBetweenFigures, oSettingsForPlay.PlayLevel);

            return l_Sequence;

        }  // of CreateSequenceAsync()

        /// <summary>
        /// This is a necessary step after constructor
        /// </summary>
        /// <returns></returns>
        public async Task PopulateFigureListAsync()
        {
            try
            {
                List<string> l_ImageFilenames = await GetFileNamesAsync();

                m_FigureList = new List<Figure>(oSettingsForPlay.NumberOfRows * oSettingsForPlay.NumberOfColumns);

                Figure l_Figure;
                int l_Mod;
                while (m_FigureList.Count < oSettingsForPlay.NumberOfRows * oSettingsForPlay.NumberOfColumns)
                {
                    l_Mod = (Int32)GenerateRndNumber(l_ImageFilenames.Count - 1);
                    if (m_FigureList.Count(f => f.ImageFilename == l_ImageFilenames[l_Mod]) > 0)
                        continue;
                    l_Figure = await FacFigures.CreateFigureAsync(l_ImageFilenames[l_Mod]);
                    m_FigureList.Add(l_Figure);
                    l_Mod = -1;
                    l_Figure = null;

                }  // of loop over positions available in grid

                IsReady = true;
            }
            catch (Exception ex)
            {
                // TODO: do something useful with the exception
                IsReady = false;
                m_FigureList = null;

            }  // of try/catch

        }  // of PopulateFigureListAsync()

        /// <summary>
        /// get all image filenames, throw error if folder doesn't exists
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetFileNamesAsync()
        {
            List<string> l_ImageFilenames = new List<string>();

            try
            {
                IReadOnlyList<StorageFile> fileList = await FacFigures.TargetFolder.GetFilesAsync();

                foreach (var file in fileList)
                {
                    l_ImageFilenames.Add(file.Name);
                }  // of foreach

                IsReady = true;
            }
            catch (Exception ex)
            {
                // TODO: do something useful with the exception
                IsReady = false;
                l_ImageFilenames = null;

            }  // of try/catch

            return l_ImageFilenames;

        }  // of GetFilesNameAsync()

        public long GenerateRndNumber(int p_MaxDesiredInteger)
        {
            // int l_Rnd = random.Next(0, p_MaxDesiredInteger);
            int l_Rnd = random.Next();
            //int l_RndBetweenZeroAnd9 = GetNewRandomNumber(0, p_MaxDesiredInteger);

            // long l_Mod = l_Rnd + l_RndBetweenZeroAnd9;
            long l_Mod = l_Rnd;

            if (l_Rnd > 0)
                l_Mod = (p_MaxDesiredInteger >= l_Rnd) ? p_MaxDesiredInteger % l_Rnd : l_Rnd % p_MaxDesiredInteger;

            ListModNumber.Add(l_Mod);

            int l_distint = ListModNumber.Distinct().Count();

            return l_Mod;

        }  // of GenerateRndNumber()

        public static List<long> ListModNumber = new List<long>();

        /// <summary>
        /// gets random number using Guid creation
        /// </summary>
        /// <param name="p_MindesiredInteger"></param>
        /// <param name="p_MaxDesiredInteger"></param>
        /// <returns></returns>
        public int GetNewRandomNumber(int p_MindesiredInteger, int p_MaxDesiredInteger)
        {
            int l_Number = -1;

            while (l_Number < p_MindesiredInteger || l_Number > p_MaxDesiredInteger)
            {
                Guid l_Guid = Guid.NewGuid();

                foreach (char s in l_Guid.ToString().ToList())
                {
                    if(Int32.TryParse(s.ToString(),out l_Number))
                    {
                        if (p_MindesiredInteger <= l_Number && l_Number <= p_MaxDesiredInteger)
                            break;
                    }  // of if

                }  // of foreach
            
            }  // of while

            return l_Number;
        
        }  // of GetNewRandomNumber()

    }  // of class FactorySequences


}  // of namespace PlayLib_Phone
