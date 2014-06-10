using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public class Task { //: ITask {
        int ProgID;
        int ID;

        // enum Contexts;
        // CurrentContext;
        // Actions;
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
