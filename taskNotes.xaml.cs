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
    public class Note {
        public taskNotes Parent;

        public String Header;
        public String Text;
        public List<String> Tags = new List<string>();

        public DateTime Created;
        public DateTime Edited;
        public DateTime Viewed;

        public Note(taskNotes parent, String text = "", String title = "") {
            Parent = parent;

            Header = title;
            Text = text;

            Created = DateTime.Now;
            Edited = Created;
            Viewed = Created;
        }

        public void Show() {
            var Task = new taskNote(this);
            Task.Show();
        }

        public void Delete() {
            if (Parent == null)
                return;

            Parent.DeleteNote(this);
        }

        public Object ConvertToNL() {
            var ans = new Object("заметка");
            ans.AddProperty("содержание", Text);
            ans.AddProperty("заголовок", Text);
            foreach (var i in Tags)
                ans.AddProperty("тег", i);
            ans.AddProperty("время создания", Created);
            ans.AddProperty("время редактирования", Edited);
            ans.AddProperty("время просмотра", Viewed);
            return ans;
        }
        public List<Predicat> Predicats() {
            var ps = new List<Predicat>();

            var obj = ConvertToNL();
            
            var p = new Predicat("написал");
            p.Action.AddSynonym("создал");
            p.Action.AddSynonym("записал");
            p.AddProperty("что", obj);
            p.AddProperty("когда", Created);
            ps.Add(p);

            p = new Predicat("смотрел");
            p.Action.AddSynonym("просматривал");
            p.Action.AddSynonym("читал");
            p.Action.AddSynonym("открывал");
            p.AddProperty("что", obj);
            p.AddProperty("когда", Viewed);
            ps.Add(p);

            p = new Predicat("редактировал");
            p.Action.AddSynonym("дописывал");
            p.Action.AddSynonym("изменял");
            p.Action.AddSynonym("менял");
            p.Action.AddSynonym("править");
            p.AddProperty("что", obj);
            p.AddProperty("когда", Edited);
            ps.Add(p);

            return ps;
        }
    }
    

    public partial class taskNotes : Alice.Task {
        public List<Note> Notes = new List<Note>();

        public taskNotes() {
            InitializeComponent();

            InitPredicatesAndUpdateDictionary();

            Notes.Add(new Note(this, "первая тестовая заметка", "Тест!"));
            Notes.Add(new Note(this, "вторая первая тестовая заметка", "Тест2"));

            UpdateNotes();

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Start();
        }


        private void dispatcherTimer_Tick(object sender, EventArgs e) {
            UpdateNotes();
        }


        public void UpdateNotes() {
            stackNotes.Children.Clear();
            foreach (var i in Notes) {
                stackNotes.Children.Add(new ucNoteMinimized(i));
            }
        }

        private Predicat prFind;

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
            prShow.Action.AddSynonym("покажи", 0.6);
            prShow.Action.AddSynonym("запусти", 0.6);
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

            prFind = new Predicat("найди");
            prFind.Action.AddSynonym("покажи", 0.6);
            prFind.Action.AddSynonym("открой", 0.6);
            prFind.AddProperty("что");

            
            AddAction(prShow, ActionShow);
            AddAction(prHide, ActionHide);
            AddAction(prFind, ActionFind);
        }


        public void DeleteNote(Note note) {
            Notes.Remove(note);
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Notes.Add(new Note(this, "", ""));
            Notes.Last().Show();
        }

        public void ActionFind(Predicat pr) {
            var p = pr.FindProperty(new Property(new Object("что"), Property.ObjectPropertyType.NoValue_NoType));
            if (p == null)
                p = pr.FindProperty(new Property(new Object("что")));

            if (p == null)
                return;

            Double conf;
            
            var note = FindNote(p.ObjectValue, out conf);
            note.Show();

            AliceGUIManager.TellText(note.ToString());
        }

        //public Property AnsverTo(Predicat p, out Double conf) {
        //    Double maxConf;
        //    Property bstAns;
        //    foreach (var i in Notes) {
        //        var ans = i.ConvertToNL().CompareTo(p);
        //    }
        //    return bstAns;
        //}
        public Note FindNote(Object p, out Double conf) {
            conf = 0;
            Object bstAns = null;
            Note bstNote = null;
            foreach (var i in Notes) {
                var obj = i.ConvertToNL();
                var c = i.ConvertToNL().CompareTo(p);
                if (conf < c) {
                    conf = c;
                    bstAns = obj;
                    bstNote = i;
                }
            }
            return bstNote;
        }

    }
}
