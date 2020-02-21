// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Numerics;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Transforms
{
    /// <summary>
    /// The function implements the bicubic kernel algorithm W(x) as described on
    /// <see href="https://en.wikipedia.org/wiki/Bicubic_interpolation#Bicubic_convolution_algorithm">Wikipedia</see>
    /// A commonly used algorithm within image processing that preserves sharpness better than triangle interpolation.
    /// </summary>
    public readonly struct BicubicResampler : IResampler
    {
        /// <inheritdoc/>
        public float Radius => 2;

        /// <inheritdoc/>
        [MethodImpl(InliningOptions.ShortMethod)]
        public float GetValue(float x)
        {
            if (x < 0F)
            {
                x = -x;
            }

            // Given the coefficient "a" as -0.5F.
            if (x <= 1F)
            {
                // Below simplified result = ((a + 2F) * (x * x * x)) - ((a + 3F) * (x * x)) + 1;
                return (((1.5F * x) - 2.5F) * x * x) + 1;
            }
            else if (x < 2F)
            {
                // Below simplified result = (a * (x * x * x)) - ((5F * a) * (x * x)) + ((8F * a) * x) - (4F * a);
                return (((((-0.5F * x) + 2.5F) * x) - 4) * x) + 2;
            }

            return 0;
        }

        /// <inheritdoc/>
        [MethodImpl(InliningOptions.ShortMethod)]
        public void ApplyResizeTransform<TPixel>(
            Configuration configuration,
            Image<TPixel> source,
            Image<TPixel> destination,
            Rectangle sourceRectangle,
            Rectangle destinationRectangle,
            bool compand)
            where TPixel : struct, IPixel<TPixel> => ResamplerExtensions.ApplyResizeTransform(
                configuration,
                in this,
                source,
                destination,
                sourceRectangle,
                destinationRectangle,
                compand);

        /// <inheritdoc/>
        [MethodImpl(InliningOptions.ShortMethod)]
        public void ApplyAffineTransform<TPixel>(
            Configuration configuration,
            ImageFrame<TPixel> source,
            ImageFrame<TPixel> destination,
            Matrix3x2 matrix)
            where TPixel : struct, IPixel<TPixel> => ResamplerExtensions.ApplyAffineTransform(
                configuration,
                in this,
                source,
                destination,
                matrix);

        /// <inheritdoc/>
        [MethodImpl(InliningOptions.ShortMethod)]
        public void ApplyProjectiveTransform<TPixel>(
            Configuration configuration,
            ImageFrame<TPixel> source,
            ImageFrame<TPixel> destination,
            Matrix4x4 matrix)
            where TPixel : struct, IPixel<TPixel> => ResamplerExtensions.ApplyProjectiveTransform(
                configuration,
                in this,
                source,
                destination,
                matrix);
    }
}
