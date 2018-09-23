using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using MeleeLib.GCX;
using MeleeLib.IO;

namespace MeleeLib.DAT
{
    /// <summary>
    /// Texture Object that holds information about a <see cref="DatMaterial"/> Texture
    /// </summary>
    public class DatTexture : DatNode
    {
        public GXTexMapID TexMapID = GXTexMapID.GX_TEXMAP0;
        public int GXTexGenSrc = 4;
        public float RX, RY, RZ;
        public float SX = 1, SY = 1, SZ = 1;
        public float TX, TY, TZ;
        public GXWrapMode WrapS = GXWrapMode.REPEAT;
        public GXWrapMode WrapT = GXWrapMode.REPEAT;
        public byte WScale = 1;
        public byte HScale = 1;
        public uint UnkFlags = 0x50010; //Research Needed
        public float Blending = 1;
        public GXTexFilter MagFilter = GXTexFilter.GX_LINEAR;

        public DatImage ImageData;
        public DatPalette Palette;
        public DatTextureLOD LOD;
        public DatTEV TEV;

        public DatMaterial Parent;

        // Dif 50010
        // Nrm 40020
        

        /// <summary>
        /// Creates a <see cref="Bitmap"/> of this Texture
        /// </summary>
        /// <returns>Texture Data as a Bitmap</returns>
        public Bitmap GetBitmap()
        {
            Bitmap B = null;
            if (B == null)
                if (Palette != null)
                {
                    B = TPL.ConvertFromTextureMelee(ImageData.Data,
                               ImageData.Width, ImageData.Height, (int)ImageData.Format,
                               Palette.Data, Palette.ColorCount, (int)Palette.Format);
                }
                else
                {
                    B = TPL.ConvertFromTextureMelee(ImageData.Data,
                                       ImageData.Width, ImageData.Height, (int)ImageData.Format,
                                       null, 0, 0);
                }
            return B;
        }


        Bitmap B;
        /// <summary>
        /// Creates a <see cref="Bitmap"/> of this Texture
        /// </summary>
        /// <returns>Texture Data as a Bitmap</returns>
        public Bitmap GetStaticBitmap()
        {
            if (B == null)
                if (Palette != null)
                {
                    B = TPL.ConvertFromTextureMelee(ImageData.Data,
                               ImageData.Width, ImageData.Height, (int)ImageData.Format,
                               Palette.Data, Palette.ColorCount, (int)Palette.Format);
                }
                else
                {
                    B = TPL.ConvertFromTextureMelee(ImageData.Data,
                                       ImageData.Width, ImageData.Height, (int)ImageData.Format,
                                       null, 0, 0);
                }
            return B;
        }

        /// <summary>
        /// Sets the texture data from a <see cref="Bitmap"/> image
        /// </summary>
        /// <param name="Image">The Image to Convert</param>
        /// <param name="format">The format to convert the <see cref="Bitmap"/> to</param>
        public void SetFromBitmap(Bitmap Image, TPL_TextureFormat imgformat = TPL_TextureFormat.RGBA8, TPL_PaletteFormat palformat = TPL_PaletteFormat.RGB565)
        {
            byte[] pal;
            if (ImageData == null)
                ImageData = new DatImage();

            ImageData.Format = imgformat;
            ImageData.Width = (ushort)Image.Width;
            ImageData.Height = (ushort)Image.Height;
            ImageData.Data = TPL.ConvertToTextureMelee(Image, (int)ImageData.Format, (int)palformat, out pal);
            
            if (pal != null)
            {
                if (Palette == null)
                    Palette = new DatPalette();
                Palette.Data = pal;
                Palette.ColorCount = pal.Length / 2;
                Palette.Format = (GXTlutFmt)palformat;
            }
            else
                Palette = null;
            ResetStaticTexture();
        }

        public void ResetStaticTexture()
        {
            if (B != null)
                B.Dispose();
            B = null;
        }


        /// <summary>
        /// Sets the texture data from a <see cref="Bitmap"/> image
        /// </summary>
        /// <param name="Image">The Image to Convert</param>
        /// <param name="format">The format to convert the <see cref="Bitmap"/> to</param>
        public void SetFromDXT1(byte[] data, int width, int height)
        {
            Palette = null;
            ImageData = new DatImage();
            ImageData.Format = (TPL_TextureFormat.CMP);
            ImageData.Width = (ushort)width;
            ImageData.Height = (ushort)height;
            ImageData.Data = TPL.ToCMP(data, width, height);

            ResetStaticTexture();
        }

