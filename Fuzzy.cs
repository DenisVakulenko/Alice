using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kim {
    //class ContextedFuzzyData<T, ContextT> {
    //    ContextedFuzzyData(T data) {
    //        Data = data;
    //    }
    //    T Data;
    //    Double Confidence = 0;

    //    List<ContextT>
    //}

    public class FuzzyData<T> {
        protected T _Data;
        protected Double _Confidence = 0;

        Int16 Correct = 0;
        Int16 NoMatter = 0;
        Int16 Incorrect = 0;

        public FuzzyData() {
        }
        public FuzzyData(T data) {
            _Data = data;
        }
    }

    public class SimpleFuzzyData<T> {
        T _Data;
        Double _Confidence = 0;

        public SimpleFuzzyData(T data, Double confidence = 1) {
            _Data = data;
            _Confidence = confidence;
        }
    }
}
