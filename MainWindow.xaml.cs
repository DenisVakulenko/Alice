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

using System.IO;
using System.Text.RegularExpressions;

using LemmatizerNET;

namespace Alice {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            var Dict = new Brain();

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


        public void TestWordsComparator() {
            Brain.Instance.AddSynonyms(Brain.Word("красный"), Brain.Word("розовый"), 0.5);
            Brain.Instance.AddSynonyms(Brain.Word("красный"), Brain.Word("фиолетовый"), 0.5);

            var cmp1 = Brain.Word("красный").CompareTo(Brain.Word("красный"));
            var cmp2 = Brain.Word("красный").CompareTo(Brain.Word("фиолетовый"));
            var cmp3 = Brain.Word("розовый").CompareTo(Brain.Word("фиолетовый"));

            Brain.Instance.AddSynonyms(Brain.Word("кот"), Brain.Word("котик"), 0.9);
            Brain.Instance.AddSynonyms(Brain.Word("собака"), Brain.Word("пес"), 0.8);

            ((Noun)Brain.Word("кот")).AddGiperonim(Brain.Word("животное"));
            ((Noun)Brain.Word("собака")).AddGiperonim(Brain.Word("животное"));

            var cmp4 = Brain.Word("кот").CompareTo(Brain.Word("животное"));
            var cmp5 = Brain.Word("собака").CompareTo(Brain.Word("животное"));
            var cmp6 = Brain.Word("котик").CompareTo(Brain.Word("животное"));
        }

        public void TestObjectsComparator() {
            var obj1 = new Object();
            obj1.AddProperty("красный");

            var obj2 = new Object();
            obj2.AddProperty("розовый");

            var cmp1 = obj1.CompareTo(obj2);


            var obj3 = new Object("кот");
            obj3.AddProperty("фиолетовый");

            var obj4 = new Object("собака");
            obj4.AddProperty("розовая");

            var obj5 = new Object("животное");

            var obj6 = new Object("пес");
            obj6.AddProperty("розовый");
            
            var cmp2 = obj3.CompareTo(obj4);
            var cmp3 = obj3.CompareTo(obj5);
            var cmp4 = obj4.CompareTo(obj5);
            var cmp5 = obj5.CompareTo(obj4);
            var cmp44 = obj6.CompareTo(obj5);


            obj5.AddProperty("красный");

            var cmp6 = obj3.CompareTo(obj4);
            var cmp7 = obj3.CompareTo(obj5);
            var cmp8 = obj4.CompareTo(obj5);
            var cmp9 = obj5.CompareTo(obj4);
            var cmp10 = obj6.CompareTo(obj5);
        }

        private void LearnSomeSemantic() {
            //var about = Brain.Word("про");
            //var insmth = Brain.Word("в");
            //var after = Brain.Word("после");

            Noun Lex = (Noun)Brain.Word("Лёха");
            Lex.AddGiperonim("человек");
            Lex.AddSynonym("Алексей");
            Lex.AddSynonym("Лёша");

            ((Noun)Brain.Word("человек")).AddGiperonim("животное");
            ((Noun)Brain.Word("животное")).AddGiperonim("кто");
            ((Noun)Brain.Word("сообщение")).AddGiperonim("что");
            ((Noun)Brain.Word("текст")).AddGiperonim("что");
            // ((Noun)Brain.Word("сообщение")).AddHolonim("заголовок");

            var Animal = Brain.Word("животное");
            Animal.AddSynonym("животинка");

            var cmp1 = Brain.Word("Лёха").CompareTo(Brain.Word("животное"));
        }

