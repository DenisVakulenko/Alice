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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using System.Text.RegularExpressions;

using LemmatizerNET;

namespace Kim {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            var Dict = new Dictionary();

            TreeViewItem treeItem = null;

            // North America 
            treeItem = new TreeViewItem();
            treeItem.Header = "North America";

            treeItem.Items.Add(new TreeViewItem() { Header = "USA" });
            treeItem.Items.Add(new TreeViewItem() { Header = "Canada" });
            treeItem.Items.Add(new TreeViewItem() { Header = "Mexico" });

            TreeViewItem treeItem2 = new TreeViewItem();
            treeItem2.Header = "North America2";

            treeItem2.Items.Add(new TreeViewItem() { Header = "USA2" });
            treeItem2.Items.Add(new TreeViewItem() { Header = "Canada2" });

            treeItem.Items.Add(treeItem2);

            //treeDictionary.Items.Add(treeItem);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) {
            var rmlPath = System.Environment.GetEnvironmentVariable("RML");
            Console.Write("\tRML directory (" + rmlPath + "): ");

            if (string.IsNullOrEmpty(rmlPath)) {
                var newRmlPath = ""; // Console.ReadLine();
                if (!string.IsNullOrEmpty(newRmlPath)) {
                    rmlPath = newRmlPath;
                }
            }

            //Console.Write("Select language 'R'-Russian, 'G'-German, 'E'-English (default - R): ");
            var langStr = "R"; // Console.ReadLine().ToUpper(CultureInfo.InvariantCulture);
            MorphLanguage lang = MorphLanguage.Russian;
            ILemmatizer lem = LemmatizerFactory.Create(lang);
            string rgt = "";
            try {
                StreamReader r = new StreamReader(rmlPath + @"\Dicts\Morph\" + langStr.ToLower() + "gramtab.tab", Encoding.GetEncoding(1251));
                rgt = r.ReadToEnd(); r.Close();
            }
            catch (Exception ex) {
            }
            try {
                var manager = FileManager.GetFileManager(rmlPath);
                lem.LoadDictionariesRegistry(manager);
            }
            catch (IOException ex) {
                this.ans.Text = "ERR LOADING DICT";
                return;
            }
            //while (true) {
                var word = txtWord.Text; //Console.ReadLine().Replace("\"", "").Replace("'", "").Trim();
                ////Позволяет декодировать грамкоды
                //if (word.ToLower().Contains("g") || word.Contains("\t") || word.Contains("\a")) { //m_gramcodes = 0x0322630c "абажай"
                //    string gc = Regex.Match(word, "[А-Яа-яёЁ]+").Groups[0].Value;
                //    string r = "";
                //    for (int i = 0; i < gc.Length / 2; i++) {
                //        Console.WriteLine(Regex.Match(rgt, "^" + gc.Substring(2 * i, 2) + "[^а-яА-яЕё]*(.*)", RegexOptions.Multiline).Groups[1].Value.Replace("\r", ""));
                //    }
                //    Console.WriteLine("");
                //    return;
                //}
                var paradigmList = lem.CreateParadigmCollectionFromForm(word, true, true); //false, false);

                //if (paradigmList.Count == 0) {
                //    try { //Позволяет декодировать граммемы, если число вместо слова
                //        string[] g = LemmatizerNET.Vars.Grammems;
                //        if (word.StartsWith("f")) { word = word.Substring(1); g = LemmatizerNET.Vars.Flags; } //декодируем флаги
                //        UInt64 gr = Convert.ToUInt64(word);
                //        for (int i = g.Length - 1; i > -1; i--)
                //            if (((1uL << i) & gr) > 0) Console.Write(g[i] + ","); Console.WriteLine("");
                //    }
                //    catch (Exception) {

                //    }

                //    Console.WriteLine("Paradigms not found");
                //    return;
                //}
                string ancodes = "";
                var ans = new StringBuilder();
                for (var i = 0; i < paradigmList.Count; i++) {
                    var paradigm = paradigmList[i];

                    ans.Append("Paradigm: ");
                    rmlPath = paradigm.SrcAncode;
                    ans.Append(paradigm.Norm + "  w" + paradigm.WordWeight + "  hw" + paradigm.HomonymWeight);
                    int k = paradigm.GetAccent(0);
                    k = paradigm.SrcAccentedVowel;
                    ans.Append("\tFounded: ");
                    ans.AppendLine(paradigm.Founded.ToString());
                    ans.Append("\tParadigmID: ");
                    ans.AppendLine(paradigm.ParadigmID.ToString());
                    ans.Append("\tAccentModelNo: ");
                    ans.AppendLine(paradigm.AccentModelNo.ToString());
                    //ans.AppendLine("=====");
                    ans.AppendLine("type_grm = " + (paradigm.TypeAncode == "??" ? "" : Regex.Match(rgt, "^" + paradigm.TypeAncode + "[^а-яА-яёЁ]*([^\r]*)", RegexOptions.Multiline).Groups[1].Value));
                    ancodes += paradigm.TypeAncode;
                    for (var j = 0; j < paradigm.Count; j++) {
                        ancodes += paradigm.GetAncode(j);
                        ans.Append("\t\t");
                        ans.Append(paradigm.GetAccent(j) == 255 ? paradigm.GetForm(j) : paradigm.GetForm(j).Insert(paradigm.GetAccent(j) + 1, "'"));
                        ans.Append("\t");
                        ans.AppendLine(paradigm.GetAncode(j)  + "--"+Regex.Match(rgt, "^" + paradigm.GetAncode(j) + "[^а-яА-яЕё]*(.*)", RegexOptions.Multiline).Groups[1].Value.Replace("\r", ""));
                    }
                    //paradigm.
                    //for (var j = 0; j < paradigm.Count; j++) {
                        //ancodes += paradigm.GetAncode(j);
                        //ans.Append("\t\t");
                        //ans.Append(paradigm.GetAccent(j) == 255 ? paradigm.GetForm(j) : paradigm.GetForm(j).Insert(paradigm.GetAccent(j) + 1, "'"));
                        //ans.Append("\t");
                        //ans.AppendLine(paradigm.SrcAncode + "--" + Regex.Match(rgt, "^" + paradigm.SrcAncode + "[^а-яА-яЕё]*(.*)", RegexOptions.Multiline).Groups[1].Value.Replace("\r", ""));
                    var a = paradigm.SrcGrammems;
                    //var b = new WetCollocationPart("после");
                    //var c = new WetCollocation("рано утром я скушал спелого сочного яблока в маршрутке");
                    //}
                    ans.AppendLine();
                }

                this.ans.Text = ans.ToString();
            //}
        }

