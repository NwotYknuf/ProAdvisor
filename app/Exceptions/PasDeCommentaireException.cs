namespace ProAdvisor.app {
    [System.Serializable]
    public class PasDeCommentaireException : System.Exception {
        public PasDeCommentaireException() { }
        public PasDeCommentaireException(string message) : base(message) { }
        public PasDeCommentaireException(string message, System.Exception inner) : base(message, inner) { }
        protected PasDeCommentaireException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}