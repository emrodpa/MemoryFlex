using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace winappMemoryFlexer
{
    public static class ButtonExtensions
    {
        public static async Task WhenClickedAsync(this Button p_Button)
        {
            var tcs = new TaskCompletionSource<object>();

            RoutedEventHandler lambda = (s, e) => tcs.TrySetResult(null);

            try
            {
                p_Button.Click += lambda;

                await tcs.Task;

            }
            finally
            {
                p_Button.Click -= lambda;

            }  // of try/finally


        }  // of WhenClickedAsync()

    }  // of  class ButtonExtensions

}  // of namespace winappMemoryFlexer