        public void TestPredicatsComparator() {
            Noun Lex = (Noun)Brain.Word("Лёха");
            Lex.AddGiperonim("человек");
            Lex.AddSynonym("Алексей");
            Lex.AddSynonym("Лёша");

            var msg = new Object("сообщение");
            msg.AddProperty(new Property(new Object("автор"), Lex));
            msg.AddProperty(new Property(new Object("содержание"), "привет, привет привет привет. диплом надо сдавать."));

            var msgPredicat1 = new Predicat("написал");
            msgPredicat1.AddProperty(new Property(new Object("автор"), Lex));
            msgPredicat1.AddProperty(new Property(new Object("где"), msg));
            msgPredicat1.AddProperty(new Property(new Object("текст"), "привет, привет привет привет. диплом надо сдавать."));
            var msgPredicat2 = new Predicat("прислал");
            msgPredicat2.AddProperty(new Property(new Object("автор"), Lex));
            msgPredicat2.AddProperty(new Property(new Object("сообщение"), msg));
            //msg.GetPregicates();

            var pr1 = new Predicat("пришло");
            pr1.AddProperty("вчера");
            //pr1.AddProperty("когда", "вчера");
            //pr1.AddProperty(msg);
            pr1.AddProperty("что", msg);

            // что там пришло вчера
            var pr2 = new Predicat("пришло", SentenceType.Interrogative, "что");
            pr2.AddProperty("вчера");
            pr2.AddProperty("там");
            
            var cmp1 = pr1.CompareTo(pr2);
            var ans1 = pr1.ObjAnsverTo(pr2);


            // что написано в сообщении которое пришло вчера
            // что есть\было написано в сообщении которое пришло вчера

            // сообщение которое пришло вчера
            var pr3 = new Predicat("пришло", SentenceType.Interrogative, "сообщение");
            pr3.AddProperty("вчера");

            var cmp2 = pr1.CompareTo(pr2);
            var ans2 = pr1.ObjAnsverTo(pr2);

            // что написано в ans2
            var pr4 = new Predicat("написано", SentenceType.Interrogative, "что");
            //pr4.AddProperty("где", ans2); // в чем

            var ans3 = msgPredicat1.StringAnsverTo(pr4);

            //var pr5 = new Predicat("написано", SentenceType.Interrogative, "кем");
            //pr5.AddProperty("где", ans2);

            //var prop = new Property(new Object("содержание"), Property.ObjectPropertyType.NoValue_NoType);
            //var ans5 = ans2.FindProperty(prop);
        }

