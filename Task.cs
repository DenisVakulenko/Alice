﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kim {
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

    class Task {
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