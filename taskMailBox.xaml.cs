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

using System.Net.Mail;

using System.Data;
using System.Configuration;
using System.Web;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Net.Sockets;

using S22.Imap;

namespace Alice {
    /// <summary>
    /// Interaction logic for taskMailBox.xaml
    /// </summary>
    /// 


    public class Letter {
        public taskMailBox Parent;

        public MailMessage Msg;

        public String Author;
        public String Recipient;

        public String Header;
        public String Text;
        public List<String> Tags = new List<string>();

        public DateTime Arrived;
        public bool WasAnsver = false;
        public bool WasViewed = false;
        public DateTime Ansvered;
        public DateTime Viewed;

        public bool MyLetter = false;

        public Letter(taskMailBox parent, String text = "", String title = "") {
            Parent = parent;

            Header = title;
            Text = text;

            Arrived = DateTime.Now;
        }

        public Letter(taskMailBox parent, String aut, String rec, String text, String title, DateTime arr) {
            Parent = parent;

            Author = aut;
            Recipient = rec;

            Header = title;
            Text = text;

            Arrived = arr;
        }

        public Letter(taskMailBox parent, MailMessage i, Boolean viewed = true, Boolean my = false) {
            Parent = parent;

            Author = i.From.DisplayName;
            Recipient = i.To[0].Address;

            Header = StrConv(i.Subject, i.SubjectEncoding);
            Text = StrConv(i.Body, i.BodyEncoding);

            Arrived = (i.Date() == null) ? new DateTime() : (DateTime)i.Date();

            WasViewed = viewed;
            MyLetter = my;
        }


        private String StrConv(String msg, Encoding enc) {
            if (enc == null)
                return msg;
            Encoding iso = enc; //Encoding.GetEncoding("ISO-8859-1");
            Encoding utf8 = Encoding.UTF8;
            byte[] utfBytes = utf8.GetBytes(msg);
            byte[] isoBytes = Encoding.Convert(utf8, iso, utfBytes);
            return iso.GetString(isoBytes);
        }

        public void Show() {
            //var Task = new taskLetter(this);
            //Task.Show();
        }

        public void Delete() {
            if (Parent == null)
                return;

            Parent.DeleteLetter(this);
        }

        public Object ConvertToNL() {
            var ans = new Object("письмо");
            ans.AddProperty("содержание", Text);
            ans.AddProperty("заголовок", Text);
            foreach (var i in Tags)
                ans.AddProperty("тег", i);
            ans.AddProperty("время создания", Arrived);
            if (WasViewed) {
                ans.AddProperty("прочитано");
                ans.AddProperty("время просмотра", Viewed);
            }
            if (WasAnsver) {
                //ans.AddProperty("отвечено");
                ans.AddProperty("время ответа", Ansvered);
            }
            return ans;
        }
        public List<Predicat> Predicats() {
            var ps = new List<Predicat>();

            var obj = ConvertToNL();

            var p = new Predicat("пришло");
            p.Action.AddSynonym("прилетело", 0.3);
            p.AddProperty("что", obj);
            p.AddProperty("когда", Arrived);
            ps.Add(p);

            p = new Predicat("смотрел");
            p.Action.AddSynonym("просматривал");
            p.Action.AddSynonym("читал");
            p.Action.AddSynonym("открывал");
            p.AddProperty("что", obj);
            p.AddProperty("когда", Viewed);
            ps.Add(p);

            p = new Predicat("отвечал");
            p.AddProperty("на", obj);
            p.AddProperty("когда", Ansvered);
            ps.Add(p);

            return ps;
        }
    }
    




    public partial class taskMailBox : Task {
        public List<Letter> Letters = new List<Letter>();

        public taskMailBox() {
            InitializeComponent();

            InitPredicatesAndUpdateDictionary();

            UpdateLetters();

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 1, 0);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e) {
            UpdateLetters();
        }


        private Dictionary<uint, MailMessage> Hash;

