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
using System.Windows.Shapes;

namespace Alice {
    /// <summary>
    /// Interaction logic for taskDialogBoc.xaml
    /// </summary>
    public partial class taskDialogBox : Window {
        public AliceGUIManager GUIMng;

        public taskDialogBox(AliceGUIManager manager) {
            InitializeComponent();

            GUIMng = manager;
        }

        private void txtMsg_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                GUIMng.NewText(txtMsg.Text);
                txtMsg.Text = "";
            }
        }

        public void TellText(String text) {
            tbHistory.Text += text;
        }
    }
}
