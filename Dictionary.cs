using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
//using System.Text.RegularExpressions;
using LemmatizerNET;
using System.Text.RegularExpressions;

namespace Alice {
    public class Connection<T, BinderT> : FuzzyData<T> {
        BinderT _Binder;

        public Connection() {
            _Confidence = 0;
        }
        public Connection(T word, Double confidence = 1, BinderT binder = default(BinderT)) {
            _Data = word;
            _Binder = binder;
            _Confidence = confidence;
        }
        public Connection(T word, BinderT binder) {
            _Data = word;
            _Binder = binder;
            _Confidence = 1;
        }

        public T Word {
            get {
                return _Data;
            }
        }
        public BinderT Binder {
            get {
                return _Binder;
            }
        }
    }
    public class WordConnection<T> : FuzzyData<T> where T : Alice.Word {
        Word _Binder;

        public WordConnection() {
            _Confidence = 0;
        }
        public WordConnection(T word, Double confidence = 1, Word binder = null) {
            _Data = word;
            _Binder = binder;
            _Confidence = confidence;
        }
        public WordConnection(T word, Word binder) {
            _Data = word;
            _Binder = binder;
            _Confidence = 1;
        }
        public WordConnection(String word, String binder) {
            _Data = (T)Brain.Word(word);
            _Binder = Brain.Word(binder);
            _Confidence = 1;
        }

        public T Word {
            get {
                return _Data;
            }
        }
        public Word Binder {
            get {
                return _Binder;
            }
        }
    }
    public class WordConnection : FuzzyData<Word> {
        Word _Binder;

        public WordConnection(Word word = null, Double confidence = 1, Word binder = null) {
            _Data = word;
            _Binder = binder;
            _Confidence = confidence;
        }
        public WordConnection(Word word, Word binder) {
            _Data = word;
            _Binder = binder;
            _Confidence = 1;
        }

        public Word Word {
            get {
                return _Data;
            }
        }
    }
    public class FuzzyWordRelation : FuzzyData<Word> {
        public FuzzyWordRelation(Word word = null, Double confidence = 1) {
            _Data = word;
            _Confidence = confidence;
        }
        public Word Word {
            get {
                return _Data;
            }
        }
    }
    public class FuzzySynonyms : FuzzyData<Word> {
        Word _DataB;
        public FuzzySynonyms(Word a = null, Word b = null, Double confidence = 1) {
            _Data  = a;
            _DataB = b;
            _Confidence = confidence;
        }
        public Word WordA {
            get {
                return _Data;
            }
        }
        public Word WordB {
            get {
                return _DataB;
            }
        }

        public Boolean HasSynonymFor(Word w) {
            return (_Data == w || _DataB == w);
        }
        public FuzzyWordRelation SynonymFor(Word w) {
            if (_Data == w)
                return new FuzzyWordRelation(_DataB, _Confidence);
            
            if (_DataB == w)
                return new FuzzyWordRelation(_Data, _Confidence);

            return null;
        }
    }


    public class Word {
        public String Spelling;
        // String Root;
        IParadigm _Paradigm;
        public List<String> VerifiedVariants = new List<string>();

        protected List<WordConnection> Relatives;
        
        //protected List<Connection<SpecifiedWord>> Rules;

        public override string ToString() {
            return Spelling.ToLower();
        }
        
        public class QItem {
            Word Q;
            List<Word> VerifiedAnsvers = new List<Word>();

            public QItem(Word q) {
                Q = q;
            }
            public QItem(Word q, Word[] verified) {
                Q = q;
                foreach (var i in verified) {
                    VerifiedAnsvers.Add(i);
                }
            }
            public QItem(String q) {
                Q = Brain.Word(q);
            }
            public QItem(String q, String[] verified) {
                Q = Brain.Word(q);
                foreach (var i in verified) {
                    VerifiedAnsvers.Add(Brain.Word(i));
                }
            }
        }
        public List<QItem> AnsQ = new List<QItem>(); // Отвечает на вопросы
        public List<QItem> AskQ = new List<QItem>(); // Задает вопросы

        //List<WordConnection> Properties;    //  = крутая. / музыкальная. название = kiss. музыкант = а1. музыкант а2. жанр = рок. жанр = рок'н'ролл.

        //public void AddProperty(Word value, Word property) {
        //    WordConnection wc = new WordConnection(value, property);
        //    Properties.Add(wc);
        //}
        //public void AddProperty(String value, String property) {
        //    //WordConnection wc = new WordConnection(value, property);
        //    //Properties.Add(wc);
        //}

        public Word(String spelling = "") {
            Spelling = spelling;
            AddVerifiedVariant(Spelling);
            //InitFrom = form;
        }
        public Word(IParadigm form, String word) {
            Spelling = form.Norm;
            _Paradigm = form;
            AddVerifiedVariant(word);
        }
        public Word(IParadigm form) {
            Spelling = form.Norm;
            _Paradigm = form;
        }

        public PsOS POS {
            get {
                String rgt = Brain.Instance.rgt;
                String r = Regex.Match(rgt, "^" + _Paradigm.GetAncode(0) + "[^а-яА-яЕё]*(.*)", RegexOptions.Multiline).Groups[1].Value.Replace("\r", "");

                int p = r.IndexOf(' ');
                if (p > 0) r = r.Substring(0, p);

                return Vars.GetPOS(r);
            }
        }

        public void AddVerifiedVariant(string word) {
            if (!VerifiedVariants.Contains(word))
                VerifiedVariants.Add(word);
        }

        public bool IsInVerifiedVariants(String word) {
            foreach (var i in VerifiedVariants)
                if (i == word)
                    return true;
            return false;
        }

        public IParadigm Paradigm {
            get {
                return _Paradigm;
            }
        }
        public int ParadigmID {
            get {
                if (_Paradigm == null)
                    return -1;
                return _Paradigm.ParadigmID;
            }
        }
        
        public bool IsParadigmIDTheSame(List<int> paradigmIDs) {
            int pid = _Paradigm.ParadigmID;
            foreach (var i in paradigmIDs)
                if (i == pid)
                    return true;
            return false;
        }
        public bool IsParadigmIDTheSame(int paradigmID) {
            int pid = _Paradigm.ParadigmID;
            return pid == paradigmID;
        }
        
        public void AddSynonym(Word word, Double conf = 1) {
            Brain.Instance.AddSynonyms(this, word, conf);
        }
        public void AddSynonym(String word, Double conf = 1) {
            Brain.Instance.AddSynonyms(this, Brain.Word(word), conf);
        }
        
        public void AddRelative(Word word) {
            foreach (var i in Relatives)
                if (i.Word == word)
                    return;

            //foreach (var i in Synonyms)
            //    if (i.Word == word)
            //        return;

            Relatives.Add(new WordConnection(word));
        }
        public void AddRule(Object word) {
            //Rules.Add(new Connection<SpecifiedWord>(word));
        }


        public virtual double CompareTo(Word n, List<Word> exc = null) {
            if (this == n)
                return 1;

            var synonyms = Brain.Instance.GetSynonyms(this);

            var syn = synonyms.Find(i => i.Word == n);
            
            if (syn != null) {
                if (exc == null)
                    exc = synonyms.ConvertAll(i => i.Word);
                else
                    exc.AddRange(synonyms.ConvertAll(i => i.Word));

                return syn.Confidence;
            }
            else {
                if (exc == null) {
                    exc = synonyms.ConvertAll(i => i.Word);
                    
                    if (synonyms.Count == 0)
                        return 0;
                }
                else {
                    synonyms.RemoveAll(i => exc.Contains(i.Word));

                    if (synonyms.Count == 0)
                        return 0;

                    exc.AddRange(synonyms.ConvertAll(i => i.Word));
                }

                foreach (var i in synonyms) {
                    var conf = i.Word.CompareTo(n, exc); // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! TEST CICLING
                    if (conf != 0)
                        return i.Confidence * conf;
                }
            }       
     
            return 0;
        }
    }

    public class WordWithGiperonims : Word {
        public List<Word> Giperonims = new List<Word>();

        public void AddGiperonim(Word word) {
            if (Giperonims.Contains(word))
                return;

            Giperonims.Add(word);
        }
        public void AddGiperonim(String word) {
            var w = Brain.Word(word);

            if (Giperonims.Contains(w))
                return;

            Giperonims.Add(w);
        }

        public override double CompareTo(Word n, List<Word> exc = null) {
            var conf = base.CompareTo(n, exc);

            if (conf == 1 || conf == -1)
                return conf;

            foreach (var i in Giperonims) {
                conf = Math.Max(conf, 0.99 * i.CompareTo(n)); //, exc));

                if (conf == 1 || conf == -1)
                    return conf;
            }

            return conf;
        }

        public WordWithGiperonims(IParadigm form, String word) : base(form, word) { }
        public WordWithGiperonims(IParadigm form) : base(form) { }
    }

    public class Verb : WordWithGiperonims { // глагол
        List<WordConnection<Adv>> Adverbiums; // наречия

        //List<Connection<Noun>> Adverbiums; // в папку. на стол

        public Verb(IParadigm form, String word) : base(form, word) { }
        public Verb(IParadigm form) : base(form) { }
    }
    public class Noun : WordWithGiperonims { // сущ.
        // List<WordConnection> Adjectives = new List<WordConnection>(); // прилагательные 
        // день - настроение. день - сложность. день - насыщенность.
        //// утро - чать дня. вечер - часть дня.
        //  день - часть. части дня - утро, вечер.

        public List<WordConnection<Verb>> ThisCan = new List<WordConnection<Verb>>();
        public List<WordConnection<Verb>> WeCan = new List<WordConnection<Verb>>();

        // вторник - день. люмен - музыкальная группа.
        // длинный - длина, короткий - длина. длина - размер. ширина - размер. большой - размер.
        // сложный - сложность. легкий - легкость. важный - важность. неважный - важность.
        // утро - чать дня. вечер - часть дня. 

        List<WordConnection> PartsOfThis = new List<WordConnection>();

        public Noun(IParadigm form, String word) : base(form, word) { }
        public Noun(IParadigm form) : base(form) { }

        //public void AddRule(String rule) {
            //Rules.Add(new Connection<SpecifiedWord>(word));
        //}
    }
    public class Adv : Word {  // наречие
        // Adj AdjParent;         // если было образовано от прил. быстро - быстрый.
        // Noun NounParent;       // если было образовано от прил. домой - дом.

        public Adv(IParadigm form, String word) : base(form, word) { }
        public Adv(IParadigm form) : base(form) { }
    }
    public class Adj : Word {  // прил
        // List<WordConnection<Adj>> Adjectives; // прил прил ex: ярко красный = кр прил + прил, нар + прил
        // красный - цвет. второй - порядковый номер.

        public Adj(IParadigm form, String word) : base(form, word) { }
        public Adj(IParadigm form) : base(form) { }
    }

    public class Property {
        public Object Name { get; set; }

        public String UnpercievedValue { get; set; }
        public Word Value { get; set; }
        public Object ObjectValue { get; set; }
        public DateTime DateTimeValue { get; set; }
        public Predicat PredicatValue { get; set; }
        public Int32 IntValue { get; set; }
        public ObjectPropertyType Type;

        public enum ObjectPropertyType {
            NoValue_NoType,
            UnpercievedValue,
            Object,
            Word,
            Predicat,
            DateTime,
            Int
        }

        public Property(Object property, ObjectPropertyType type) {
            Name = property;
            Type = type;
        }
        public Property(Object property, String unprValue) {
            Name = property;
            UnpercievedValue = unprValue;
            Type = ObjectPropertyType.UnpercievedValue;
        }
        public Property(Object property, Word value) {
            Name = property;
            Value = value;
            Type = ObjectPropertyType.Word;
        }
        public Property(Object property, Object value) {
            Name = property;
            ObjectValue = value;
            Type = ObjectPropertyType.Object;
        }
        public Property(Object property, DateTime value) {
            Name = property;
            DateTimeValue = value;
            Type = ObjectPropertyType.DateTime;
        }
        public Property(Object property, int value) {
            Name = property;
            IntValue = value;
            Type = ObjectPropertyType.DateTime;
        }

