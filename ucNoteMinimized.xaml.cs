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

namespace Alice {
    /// <summary>
    /// Interaction logic for ucNoteMinimized.xaml
    /// </summary>
    public partial class ucNoteMinimized : UserControl {
        public Note _Note;

        public Note Note {
            get {
                return _Note;
            }
            set {
                _Note = value;
            }
        }

        public String Header {
            get {
                return _Note.Header;
            }
            set {
                _Note.Header = value;
            }
        }
        public String Text {
            get {
                return _Note.Text;
            }
            set {
                _Note.Text = value;
            }
        }

        public ucNoteMinimized(Note note) {
            InitializeComponent();

            if (note == null)
                throw new Exception("exc: note == null");
            
            _Note = note;

            Main.DataContext = this;
        }
    }
}
