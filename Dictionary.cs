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
            _Data = (T)Brain.GetWord(word);
            _Binder = Brain.GetWord(binder);
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
 

    public class Word {
        String Spelling;
        String Root;
        private IParadigm InitFrom;
        public List<String> VerifiedVariants = new List<string>();

        protected List<WordConnection> Relatives;
        protected List<WordConnection> Synonyms;

        //protected List<Connection<SpecifiedWord>> Rules;

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
                Q = Brain.GetWord(q);
            }
            public QItem(String q, String[] verified) {
                Q = Brain.GetWord(q);
                foreach (var i in verified) {
                    VerifiedAnsvers.Add(Brain.GetWord(i));
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
            InitFrom = form;
            AddVerifiedVariant(word);
        }

        public PsOS POS {
            get {
                String rgt = Brain.Instance.rgt;
                String r = Regex.Match(rgt, "^" + InitFrom.GetAncode(0) + "[^а-яА-яЕё]*(.*)", RegexOptions.Multiline).Groups[1].Value.Replace("\r", "");

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

        public int ParadigmID {
            get {
                if (InitFrom == null)
                    return -1;
                return InitFrom.ParadigmID;
            }
        }
        
        public bool IsParadigmIDTheSame(List<int> paradigmIDs) {
            int pid = InitFrom.ParadigmID;
            foreach (var i in paradigmIDs)
                if (i == pid)
                    return true;
            return false;
        }
        public bool IsParadigmIDTheSame(int paradigmID) {
            int pid = InitFrom.ParadigmID;
            return pid == paradigmID;
        }
        
        public void AddSynonym(Word word) {
            foreach (var i in Synonyms)
                if (i.Word == word)
                    return;
            
            foreach (var i in Relatives)
                if (i.Word == word)
                    return;

            Synonyms.Add(new WordConnection(word));
        }
        public void AddRelative(Word word) {
            foreach (var i in Relatives)
                if (i.Word == word)
                    return;

            foreach (var i in Synonyms)
                if (i.Word == word)
                    return;

            Relatives.Add(new WordConnection(word));
        }
        public void AddRule(SpecifiedWord word) {
            foreach (var i in Relatives)
                if (i.Word == word.Word)
                    return;

            foreach (var i in Synonyms)
                if (i.Word == word.Word)
                    return;

            //Rules.Add(new Connection<SpecifiedWord>(word));
        }
    }

    public class Verb : Word { // глагол
        List<WordConnection<Adv>> Adverbiums; // наречия
        
        //List<Connection<Noun>> Adverbiums; // в папку. на стол

        public Verb(IParadigm form, String word) : base(form, word) { }
    }
    public class Adv : Word {  // наречие
        Adj AdjParent;         // если было образовано от прил. быстро - быстрый.
        Noun NounParent;       // если было образовано от прил. домой - дом.

        public Adv(IParadigm form, String word) : base(form, word) { }
    }
    public class Adj : Word {  // прил
        Cases Case;
        Numbers Number;

        List<WordConnection<Adj>> Adjectives; // прил прил ex: ярко красный = кр прил + прил

        List<WordConnection> GeneralizeParent; // красный - цвет. второй - порядковый номер.

        public Adj(IParadigm form, String word) : base(form, word) { }
    }
    public class Noun : Word { // сущ.
        List<WordConnection> Adjectives = new List<WordConnection>(); // прилагательные 
        // день - настроение. день - сложность. день - насыщенность.
        //// утро - чать дня. вечер - часть дня.
        //  день - часть. части дня - утро, вечер.

        public List<WordConnection<Verb>> ThisCan = new List<WordConnection<Verb>>();
        public List<WordConnection<Verb>> WeCan = new List<WordConnection<Verb>>();

        public List<Word> GeneralizeParents = new List<Word>();
        
        // вторник - день. люмен - музыкальная группа.
        // длинный - длина, короткий - длина. длина - размер. ширина - размер. большой - размер.
        // сложный - сложность. легкий - легкость. важный - важность. неважный - важность.
        // утро - чать дня. вечер - часть дня. 
        
        List<WordConnection> PartsOfThis = new List<WordConnection>();

        public Noun(IParadigm form, String word) : base(form, word) { }

        public void AddRule(String rule) {
            

            //Rules.Add(new Connection<SpecifiedWord>(word));
        }
    }


    public class SpecifiedPart<T> where T : Alice.Word {
        public T Word;
        Vars.WordForm Form;
        String Collocation;

        List<WordConnection> Properties;    //  = крутая. / музыкальная. название = kiss. музыкант = а1. музыкант а2. жанр = рок. жанр = рок'н'ролл.

        public void AddProperty(Word value, Word property) {
            WordConnection wc = new WordConnection(value, property);
            Properties.Add(wc);
        }
        public void AddProperty(String value, String property) {
            //WordConnection wc = new WordConnection(value, property);
            //Properties.Add(wc);
        }
    }
    public class SpecifiedWord : SpecifiedPart<Word> { }
    public class SpecifiedNoun : SpecifiedPart<Noun> {
        // Word =        день         время                 группа         группа
        // Collocation = день недели. время редактирования. крутая группа. kiss.
        Verb ThisDo;    // наступил.
        Verb WeDo;      // изменить. назначить.

        SpecifiedNoun GeneralizeParent;     // промежуток времени. / редактирование. В.П. Е.Ч.

        List<WordConnection> PartsOfThis;   // 

        public SpecifiedNoun(String collocation) {
            List<Word> Words = new List<Word>();
            List<int> Nouns = new List<int>();

            foreach (String Word in collocation.Split(' ')) {
                Words.Add(Brain.GetWord(Word));
                //if (Words.Last().Part
            }
        }
    }

    public class Thing { 
        List<WordConnection> Properties;    //  = крутая. / музыкальная. название = kiss. музыкант = а1. музыкант а2. жанр = рок. жанр = рок'н'ролл.


        //рок - жанр
        //вечер - часть чего? дня - 0.9
        //ночь - часть дня - 0.3
        //привет, как дела? - содержание
        //Alexey Mogilnikov - автор
        //22.03.2014 - Дата
        //весна + ранняя - время чего? года
        //2014 - год
        //21:23 - Время
        //21 - час
        //9 чего? вечера - время - 0.3
        //пол чего? 9 - время - 0.5

        //жесткий[ - жесткость]
        //любимый
        //важный
        //какой? двадцать второй - день
        //новый
        //красный - цвет

        public void AddProperty(Thing value, Thing property) {
            //WordConnection wc = new WordConnection(value, property);
            //Properties.Add(wc);
        }
        public void AddProperty(String value, String property) {
            //WordConnection wc = new WordConnection(value, property);
            //Properties.Add(wc);
        }


    }

    [Flags] public enum Matchings {
        N = 0,
        Gender = 1,
        Number = 2,
        Case = 4,
        GNC = 7,
        Tense = 8,
        Litso = 16,
    }
    public enum ABLocations {
        Any,
        AB,
        BA,
        A_B,
        B_A
    }
    public class PartsConnectionRule {
        public Vars.WordForm AForm;
        public Vars.WordForm BForm;
        ABLocations ABLocation;
        Matchings Match;

        public PartsConnectionRule(Vars.WordForm aform, Vars.WordForm bform, ABLocations abLocation = ABLocations.Any, Matchings match = Matchings.N) {
            AForm = aform;
            BForm = bform;
            ABLocation = abLocation;
            Match = match;
        }
        public PartsConnectionRule(String aform, String bform, ABLocations abLocation = ABLocations.Any, Matchings match = Matchings.N) {
            AForm = new Vars.WordForm(aform);
            BForm = new Vars.WordForm(bform);
            ABLocation = abLocation;
            Match = match;
        }
        public PartsConnectionRule() {
        }

        public bool CheckForms(Vars.WordForm a, Vars.WordForm b) {
            if (Match.HasFlag(Matchings.Gender) && a.Gender != b.Gender)
                return false;
            if (Match.HasFlag(Matchings.Number) && a.Number != b.Number)
                return false;
            if (Match.HasFlag(Matchings.Case) && a.Case != b.Case)
                return false;
            if (Match.HasFlag(Matchings.Tense) && a.Tense != b.Tense)
                return false;
                
            return true;
        }
        public bool CheckForm(Vars.WordForm form, Vars.WordForm template) {
            if (form.POS != template.POS)
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
            }}}

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

            }}

            if (ABLocation == ABLocations.A_B)
            for (int ai = 0; ai < parts.Count; ai++) {
            var aPart = parts[ai];
            for (int bi = ai+1; bi < parts.Count; bi++) {
            var bPart = parts[bi];

                var relations = FindRelations(aPart, bPart, ai, bi);
                if (relations != null) ans.AddRange(relations);

            }}

            if (ABLocation == ABLocations.AB)
            for (int ai = 0; ai < parts.Count-1; ai++) {
            var aPart = parts[ai];
            var bPart = parts[ai+1];

                var relations = FindRelations(aPart, bPart, ai, ai+1);
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

        List<PartsConnectionRule> Rules = new List<PartsConnectionRule>();

        private TreeBuilder() {
            Rules.Add(new PartsConnectionRule("Г", "С им", ABLocations.Any, Matchings.Number));
            Rules.Add(new PartsConnectionRule("Г", "С", ABLocations.Any, Matchings.N));
            // мы писали письма // письма приходили
            // письма писали мы // письма писали мы хотим чтобы нас читали.
            Rules.Add(new PartsConnectionRule("Г", "МС им", ABLocations.Any, Matchings.Number | Matchings.Litso));
            Rules.Add(new PartsConnectionRule("Г", "МС", ABLocations.Any, Matchings.N));
            Rules.Add(new PartsConnectionRule("Г", "МС-П", ABLocations.Any, Matchings.N));
            Rules.Add(new PartsConnectionRule("С", "П", ABLocations.Any, Matchings.GNC));
            Rules.Add(new PartsConnectionRule("С", "ПРЕДЛ", ABLocations.Any, Matchings.N));
            Rules.Add(new PartsConnectionRule("С", "ЧИСЛ", ABLocations.BA, Matchings.N));
            Rules.Add(new PartsConnectionRule("Г", "ПРЕДЛ", ABLocations.Any, Matchings.N));
            Rules.Add(new PartsConnectionRule("ПРЕДЛ", "С", ABLocations.A_B, Matchings.N));
            Rules.Add(new PartsConnectionRule("ПРЕДЛ", "МС", ABLocations.A_B, Matchings.N));
            Rules.Add(new PartsConnectionRule("ПОСЛЕЛ", "МС", ABLocations.BA, Matchings.N));
            Rules.Add(new PartsConnectionRule("ПОСЛЕЛ", "С", ABLocations.BA, Matchings.N));
            Rules.Add(new PartsConnectionRule("Г", "Н", ABLocations.Any, Matchings.N));
            Rules.Add(new PartsConnectionRule("Н", "ПРЕДК", ABLocations.B_A, Matchings.N));
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

        public PartsConnectionRule UsedRule;

        public float Confirmed = 0;

        public WetPartsRelation() { }
        public WetPartsRelation(WetCollocationPartID a, WetCollocationPartID b, PartsConnectionRule usedRule, WetCollocation collocation) {
            A = a;
            B = b;
            UsedRule = usedRule;
            Collocation = collocation;
        }

        public new string ToString {
            get {
                return Collocation.Parts[A.PartIndex].Word + "->" + Collocation.Parts[B.PartIndex].Word;
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
        }
    }
    public class WetCollocationPart {
        public WetPartsRelation IngoingConnection = null;
        public WetPartsRelation OutgoingConnection = null;

        public WetCollocation Collocation;
        public String Word;
        public List<WetCollocationPartVariant> Variants = new List<WetCollocationPartVariant>();

        public WetCollocationPart(String word, WetCollocation collocation) {
            Word = word;
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
                Parts.Add(new WetCollocationPart(Word, this));
            }
            BuildRelations();
            ProcessPreposotions();
            ProcessPostpositions();
            GetTrees();

            //FilterParadigms();
        }

        private void ProcessPostpositions() {
            for (int j = 0; j < Relations.Count; j++) {
                var r = Relations[j];
                if (r.UsedRule.AForm.POS == PsOS.Postposition) {
                //if (r.WetPartVariantA.Forms[0].POS == PsOS.Postposition) {
                    r.WetPartA.OutgoingConnection = r;
                    r.WetPartB.IngoingConnection = r;

                    RemoveAllRelations(r.A.PartIndex, r.B.PartIndex);

                    ProcessPreposotions();
                }
            }
        }
        private void ProcessPreposotions() {
            for (int i = 0; i < Parts.Count; i++) {
                var a = Parts[i];
                if (a.OutgoingConnection == null)
                    if (a.Variants[0].Forms[0].POS == PsOS.Proposition) {
                        int min = Parts.Count;
                        WetPartsRelation minRelation = null;
                        bool shit = false;

                        for (int j = 0; j < Relations.Count; j++) {
                            var r = Relations[j];
                            if (r.A.PartIndex == i && Parts[r.A.PartIndex].IngoingConnection == null) {
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
                                
                                RemoveAllRelations(i, minRelation.B.PartIndex);

                                Relations.RemoveAll(ir => (ir.A.PartIndex == minRelation.B.PartIndex && ir.B.PartIndex <= i));

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
        
        void RemoveAllRelations(int A, int B) {
            Relations.RemoveAll(i => (i.A.PartIndex == A || i.B.PartIndex == B));
        }

        // повествовате-льных, побудитель-ных, вопроситель-ных предложений, 
        private void GetTrees() {
            WetCollocationPartID Predicate = null;

            for (int i = 0; i < Parts.Count; i++) {
                var a = Parts[i];
                for (int iVar = 0; iVar < a.Variants.Count; iVar++)
                if (a[iVar].HasOutgoingConnection > 0) {
                    var aForms = a.Variants[iVar].Forms;
                    for (int iForm = 0; iForm < aForms.Count; iForm++) {
                        var aForm = aForms[iForm];
                        if (aForm.POS == PsOS.Verb) {

                            List<WetPartsRelation> TreeRelations = new List<WetPartsRelation>();
                            FindChildren(new CollocationPart(this, new WetCollocationPartID(i, iVar, iForm)), TreeRelations, 0, Parts.Count);
                            
                            //var relations = FindRelations(new CollocationPart(collocation, new WetCollocationPartID(i, iVar, iForm)), 0, collocation.Parts.Count);
                            //if (relations != null) ans.AddRange(relations);

                        }
                    }
                }
            }
        }

        private void FindChildren(CollocationPart part, List<WetPartsRelation> TreeRelations, int l, int r) {
            List<WetPartsRelation> NewRelations = new List<WetPartsRelation>();

            foreach (var ir in Relations.Except(TreeRelations)) {
            //for (int i = l; i < r; i++) {
                //var p = Relations[i];
                if (ir.B.PartIndex >= l && ir.B.PartIndex < r)
                if (ir.A == part.PWetCollocationPartID) {
                    var s = ir.ToString;
                    
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

        private void BuildRelations() {
            Relations = TreeBuilder.FindRelations(this);
        }

        private void FilterParadigms() {
            //Relations = TreeBuilder.FindRelations(this);
            foreach (var part in Parts)
                for (int i = 0; i < part.Variants.Count; i++) {
                    var variant = part.Variants[i];
                    if (!variant.HasConnection) {
                        //variant.
                        i--;
                    }
                }


        }

        public CollocationPart GetPart(WetCollocationPartID id) {
            WetCollocationPart wcp = Parts[id.PartIndex];
            WetCollocationPartVariant wcpv = Parts[id.PartIndex].Variants[id.VariantIndex];

            return new CollocationPart(wcp.Word, wcpv.Paradigm, wcpv.Forms[id.FormIndex], this, id);
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

        public Brain() {
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

        public List<Word> GetWords(String word) {

            return null;
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

        public static Word GetWord(String word) {
            List<IParadigm> paradigms = ParadigmsList(word);

            if (paradigms != null) {
                List<Word> hypoteses = new List<Word>();

                foreach (var i in paradigms) {
                    int id = i.ParadigmID;

                    foreach (Word j in Instance.Words) {
                        if (j.ParadigmID == id) {
                            if (j.IsInVerifiedVariants(word))
                                return j;
                            else {
                                hypoteses.Add(j);
                                //bool Workspace.AddQuestion(word + " = " + string.Join(", ", j.VerifiedVariants.ToArray()) + "?");
                            }
                        }
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
                
                //if (Paradigms.First().

                IParadigm p = paradigms.First();

                Word ans = new Word(p, word);

                switch (ans.POS) {
                case PsOS.Noun:
                    ans = new Noun(p, word);
                    break;
                case PsOS.Adj:
                    ans = new Adj(p, word);
                    break;
                case PsOS.Verb :
                case PsOS.Infinitive :
                    ans = new Verb(p, word);
                    break;
                }
                Instance.Words.Add(ans);
                return ans;
            }
            else {
                Word ans = new Word(word);
                Instance.Words.Add(ans);
                return ans;
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