        public Property(String value) {
            Name = null;
            UnpercievedValue = value;
            Type = ObjectPropertyType.UnpercievedValue;
        }
        public Property(Word value) {
            Name = null;
            Value = value;
            Type = ObjectPropertyType.Word;
        }
        public Property(Object value) {
            Name = null;
            ObjectValue = value;
            Type = ObjectPropertyType.Object;
        }
        public Property(Predicat value) {
            Name = null;
            PredicatValue = value;
            Type = ObjectPropertyType.Predicat;
        }


        public override string ToString() {
            if (Type == ObjectPropertyType.UnpercievedValue) {
                return UnpercievedValue;
            }
            else if (Type == ObjectPropertyType.Object) {
                return ObjectValue.ToString();
            }
            else if (Type == ObjectPropertyType.Predicat) {
                return PredicatValue.ToString();
            }
            else if (Type == ObjectPropertyType.Word) {
                return Value.Spelling;
            }
            return "type shit";
        }


        public double CompareTo(Property p) {
            Double conf = 0;
            if (p.Type == ObjectPropertyType.NoValue_NoType) {
                if (p.Name != null) {
                    if (Name != null)
                        conf = Name.CompareTo(p.Name);
                    else
                        return 0; //conf = 0;
                }
                else {
                    conf = 1;
                }
            }
            if (Type == p.Type) {
                if (p.Name != null) {
                    if (Name != null)
                        conf = Name.CompareTo(p.Name);
                    else
                        return 0; //conf = 0;
                }
                else {
                    conf = 1;
                }

                if (Type == ObjectPropertyType.UnpercievedValue) {
                    conf *= UnpercievedValue == p.UnpercievedValue ? 1 : 0;
                }
                else if (Type == ObjectPropertyType.Object) {
                    conf *= ObjectValue.CompareTo(p.ObjectValue);
                }
                else if (Type == ObjectPropertyType.Word) {
                    conf *= Value.CompareTo(p.Value);
                }
            }
            else {
                if (p.Type == ObjectPropertyType.Object && Type == ObjectPropertyType.DateTime) {
                    Double c = 0;
                    c = p.ObjectValue.CompareTo(new Object("сегодняшняя"));
                    if (c > 0)
                        if (DateTimeValue.Day == DateTime.Now.Day)
                            return c;
                    c = p.ObjectValue.CompareTo(new Object("вчерашняя"));
                    if (c > 0)
                        if (DateTimeValue.Day == DateTime.Now.AddDays(-1).Day)
                            return c;
                    c = p.ObjectValue.CompareTo(new Object("позавчерашняя"));
                    if (c > 0)
                        if (DateTimeValue.Day == DateTime.Now.AddDays(-2).Day)
                            return c;
                    var week = new Object("неделя");
                    c = p.ObjectValue.CompareTo(week);
                    if (c > 0) {
                        week.AddProperty("эта");
                        c = p.ObjectValue.CompareTo(week);
                        if (c > 0)
                            if (DateTimeValue.Day > DateTime.Now.AddDays(-7).Day)
                                return c;
                        week = new Object("неделя");
                        week.AddProperty("прошлая");
                        c = p.ObjectValue.CompareTo(week);
                        if (c > 0)
                            if (DateTimeValue.Day > DateTime.Now.AddDays(-14).Day &&
                                DateTimeValue.Day < DateTime.Now.AddDays(-7).Day)
                                return c;
                        week = new Object("неделя");
                        week.AddProperty("позапрошлая");
                        c = p.ObjectValue.CompareTo(week);
                        if (c > 0)
                            if (DateTimeValue.Day > DateTime.Now.AddDays(-21).Day &&
                                DateTimeValue.Day < DateTime.Now.AddDays(-14).Day)
                                return c;
                    }
                }
            }
            return conf;
        }
    }

    public class SmthWithProperties {
        List<Property> Properties = new List<Property>();


        public override String ToString() {
            String ans = "";
            foreach (var i in Properties) {
                foreach (var j in i.ToString().Split('\n'))
                    ans += "\n-" + j;
            }
            return ans;
        }

        public void AddProperty(Property prop) {
            Properties.Add(prop);
        }

        public void AddProperty(String value) {
            if (value.IndexOf(' ') == -1)
                Properties.Add(new Property(new Object(Brain.Word(value))));
                //Properties.Add(new Property(Brain.Word(value)));
            else if (value.Length > 100)
                Properties.Add(new Property(value));
            else
                Properties.Add(new Property(value));
        }
        public void AddProperty(String property, String value) {
            Object prop = new Object(property);

            if (value.IndexOf(' ') == -1)
                Properties.Add(new Property(new Object(Brain.Word(value))));
                //Properties.Add(new Property(prop, Brain.Word(value)));
            else if (value.Length > 100)
                Properties.Add(new Property(prop, value));
            else
                Properties.Add(new Property(prop, new Object(property)));
        }
        public void AddProperty(String property, DateTime value) {
            Object prop = new Object(property);

            Properties.Add(new Property(prop, value));
        }

        public void AddProperty(Object value) {
            Properties.Add(new Property(value));
        }
        public void AddProperty(String property, Object value) {
            Object prop = new Object(property);
            Properties.Add(new Property(prop, value));
        }

        public Double CompareProperties(SmthWithProperties p) {
            if (p.Properties.Count > 0) {
                Double sumConf = 0;
                foreach (var i in p.Properties) {
                    Double conf = 0;
                    foreach (var j in Properties) {
                        conf = Math.Max(conf, j.CompareTo(i));
                    }
                    sumConf += conf;
                }
                return sumConf / p.Properties.Count;
            }
            else
                return 1;
        }

        public Property FindProperty(Property p) {
            Double maxConf = 0;
            Property maxProp = null;

            Double conf = 0;
            foreach (var i in Properties) {
                conf = i.CompareTo(p);
                if (conf > maxConf) {
                    maxConf = conf;
                    maxProp = i;
                }
            }

            return maxProp;
        }
    }

    public enum SentenceType {
        Declarative,
        Imperative,
        Interrogative
    }
    public class Predicat : SmthWithProperties {
        public Verb Action;
        public SentenceType Type = SentenceType.Declarative;
        public Property Q;

        public override String ToString() {
            String ans = Action.ToString() + " | predicate " + Type.ToString();
            ans += base.ToString();
            return ans;
        }

        public Predicat(Verb action = null, SentenceType type = SentenceType.Declarative, Property q = null) {
            Action = action;
            Type = type;
            Q = q;
        }
        public Predicat(String action = null, SentenceType type = SentenceType.Declarative, String q = "") {
            if (action.IndexOf(' ') == -1)
                Action = (Verb)Brain.GetRefinedWord(action + "$ИНФИНИТИВ");
            if (q.IndexOf(' ') == -1)
                Q = new Property(new Object(q), Property.ObjectPropertyType.NoValue_NoType);
            Type = type;
        }
        public Predicat(String action) {
            if (action.IndexOf(' ') == -1)
                Action = (Verb)Brain.GetRefinedWord(action + "$ИНФИНИТИВ");
        }

        public Double CompareTo(Predicat p) {
            Double ans = 1;

            if (Action != null && p.Action != null) {
                ans *= Action.CompareTo(p.Action);
            }
            else if (Action == null) {
                return 0;
            }

            ans *= CompareProperties(p);

            return ans;
        }

        public Property AnsverTo(Predicat p) {
            if (p.Type != SentenceType.Interrogative || p.Q == null)
                return null;

            var prop = FindProperty(p.Q);

            return prop;
        }
        public Object ObjAnsverTo(Predicat p) {
            if (p.Type != SentenceType.Interrogative || p.Q == null)
                return null;
            
            Object obj = null;

            var prop = FindProperty(p.Q);

            if (prop != null)
                obj = prop.ObjectValue;

            return obj;
        }
        public String StringAnsverTo(Predicat p) {
            if (p.Type != SentenceType.Interrogative || p.Q == null)
                return null;

            Object obj = null;

            var prop = FindProperty(p.Q);

            if (prop != null)
                return prop.ToString();

            return "";
        }
    }

    public class Object : SmthWithProperties {
        public Word Class; //Noun Class;

        public override String ToString() {
            String ans = Class.ToString() + " | object";
            ans += base.ToString();
            return ans;
        }

        public Object(Word objectClass = null) {
            Class = objectClass;
        }
        public Object(String objClass) {
            if (objClass.IndexOf(' ') == -1)
                Class = (Word)Brain.GetRefinedWord(objClass); //(objClass + "$С"); !!!!!!
            //else if (value.Length > 100)
            //    Properties.Add(new Property(value));
            //else
            //    Properties.Add(new Property(value));
        }

        public Double CompareTo(Object p) {
            Double ans = 1;

            if (Class != null && p.Class != null) {
                ans *= Class.CompareTo(p.Class);
            }
            else if (Class == null) {
                return 0;
            }

            ans *= CompareProperties(p);

            return ans;
        }
    }


    [Flags] public enum FormsMatchings {
        N = 0,
        Gender = 1,
        Number = 2,
        Case = 4,
        GNC = 7,
        Tense = 8,
        Litso = 16,
    }
    [Flags] public enum ABLocations {
        Any = 15, //0b1111,
        AB = 4,   //0b0100,
        BA = 2,   //0b0010,
        A_B = 12, //0b1100,
        B_A = 3,  //0b0011
    }
    public class PartsRelationRule {
        public Vars.WordForm AForm;
        public Vars.WordForm BForm;
        public ABLocations ABLocation;
        public FormsMatchings ABMatching;

        public PartsRelationRule(Vars.WordForm aform, Vars.WordForm bform, ABLocations abLocation = ABLocations.Any, FormsMatchings match = FormsMatchings.N) {
            AForm = aform;
            BForm = bform;
            ABLocation = abLocation;
            ABMatching = match;
        }
        public PartsRelationRule(String aform, String bform, ABLocations abLocation = ABLocations.Any, FormsMatchings match = FormsMatchings.N) {
            AForm = new Vars.WordForm(aform);
            BForm = new Vars.WordForm(bform);
            ABLocation = abLocation;
            ABMatching = match;
        }
        public PartsRelationRule() {
        }

        public bool CheckForms(Vars.WordForm a, Vars.WordForm b) {
            return CheckForms(a, b, ABMatching);
        }
        public static bool CheckForms(Vars.WordForm a, Vars.WordForm b, FormsMatchings match) {
            if (match.HasFlag(FormsMatchings.Number) && a.Number != b.Number)
                return false;
            if (match.HasFlag(FormsMatchings.Gender) && (a.Gender != b.Gender))
            if (!(a.Gender == Genders.Neuter && a.Number == Numbers.Pl || b.Gender == Genders.Neuter && b.Number == Numbers.Pl) &&
                !(a.Gender == Genders.N && a.Number == Numbers.Pl || b.Gender == Genders.N && b.Number == Numbers.Pl))
                return false;
            if (match.HasFlag(FormsMatchings.Case) && a.Case != b.Case)
                return false;
            if (match.HasFlag(FormsMatchings.Tense) && a.Tense != b.Tense)
                return false;

            return true;
        }
        public static bool CheckForm(Vars.WordForm form, Vars.WordForm template) {
            if (form.POS != template.POS)
                if (!(form.POS == PsOS.Infinitive && template.POS == PsOS.Verb))
                return false;
            if (template.Case != Cases.N && form.Case != template.Case)
                return false;
            if (template.Number != Numbers.N && form.Number != template.Number)
                return false;

            return true;
        }
       
