using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alice {
    public class AliceGUIManager {
        private static AliceGUIManager instance;
        public static AliceGUIManager Instance {
            get {
                if (instance == null) {
                    instance = new AliceGUIManager();
                }
                return instance;
            }
        }

        public List<ITask> RegisteredTasks = new List<ITask>();
        public List<ITask> ExecutedTasks = new List<ITask>();

        public taskDialogBox DB;

        public AliceGUIManager() {
            DB = new taskDialogBox(this);
            DB.Show();

            RegisterTask(new taskNotes());
            RegisterTask(new taskMailBox());
        }

        public void RegisterTask(ITask task) {
            RegisteredTasks.Add(task);
        }

        ITask LastActiveTask = null;
        public void SetActiveTask(ITask task) {
            LastActiveTask = task;
        }

        public void RegisterExecTask(ITask task) {
            if (!ExecutedTasks.Contains(task))
                ExecutedTasks.Add(task);
        }
        public void UnregisterExecTask(ITask task) {
            if (LastActiveTask == task)
                LastActiveTask = null;
            ExecutedTasks.Remove(task);
        }

        public void NewText(String text) {
            TellText(text, "вы");

            var WColl = new WetCollocation(text);

            Predicat pr = WColl.GetPredicat(); //new Predicat("покажи");
            
            String s = pr.ToString();
            //TellText(s);
            if (pr == null)
                return;
            //pr.AddProperty("заметки");

            Double maxConf = 0;
            ITask task = null;

            if (LastActiveTask != null && ExecutedTasks.Contains(LastActiveTask)) {
                var c = LastActiveTask.CanExecute(pr, true);
                if (c > 0.1) {
                    maxConf = c;
                    task = LastActiveTask;
                    LastActiveTask.Execute(pr, true);
                    return;
                }
            }
            
            foreach (var i in ExecutedTasks) {
                var c = i.CanExecute(pr, false);
                if (c > maxConf) {
                    maxConf = c;
                    task = i;
                }
            }
            if (maxConf > 0.1 && task != null) {
                task.Execute(pr, false);
                return;
            }

            foreach (var i in RegisteredTasks) {
                var c = i.CanExecute(pr, false);
                if (c > maxConf) {
                    maxConf = c;
                    task = i;
                }
            }
            if (task != null)
                task.Execute(pr, false);
        }

        public void TryToAnsver(Predicat p) {
            Double maxConf = 0;
            Property ans;
            foreach (var i in ExecutedTasks) {
                Double conf;
                //var a = i.AnsverTo(p, out conf);
                //if (conf > maxConf) {
                //    maxConf = conf;
                //    ans = a;
                //}
            }
        }

        //public void TellText(String text) {
        //    DB.TellText(text);
        //}
        public static void TellText(String text, String au = "") {
            Instance.DB.TellText(text, au);
        }

    }
}