        /// <summary>
        /// Reads this node from a file
        /// </summary>
        /// <param name="d">FileData</param>
        /// <param name="Root">The <see cref="DATRoot"/> this object belongs to</param>
        public void Deserialize(DATReader d, DATRoot Root, DatMaterial mat)
        {
            mat.AddTexture(this);
            d.Int(); // string offset
            int NextOff = d.Int();
            TexMapID = (GXTexMapID)d.Int();
            GXTexGenSrc = d.Int();
            RX = d.Float(); RY = d.Float(); RZ = d.Float();
            SX = d.Float(); SY = d.Float(); SZ = d.Float();
            TX = d.Float(); TY = d.Float(); TZ = d.Float();

            WrapS = (GXWrapMode)d.Int();
            WrapT = (GXWrapMode)d.Int();
            WScale = d.Byte();
            HScale = d.Byte();

            d.Skip(2); // padding
            UnkFlags = (uint)d.Int();

            Blending = d.Float();
            MagFilter = (GXTexFilter)d.Int();

            int ImgOff = d.Int();
            int PalOff = d.Int();
            int LodOff = d.Int();
            int TevOff = d.Int();

            if (ImgOff != 0)
            {
                d.Seek(ImgOff);
                ImageData = new DatImage();
                ImageData.Deserialize(d, Root);
            }
            if (PalOff != 0)
            {
                d.Seek(PalOff);
                Palette = new DatPalette();
                Palette.Deserialize(d, Root);
            }
            if (LodOff != 0)
            {
                d.Seek(LodOff);
                LOD = new DatTextureLOD();
                LOD.Deserialize(d, Root);
            }
            if (TevOff != 0)
            {
                d.Seek(TevOff);
                TEV = new DatTEV();
                TEV.Deserialize(d, Root);
            }
            if(NextOff != 0)
            {
                d.Seek(NextOff);
                DatTexture t = new DatTexture();
                t.Deserialize(d, Root, mat);
            }

            /*if ((UnkFlags & 0xFF) >> 4 == 1) mat.Texture_Diffuse = this;
            else
            if ((UnkFlags & 0xFF) >> 4 == 2) mat.Texture_Specular = this;
            else
            /*if ((UnkFlags & 0xFF) >> 4 == 3) mat.Texture_Unknown = this;
            else
            if ((UnkFlags & 0xFF) >> 4 == 8) mat.Texture_Unknown = this;
            else*/
            {
                //GetBitmap().Save(UnkFlags.ToString("x") + ".png");
                //`throw new Exception("New Texture " + UnkFlags.ToString("x"));
            }
        }
        
        /// <summary>
        /// Saves this node to a file
        /// </summary>
        /// <param name="Node"><see cref="DATWriter"/></param>
        public override void Serialize(DATWriter Node)
        {
            if (ImageData != null)
                ImageData.Serialize(Node);

            if (Palette != null)
                Palette.Serialize(Node);

            if (LOD != null)
                LOD.Serialize(Node);

            if (TEV != null)
                TEV.Serialize(Node);

            Node.AddObject(this);
            Node.Int(0);
            List<DatTexture> ParentTextures = new List<DatTexture>();
            ParentTextures.AddRange(Parent.Textures);
            if (ParentTextures.IndexOf(this) + 1 < ParentTextures.Count)
                Node.Object(ParentTextures[ParentTextures.IndexOf(this) + 1]);
            else
                Node.Int(0);
            Node.Int((int)TexMapID);
            Node.Int(GXTexGenSrc);
            Node.Float(RX); Node.Float(RX); Node.Float(RX);
            Node.Float(SX); Node.Float(SX); Node.Float(SX);
            Node.Float(TX); Node.Float(TX); Node.Float(TX);

            Node.Int((int)WrapS);
            Node.Int((int)WrapT);
            Node.Byte(WScale);
            Node.Byte(HScale);

            Node.Short(0);
            Node.Int((int)UnkFlags);
            Node.Float(Blending);
            Node.Int((int)MagFilter);
            
            Node.Object(ImageData);
            if (Palette == null) Node.Int(0);else Node.Object(Palette);
            if (LOD == null) Node.Int(0); else Node.Object(LOD);
            if (TEV == null) Node.Int(0); else Node.Object(TEV);
        }
    }
}