        public List<WetPartsRelation> FindRelations(WetCollocationPart a, WetCollocationPart b) {
            var ans = new List<WetPartsRelation>();
            
            for (int ia = 0; ia < a.Variants.Count; ia++) {
            var aForms = a.Variants[ia].Forms;
            for (int ja = 0; ja < aForms.Count; ja++) {
            var aForm = aForms[ja];

            if (CheckForm(aForm, AForm)) {

            for (int ib = 0; ib < b.Variants.Count; ib++) {
            var bForms = b.Variants[ib].Forms;
            for (int jb = 0; jb < bForms.Count; jb++) {
            var bForm = bForms[jb];

            if (CheckForm(bForm, BForm)) {
                if (CheckForms(aForm, bForm)) {
                    a.Variants[ia].HasOutgoingConnection ++;
                    b.Variants[ib].HasIngoingConnection ++;

                    ans.Add(new WetPartsRelation(new WetCollocationPartID(-1, ia, ja),
                                                 new WetCollocationPartID(-1, ib, jb),
                                                 this, a.Collocation));
                }
            }

            }
            }}}}

            return ans.Count == 0 ? null : ans;
        }
        //public List<WetPartsRelation> FindRelations(WetCollocationPartID aID, WetCollocationPart b) {
        //    var ans = new List<WetPartsRelation>();

        //    var aForm = b.Collocation.GetPart(aID).Form;

        //    if (CheckForm(aForm, AForm)) {

        //        for (int ib = 0; ib < b.Variants.Count; ib++) {
        //            var bForms = b.Variants[ib].Forms;
        //            for (int jb = 0; jb < bForms.Count; jb++) {
        //                var bForm = bForms[jb];

        //                if (CheckForm(bForm, BForm)) {
        //                    if (CheckForms(aForm, bForm)) {
                                
        //                        a.Variants[ia].HasIngoingConnection++;
        //                        b.Variants[ib].HasIngoingConnection++;

        //                        ans.Add(new WetPartsRelation(aID, new WetCollocationPartID(-1, ib, jb),
        //                                                     this, b.Collocation));
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return ans.Count == 0 ? null : ans;
        //}
        public List<WetPartsRelation> FindRelations(WetCollocation collocation) { // WetCollocationPart a, WetCollocationPart b) {
            var ans = new List<WetPartsRelation>();

            var parts = collocation.Parts;

            if (ABLocation == ABLocations.Any)
                for (int ai = 0; ai < parts.Count; ai++) {
                    var aPart = parts[ai];
                    for (int bi = 0; bi < parts.Count; bi++) {
                        if (ai == bi) continue;
                        var bPart = parts[bi];

                        var relations = FindRelations(aPart, bPart, ai, bi);
                        if (relations != null) ans.AddRange(relations);
                    }
                }
            else if (ABLocation == ABLocations.A_B)
                for (int ai = 0; ai < parts.Count; ai++) {
                    var aPart = parts[ai];
                    for (int bi = ai + 1; bi < parts.Count; bi++) {
                        var bPart = parts[bi];

                        var relations = FindRelations(aPart, bPart, ai, bi);
                        if (relations != null) ans.AddRange(relations);
                    }
                }
            else if (ABLocation == ABLocations.B_A)
                for (int ai = 0; ai < parts.Count; ai++) {
                    var aPart = parts[ai];
                    for (int bi = 0; bi < ai; bi++) {
                        var bPart = parts[bi];

                        var relations = FindRelations(aPart, bPart, ai, bi);
                        if (relations != null) ans.AddRange(relations);
                    }
                }
            else if (ABLocation == ABLocations.AB)
                for (int ai = 0; ai < parts.Count - 1; ai++) {
                    var aPart = parts[ai];
                    var bPart = parts[ai + 1];

                    var relations = FindRelations(aPart, bPart, ai, ai + 1);
                    if (relations != null) ans.AddRange(relations);
                }
            else if (ABLocation == ABLocations.BA)
                for (int ai = 1; ai < parts.Count; ai++) {
                    var aPart = parts[ai];
                    var bPart = parts[ai - 1];

                    var relations = FindRelations(aPart, bPart, ai, ai - 1);
                    if (relations != null) ans.AddRange(relations);
                }

            return ans.Count == 0 ? null : ans;
        }
        public List<WetPartsRelation> FindRelations(WetCollocationPart a, WetCollocationPart b, int ai, int bi) {
            var relations = FindRelations(a, b);
            if (relations != null) {
                foreach (var relation in relations) {
                    relation.A.PartIndex = ai;
                    relation.B.PartIndex = bi;
                }
            }
            return relations;
        }
        public List<WetPartsRelation> FindRelations(CollocationPart a, WetCollocationPart b, int bi) {
            var relations = FindRelations(a, b);
            if (relations != null) {
                foreach (var relation in relations) {
                    relation.B.PartIndex = bi;
                }
            }
            return relations;
        }
        public List<WetPartsRelation> FindRelations(CollocationPart a, WetCollocationPart b) {
            var ans = new List<WetPartsRelation>();
            var aForm = a.ParentWetCollocationPartForm;

            if (CheckForm(aForm, AForm)) {
                for (int ib = 0; ib < b.Variants.Count; ib++) {
                    var bForms = b.Variants[ib].Forms;
                    for (int jb = 0; jb < bForms.Count; jb++) {
                        var bForm = bForms[jb];

                        if (CheckForm(bForm, BForm)) {
                            if (CheckForms(aForm, bForm)) {

                                a.ParentWetCollocationPartVariant.HasOutgoingConnection++;
                                b.Variants[ib].HasIngoingConnection++;

                                ans.Add(new WetPartsRelation(a.PWetCollocationPartID, new WetCollocationPartID(-1, ib, jb),
                                                                this, b.Collocation));
                            }
                        }
                    }
                }
            }

            return ans.Count == 0 ? null : ans;
        }
        
        public List<WetPartsRelation> FindRelations(WetCollocation collocation, CollocationPart part, int left, int right) { // WetCollocationPart a, WetCollocationPart b) {
            var ans = new List<WetPartsRelation>();

            //var parts = collocation.Parts;

            //if (ABLocation == ABLocations.Any)
            //    //for (int ai = 0; ai < parts.Count; ai++) {
            //        var aPart = part;
            //        for (int bi = 0; bi < parts.Count; bi++) {
            //            if (part.PWetCollocationPartID.PartIndex == bi) continue;
            //            var bPart = parts[bi];

            //            var relations = FindRelations(aPart, bPart, bi);
            //            if (relations != null) ans.AddRange(relations);

            //        }
            //    //}

            //if (ABLocation == ABLocations.A_B)
            //    for (int ai = 0; ai < parts.Count; ai++) {
            //        var aPart = parts[ai];
            //        for (int bi = ai + 1; bi < parts.Count; bi++) {
            //            var bPart = parts[bi];

            //            var relations = FindRelations(aPart, bPart, ai, bi);
            //            if (relations != null) ans.AddRange(relations);

            //        }
            //    }

            //if (ABLocation == ABLocations.AB)
            //    for (int ai = 0; ai < parts.Count - 1; ai++) {
            //        var aPart = parts[ai];
            //        var bPart = parts[ai + 1];

            //        var relations = FindRelations(aPart, bPart, ai, ai + 1);
            //        if (relations != null) ans.AddRange(relations);

            //    }

            return ans.Count == 0 ? null : ans;
        }
    }

    class WetTree {
        //WetCollocation Collocation;

    }

    public class TreeBuilder {
        private static TreeBuilder instance;
        public static TreeBuilder Instance {
            get {
                if (instance == null) {
                    instance = new TreeBuilder();
                }
                return instance;
            }
        }

        List<PartsRelationRule> Rules = new List<PartsRelationRule>();

        public class AccessoryInfo {
            //int     UnionsGroupIndex;
            string  UnionsGroupDescription;
            //PartsConnectionRule Rule;
            Vars.WordForm HeadForm;
            ABLocations ABLocation;
            FormsMatchings ABMatching;
            List<Word> Unions = new List<Word>();

            public AccessoryInfo(String headForm, ABLocations location, FormsMatchings matching, string[] unions) {
                HeadForm = new Vars.WordForm(headForm);
                ABLocation = location;
                ABMatching = matching;
                foreach (var i in unions)
                    Unions.Add(Brain.GetRefinedWord(i));
            }

            public bool CheckPartsRelation(WetPartsRelation relation) {
                if (relation.WetPartFormA.POS != HeadForm.POS)
                    return false;

                if ((ABLocation & relation.ABLocation) == 0)
                    return false;

                if (!PartsRelationRule.CheckForms(relation.WetPartFormA, relation.WetPartFormB, ABMatching))
                    return false;

                foreach (var i in Unions) {
                    if (relation.WetPartVariantB.Paradigm.ParadigmID == i.ParadigmID)
                        return true;
                }
                return false;
            }
        }

        public static class AccessoriesChecker {
            static AccessoryInfo[] AccessoriesInfo = {
                new AccessoryInfo("Г",  ABLocations.A_B, FormsMatchings.N, Vars.strUnions[0]),

                new AccessoryInfo("С",  ABLocations.A_B, FormsMatchings.Number | FormsMatchings.Gender, Vars.strUnions[1]),
                new AccessoryInfo("МС", ABLocations.A_B, FormsMatchings.Number | FormsMatchings.Gender, Vars.strUnions[1]),
                new AccessoryInfo("МС-П", ABLocations.A_B, FormsMatchings.Number | FormsMatchings.Gender, Vars.strUnions[1]),
            
                new AccessoryInfo("Г",  ABLocations.A_B, FormsMatchings.N, Vars.strUnions[2]),

                new AccessoryInfo("Г",  ABLocations.A_B, FormsMatchings.N, Vars.strUnions[3]),
                new AccessoryInfo("Г",  ABLocations.Any, FormsMatchings.N, Vars.strUnions[4]),
                new AccessoryInfo("Г",  ABLocations.Any, FormsMatchings.N, Vars.strUnions[5]),
                new AccessoryInfo("Г",  ABLocations.Any, FormsMatchings.N, Vars.strUnions[6]),
                new AccessoryInfo("Г",  ABLocations.Any, FormsMatchings.N, Vars.strUnions[7]),
                new AccessoryInfo("Г",  ABLocations.A_B, FormsMatchings.N, Vars.strUnions[8]),
                new AccessoryInfo("Г",  ABLocations.A_B, FormsMatchings.N, Vars.strUnions[9]),
                new AccessoryInfo("Г",  ABLocations.A_B, FormsMatchings.N, Vars.strUnions[10]),

                new AccessoryInfo("Г",  ABLocations.A_B, FormsMatchings.N, Vars.strUnions[11]),
                new AccessoryInfo("Н",  ABLocations.AB,  FormsMatchings.N, Vars.strUnions[11]),
                new AccessoryInfo("Г",  ABLocations.A_B, FormsMatchings.N, Vars.strUnions[12]),
                new AccessoryInfo("Н",  ABLocations.AB,  FormsMatchings.N, Vars.strUnions[12])
            };

            public static List<WetPartsRelation> CheckRelations(List<WetPartsRelation> relation) {
                var ans = new List<WetPartsRelation>();
                foreach (var i in AccessoriesInfo)
                    foreach (var j in relation)
                        if (i.CheckPartsRelation(j)) {
                            j.AccessoryType = i;
                            ans.Add(j);
                        }
                return ans;
            }
        }