        Alice.Brain Dict = new Brain();
        Note i = new Note(null, "Первая тестовая заметка", "Тест");
        private void Button_Click_1(object sender, RoutedEventArgs e) {

            //i.Show();
            //i.Show();

            TestWordsComparator();
            TestObjectsComparator();

            LearnSomeSemantic();
            
            TestPredicatsComparator();

            // TestFinder();
            
            //var c = new WetCollocation(
            //"поздно вечером уставший я скушал спелого сочного яблока из бабушкиного " +
            //"сада в электричке после долгого дня");

            //скушал из сада - нельзя
 
            //скушал из зависти - ок мб. решить заменой из на от
            //скушал от зависти - ок

            //скушал от голода - ок

            //прибежал из сада - можно

            // Noun Class;     // промежуток времени. / редактирование. В.П. Е.Ч.

            // Word =        день         время                 группа         группа
            // Collocation = день недели. время редактирования. крутая группа. kiss.

            //  = крутая. / музыкальная. название = kiss. музыкант = а1. музыкант а2. жанр = рок. жанр = рок'н'ролл.


            var write = Brain.Word("писал");
            write.AskQ.Add(new Word.QItem("что", new String[]{"сообщение", "экзамен", "письмо"}));
            write.AskQ.Add(new Word.QItem("про_что", new String[] { "экзамены", "выходные", "репетицию", "событие" }));
            write.AskQ.Add(new Word.QItem("о ком"));
            write.AskQ.Add(new Word.QItem("как", new String[]{"быстро"})); // скорость // поздно - и как? и когда?
            write.AskQ.Add(new Word.QItem("чем"));
            write.AskQ.Add(new Word.QItem("когда", new String[]{"вчера", "вечером", "завтра"}));
            write.AskQ.Add(new Word.QItem("в_чем", new String[]{"классе", "универе", "блокноте"}));
            write.AskQ.Add(new Word.QItem("в_что", new String[]{"обед", "блокнот", "тетрадь"}));
            write.AskQ.Add(new Word.QItem("где"));
            write.AskQ.Add(new Word.QItem("кто", new String[]{"человек"}));

            Noun human = (Noun)Brain.Word("человек");
            human.AskQ.Add(new Word.QItem("имя", new String[] { "Лёха", "Макс", "Денис", "Ден" }));
            human.AskQ.Add(new Word.QItem("фамилия", new String[] { "Mogilnikov", "Наумов", "Вакуленко", "Просуков" }));
            human.AskQ.Add(new Word.QItem("возраст", new String[] { "промежуток времени" }));
            human.AskQ.Add(new Word.QItem("_", new String[] { "друг", "брат" }));
            
            human.ThisCan.Add(new WordConnection<Verb>("писать", "что делать"));
            human.WeCan.Add(new WordConnection<Verb>("написать", "что сделать"));

            

            //human.GeneralizeParents.Add(Brain.GetWord("животное"));

            // положить в коробку
            // жить в коробке

            // оказался в стоявшем на высоком холме доме.
            // оказался в постаревшем на высоком холме доме.

            // на холме
            // постаревшем на холме


            

            //Lex.
            //Lex.a

            //write.AskQ.Add(Dictionary.GetWord("j"));
            
            var c1 = new WetCollocation("что там написал леха");
            var c2 = new WetCollocation("что там неделю назад писали про экзамены");
            var c3 = new WetCollocation("спроси его когда мне идти сдавать документы");

            var c44 = new WetCollocation("собака с длинной шерстью");

            //("Я увидел, что буря утихла");
            var c4 = new WetCollocation("прочитай то что написали");
            var c5 = new WetCollocation("кто написал");
            var c6 = new WetCollocation("ответь что я занят");
            var c16 = new WetCollocation("ответь то что я занят");

            var c17 = new WetCollocation("найди вчерашнюю заметку");

            var c7 = new WetCollocation("прозвенел звонок веселый начинается урок");
            var c8 = new WetCollocation("прозвенел звонок веселый урок начинается");
            var c10 = new WetCollocation("звонок прозвенел веселый начинается урок");

            var c9 = new WetCollocation("кот вася который пришел домой ночью спал мертвым сном");

            // он сделал те задания, что помогло ему сдать экзамен.
            // он сделал те задания, что дал ему учитель.

            //1) Возьми книгу, что лежит на столе. 
            //2) Я знаю, что нам задали на дом. 
            //3) Как хорошо, что завтра начинаются каникулы. 
            //4) Что ты здесь делаешь?

            //ответ 3 (соединительный союз в сложноподчиненном предложении) 
            //в 1 оно является местоимением (подлежащим в сложном предложении) 
            //во 2 - местоимением (дополнение) 
            //в 4 - вопросительным местоимением (обстоятельство)

            
            // кот вася который пришел домой ночью [и] спал мертвым сном весь день получил веником по попе.

            Brain.Word("коты");
            Brain.Word("кота");
            Brain.Word("кот");
            Brain.Word("кота");

            
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


            // рок - жанр
            // вечер - часть чего? дня - 0.9
            // ночь - часть дня - 0.3
            // привет, как дела? - содержание
            // Alexey Mogilnikov - автор
            // 22.03.2014 - Дата
            // весна + ранняя - время чего? года
            // 2014 - год
            // 21:23 - Время
            // 21 - час
            // 9 чего? вечера - время - 0.3
            // пол чего? 9 - время - 0.5

            // жесткий[ - жесткость] 
            // любимый 
            // важный 
            // какой? двадцать второй - день 
            // новый 
            // красный - цвет 

            
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


            Object Msg = new Object("Сообщение");
            Msg.AddProperty("Автор", "Alexey Mogilnikov");
            Msg.AddProperty("Содержание", "Привет, как дела?");
            Msg.AddProperty("Время", "18:23");
            Msg.AddProperty("Дата", "10.03.2014");
            Msg.AddProperty("Время бинарное", DateTime.Parse("10.03.2014 18:23").ToBinary().ToString());
            Msg.AddProperty("Не прочитано"); // прочитать читать. Что сделать?

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
