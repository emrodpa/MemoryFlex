using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Controls;

namespace PhoneAppMemoryFlex
{
    /// <summary>
    /// extends Navigate method of Frame
    /// </summary>
    public static class FrameNavigationExtension
    {
        private static object _navigationData = null;

        public static bool Navigate(this Frame frame, string page, object data)
        {
            bool l_bSuccess = true;

            try
            {
                _navigationData = data;
                frame.Navigate(new Uri(page, UriKind.Relative));
                return true;
            }
            catch(Exception ex)
            {
                l_bSuccess = false;
            }  // of try/catch

            return l_bSuccess;


        }  // of Navigate()

        public static object GetLastNavigationData(this Frame frame)
        {
            object data = _navigationData;
            _navigationData = null;
            return data;

        }  // of GetLastNavigationData()




    }  // of class FrameNavigationExtension

}  // of namespace PhoneAppMemoryFlex