        private TreeBuilder() {
            //Rules.Add(new PartsRelationRule("Г", "С им", ABLocations.Any, FormsMatchings.Number));
            Rules.Add(new PartsRelationRule("Г", "С", ABLocations.Any, FormsMatchings.N));
            
            // мы писали письма // письма приходили
            // письма писали мы // письма писали мы хотим чтобы нас читали.

            //Rules.Add(new PartsRelationRule("Г", "МС им", ABLocations.Any, FormsMatchings.Number | FormsMatchings.Litso));
            Rules.Add(new PartsRelationRule("Г", "МС", ABLocations.Any, FormsMatchings.N));
            Rules.Add(new PartsRelationRule("Г", "МС-П", ABLocations.Any, FormsMatchings.N));
            Rules.Add(new PartsRelationRule("Г", "СОЮЗ", ABLocations.Any, FormsMatchings.N));

            //Rules.Add(new PartsRelationRule("Г", "С", ABLocations.Any, FormsMatchings.N));
            //Rules.Add(new PartsRelationRule("Г", "МС", ABLocations.Any, FormsMatchings.N));
            //Rules.Add(new PartsRelationRule("Г", "МС-П", ABLocations.Any, FormsMatchings.N));
            //Rules.Add(new PartsRelationRule("Г", "СОЮЗ", ABLocations.Any, FormsMatchings.N));

            //Rules.Add(new PartsRelationRule("С", "С имя", ABLocations.Any, FormsMatchings.GNC));
            Rules.Add(new PartsRelationRule("С", "П", ABLocations.Any, FormsMatchings.GNC));
            Rules.Add(new PartsRelationRule("С", "КР_ПРИЛ", ABLocations.Any, FormsMatchings.GNC));
            Rules.Add(new PartsRelationRule("С", "КР_ПРИЧАСТИЕ", ABLocations.Any, FormsMatchings.GNC));
            Rules.Add(new PartsRelationRule("С", "ПРИЧАСТИЕ", ABLocations.Any, FormsMatchings.GNC));
            Rules.Add(new PartsRelationRule("С", "ПРЕДЛ", ABLocations.Any, FormsMatchings.N));
            Rules.Add(new PartsRelationRule("С", "ЧИСЛ", ABLocations.BA, FormsMatchings.N));
            Rules.Add(new PartsRelationRule("С", "МС-П", ABLocations.A_B, FormsMatchings.Number | FormsMatchings.Gender));
            Rules.Add(new PartsRelationRule("С", "МС", ABLocations.A_B, FormsMatchings.GNC));

            Rules.Add(new PartsRelationRule("МС", "П", ABLocations.Any, FormsMatchings.GNC));
            Rules.Add(new PartsRelationRule("МС", "КР_ПРИЛ", ABLocations.Any, FormsMatchings.GNC));
            Rules.Add(new PartsRelationRule("МС", "КР_ПРИЧАСТИЕ", ABLocations.Any, FormsMatchings.GNC));
            Rules.Add(new PartsRelationRule("МС", "ПРИЧАСТИЕ", ABLocations.Any, FormsMatchings.GNC));
            Rules.Add(new PartsRelationRule("МС", "ПРЕДЛ", ABLocations.Any, FormsMatchings.N));
            Rules.Add(new PartsRelationRule("МС", "ЧИСЛ", ABLocations.BA, FormsMatchings.N));
            Rules.Add(new PartsRelationRule("МС", "МС-П", ABLocations.A_B, FormsMatchings.GNC));
            Rules.Add(new PartsRelationRule("МС", "МС", ABLocations.A_B, FormsMatchings.GNC));

            //Rules.Add(new PartsRelationRule("МС-П", "П", ABLocations.Any, FormsMatchings.GNC));
            Rules.Add(new PartsRelationRule("МС-П", "КР_ПРИЛ", ABLocations.Any, FormsMatchings.GNC));
            Rules.Add(new PartsRelationRule("МС-П", "КР_ПРИЧАСТИЕ", ABLocations.Any, FormsMatchings.GNC));
            //Rules.Add(new PartsRelationRule("МС-П", "ПРЕДЛ", ABLocations.Any, FormsMatchings.N));
            //Rules.Add(new PartsRelationRule("МС-П", "ЧИСЛ", ABLocations.BA, FormsMatchings.N));
            Rules.Add(new PartsRelationRule("МС-П", "МС-П", ABLocations.A_B, FormsMatchings.GNC));
            Rules.Add(new PartsRelationRule("МС-П", "МС", ABLocations.A_B, FormsMatchings.GNC));

            Rules.Add(new PartsRelationRule("Г", "ПРЕДЛ", ABLocations.Any, FormsMatchings.N));
            Rules.Add(new PartsRelationRule("ПРИЧАСТИЕ", "ПРЕДЛ", ABLocations.Any, FormsMatchings.N));
            Rules.Add(new PartsRelationRule("ПРЕДЛ", "С", ABLocations.A_B, FormsMatchings.N));
            Rules.Add(new PartsRelationRule("ПРЕДЛ", "МС", ABLocations.A_B, FormsMatchings.N));
            Rules.Add(new PartsRelationRule("ПРЕДЛ", "МС-П", ABLocations.AB, FormsMatchings.N)); // в котором
            Rules.Add(new PartsRelationRule("ПОСЛЕЛ", "МС", ABLocations.BA, FormsMatchings.N));
            Rules.Add(new PartsRelationRule("ПОСЛЕЛ", "С", ABLocations.BA, FormsMatchings.N));
            Rules.Add(new PartsRelationRule("Г", "Н", ABLocations.Any, FormsMatchings.N));
            Rules.Add(new PartsRelationRule("Н", "ПРЕДК", ABLocations.B_A, FormsMatchings.N));
        }

        public static List<WetPartsRelation> FindRelationsFromPredicate(WetCollocation collocation) {
            List<WetPartsRelation> ans = new List<WetPartsRelation>();
            WetCollocationPartID Predicate = null;

            for (int i = 0; i < collocation.Parts.Count; i++) {
            var a = collocation.Parts[i];
                for (int iVar = 0; iVar < a.Variants.Count; iVar++) {
                var aForms = a.Variants[iVar].Forms;
                    for (int iForm = 0; iForm < aForms.Count; iForm++) {
                    var aForm = aForms[iForm];
                        if (aForm.POS == PsOS.Verb) { //кр прич; приедикатив

                            var relations = FindRelations(collocation, new CollocationPart(collocation, new WetCollocationPartID(i, iVar, iForm)), 0, collocation.Parts.Count);
                            if (relations != null) ans.AddRange(relations);

                        }
                    }
                }
            }
            return ans;
        }

        public static List<WetPartsRelation> FindRelations(WetCollocation collocation) {
            List<WetPartsRelation> ans = new List<WetPartsRelation>();
            foreach (var rule in TreeBuilder.Instance.Rules) {
                var relations = rule.FindRelations(collocation);
                if (relations != null) ans.AddRange(relations);
            }
            return ans;
        }
        public static List<WetPartsRelation> FindRelations(WetCollocation collocation, CollocationPart part, int left, int right) {
            List<WetPartsRelation> ans = new List<WetPartsRelation>();

            foreach (var rule in TreeBuilder.Instance.Rules) {
                var relations = rule.FindRelations(collocation, part, left, right);
                if (relations != null) ans.AddRange(relations);
            }

            return ans;
        }
    }

    public class WetPartsRelation {
        public WetCollocation Collocation;

        public WetCollocationPartID A;
        public WetCollocationPartID B;

        public PartsRelationRule UsedRule;

        public float Confirmed = 0;

        public TreeBuilder.AccessoryInfo AccessoryType;

        public WetPartsRelation() { }
        public WetPartsRelation(WetCollocationPartID a, WetCollocationPartID b, PartsRelationRule usedRule, WetCollocation collocation) {
            A = a;
            B = b;
            UsedRule = usedRule;
            Collocation = collocation;
        }

        public override string ToString() {
            return Collocation.Parts[A.PartIndex].StrWord + "->" + Collocation.Parts[B.PartIndex].StrWord;
        }
        //public new string ToString {
        //    get {
        //        return Collocation.Parts[A.PartIndex].StrWord + "->" + Collocation.Parts[B.PartIndex].StrWord;
        //    }
        //}

        public ABLocations ABLocation {
            get {
                int d = B.PartIndex - A.PartIndex;
                if (d > 0)
                    if (d == 1)
                        return ABLocations.AB;
                    else
                        return ABLocations.A_B;
                else
                    if (d == -1)
                        return ABLocations.BA;
                    else
                        return ABLocations.B_A;
            }
        }

        public Vars.WordForm WetPartFormA {
            get {
                return Collocation.GetPartForm(A);
            }
        }
        public Vars.WordForm WetPartFormB {
            get {
                return Collocation.GetPartForm(B);
            }
        }
        public WetCollocationPartVariant WetPartVariantA {
            get {
                return Collocation.GetPartVariant(A);
            }
        }
        public WetCollocationPartVariant WetPartVariantB {
            get {
                return Collocation.GetPartVariant(B);
            }
        }
        public WetCollocationPart WetPartA {
            get {
                return Collocation.GetWetPart(A);
            }
        }
        public WetCollocationPart WetPartB {
            get {
                return Collocation.GetWetPart(B);
            }
        }
    }

    public class WetCollocationPartID {
        public int PartIndex;
        public int VariantIndex;
        public int FormIndex;
        //int WordIndex;

        public override bool Equals(object obj) {
            return this.Equals(obj as WetCollocationPartID);
        }
        public bool Equals(WetCollocationPartID p) {
            if (Object.ReferenceEquals(p, null))
                return false;

            if (Object.ReferenceEquals(this, p))
                return true;

            if (this.GetType() != p.GetType())
                return false;

            return (PartIndex == p.PartIndex) && (VariantIndex == p.VariantIndex) && (FormIndex == p.FormIndex);
        }
        public static bool operator ==(WetCollocationPartID lhs, WetCollocationPartID rhs) {
            // Check for null on left side. 
            if (Object.ReferenceEquals(lhs, null)) {
                if (Object.ReferenceEquals(rhs, null)) {
                    // null == null = true. 
                    return true;
                }

                // Only the left side is null. 
                return false;
            }
            // Equals handles case of null on right side. 
            return lhs.Equals(rhs);
        }
        public static bool operator !=(WetCollocationPartID lhs, WetCollocationPartID rhs) {
            return !(lhs == rhs);
        }
        public override int GetHashCode() {
            return PartIndex * 0x00010000 + VariantIndex * 0x000000100 + FormIndex;
        }

        public WetCollocationPartID(int collocationIndex, int variantIndex, int formIndex) {
            PartIndex = collocationIndex;
            VariantIndex = variantIndex;
            FormIndex = formIndex;
        }
    }

    public class WetCollocationPartVariant {
        public IParadigm Paradigm;
        public Word WordInDict;
        public List<Vars.WordForm> Forms = new List<Vars.WordForm>();

        public int HasIngoingConnection = 0;
        public int HasOutgoingConnection = 0;

        public bool HasConnection {
            get { return HasIngoingConnection > 0 || HasOutgoingConnection > 0; }
        }

        public WetCollocationPartVariant(IParadigm paradigm) {
            Paradigm = paradigm;

            var grms = Paradigm.SrcGrammems;
            foreach (var grm in grms) {
                Forms.Add(new Vars.WordForm(grm));
            }

            WordInDict = Brain.WordByParadigm(paradigm);
        }
    }
    public class WetCollocationPart {
        public WetPartsRelation IngoingConnection = null;
        public WetPartsRelation OutgoingConnection = null;

        public WetCollocation Collocation;
        public String StrWord;
        public List<WetCollocationPartVariant> Variants = new List<WetCollocationPartVariant>();

        public WetCollocationPart(String word, WetCollocation collocation) {
            StrWord = word;
            var paradigms = Brain.ParadigmsList(word);
            foreach (var paradigm in paradigms) {
                Variants.Add(new WetCollocationPartVariant(paradigm));
            }
            Collocation = collocation;
        }

        public WetCollocationPartVariant this[int i] {
            get { return Variants[i]; }
        }
    }


    public class WetCollocation {
        public List<WetCollocationPart> Parts = new List<WetCollocationPart>();
        List<WetPartsRelation> Relations = new List<WetPartsRelation>();

