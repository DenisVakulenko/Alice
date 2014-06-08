﻿using System;
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
            ans.AddProperty("время создания", Created.ToString());
            ans.AddProperty("время редактирования", Edited.ToString());
            ans.AddProperty("время просмотра", Viewed.ToString());
            return ans;
        }
    }
    

    public partial class taskNotes : Window {
        public List<Note> Notes = new List<Note>();

        public taskNotes() {
            InitializeComponent();

            Notes.Add(new Note(this, "первая тестовая заметка", "Тест!"));
            Notes.Add(new Note(this, "вторая первая тестовая заметка", "Тест2"));
        }

        public void DeleteNote(Note note) {
            Notes.Remove(note);
        }

        private void Button_Click(object sender, RoutedEventArgs e) {

        }
    }
}
