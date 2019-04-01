using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EF.Pages
{
    /// <summary>
    /// Interaction logic for SerialInfo.xaml
    /// </summary>
    public partial class SerialInfoPage : Page
    {
        public SerialInfoPage()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string newValue = userMark.Text + e.Text[0];
            try
            {
                int res = Int32.Parse(newValue);
                if (res < 0 || res > 100)
                {
                    e.Handled = true;
                }
            }
            catch
            {
                 e.Handled = true;
            }
            

            
            
        }
    }
}