        public WetCollocation(String collocation = "") {
            foreach (String Word in collocation.Split(' ')) {
                if (Word != "")
                Parts.Add(new WetCollocationPart(Word, this));
            }
            
            BuildRelations();
            
            ProcessPreposotions();
            ProcessPostpositions();
            
            ProcessAccessories();
            
            DeleteFalseRelations();
            SolveAmbiguity();

            // GetTrees();
            // FilterParadigms();
        }

        public Predicat GetAmbigPredicat() {
            Predicat p;

            WetCollocationPartID Predicate = null;

            List<WetPredicat> Roots = new List<WetPredicat>();

            for (int i = 0; i < Parts.Count; i++) {
                var a = Parts[i];
                for (int iVar = 0; iVar < a.Variants.Count; iVar++)
                    if (a[iVar].HasOutgoingConnection > 0) {
                        var aForms = a.Variants[iVar].Forms;
                        for (int iForm = 0; iForm < aForms.Count; iForm++) {
                            var aForm = aForms[iForm];
                            if (aForm.POS == PsOS.Verb) {
                                Roots.Add(new WetPredicat(new WetCollocationPartID(i, iVar, iForm)));
                            }
                        }
                    }
            }

            if (Roots.Count == 0) {

            }
            else if (Roots.Count == 1) {
                List<WetPartsRelation> TreeRelations = new List<WetPartsRelation>();
                var prs = FindProperties(new CollocationPart(this, Roots[0].RootID), TreeRelations, 0, Parts.Count);

                //var prs = FindProperties(new CollocationPart(this, i.B), TreeRelations, newL, newR);
                p = new Predicat((Verb)GetPartVariant(Roots[0].RootID).WordInDict);

                foreach (var j in prs) {
                    p.AddProperty(j);
                }

                return p;
            }
            else {
                for (int i = 0; i < Roots.Count; i++) {

                    List<WetPartsRelation> TreeRelations = new List<WetPartsRelation>();
                    FindChildren(new CollocationPart(this, Roots[i].RootID), TreeRelations, 0, Parts.Count);

                    //var Unions = FindUnions(Roots[i].RootID.PartIndex + 1, Roots[i + 1].RootID.PartIndex);
                    //if (Unions.Count == 0) {

                    //}
                    //else if (Unions.Count == 1) {

                    //}
                }
            }
            return null;
        }
        public Predicat GetPredicat() {
            Predicat p;

            WetCollocationPartID Predicate = null;

            List<WetPredicat> Roots = new List<WetPredicat>();

            for (int i = 0; i < Parts.Count; i++) {
                var a = Parts[i];
                if (a.IngoingConnection == null)
                for (int iVar = 0; iVar < a.Variants.Count; iVar++)
                    if (a[iVar].HasOutgoingConnection > 0) {
                        var aForms = a.Variants[iVar].Forms;
                        for (int iForm = 0; iForm < aForms.Count; iForm++) {
                            var aForm = aForms[iForm];
                            if (aForm.POS == PsOS.Verb) {
                                Roots.Add(new WetPredicat(new WetCollocationPartID(i, iVar, iForm)));
                            }
                        }
                    }
            }

            if (Roots.Count == 0) {

            }
            else if (Roots.Count == 1) {
                List<WetPartsRelation> TreeRelations = new List<WetPartsRelation>();
                var prs = FindProperties(new CollocationPart(this, Roots[0].RootID), TreeRelations, 0, Parts.Count);

                //var prs = FindProperties(new CollocationPart(this, i.B), TreeRelations, newL, newR);
                p = new Predicat((Verb)GetPartVariant(Roots[0].RootID).WordInDict);
                
                foreach (var j in prs) {
                    p.AddProperty(j);
                }

                return p;
            }
            else {
                for (int i = 0; i < Roots.Count; i++) {

                    List<WetPartsRelation> TreeRelations = new List<WetPartsRelation>();
                    FindChildren(new CollocationPart(this, Roots[i].RootID), TreeRelations, 0, Parts.Count);

                    //var Unions = FindUnions(Roots[i].RootID.PartIndex + 1, Roots[i + 1].RootID.PartIndex);
                    //if (Unions.Count == 0) {

                    //}
                    //else if (Unions.Count == 1) {

                    //}
                }
            }
            return null;
        }

        // Preprocessing

        private void BuildRelations() {
            Relations = TreeBuilder.FindRelations(this);
        }

        private void ProcessPostpositions() {
            for (int j = 0; j < Relations.Count; j++) {
                var r = Relations[j];
                if (r.UsedRule.AForm.POS == PsOS.Postposition) {
                    r.WetPartA.OutgoingConnection = r;
                    r.WetPartB.IngoingConnection = r;

                    RemoveRelations(r.A.PartIndex, r.B.PartIndex);

                    ProcessPreposotions();
                }
            }
        }
        private void ProcessPreposotions() {
            for (int i = 0; i < Parts.Count; i++) {
                var a = Parts[i];
                if (a.OutgoingConnection == null)
                    if (a.Variants[0].Forms[0].POS == PsOS.Proposition) {
                        int min = Parts.Count+1;
                        WetPartsRelation minRelation = null;
                        bool shit = false;

                        for (int j = 0; j < Relations.Count; j++) {
                            var r = Relations[j];
                            if (min > r.B.PartIndex && r.A.PartIndex == i && Parts[r.A.PartIndex].IngoingConnection == null) {
                                min = r.B.PartIndex;
                                minRelation = r;
                            }
                        }

                        if (minRelation != null) {
                            for (int j = i+1; j < min; j++) {
                                var pj = Parts[j];
                                if (pj.OutgoingConnection == null)
                                    for (int k = 0; k < pj.Variants.Count; k++) {
                                        if (pj.Variants[k].Forms[0].POS == PsOS.Proposition) {
                                            shit = true;
                                        }
                                    }
                            }

                            if (shit == false) {
                                a.OutgoingConnection = minRelation;
                                var b = Parts[minRelation.B.PartIndex];
                                b.IngoingConnection = minRelation;
                                
                                RemoveRelations(i, minRelation.B.PartIndex);

                                Relations.RemoveAll(ir => (ir.A.PartIndex == minRelation.B.PartIndex && ir.B.PartIndex <= i));
                                Relations.RemoveAll(ir => (ir.B.PartIndex == i && ir.A.PartIndex > i && ir.A.PartIndex < minRelation.B.PartIndex));
                                Relations.RemoveAll(ir => ((ir.A.PartIndex < i || ir.A.PartIndex > minRelation.B.PartIndex) && ir.B.PartIndex > i && ir.B.PartIndex < minRelation.B.PartIndex));

                                ProcessPreposotions();
                            }
                        }
                        else {
                            if (a.Variants.Count == 1) {
                                //HZ
                            }
                        }
                    }
            }
        }
        private void RemoveRelations(int A, int B) {
            Relations.RemoveAll(i => (i.A.PartIndex == A || i.B.PartIndex == B));
        }

        private void ProcessAccessories() {
            var acc = TreeBuilder.AccessoriesChecker.CheckRelations(Relations);

            acc.Sort((a, b) => a.B.PartIndex.CompareTo(b.B.PartIndex));

            while (acc.Count > 0) {
                var i = acc.First();
                
                Int32 predicatStarts = i.B.PartIndex;
                WetPartsRelation accessoryRoot = null;
                Int32 predicatRoot = -1;
                foreach (var j in Relations) {
                    if ((j.WetPartFormA.POS == PsOS.Infinitive || j.WetPartFormA.POS == PsOS.Verb) && j.A.PartIndex > predicatStarts && j.WetPartFormA.Povelit != isPovelit.Povelit) {
                        if (accessoryRoot == null || accessoryRoot.A.PartIndex > j.A.PartIndex ||
                            (accessoryRoot.A.PartIndex == j.A.PartIndex && j.B.PartIndex == predicatStarts)) {
                            accessoryRoot = j;
                            predicatRoot = accessoryRoot.A.PartIndex;
                        }
                    }
                }

                Boolean allRight = true;
                foreach (var j in acc) {
                    if (j.B.PartIndex > predicatStarts && j.B.PartIndex < predicatRoot) {
                        allRight = false;
                        break;
                    }
                }

                var alts = acc.FindAll(j => j.B.PartIndex == i.B.PartIndex);
                
                foreach (var alt in alts)
                    acc.Remove(alt);

                if (allRight) {
                    if (accessoryRoot != null) {
                        alts.RemoveAll(j => j.A.PartIndex == accessoryRoot.A.PartIndex);
                        if (alts.Count > 1) {
                            var context = new IntContext();
                            context.Vars.Add((int)accessoryRoot.WetPartVariantA.Paradigm.ParadigmID);
                            context.Vars.Add((int)i.WetPartVariantB.Paradigm.ParadigmID);
                            context.AddFormInfo(accessoryRoot.WetPartFormA);
                            var alternatives = new List<IntContext>();

                            Int32 d = 100500;
                            foreach (var j in alts) {
                                d = Math.Min(Math.Abs(j.A.PartIndex - accessoryRoot.A.PartIndex), d); 
                            }

                            foreach (var j in alts) {
                                var alternative = new IntContext();
                                alternative.Vars.Add(j.A.PartIndex > accessoryRoot.A.PartIndex ? 2 : 1);
                                alternative.Vars.Add(j.A.PartIndex == d ? 1 : 2);
                                alternative.Vars.Add((int)j.WetPartVariantA.Paradigm.ParadigmID);
                                alternative.AddFormInfo(j.WetPartFormA);

                                alternatives.Add(alternative);
                            }
                            i = alts[Brain.Instance.AccessoriesAD.TryToResolve(alternatives, context)];
                        }
                        else {
                            var context = new IntContext();
                            context.Vars.Add((int)accessoryRoot.WetPartVariantA.Paradigm.ParadigmID);
                            context.Vars.Add((int)i.WetPartVariantB.Paradigm.ParadigmID);
                            context.AddFormInfo(accessoryRoot.WetPartFormA);
                            var alternative = new IntContext();
                            alternative.Vars.Add(i.A.PartIndex > accessoryRoot.A.PartIndex ? 2 : 1);
                            alternative.Vars.Add(Math.Abs(i.A.PartIndex - accessoryRoot.A.PartIndex));
                            alternative.Vars.Add((int)i.WetPartVariantA.Paradigm.ParadigmID);
                            alternative.AddFormInfo(i.WetPartFormA);
                            Brain.Instance.AccessoriesAD.Add(alternative, context);
                        }

                        i.WetPartB.IngoingConnection = accessoryRoot;
                        i.WetPartA.OutgoingConnection = accessoryRoot;

                        var newRelation = new WetPartsRelation(i.A, accessoryRoot.A, null, this);
                        Relations.Add(newRelation);
                        accessoryRoot.WetPartA.IngoingConnection = newRelation;

                        Relations.RemoveAll(j => (j.A.PartIndex == predicatRoot &&
                                                  (j.B.PartIndex < predicatStarts))); // || j.B.PartIndex > predicatRoot)));
                        Relations.RemoveAll(j => ((j.A.PartIndex > predicatStarts && j.A.PartIndex < predicatRoot) &&
                                                  (j.B.PartIndex < predicatStarts || j.B.PartIndex > predicatRoot)));
                        Relations.RemoveAll(j => ((j.A.PartIndex < predicatStarts || j.A.PartIndex > predicatRoot) &&
                                                  (j.B.PartIndex > predicatStarts && j.B.PartIndex < predicatRoot)));

                        var headPos = i.A.PartIndex;
                        Relations.RemoveAll(j => ((j.A.PartIndex > headPos && j.A.PartIndex < predicatStarts) &&
                                                  (j.B.PartIndex < headPos || j.B.PartIndex > predicatStarts)));
                        Relations.RemoveAll(j => ((j.A.PartIndex < headPos || j.A.PartIndex > predicatStarts) &&
                                                  (j.B.PartIndex > headPos && j.B.PartIndex < predicatStarts)));
                    }
                }
                else {
                    foreach (var alt in alts)
                        acc.Add(alt);
                }
                acc.RemoveAll(j => !Relations.Contains(j));
            }
        }
        private List<WetPartsRelation> IngoingRelations(WetCollocationPartID id) {
            return Relations.FindAll(i => i.B == id);
        }

