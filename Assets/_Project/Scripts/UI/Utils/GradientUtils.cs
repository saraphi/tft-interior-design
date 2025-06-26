using UnityEngine;

public static class GradientUtils
{
    public static Sprite CreateGradientSprite(Gradient gradient, int width = 128, int height = 16)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.wrapMode = TextureWrapMode.Clamp;

        for (int x = 0; x < width; x++)
        {
            Color color = gradient.Evaluate((float)x / (width - 1));
            for (int y = 0; y < height; y++)
                texture.SetPixel(x, y, color);
        }

        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
    }
}