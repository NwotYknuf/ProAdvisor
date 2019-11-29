using System;

namespace ProAdvisor.app {
    public class Source {

        public Source(string source, bool respecteAFNOR) {
            this.source = source;
            this.respecteAFNOR = respecteAFNOR;
        }

        public string source { get; set; }
        public bool respecteAFNOR { get; set; }

    }
}