        private void DeleteFalseRelations() {
            List<WetPartsRelation> toDelete = new List<WetPartsRelation>();
            do {
                toDelete.Clear();
                for (int i = 0; i < Relations.Count; i++) {
                    var r = Relations[i];
                    if (r.WetPartFormA.POS != PsOS.Verb && (r.WetPartA.IngoingConnection == null || r.WetPartA.IngoingConnection.B != r.A)) {
                        if (Relations.Find(j => j.B == r.A) == null)
                            toDelete.Add(r);
                    }
                }
                foreach (var i in toDelete)
                    Relations.Remove(i);
            } while (toDelete.Count > 0);
        }
        private void SolveAmbiguity() {
            for (int i = 0; i < Relations.Count; i++) {
                var ir = Relations[i];
                var partID = Relations[i].B;
                //var a = Parts[i];
                if (ir.WetPartB.IngoingConnection == null) {
                    var ingConn = Relations.FindAll(j => j.B == partID);

                    var context = new IntContext();
                    context.Vars.Add((int)ir.WetPartVariantB.Paradigm.ParadigmID);
                    context.AddFormInfo(ir.WetPartFormB);

                    if (ingConn.Count > 1) {
                        var alternatives = new List<IntContext>();

                        Int32 d = 100500;
                        foreach (var j in ingConn)
                            d = Math.Min(Math.Abs(j.A.PartIndex - ir.B.PartIndex), d);

                        foreach (var j in ingConn) {
                            var alternative = new IntContext();
                            alternative.Vars.Add((int)j.WetPartVariantA.Paradigm.ParadigmID);
                            alternative.Vars.Add(Math.Abs(j.A.PartIndex - ir.B.PartIndex) == d ? 1 : 2);
                            alternative.AddFormInfo(j.WetPartFormA);

                            alternatives.Add(alternative);
                        }

                        var ans = ingConn[Brain.Instance.IngoingConnectionsAD.TryToResolve(alternatives, context)];

                        ingConn.Remove(ans);
                        Relations.RemoveAll(r => ingConn.Contains(r));

                        ir.WetPartB.IngoingConnection = ans;
                    }
                    else {
                        var correctAlternative = new IntContext();
                        var relation = ingConn[0];
                        correctAlternative.Vars.Add((int)relation.WetPartVariantA.Paradigm.ParadigmID);
                        correctAlternative.Vars.Add(1);
                        correctAlternative.AddFormInfo(relation.WetPartFormA);

                        Brain.Instance.IngoingConnectionsAD.Add(correctAlternative, context);
                        ir.WetPartB.IngoingConnection = relation;
                    }
                }
                else {
                    Relations.RemoveAll(r => r.B == ir.B && r != ir.WetPartB.IngoingConnection);
                }
            }
        }
        
        // Preprocessing

        class WetPredicat {
            public WetCollocationPartID RootID;
            public int Left = -1;
            public int Right = -1;

            public WetPredicat(WetCollocationPartID root, int left = -1, int right = -1) {
                RootID = root;
                Left = left;
                Right = right;
            }
        }
        // повествовате-льных, побудитель-ных, вопроситель-ных предложений, 
        private void GetTrees() {
            WetCollocationPartID Predicate = null;

            List<WetPredicat> Roots = new List<WetPredicat>();

            for (int i = 0; i < Parts.Count; i++) {
                var a = Parts[i];
                for (int iVar = 0; iVar < a.Variants.Count; iVar++)
                    if (a[iVar].HasOutgoingConnection > 0) {
                        var aForms = a.Variants[iVar].Forms;
                        for (int iForm = 0; iForm < aForms.Count; iForm++) {
                            var aForm = aForms[iForm];
                            if (aForm.POS == PsOS.Verb) {
                                Roots.Add(new WetPredicat(new WetCollocationPartID(i, iVar, iForm)));
                            }
                        }
                    }
            }

            if (Roots.Count == 0) {

            }
            else if (Roots.Count == 1) {
                List<WetPartsRelation> TreeRelations = new List<WetPartsRelation>();
                FindChildren(new CollocationPart(this, Roots[0].RootID), TreeRelations, 0, Parts.Count);
            }
            else {
                for (int i = 0; i < Roots.Count; i++) {

                    List<WetPartsRelation> TreeRelations = new List<WetPartsRelation>();
                    FindChildren(new CollocationPart(this, Roots[i].RootID), TreeRelations, 0, Parts.Count);

                    //var Unions = FindUnions(Roots[i].RootID.PartIndex + 1, Roots[i + 1].RootID.PartIndex);
                    //if (Unions.Count == 0) {

                    //}
                    //else if (Unions.Count == 1) {

                    //}
                }
            }
        }

        private List<WetCollocationPartID> FindUnions(int l, int r) {
            List<WetCollocationPartID> res = new List<WetCollocationPartID>();
            for (int i = l; i < r; i++) {
                var a = Parts[i];
                for (int iVar = 0; iVar < a.Variants.Count; iVar++) {
                    var aForms = a.Variants[iVar].Forms;
                    for (int iForm = 0; iForm < aForms.Count; iForm++) {
                        var aForm = aForms[iForm];
                        if (aForm.POS == PsOS.Union) {
                            res.Add(new WetCollocationPartID(i, iVar, iForm));
                        }
                    }
                }
            }
            return res;
        }

        private List<Property> FindProperties(CollocationPart part, List<WetPartsRelation> TreeRelations, int l, int r) {
            List<WetPartsRelation> newRelations = new List<WetPartsRelation>();
            List<Property> newProperties = new List<Property>();

            foreach (var ir in Relations.Except(TreeRelations)) {
                //for (int i = l; i < r; i++) {
                //var p = Relations[i];
                if (ir.B.PartIndex >= l && ir.B.PartIndex < r)
                    if (ir.A.PartIndex == part.PWetCollocationPartID.PartIndex &&
                        ir.A.VariantIndex == part.PWetCollocationPartID.VariantIndex) { // part.PWetCollocationPartID) {
                        var s = ir.ToString();

                        var altR = newRelations.Find(jr => jr.B.PartIndex == ir.B.PartIndex);
                        if (altR == null) {
                            newRelations.Add(ir);
                        }
                        else if (altR.WetPartVariantB.Paradigm.WordWeight < ir.WetPartVariantB.Paradigm.WordWeight) {
                            newRelations.Remove(altR);
                            newRelations.Add(ir);
                        }
                    }
            }

            foreach (var i in newRelations) {
                TreeRelations.Add(i);
            }

            foreach (var i in newRelations) {
                int newL = l;
                int newR = r;

                foreach (var j in TreeRelations) {
                    if (j.A.PartIndex < i.B.PartIndex && j.A.PartIndex > newL)
                        newL = j.A.PartIndex;
                    if (j.A.PartIndex > i.B.PartIndex && j.A.PartIndex < newR)
                        newR = j.A.PartIndex;

                    if (j.B.PartIndex < i.B.PartIndex && j.B.PartIndex > newL)
                        newL = j.B.PartIndex;
                    if (j.B.PartIndex > i.B.PartIndex && j.B.PartIndex < newR)
                        newR = j.B.PartIndex;
                }

                if (i.WetPartFormB.POS == PsOS.Postposition || i.WetPartFormB.POS == PsOS.Proposition) {
                    if (i.WetPartB.OutgoingConnection != null) {
                        var nextObj = new Object(i.WetPartB.OutgoingConnection.WetPartVariantB.WordInDict);
                        var prs = FindProperties(new CollocationPart(this, i.WetPartB.OutgoingConnection.B), TreeRelations, newL, newR);
                        foreach (var j in prs) {
                            nextObj.AddProperty(j);
                        }
                        newProperties.Add(new Property(new Object(i.WetPartVariantB.WordInDict), nextObj));
                    }
                }
                else {
                    if (i.WetPartVariantB.WordInDict.POS == PsOS.Communion ||
                        i.WetPartVariantB.WordInDict.POS == PsOS.Gerund ||
                        i.WetPartVariantB.WordInDict.POS == PsOS.Infinitive ||
                        i.WetPartVariantB.WordInDict.POS == PsOS.Verb) {
                        var nextObj = new Predicat((Verb)i.WetPartVariantB.WordInDict);
                        var prs = FindProperties(new CollocationPart(this, i.B), TreeRelations, newL, newR);
                        foreach (var j in prs) {
                            nextObj.AddProperty(j);
                        }
                        newProperties.Add(new Property(nextObj));
                    }
                    else {
                        var nextObj = new Object(i.WetPartVariantB.WordInDict);
                        var prs = FindProperties(new CollocationPart(this, i.B), TreeRelations, newL, newR);
                        foreach (var j in prs) {
                            nextObj.AddProperty(j);
                        }
                        newProperties.Add(new Property(nextObj));
                        //FindProperties(new CollocationPart(this, i.B), TreeRelations, newL, newR);
                    }
                }
            }

            return newProperties;
        }

        private void FindChildren(CollocationPart part, List<WetPartsRelation> TreeRelations, int l, int r) {
            List<WetPartsRelation> NewRelations = new List<WetPartsRelation>();

            foreach (var ir in Relations.Except(TreeRelations)) {
            //for (int i = l; i < r; i++) {
                //var p = Relations[i];
                if (ir.B.PartIndex >= l && ir.B.PartIndex < r)
                if (ir.A == part.PWetCollocationPartID) {
                    var s = ir.ToString();
                    
                    var altR = NewRelations.Find(jr => jr.B.PartIndex == ir.B.PartIndex);
                    if (altR == null) {
                        NewRelations.Add(ir);
                    }
                    else if (altR.WetPartVariantB.Paradigm.WordWeight < ir.WetPartVariantB.Paradigm.WordWeight) {
                        NewRelations.Remove(altR);
                        NewRelations.Add(ir);
                    }
                }
            }

            foreach (var i in NewRelations) {
                TreeRelations.Add(i);
            }

            foreach (var i in NewRelations) {
                int newL = l;
                int newR = r;

                foreach (var j in TreeRelations) {
                    if (j.A.PartIndex < i.B.PartIndex && j.A.PartIndex > newL)
                        newL = j.A.PartIndex;
                    if (j.A.PartIndex > i.B.PartIndex && j.A.PartIndex < newR)
                        newR = j.A.PartIndex;

                    if (j.B.PartIndex < i.B.PartIndex && j.B.PartIndex > newL)
                        newL = j.B.PartIndex;
                    if (j.B.PartIndex > i.B.PartIndex && j.B.PartIndex < newR)
                        newR = j.B.PartIndex;
                }

                FindChildren(new CollocationPart(this, i.B), TreeRelations, newL, newR);
            }
        }

        

        //private void FilterParadigms() {
        //    //Relations = TreeBuilder.FindRelations(this);
        //    foreach (var part in Parts)
        //        for (int i = 0; i < part.Variants.Count; i++) {
        //            var variant = part.Variants[i];
        //            if (!variant.HasConnection) {
        //                //variant.
        //                i--;
        //            }
        //        }
        //}

