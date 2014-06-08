using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alice {
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

        Int16 NUsed = 0;
        Int16 NCorrect = 0;
        Int16 NWrong = 0;

        public Double Confidence {
            get {
                return _Confidence;
            }
            set {
                _Confidence = value;
                
                NUsed = 0;
                NCorrect = 0;
                NWrong = 0;
            }
        }

        public void Used() {
            NUsed++;
        }
        public void WasCorrect() {
            NCorrect++;
        }
        public void WasWrong() {
            NWrong++;
        }

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
