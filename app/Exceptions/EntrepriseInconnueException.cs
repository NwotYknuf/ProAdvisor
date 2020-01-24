namespace ProAdvisor.app {
    [System.Serializable]
    public class EntrepriseInconnueException : System.Exception {
        public EntrepriseInconnueException() { }
        public EntrepriseInconnueException(string message) : base(message) { }
        public EntrepriseInconnueException(string message, System.Exception inner) : base(message, inner) { }
        protected EntrepriseInconnueException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}