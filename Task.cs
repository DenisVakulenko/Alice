using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Alice {
    class TextCommandParsed {
        List<Word> Words;

        //List<Word> HasRelativesFor(

        List<Word> DatePart() {
            //if (
            return null;
        }
    }

    enum Owners {
            Kim,
            Environment,
            Speaker
        }
    enum Events {
            Start,
            NewTaskOpened,
            TaskSwitched,
            TempTaskSwitched,
            TaskClosed
        }

    class Event {
        
    }

    class Action {
        //virtual void Raise(String Q);
        //virtual float Fit(String Q);
    }

    class Context {
        int IntRepresentation;

        
    }

    public interface ITask {
        Double CanExecute(Predicat pr, Boolean isOnTop);
        Double Execute(Predicat pr, Boolean isOnTop);
        //Property AnsverTo(Predicat pr, out Double conf);
    }

    public class Task : Window, ITask {
        //int ProgID;
        //int ID;

        public delegate void TaskActionExecuter(Predicat predicat);
        public List<TaskAction> Actions = new List<TaskAction>();

        public class TaskAction {
            public Predicat PredicatPattern;
            public TaskActionExecuter Executer;
            public Boolean NeedToBeOnTop;

            public TaskAction(Predicat predicatPattern, TaskActionExecuter executer, Boolean needToBeOnTop = false) {
                PredicatPattern = predicatPattern;
                Executer = executer;
                NeedToBeOnTop = needToBeOnTop;
            }
        }

        public Task() {
            Closing += Task_Closing;
            StateChanged += Task_StateChanged;
            Activated += Task_Activated;
        }

        void Task_Activated(object sender, EventArgs e) {
            AliceGUIManager.Instance.SetActiveTask(this);
            AliceGUIManager.Instance.RegisterExecTask(this);
        }

        void Task_StateChanged(object sender, EventArgs e) {
            if ((sender as Window).IsVisible == true) { //System.Windows.WindowState.Normal) {
                AliceGUIManager.Instance.RegisterExecTask(this);
            }
            else {
                AliceGUIManager.Instance.UnregisterExecTask(this);
            }
        }

        void Task_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            e.Cancel = true;
            this.Hide();
            AliceGUIManager.Instance.UnregisterExecTask(this);
        }

        public void AddAction(Predicat p, TaskActionExecuter a) {
            Actions.Add(new TaskAction(p, a));
        }

        public Double CanExecute(Predicat pr, Boolean isOnTop) {
            Double conf = 0;
            Double c;

            foreach (var i in Actions) {
                if (i.NeedToBeOnTop == true && !isOnTop)
                    continue;
                c = pr.CompareTo(i.PredicatPattern);
                if (c > conf) {
                    conf = c;
                }
            }

            return conf;
        }

        public Double Execute(Predicat pr, Boolean isOnTop) {
            Double conf = 0;
            Double c;

            TaskAction a = null;

            foreach (var i in Actions) {
                //if (i.NeedToBeOnTop == true && !isOnTop)
                //    continue;
                c = pr.CompareTo(i.PredicatPattern);
                if (c > conf) {
                    conf = c;
                    a = i;
                }
            }

            a.Executer(pr);

            return 1;
        }

        public virtual void ActionShow(Predicat p) {
            this.Show();
        }
        public void ActionHide(Predicat p) {
            this.Hide();
        }
    }
}
