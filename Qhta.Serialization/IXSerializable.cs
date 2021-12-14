namespace Qhta.Serialization
{
  public interface IXSerializable
  {
    void Serialize(IXSerializer serializer, IXWriter writer);
    void Deserialize(IXSerializer baseSerializer);
  }
}
