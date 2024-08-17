using System.Drawing;
using GMSL;
using Underanalyzer;
using UndertaleModLib;
using UndertaleModLib.Util;
using UndertaleModLib.Models;

namespace restoreoldcolors;

public class Mod : GMSLMod
{
    public override void Patch()
    {
        Console.WriteLine($"[restoreOldColors]: Patching assets...");
        PatchAssets();
    }

    // Runs before every startup.
    public override void Start() {}

    public void PatchAssets() {
        var oldLane = moddingData.Sprites.First(s => s.Name.Content == "sp_laneOverlay_old");
        var lane = moddingData.Sprites.First(s => s.Name.Content == "sp_laneOverlay");
        ReplaceTexture(lane.Textures[0].Texture, oldLane.Textures[0].Texture);
        Console.WriteLine($"[restoreOldColors]: Replaced lanes.");

        var holdFill = moddingData.Sprites.First(s => s.Name.Content == "sp_holdFill");
        var fill1 = holdFill.Textures[1];
        var fill2 = holdFill.Textures[2];
        fill1.Texture.ReplaceTexture(Image.FromFile(Path.Combine(assetsDir, "sprites", "sp_holdFill_1.png")));
        fill2.Texture.ReplaceTexture(Image.FromFile(Path.Combine(assetsDir, "sprites", "sp_holdFill_2.png")));
        Console.WriteLine($"[restoreOldColors]: Replaced hold notes.");

        var noteTypes = moddingData.Sprites.First(s => s.Name.Content == "sp_notetypesnew");
        var oldNoteTypes = moddingData.Sprites.First(s => s.Name.Content == "sp_notetypesnew_2023");

        for (var i = 0; i < oldNoteTypes.Textures.Count; i++) {
            ReplaceTexture(noteTypes.Textures[i].Texture, oldNoteTypes.Textures[i].Texture);
        }
        Console.WriteLine($"[restoreOldColors]: Replaced notes.");
    }

    // TODO include in modapi
    public void ReplaceTexture(UndertaleTexturePageItem source, UndertaleTexturePageItem replacement, bool disposeImage = true)
    {
        lock (source.TexturePage.TextureData)
        {
            TextureWorker worker = new TextureWorker();
            Bitmap replacementImage = worker.GetEmbeddedTexture(replacement.TexturePage);
            Bitmap sourceImage = worker.GetEmbeddedTexture(source.TexturePage);

            Graphics g = Graphics.FromImage(sourceImage);
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            g.DrawImageUnscaled(replacementImage.Clone(new Rectangle(replacement.SourceX, replacement.SourceY, replacement.SourceWidth, replacement.SourceHeight), replacementImage.PixelFormat), source.SourceX, source.SourceY);
            g.Dispose();

            source.TexturePage.TextureData.TextureBlob = TextureWorker.GetImageBytes(sourceImage);

            worker.Cleanup();
        }
    }
}
