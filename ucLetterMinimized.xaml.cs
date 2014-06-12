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

using System.Drawing;

namespace Alice {
    /// <summary>
    /// Interaction logic for ucNoteMinimized.xaml
    /// </summary>
    public partial class ucLetterMinimized : UserControl {
        private Letter _Letter;
        private String _DisplayText;

        public Letter Letter {
            get {
                return _Letter;
            }
            set {
                _Letter = value;
                if (_Letter.Text.Length > 200)
                    _DisplayText = _Letter.Text.Substring(0, 200).Trim() + "...";
                else
                    _DisplayText = _Letter.Text.Trim();

                if (_Letter.WasViewed) {
                    this.Background = new SolidColorBrush(Color.FromArgb(10, 0, 0, 0));
                }
            }
        }

        public String Header {
            get {
                return _Letter.Header;
            }
        }
        public String From {
            get {
                if (_Letter.MyLetter)
                    return "кому: " + _Letter.Recipient;
                else
                    return "от: " + _Letter.Author;
            }
        }
        public String Text {
            get {
                return _DisplayText;
            }
        }
        public String StrDate {
            get {
                String s = "";

                if (_Letter.Arrived.Day == DateTime.Now.Day)
                    s = "сегодня";
                else if (_Letter.Arrived.Day == DateTime.Now.Day - 1)
                    s = "вчера";
                else if (_Letter.Arrived.Day == DateTime.Now.Day - 2)
                    s = "позавчера";
                else
                    s = _Letter.Arrived.Date.ToShortDateString();

                s += '\n' + _Letter.Arrived.TimeOfDay.ToString();

                return s;
            }
        }

        public ucLetterMinimized(Letter letter) {
            InitializeComponent();

            if (letter == null)
                throw new Exception("exc: note == null");
            
            Letter = letter;

            Main.DataContext = this;
        }
    }
}
