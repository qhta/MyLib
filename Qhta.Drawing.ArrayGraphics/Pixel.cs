namespace Qhta.Drawing
{
  public struct Pixel
  {
    public byte A;
    public byte R;
    public byte G;
    public byte B;

    public static implicit operator uint(Pixel value)
    {
      return (uint)(value.A)<<24 | (uint)(value.R)<<16 | (uint)(value.G)<<8 | (uint)(value.B);
    }

    public static implicit operator Pixel(uint value)
    {
      return new Pixel { A=(byte)(value>>24), R=(byte)(value>>16), G=(byte)(value>>8), B=(byte)(value) };
    }


    public static implicit operator System.Drawing.Color(Pixel value)
    {
      return System.Drawing.Color.FromArgb((int)(uint)value);
    }

    public static implicit operator Pixel(System.Drawing.Color value)
    {
      return new Pixel { A=value.A, R=value.R, G=value.G, B=value.B };
    }

    public override bool Equals(object obj)
    {
      if (obj is Pixel otherPixel)
        return this.A==otherPixel.A && this.R==otherPixel.R && this.G==otherPixel.G && this.B==otherPixel.B;
      if (obj is System.Drawing.Color otherColor)
        return this.A==otherColor.A && this.R==otherColor.R && this.G==otherColor.G && this.B==otherColor.B;
      else
        throw new System.NotSupportedException($"Comparison between Pixel and {obj} not supported");
    }

    public override int GetHashCode()
    {
      return (int)(uint)this;
    }

  }
}
