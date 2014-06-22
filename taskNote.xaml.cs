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
    /// Interaction logic for taskNote.xaml
    /// </summary>
    public partial class taskNote : Task {
        public Note _Note;

        public Note GetNote {
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

        public taskNote(Note note) {
            InitializeComponent();

            if (note == null)
                throw new Exception("exc: note == null");
            _Note = note;
            Main.DataContext = this;

            InitPredicatesAndUpdateDictionary();
        }

        private void InitPredicatesAndUpdateDictionary() {
            Predicat prShow;
            Predicat prHide;
            Predicat prFix;

            var notes = (Noun)Brain.Word("заметки");
            notes.AddSynonym("записки");
            notes.AddSynonym("пометки");
            notes.AddSynonym("записи");
            notes.AddGiperonim("что");

            prShow = new Predicat("открой");
            prShow.Action.AddSynonym("покажи", 0.8);
            prShow.Action.AddSynonym("запусти", 0.8);
            prShow.Action.AddSynonym("дай", 0.3);
            prShow.AddProperty("заметки");

            prHide = new Predicat("закрой");
            prHide.Action.AddSynonym("скрой", 0.6);
            prHide.Action.AddSynonym("убери", 0.6);
            prHide.Action.AddSynonym("сверни", 0.3);
            prHide.AddProperty("заметки");

            prFix = new Predicat("запиши");
            prFix.Action.AddSynonym("сохрани", 0.6);
            prFix.AddProperty("что");

            var prNew = new Predicat("создай");
            prNew.Action.AddSynonym("сделай", 0.6);
            prNew.AddProperty("заметка");

            var prFind = new Predicat("найди");
            prFind.Action.AddSynonym("покажи", 0.6);
            prFind.Action.AddSynonym("открой", 0.6);
            prFind.AddProperty("что");


            //AddAction(prShow, ActionShow);
            AddAction(prHide, ActionHide);
            //AddAction(prFind, ActionFind);
            //AddAction(prNew, ActionNew);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {

        }
    }
}
