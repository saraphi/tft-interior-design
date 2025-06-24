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

    public static Sprite CreateGradientSpriteFrom(Sprite originalSprite, Gradient gradient)
    {
        Texture2D originalTexture = originalSprite.texture;

        if (!originalTexture.isReadable) return null;

        // Recorta la región del sprite (por si es parte de un atlas)
        Rect spriteRect = originalSprite.rect;
        int width = (int)spriteRect.width;
        int height = (int)spriteRect.height;

        // Copiamos solo la región del sprite
        Texture2D newTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] originalPixels = originalTexture.GetPixels(
            (int)spriteRect.x,
            (int)spriteRect.y,
            width,
            height
        );

        // Aplicamos el gradiente sobre cada columna horizontal
        for (int x = 0; x < width; x++)
        {
            Color gradientColor = gradient.Evaluate((float)x / (width - 1));
            for (int y = 0; y < height; y++)
            {
                int index = y * width + x;
                // Combina el color original con el gradiente (puedes cambiar la operación)
                originalPixels[index] *= gradientColor;
            }
        }

        newTexture.SetPixels(originalPixels);
        newTexture.Apply();

        // Creamos el nuevo sprite con la textura modificada
        Sprite newSprite = Sprite.Create(
            newTexture,
            new Rect(0, 0, width, height),
            new Vector2(0.5f, 0.5f),
            originalSprite.pixelsPerUnit
        );

        return newSprite;
    }
}