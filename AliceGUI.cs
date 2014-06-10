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

        public List<ITask> RegisteredTasks;
        public List<ITask> ExecutedTasks;

        public taskDialogBox DB;

        public AliceGUIManager() {
            DB = new taskDialogBox(this);

            RegisterTask(new taskNotes());
        }

        public void RegisterTask(ITask task) {
            RegisteredTasks.Add(task);
        }

        public void NewText(String text) {

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
        public static void TellText(String text) {
            Instance.DB.TellText(text);
        }

    }
}