        public CollocationPart GetPart(WetCollocationPartID id) {
            WetCollocationPart wcp = Parts[id.PartIndex];
            WetCollocationPartVariant wcpv = Parts[id.PartIndex].Variants[id.VariantIndex];

            return new CollocationPart(wcp.StrWord, wcpv.Paradigm, wcpv.Forms[id.FormIndex], this, id);
        }
        public WetCollocationPart GetWetPart(WetCollocationPartID id) {
            return Parts[id.PartIndex];
        }
        public WetCollocationPartVariant GetPartVariant(WetCollocationPartID id) {
            WetCollocationPartVariant wcpv = Parts[id.PartIndex].Variants[id.VariantIndex];
            return wcpv;
        }
        public Vars.WordForm GetPartForm(WetCollocationPartID id) {
            WetCollocationPartVariant wcpv = Parts[id.PartIndex].Variants[id.VariantIndex];
            return wcpv.Forms[id.FormIndex];
        }
    }


    public class CollocationPart {
        public Word DictWord;

        public String Word;
        public IParadigm Paradigm;
        public Vars.WordForm Form;

        public WetCollocation PWetCollocation;
        public WetCollocationPartID PWetCollocationPartID;
        
        //ConcretePart<Word> Variants;

        public CollocationPart(String word, IParadigm paradigm, Vars.WordForm form, WetCollocation wetCollocationFrom, WetCollocationPartID inWetCollocationPartID) {
            Word = word;
            Paradigm = paradigm;
            Form = form;

            PWetCollocation = wetCollocationFrom;
            PWetCollocationPartID = inWetCollocationPartID;
        }
        public CollocationPart(WetCollocation wetCollocationFrom, WetCollocationPartID inWetCollocationPartID) {
            PWetCollocation = wetCollocationFrom;
            PWetCollocationPartID = inWetCollocationPartID;

            var part = PWetCollocation.GetPart(PWetCollocationPartID);

            Word = part.Word;
            Paradigm = part.Paradigm;
            Form = part.Form;
        }

        public WetCollocationPartVariant ParentWetCollocationPartVariant {
            get {
                return PWetCollocation.GetPartVariant(PWetCollocationPartID);
            }
        }
        public Vars.WordForm ParentWetCollocationPartForm {
            get {
                return PWetCollocation.GetPartForm(PWetCollocationPartID);
            }
        }
    }

    class ConcretePart<T> {
        public T Word;
        IParadigm Form;
        //Vars.WordForm 

        String Collocation; // дня недели. времен редактирования.
        //public Word Word;

        public ConcretePart() {
            Word = default(T);// part;
        }
        public ConcretePart(T part) {
            Word = part;
        }
    }
    class ConcreteWord : ConcretePart<Word> { }
    class ConcreteNoun : ConcretePart<Noun> {

    }

    //скушал спелого сочного яблока
    class Collocation {
        List<CollocationPart> Words;

        public Collocation(WetCollocation wetCollocation) {
            ProcessWetCollocation(wetCollocation);
        }
        public Collocation(String collocation = "") {
            ProcessWetCollocation(new WetCollocation(collocation));
        }

        public void ProcessWetCollocation(WetCollocation wetColl) {
            foreach (var word in wetColl.Parts) {
                //Words.Add(new CollocationPart(  ConcreteWord Dictionary.Instance.GetWord(Word));
            }
        }
    }

    //public class TreeBuilder {
    //    private static TreeBuilder instance;

    //    private TreeBuilder() { }

    //    public static TreeBuilder Instance {
    //        get {
    //            if (instance == null) {
    //                instance = new TreeBuilder();
    //            }
    //            return instance;
    //        }
    //    }
    //}

    #region Ambiguity

    interface IMultipleComparable {
        List<Boolean> CompareTo(IMultipleComparable b);
        Double CompareDbl(IMultipleComparable b);
    }
    class IntContext : IMultipleComparable {
        public List<int> Vars = new List<int>();

        public void AddFormInfo(Vars.WordForm wf) {
            Vars.Add((int)wf.POS);
            Vars.Add((int)wf.Case);
            Vars.Add((int)wf.Deistvit);
            Vars.Add((int)wf.Gender);
            Vars.Add((int)wf.Litso);
            Vars.Add((int)wf.Number);
            Vars.Add((int)wf.Odushevl);
            Vars.Add((int)wf.Perehodn);
            Vars.Add((int)wf.Povelit);
            Vars.Add((int)wf.Soversh);
            Vars.Add((int)wf.Sravnit);
            Vars.Add((int)wf.Tense);
        }
        //public override bool Equals(object obj) {
        //    return this.Equals(obj as IntContext);
        //}
        //public bool Equals(IntContext p) {
        //    if (Object.ReferenceEquals(p, null))
        //        return false;

        //    if (Object.ReferenceEquals(this, p))
        //        return true;

        //    if (this.GetType() != p.GetType())
        //        return false;

        //    return (PartIndex == p.PartIndex) && (VariantIndex == p.VariantIndex) && (FormIndex == p.FormIndex);
        //}
        //public static bool operator ==(IntContext lhs, IntContext rhs) {
        //    // Check for null on left side. 
        //    if (Object.ReferenceEquals(lhs, null)) {
        //        if (Object.ReferenceEquals(rhs, null)) {
        //            // null == null = true. 
        //            return true;
        //        }

        //        // Only the left side is null. 
        //        return false;
        //    }
        //    // Equals handles case of null on right side. 
        //    return lhs.Equals(rhs);
        //}
        //public static bool operator !=(IntContext lhs, IntContext rhs) {
        //    return !(lhs == rhs);
        //}
        //public override int GetHashCode() {
        //    int h = 0;
        //    for (int i = 0; i < Vars.Count; i++) {
        //        i += Vars[i] * (2 << i);//(int)Math.Pow(2, i);
        //    }
        //    return h;
        //}

        //public List<Boolean> CompareTo(Object obj) {
        //    var ans = new List<Boolean>();
        //    for (int i = 0; i < Vars.Count; i++) {
        //        ans.Add(Vars[i] == context[i]);
        //    }
        //    return ans;
        //}
        public List<Boolean> CompareTo(IMultipleComparable context) {
            IntContext b = (IntContext)context;
            var ans = new List<Boolean>();
            for (int i = 0; i < Math.Min(b.Vars.Count, Vars.Count); i++) {
                ans.Add(Vars[i] == b[i]);
            }
            return ans;
        }
        public Double CompareDbl(IMultipleComparable context) {
            IntContext b = (IntContext)context;
            Double ans = 0;
            Int16 N = 0;
            for (int i = 0; i < Math.Min(b.Vars.Count, Vars.Count); i++) {
                if (Vars[i] == 0 && b[i] == 0)
                    continue;
                N++;
                if (Vars[i] != 0 && b[i] == 0)
                    ans += 0.1;
                else
                    ans += Vars[i] == b[i] ? 1 : 0;
            }
            if (N == 0)
                return 1;
            return ans / N;
        }

        public int this[int key] {
            get {
                return Vars[key];
            }
            set {
                Vars[key] = value;
            }
        }
    }
    class AmbiguityItem<T, ContextT> where ContextT : IMultipleComparable where T : IMultipleComparable {
        public T Correct;
        public List<T> Alternatives;
        public ContextT Context;
        public Boolean Confirmed = false;

        public AmbiguityItem(T correct, List<T> alternatives, ContextT context, Boolean confirmed = false) {
            Correct = correct;
            Alternatives = alternatives;
            Context = context;
            Confirmed = confirmed;
        }
    }
    class AmbuguityDictionary<T, ContextT> where ContextT : IMultipleComparable where T : IMultipleComparable {
        List<AmbiguityItem<T, ContextT>> Items = new List<AmbiguityItem<T,ContextT>>();

        public void Add(AmbiguityItem<T, ContextT> item) {
            Items.Add(item);
        }
        public void Add(T correct, List<T> alternatives, ContextT context, Boolean confirmed = false) {
            Items.Add(new AmbiguityItem<T, ContextT>(correct, alternatives, context, confirmed));
        }
        public void Add(T correct, ContextT context, Boolean confirmed = false) {
            Items.Add(new AmbiguityItem<T, ContextT>(correct, new List<T>(), context, confirmed));
        }
        class AmbiguityItemWeights {
            public double OneAlt_NonConfirmed;
            public double OneAlt_Confirmed;
            public void Add_OneAlt_NonConfirmed(double c) {
                OneAlt_NonConfirmed = Math.Max(OneAlt_NonConfirmed, c);
            }
            public void Add_OneAlt_Confirmed(double c) {
                OneAlt_Confirmed = Math.Max(OneAlt_Confirmed, c);
            }

            public double Wrong_OneAlt_NonConfirmed;
            public double Wrong_OneAlt_Confirmed;
            public void Add_Wrong_OneAlt_NonConfirmed(double c) {
                Wrong_OneAlt_NonConfirmed = Math.Max(Wrong_OneAlt_NonConfirmed, c);
            }
            public void Add_Wrong_OneAlt_Confirmed(double c) {
                Wrong_OneAlt_Confirmed = Math.Max(Wrong_OneAlt_Confirmed, c);
            }

            public double ContextComparation;
            public double AlternativesMatch;
            public double AlternativesMismatch;

            public double CurrentThruth;

            private double _Thruth;
            private double _Sum;

            public void AddThruth(Double thruth, Double k) {
                _Thruth += thruth*k;
                _Sum += k;
            }
            public Double Thruth {
                get {
                    return _Thruth / _Sum;
                }
            }

            public AmbiguityItemWeights(double contextComparation = 0, double alternativesMatch = 0, double alternativesMismatch = 0) {
                ContextComparation = contextComparation;
                AlternativesMatch = alternativesMatch;
                AlternativesMismatch = alternativesMismatch;
            }
        }
        public Int32 TryToResolve(List<T> alternatives, ContextT context) {
            Dictionary<T, AmbiguityItemWeights> weights = new Dictionary<T, AmbiguityItemWeights>();

            foreach (var i in alternatives) {
                weights.Add(i, new AmbiguityItemWeights());
            }

            foreach (var item in Items) {
                var contextCmp = context.CompareDbl(item.Context);
                if (contextCmp > 0) {
                    Double SumConf = 0;
                    foreach (var j in alternatives) {
                        weights[j].CurrentThruth = 0;

                        var maxCmp = item.Correct.CompareDbl(j);
                        weights[j].CurrentThruth += maxCmp;

                        foreach (var k in item.Alternatives) {
                            var cmp = k.CompareDbl(j);
                            weights[j].CurrentThruth += 1-cmp;
                            maxCmp = Math.Max(maxCmp, cmp);
                        }

                        SumConf += maxCmp;

                        weights[j].CurrentThruth /= item.Alternatives.Count + 1;
                    }
                    SumConf /= alternatives.Count;

                    foreach (var j in alternatives) {
                        weights[j].AddThruth(weights[j].CurrentThruth, SumConf * contextCmp);
                    }

                    //if (item.Alternatives.Count == 0) {
                    //    Double bstCmp = 0;
                    //    if (item.Confirmed) {
                    //        foreach (var j in alternatives)
                    //            weights[j].Add_OneAlt_Confirmed(item.Correct.CompareDbl(j) * contextCmp);
                    //    }
                    //    else {
                    //        foreach (var j in alternatives)
                    //            weights[j].Add_OneAlt_NonConfirmed(item.Correct.CompareDbl(j) * contextCmp);
                    //    }
                    //}
                    //else {
                    //    //Double bstCmp = 0;
                    //    //T bstAlt;
                    //    //if (i.Confirmed) {
                    //    //    foreach (var j in alternatives)
                    //    //        weights[bstAlt].Add_OneAlt_Confirmed(i.Correct.CompareDbl(j) * contextCmp);
                    //    //}
                    //    //else {
                    //    //    foreach (var j in alternatives)
                    //    //        weights[bstAlt].Add_OneAlt_NonConfirmed(i.Correct.CompareDbl(j) * contextCmp);
                    //    //}
                    //}
                }

                //T correct = alternatives.Find(alt => alt.Equals(i.Correct));
                //if (correct != null) {
                //    var contextCmp = context.CompareDbl(i.Context);
                //    if (contextCmp > 0) {
                //        weights[correct].ContextComparation = contextCmp;
                //      //int match = 0;
                //        int mismatch = 0;
                //        foreach (var j in alternatives) {
                //            if (!i.Alternatives.Contains(j))
                //                mismatch++;
                //        }
                //      //weights[correct].AlternativesMatch = match;
                //        weights[correct].AlternativesMismatch = mismatch;
                //    }
                //}
            }

            Double maxThruth = -1;
            T maxAlt = default(T);
            var index = 0;
            var cc = 0;
            foreach (var i in weights) {
                if (i.Value.Thruth > maxThruth) {
                    maxThruth = i.Value.Thruth;
                    maxAlt = i.Key;
                    index = cc;
                }
                cc++;
            }

            return index;
        }
    }