        public ImapClient GetClient() {
            return new ImapClient("imap.gmail.com", 993, "denisvvakulenko@gmail.com", "i'll make alice", AuthMethod.Login, true);
        }
        public void UpdateLetters() {
            if (this.Visibility != System.Windows.Visibility.Visible)
                return;

            Boolean inbx = (String)btnInOut.Content == "исх.";
            if (inbx)
            using (var client = GetClient()) {
                IEnumerable<uint> uids = client.Search(SearchCondition.Unseen());
                IEnumerable<MailMessage> messages = client.GetMessages(uids, FetchOptions.TextOnly);

                foreach (var i in messages)
                    Letters.Add(new Letter(this, i, false));

                foreach (var i in uids)
                    client.RemoveMessageFlags(i, null, MessageFlag.Seen);

                uids = client.Search(SearchCondition.Seen());
                List<uint> lastUids = new List<uint>();
                for (int i = uids.Count() - 1; i > uids.Count() - 10; i--) 
                    lastUids.Add(uids.ElementAt(i));

                messages = client.GetMessages(lastUids, FetchOptions.TextOnly);
                
                foreach (var i in messages)
                    Letters.Add(new Letter(this, i));
            }
            else
            using (var client = GetClient()) {
                var a = client.ListMailboxes();
                client.DefaultMailbox = "[Gmail]/Отправленные";

                IEnumerable<uint> uids = client.Search(SearchCondition.All());
                List<uint> lastUids = new List<uint>();
                for (int i = uids.Count() - 1; i > uids.Count() - 10; i--)
                    lastUids.Add(uids.ElementAt(i));

                IEnumerable<MailMessage> messages = client.GetMessages(lastUids, FetchOptions.TextOnly);

                foreach (var i in messages)
                    Letters.Add(new Letter(this, i, true, true));
            }

            stackNotes.Children.Clear();
            foreach (var i in Letters) {
                if (!inbx && i.MyLetter || inbx && !i.MyLetter)
                    stackNotes.Children.Add(new ucLetterMinimized(i));
            }
        }

        private void InitPredicatesAndUpdateDictionary() {
            Predicat prShow;
            Predicat prHide;
            Predicat prAnsver;

            var letters = (Noun)Brain.Word("письма");
            letters.AddSynonym("бандероли");
            letters.AddGiperonim("что");

            prShow = new Predicat("открой");
            prShow.AddProperty("почту");

            var prShow2 = new Predicat("открой");
            prShow.AddProperty("письма");

            prHide = new Predicat("закрой");
            prHide.AddProperty("почту");

            var prHide2 = new Predicat("закрой");
            prHide.AddProperty("письма");

            prAnsver = new Predicat("напиши");
            prAnsver.Action.AddSynonym("ответь", 0.9);
            prAnsver.AddProperty("что");
            prAnsver.AddProperty("кому");

            //prFind = new Predicat("найди");
            //prFind.Action.AddSynonym("покажи", 0.6);
            //prFind.Action.AddSynonym("открой", 0.6);
            //prFind.AddProperty("что");


            AddAction(prShow, ActionShow);
            AddAction(prHide, ActionHide);

            AddAction(prShow2, ActionShow);
            AddAction(prHide2, ActionHide);
            //AddAction(prFind, ActionFind);
        }

        public void DeleteLetter(Letter l) {
            Letters.Remove(l);
        }


        private void Button_Click(object sender, RoutedEventArgs e) {
            //Notes.Add(new Note(this, "", ""));
            //Notes.Last().Show();
        }

        public override void ActionShow(Predicat p) {
            base.ActionShow(p);
            var pp = p.FindProperty(new Property("что")).ObjectValue;
            if (pp != null)
            if (pp.FindProperty(new Property("исходящий")) != null) {
                btnInOut.Content = "вх.";
            }
            UpdateLetters();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            if ((String)btnInOut.Content == "исх.")
                btnInOut.Content = "вх.";
            else
                btnInOut.Content = "исх.";

            UpdateLetters();
        }
    }
}
