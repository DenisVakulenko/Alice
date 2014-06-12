﻿using System;
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
        Double CanExecute(Predicat pr);
        Double Execute(Predicat pr);
        //Property AnsverTo(Predicat pr, out Double conf);
    }

    public class Task : Window, ITask {
        int ProgID;
        int ID;

        public delegate void TaskActionExecuter(Predicat predicat);
        public List<TaskAction> Actions = new List<TaskAction>();

        public class TaskAction {
            public Predicat PredicatPattern;
            public TaskActionExecuter Executer;

            public TaskAction(Predicat predicatPattern, TaskActionExecuter executer) {
                PredicatPattern = predicatPattern;
                Executer = executer;
            }
        }

        public void AddAction(Predicat p, TaskActionExecuter a) {
            Actions.Add(new TaskAction(p, a));
        }

        public Double CanExecute(Predicat pr) {
            Double conf = 0;
            Double c;

            foreach (var i in Actions) {
                c = pr.CompareTo(i.PredicatPattern);
                if (c > conf) {
                    conf = c;
                }
            }

            return conf;
        }

        public Double Execute(Predicat pr) {
            Double conf = 0;
            Double c;

            TaskAction a = null;

            foreach (var i in Actions) {
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            e.Cancel = true;
            this.Hide();
        }
    }

    class Messenger : Task {

        class Dialogue : Messenger {

        }
        class SearchInDialogue : Messenger {

        }

        class Dialogues : Messenger {

        }
        class SearchInDialogues : Messenger {

        }
    }


    class NotesTask : Task {
        class Context {
            class SearchData {
                List<String> S;
                
            }
        }
        enum Contexts {
            ViewAll,
            ViewOnlyHeaders,
            
        }
        // 
        //TestQ() {};


    }
}