    #endregion

    class Brain {
        private static Brain instance;
        public static Brain Instance {
            get {
                if (instance == null) {
                    instance = new Brain();
                }
                return instance;
            }
        }

        ILemmatizer Lemmatizer;
        public string rgt;

        public List<Word> Words = new List<Word>();
        public List<FuzzySynonyms> Synonyms = new List<FuzzySynonyms>();

        public AmbuguityDictionary<IntContext, IntContext> IngoingConnectionsAD = new AmbuguityDictionary<IntContext, IntContext>();
        public AmbuguityDictionary<IntContext, IntContext> AccessoriesAD = new AmbuguityDictionary<IntContext, IntContext>();

        public FuzzySynonyms AddSynonyms(Word a, Word b, Double conf = 1) {
            var newSyn = new FuzzySynonyms(a, b, conf);
            Synonyms.Add(newSyn);
            return newSyn;
        }
        public FuzzySynonyms AddAntonyms(Word a, Word b, Double conf = -1) {
            return AddSynonyms(a, b, conf);
        }

        public List<FuzzySynonyms> GetSynonymsPairs(Word a) {
            return Synonyms.FindAll(i => i.HasSynonymFor(a));
        }
        public List<FuzzyWordRelation> GetSynonyms(Word a) {
            return Synonyms.FindAll(i => i.HasSynonymFor(a)).ConvertAll(i => i.SynonymFor(a));
        }

        public void InitIngoingConnectionsAD() {
            AddToIngoingConnectionsAD("Г", "", "ПРЕДЛ");
            AddToIngoingConnectionsAD("ПРИЧАСТИЕ", "", "ПРЕДЛ");
            AddToIngoingConnectionsAD("ДЕЕПРИЧАСТИЕ", "", "ПРЕДЛ");
            AddToIngoingConnectionsAD("ИНФИНИТИВ", "", "ПРЕДЛ");

            var correct = new IntContext();
            correct.Vars.Add(0);
            correct.Vars.Add(1);
            correct.AddFormInfo(new Vars.WordForm(""));
            //var alt = new IntContext();
            //alt.Vars.Add(0);
            //alt.Vars.Add(0);
            //alt.AddFormInfo(new Vars.WordForm(altp));
            var context = new IntContext();
            context.Vars.Add(0);
            context.AddFormInfo(new Vars.WordForm(""));

            IngoingConnectionsAD.Add(correct, new List<IntContext>() { }, context, true);
        }
        public void AddToIngoingConnectionsAD(String corr, String altp, String cont) {
            var correct = new IntContext();
            correct.Vars.Add(0);
            correct.Vars.Add(0);
            correct.AddFormInfo(new Vars.WordForm(corr));
            var alt = new IntContext();
            alt.Vars.Add(0);
            alt.Vars.Add(0);
            alt.AddFormInfo(new Vars.WordForm(altp));
            var context = new IntContext();
            context.Vars.Add(0);
            context.AddFormInfo(new Vars.WordForm(cont));
            
            IngoingConnectionsAD.Add(correct, new List<IntContext>(){alt}, context, true);
        }

        public void InitAccessoriesAD() {
            var correct = new IntContext();
            correct.Vars.Add(1);
            correct.Vars.Add(0);
            correct.Vars.Add(0);
            correct.AddFormInfo(new Vars.WordForm(""));
            var context = new IntContext();
            context.Vars.Add(0);
            context.Vars.Add(0);
            context.AddFormInfo(new Vars.WordForm(""));

            AccessoriesAD.Add(correct, new List<IntContext>(), context, true);

            correct = new IntContext();
            correct.Vars.Add(1);
            correct.Vars.Add(1);
            correct.Vars.Add(0);
            correct.AddFormInfo(new Vars.WordForm(""));
            context = new IntContext();
            context.Vars.Add(0);
            context.Vars.Add(0);
            context.AddFormInfo(new Vars.WordForm(""));

            AccessoriesAD.Add(correct, new List<IntContext>(), context, true);
        }

        public Brain() {
            InitIngoingConnectionsAD();
            InitAccessoriesAD();

            var rmlPath = System.Environment.GetEnvironmentVariable("RML");

            if (string.IsNullOrEmpty(rmlPath)) {
                var newRmlPath = "";
                if (!string.IsNullOrEmpty(newRmlPath)) {
                    rmlPath = newRmlPath;
                }
            }

            var langStr = "R";
            MorphLanguage lang = MorphLanguage.Russian;
            Lemmatizer = LemmatizerFactory.Create(lang);
            try {
                StreamReader r = new StreamReader(rmlPath + @"\Dicts\Morph\" + langStr.ToLower() + "gramtab.tab", Encoding.GetEncoding(1251));
                rgt = r.ReadToEnd(); r.Close();
            }
            catch (Exception ex) {
            }
            try {
                var manager = FileManager.GetFileManager(rmlPath);
                Lemmatizer.LoadDictionariesRegistry(manager);
            }
            catch (IOException ex) {
                //this.ans.Text = "ERR LOADING DICT";
                return;
            }
        }

        public static List<Word> GetWords(String word) {
            List<IParadigm> paradigms = ParadigmsList(word);
            List<Word> hypoteses = new List<Word>();

            if (paradigms != null) {
                foreach (var p in paradigms) {
                    int id = p.ParadigmID;
                    bool found = false;

                    foreach (Word j in Instance.Words)
                        if (j.ParadigmID == id) {
                            hypoteses.Add(j);
                            found = true;
                        }

                    if (!found) {
                        Word newWord = NewWordFromParadigm(p, word);
                        Instance.Words.Add(newWord);
                        hypoteses.Add(newWord);
                    }
                }

                if (hypoteses.Count == 1)
                    hypoteses[0].AddVerifiedVariant(word);
                
                return hypoteses;
            }
            else {
                foreach (Word j in Instance.Words) {
                    if (j.IsInVerifiedVariants(word))
                        hypoteses.Add(j);
                }
                return hypoteses;
            }
        }

        public static List<IParadigm> ParadigmsList(String word) {
            var paradigmCollection = Instance.Lemmatizer.CreateParadigmCollectionFromForm(word, true, true); //false, false);

            if (paradigmCollection.Count > 0) {
                List<IParadigm> paradigms = new List<IParadigm>();
                paradigms.Add(paradigmCollection[0]);

                for (var i = 1; i < paradigmCollection.Count; i++) {
                    var paradigm = paradigmCollection[i];
                    int w = paradigm.WordWeight;

                    if (w < paradigms.Last().WordWeight)
                        paradigms.Add(paradigm);
                    else
                        for (var j = 0; j < paradigms.Count; j++)
                            if (w > paradigms[j].WordWeight) {
                                paradigms.Insert(j, paradigm);
                                break;
                            }
                }

                return paradigms;
            }

            return null;
        }
        public static Word WordByParadigm(IParadigm paradigm) {
            int id = paradigm.ParadigmID;

            foreach (Word j in Instance.Words)
                if (j.ParadigmID == id)
                     return j;

            Word word = NewWordFromParadigm(paradigm);
            Instance.Words.Add(word);
            return word;
        }
        public static Word NewWordFromParadigm(IParadigm paradigm) {
            Word word = new Word(paradigm);
            switch (word.POS) {
                case PsOS.Noun:
                    word = new Noun(paradigm);
                    break;
                case PsOS.Adj:
                    word = new Adj(paradigm);
                    break;
                case PsOS.Verb:
                case PsOS.Infinitive:
                    word = new Verb(paradigm);
                    break;
            }
            return word;
        }
        public static Word NewWordFromParadigm(IParadigm paradigm, String spelling) {
            Word word = new Word(paradigm, spelling);
            switch (word.POS) {
                case PsOS.Noun:
                    word = new Noun(paradigm, spelling);
                    break;
                case PsOS.Adj:
                    word = new Adj(paradigm, spelling);
                    break;
                case PsOS.Verb:
                case PsOS.Infinitive:
                    word = new Verb(paradigm, spelling);
                    break;
            }
            return word;
        }
        public static Word Word(String word) {
            List<IParadigm> paradigms = ParadigmsList(word);

            if (paradigms != null) {
                List<Word> hypoteses = new List<Word>();

                foreach (var i in paradigms) {
                    int id = i.ParadigmID;

                    foreach (Word j in Instance.Words)
                        if (j.ParadigmID == id)
                            if (j.IsInVerifiedVariants(word))
                                return j;
                            else {
                                hypoteses.Add(j);
                            }
                }
                if (hypoteses.Count == 1) {
                    hypoteses[0].AddVerifiedVariant(word);
                    return hypoteses[0];
                }
                else if (hypoteses.Count == 0) {
                    foreach (Word j in Instance.Words) {
                        if (j.IsInVerifiedVariants(word))
                            return j;
                    }
                }

                IParadigm p = paradigms.First();
                Word newWord = NewWordFromParadigm(p, word);
                Instance.Words.Add(newWord);
                return newWord;
            }
            else {
                foreach (Word j in Instance.Words) {
                    if (j.IsInVerifiedVariants(word))
                        return j;
                }
                Word ans = new Word(word);
                Instance.Words.Add(ans);
                return ans;
            }
        }
        public static Word GetRefinedWord(String word) {
            var pos = word.IndexOf('$');
            if (pos > 0) {
                PsOS POS = Vars.GetPOS(word.Substring(pos + 1));
                var words = GetWords(word.Substring(0, pos));
                foreach (var i in words) {
                    if (i.POS == POS) {
                        return i;
                    }
                }
                return Word(word);
            }
            else {
                return Word(word);
            }
        }

        public Collocation GetCollocation(String phrase) {
            return new Collocation();
        }

        void LearnWordsFromFile(String path) {
            String[] File = System.IO.File.ReadAllLines(path);

            foreach (String Line in File) {
                Learn(Line);
            }
        }

        void LearnFromFile(String path) {
            String[] File = System.IO.File.ReadAllLines(path);

            foreach (String Line in File) {
                Learn(Line);
            }
        }

        void Disassemble(String line) {
            String[] Words = line.Split(' ');

        }
        void Learn(String line) {
            
        }


        class ParsedSearchData {
            List<String> Keywords;
            class DateTimeRule {
                String _Rule;

                enum Type {
                    Date,           //   rule for date
                    Time,           //   rule for time
                    TimeAMPM,       //   not mentioned is it pm or am
                }
                enum CurveType {
                    Concrete_ABC,   //   ___/'\___
                    ConcreteRelToNow_ABC,   //   ___/'\___
                    Before_AB,      //   '''\___
                    After_AB,       //   ___/'''
                    Span_ABCD       //   __/''\__
                }

                DateTime A;
                DateTime B;
                DateTime C;
                DateTime D;

                public String Rule {
                    get {
                        return _Rule;
                    }
                    set {
                        _Rule = value;
                    }
                }

                private void Parse() {
                    //Вчера
                }

                public float TestDate() {
                    return 0;
                }
            }
            TimeSpan DaysBefore;
            DateTime Days;
            TimeSpan DaySpan;
        }
    }
}





