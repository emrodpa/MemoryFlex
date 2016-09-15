using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Storage;

namespace PlayLib_Phone
{

    /// <summary>
    /// TODO: implement interface IError
    /// </summary>

    public partial class FactoryFigures
    {
        public StorageFolder TargetFolder { get; protected set; }

        /// <summary>
        /// keeps track of the file used to create previous figures
        /// </summary>
        public List<string> PreviousFigureFilenameList { get; protected set; }

        protected FactoryFigures()
        {
            PreviousFigureFilenameList = new List<string>();

        }  // of parameterless constructor

        public FactoryFigures(StorageFolder p_TargetFolder)
            : this()
        {
            TargetFolder = p_TargetFolder;

        }  // of overloaded constructor

        /// <summary>
        /// imperative to use this method as creation of figures
        /// could be for more of one purpose
        /// </summary>
        public void ResetPreviousFigureFilenameList()
        {
            PreviousFigureFilenameList = new List<string>();

        }  // of ResetPreviousFigureFilenameList()

        public async Task<Figure> CreateFigureAsync(string p_ImageFilename)
        {
            // TODO: check p_ImagePath is a valid path, through error event otherwise
            Task<bool> l_TaskFileExists = DoesFileExistAsync(p_ImageFilename);

            bool l_FileExists = await l_TaskFileExists;

            if (!l_FileExists)
                throw new Exception(string.Format("File {0} doesn't exists in folder {1}", p_ImageFilename, TargetFolder.DisplayName));

            // check figure hasn't been created yet
            if (PreviousFigureFilenameList.Contains(p_ImageFilename))
                throw new Exception(string.Format("Figure already created for file {0}", p_ImageFilename));
            else
                PreviousFigureFilenameList.Add(p_ImageFilename);

            return new Figure(p_ImageFilename);

        }  // of CreateFigureAsync()

        public async Task<bool> DoesFileExistAsync(string p_FileName)
        {
            try
            {
                await TargetFolder.GetFileAsync(p_FileName);
                return true;
            }
            catch
            {
                return false;
            }  // of try/catch

        }  // of DoesFileExistAsync()

    }  // of class FactoryFigures

}  // of namespace PlayLib_Phone