        Kim.Dictionary Dict = new Dictionary();
        private void Button_Click_1(object sender, RoutedEventArgs e) {
            //var c = new WetCollocation(
            //"поздно вечером уставший я скушал спелого сочного яблока из бабушкиного " +
            //"сада в электричке после долгого дня");

            //скушал из сада - нельзя
 
            //скушал из зависти - ок мб. решить заменой из на от
            //скушал от зависти - ок

            //скушал от голода - ок

            //прибежал из сада - можно

            var write = Dictionary.GetWord("писал");
            write.AskQ.Add(new Word.QItem("что", new String[]{"сообщение", "экзамен", "письмо"}));
            write.AskQ.Add(new Word.QItem("про что", new String[] { "экзамены", "выходные", "репетицию", "событие" }));
            write.AskQ.Add(new Word.QItem("о ком"));
            write.AskQ.Add(new Word.QItem("как", new String[]{"быстро"})); // скорость // поздно - и как? и когда?
            write.AskQ.Add(new Word.QItem("чем"));
            write.AskQ.Add(new Word.QItem("когда", new String[]{"вчера", "вечером", "завтра"}));
            write.AskQ.Add(new Word.QItem("в чем", new String[]{"классе", "универе", "блокноте"}));
            write.AskQ.Add(new Word.QItem("в что", new String[]{"обед", "блокнот", "тетрадь"}));
            write.AskQ.Add(new Word.QItem("где"));
            write.AskQ.Add(new Word.QItem("кто", new String[]{"человек"}));

            var name = Dictionary.GetWord("человек");
            name.AskQ.Add(new Word.QItem("имя", new String[] { "Лёха", "Макс", "Денис", "Ден" }));
            name.AskQ.Add(new Word.QItem("фамилия", new String[] { "Mogilnikov", "Наумов", "Вакуленко", "Просуков" }));
            name.AskQ.Add(new Word.QItem("возраст", new String[] { "промежуток времени" }));
            name.AskQ.Add(new Word.QItem("", new String[] { "друг", "брат" }));

            Noun Lex = (Noun)Dictionary.GetWord("Лёха");
            //Lex.
            //Lex.a

            //write.AskQ.Add(Dictionary.GetWord(""));
            
            var c1 = new WetCollocation("что там написал леха");
            var c2 = new WetCollocation("что там неделю назад писали про экзамены");
            var c3 = new WetCollocation("спроси его когда мне идти сдавать документы");
            var c4 = new WetCollocation("прочитай то что написали");
            var c5 = new WetCollocation("кто написал");
            var c6 = new WetCollocation("ответь что я занят");

            Dictionary.GetWord("коты");
            Dictionary.GetWord("кота");
            Dictionary.GetWord("кот");
            Dictionary.GetWord("кота");

            //Dict.GetWord("котик").AddRule("ласковый кот");
            //Dict.GetWord("котик").AddRule("добрый кот");
            //Dict.GetWord("котик").AddRule("милый кот");

            //"писать" в контексте "сообщение" = "отправлял"

            //Log;
            //Log.Add(Obj(Msg), SpecifiedVerb("Пришел")); + rjyt
            //Log.Add(Obj(Owner), SpecifiedVerb("Искал то, что он(я) писал Лёхе за прошлую неделю")); + rjyt
            //Log.Add(Obj(Owner), SpecifiedVerb("Искал вчерашние")); + rjyt

            //Покажи всё вчерашнее
            //Покажи = Найди

            //писал 2 часа назад
            //писал отсчитав от настоящего 2 часа назад
            //писал 2 часа назад
            //писал 2 часа прошли назад с настоящего момента
            //писал в двух часах прошедших назад с настоящего момента
            //писал двумя часами раньше
            //came back 2 hours ago
            //came back 2 hours have agone
            //писал когда???? 2 часа назад
            //писал 2 часа от настоящего назад
            //был в двух метрах от остановки

            
            //прошел в двух метрах от остановки

            //прошел два метрах от остановки назад

            //Lex.AddSynonym("Лёха");

            //Lex.AddGiperonim("Человек");
            //Lex.AddPartonym("Мои друзья"); //part of
            //Lex.AddPartonym("Поток в университете"); //part of
            
            //"Прочитал".AddConvercive("Непрочитал");
            //Friends.AddHasA(Lex);

            //Family.AddHolonum("Мама"); //has a
            //Week.AddHasA(Day);

            
            //Dog.AddGiponim("Retriver");

            //2 часа назад
            // => время 23:34
            // => час 11
            // 23 + Gen(час) = 22 + Gen(час) - 0.5
            // 23 + Gen(час) = 21 + Gen(час) - 0.2
            // 23 + Gen(час) = 11 + Gen(час) + Prop(вечер, время + Prop(дня)) - 0.9
            // 23 + Gen(час) = 11 + Gen(час) + Prop(ночь, время + Prop(дня)) - 1.0
            // 23 + Gen(час) = 11 + Gen(час) - 0.2
            // 23 + Gen(час) = ночь + Prop(поздно, ) - 0.2
            // поздно parent поздний


            SpecifiedNoun Msg = new SpecifiedNoun("Сообщение");
            Msg.AddProperty("Alexey Mogilnikov", "Автор");
            Msg.AddProperty("Привет, как дела?", "Содержание");
            Msg.AddProperty("18:23", "Время");
            Msg.AddProperty("10.03.2014", "Дата");
            Msg.AddProperty(DateTime.Parse("10.03.2014 18:23").ToBinary().ToString(), "Время бинарное");
            Msg.AddProperty("Не прочитано", ""); // прочитать читать. Что сделать?

            //нар. когда? после
            //предлог после чего?
            //встреча где? в чем? лесу когда? после чего? обеда

            //AddRule("NN:NN", "NN часов");

            //"Удалить".AddSynonym("Стереть");
            //"Удалить".AddSynonym("Стереть");


            //SpecifiedNoun Msg = new SpecifiedNoun("Сообщение");
            //Msg.AddProperty("Alexey Mogilnikov", "Автор");
            //Msg.AddProperty("Привет, как дела?", "Содержание");
            //Msg.AddProperty("18:23", "Время");
            //Msg.AddProperty("10.03.2014", "Дата");
            //Msg.AddProperty(DateTime.Parse("10.03.2014 18:23").ToBinary().ToString(), "Время бинарное");
            //Msg.AddProperty("Прочитано", "");

            //DateTime.Now.ToBinary

            Dict.Words.Count();
        }
    }
}