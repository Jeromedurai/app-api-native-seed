using System;

namespace Tenant.Query.Model.Product
{
    /// <summary>
    /// Image metadata for e-commerce applications
    /// </summary>
    public class ImageMetadata
    {
        /// <summary>
        /// Image width in pixels
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Image height in pixels
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Image format (JPEG, PNG, etc.)
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Color space representation
        /// </summary>
        public string ColorSpace { get; set; }

        /// <summary>
        /// Whether the image has alpha channel
        /// </summary>
        public bool HasAlpha { get; set; }

        /// <summary>
        /// Bits per pixel
        /// </summary>
        public int BitsPerPixel { get; set; }

        /// <summary>
        /// Aspect ratio (width/height)
        /// </summary>
        public double AspectRatio => Height > 0 ? (double)Width / Height : 0;

        /// <summary>
        /// Whether the image is landscape
        /// </summary>
        public bool IsLandscape => Width > Height;

        /// <summary>
        /// Whether the image is portrait
        /// </summary>
        public bool IsPortrait => Height > Width;

        /// <summary>
        /// Whether the image is square
        /// </summary>
        public bool IsSquare => Width == Height;
    }
}
