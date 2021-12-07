namespace Qhta.Serialization
{
  public class SerializationOptions
  {
    public bool LowercaseAttributeName { get; set; }
    public bool LowercasePropertyName { get; set; }
    public bool PrecedePropertyNameWithElementName { get; set; }

    public string ItemTag { get; set; }
  }
}
