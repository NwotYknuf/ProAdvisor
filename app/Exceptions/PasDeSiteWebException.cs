[System.Serializable]
public class PasDeSiteWebException : System.Exception {
    public PasDeSiteWebException() { }
    public PasDeSiteWebException(string message) : base(message) { }
    public PasDeSiteWebException(string message, System.Exception inner) : base(message, inner) { }
    protected PasDeSiteWebException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}