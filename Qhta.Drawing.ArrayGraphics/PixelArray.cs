using System.Drawing;

namespace Qhta.Drawing
{
  public class PixelArray
  {
    private Color[,] pixels;

    public PixelArray(int pixelWidth, int pixelHeight)
    {
      pixels = new Color[pixelWidth, pixelHeight];
      PixelWidth = pixelWidth;
      PixelHeight = pixelHeight;
    }

    public int PixelWidth { get; private set; }
    public int PixelHeight { get; private set; }

    public Color this[int x,int y]
    {
      get => pixels[x, y];
      set => pixels[x, y] = value;
    }

    public void FillAll(Color fillColor)
    {
      for (int y = 0; y < PixelWidth; y++)
        for (int x = 0; x < PixelHeight; x++)
          pixels[x, y]=fillColor;
    }

    public void FloodFill(int X, int Y, Color fillColor)
    {
      var startColor = pixels[X, Y];
      FloodFill(X, Y, startColor, fillColor);
    }

    private void FloodFill(int X, int Y, Color oldColor, Color newColor)
    {
      if (pixels[X, Y] == oldColor)
      {
        pixels[X, Y] = newColor;
        if (X>0)
          FloodFill(X-1, Y, oldColor, newColor);
        if (X<pixels.GetUpperBound(0))
          FloodFill(X+1, Y, oldColor, newColor);
        if (Y>0)
          FloodFill(X, Y-1, oldColor, newColor);
        if (Y<pixels.GetUpperBound(1))
          FloodFill(X, Y+1, oldColor, newColor);
      }
    }

    public PixelArray WandMask(int X, int Y, Color maskColor, Color unmaskColor)
    {
      var startColor = pixels[X, Y];
      PixelArray mask = new PixelArray(PixelWidth, PixelHeight);
      mask.FillAll(unmaskColor);
      WandFill(mask, X, Y, startColor, maskColor);
      return mask;
    }

    private void WandFill(PixelArray mask, int X, int Y, Color oldColor, Color maskColor)
    {
      if (pixels[X, Y] == oldColor && mask.pixels[X, Y]!=maskColor)
      {
        mask.pixels[X, Y] = maskColor;
        if (X>0)
          WandFill(mask, X-1, Y, oldColor, maskColor);
        if (X<pixels.GetUpperBound(0))
          WandFill(mask, X+1, Y, oldColor, maskColor);
        if (Y>0)
          WandFill(mask, X, Y-1, oldColor, maskColor);
        if (Y<pixels.GetUpperBound(1))
          WandFill(mask, X, Y+1, oldColor, maskColor);
      }
    }
  }
}
