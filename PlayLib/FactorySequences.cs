using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Security.Cryptography;

namespace PlayLib
{
    public enum PlayLevels { Beginner=0, Intermediate, Advance }

    /// <summary>
    /// TODO: inherit from IError
    /// </summary>
    public partial class FactorySequences
    {
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
            catch(Exception ex)
            {
                // TODO: do something useful with the exception
                IsReady = false;
                l_ImageFilenames = null;

            }  // of try/catch

            return l_ImageFilenames;

        }  // of GetFilesNameAsync()

        public long GenerateRndNumber(int p_MaxDesiredInteger)
        {
            // Generate a random number.
            UInt32 Rnd = CryptographicBuffer.GenerateRandomNumber();

            long l_Mod = (p_MaxDesiredInteger >= Rnd) ? p_MaxDesiredInteger % Rnd : Rnd % p_MaxDesiredInteger;

            return l_Mod;

        }  // of GenerateRndNumber()

    }  // of class FactorySequences

}  // of namespace PlayLib
