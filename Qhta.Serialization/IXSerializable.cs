namespace Qhta.Serialization
{
  public interface IXSerializable
  {
    void Serialize(IXSerializer serializer);
    void Deserialize(IXSerializer baseSerializer);
  }
}
