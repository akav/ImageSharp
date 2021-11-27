// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Buffers;
using System.IO;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Pbm
{
    /// <summary>
    /// Pixel encoding methods for the PBM binary encoding.
    /// </summary>
    internal class BinaryEncoder
    {
        /// <summary>
        /// Decode pixels into the PBM binary encoding.
        /// </summary>
        /// <typeparam name="TPixel">The type of input pixel.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <param name="stream">The bytestream to write to.</param>
        /// <param name="image">The input image.</param>
        /// <param name="colorType">The ColorType to use.</param>
        /// <param name="maxPixelValue">The maximum expected pixel value</param>
        /// <exception cref="InvalidImageContentException">
        /// Thrown if an invalid combination of setting is requested.
        /// </exception>
        public static void WritePixels<TPixel>(Configuration configuration, Stream stream, ImageFrame<TPixel> image, PbmColorType colorType, int maxPixelValue)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            if (colorType == PbmColorType.Grayscale)
            {
                if (maxPixelValue < 256)
                {
                    WriteGrayscale(configuration, stream, image);
                }
                else
                {
                    WriteWideGrayscale(configuration, stream, image);
                }
            }
            else if (colorType == PbmColorType.Rgb)
            {
                if (maxPixelValue < 256)
                {
                    WriteRgb(configuration, stream, image);
                }
                else
                {
                    WriteWideRgb(configuration, stream, image);
                }
            }
            else
            {
                WriteBlackAndWhite(configuration, stream, image);
            }
        }

        private static void WriteGrayscale<TPixel>(Configuration configuration, Stream stream, ImageFrame<TPixel> image)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            int width = image.Width;
            int height = image.Height;
            MemoryAllocator allocator = configuration.MemoryAllocator;
            using IMemoryOwner<byte> row = allocator.Allocate<byte>(width);
            Span<byte> rowSpan = row.GetSpan();

            for (int y = 0; y < height; y++)
            {
                Span<TPixel> pixelSpan = image.GetPixelRowSpan(y);

                PixelOperations<TPixel>.Instance.ToL8Bytes(
                    configuration,
                    pixelSpan,
                    rowSpan,
                    width);

                stream.Write(rowSpan);
            }
        }

        private static void WriteWideGrayscale<TPixel>(Configuration configuration, Stream stream, ImageFrame<TPixel> image)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            int width = image.Width;
            int height = image.Height;
            int bytesPerPixel = 2;
            MemoryAllocator allocator = configuration.MemoryAllocator;
            using IMemoryOwner<byte> row = allocator.Allocate<byte>(width * bytesPerPixel);
            Span<byte> rowSpan = row.GetSpan();

            for (int y = 0; y < height; y++)
            {
                Span<TPixel> pixelSpan = image.GetPixelRowSpan(y);

                PixelOperations<TPixel>.Instance.ToL16Bytes(
                    configuration,
                    pixelSpan,
                    rowSpan,
                    width);

                stream.Write(rowSpan);
            }
        }

        private static void WriteRgb<TPixel>(Configuration configuration, Stream stream, ImageFrame<TPixel> image)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            int width = image.Width;
            int height = image.Height;
            int bytesPerPixel = 3;
            MemoryAllocator allocator = configuration.MemoryAllocator;
            using IMemoryOwner<byte> row = allocator.Allocate<byte>(width * bytesPerPixel);
            Span<byte> rowSpan = row.GetSpan();

            for (int y = 0; y < height; y++)
            {
                Span<TPixel> pixelSpan = image.GetPixelRowSpan(y);

                PixelOperations<TPixel>.Instance.ToRgb24Bytes(
                    configuration,
                    pixelSpan,
                    rowSpan,
                    width);

                stream.Write(rowSpan);
            }
        }

        private static void WriteWideRgb<TPixel>(Configuration configuration, Stream stream, ImageFrame<TPixel> image)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            int width = image.Width;
            int height = image.Height;
            int bytesPerPixel = 6;
            MemoryAllocator allocator = configuration.MemoryAllocator;
            using IMemoryOwner<byte> row = allocator.Allocate<byte>(width * bytesPerPixel);
            Span<byte> rowSpan = row.GetSpan();

            for (int y = 0; y < height; y++)
            {
                Span<TPixel> pixelSpan = image.GetPixelRowSpan(y);

                PixelOperations<TPixel>.Instance.ToRgb48Bytes(
                    configuration,
                    pixelSpan,
                    rowSpan,
                    width);

                stream.Write(rowSpan);
            }
        }

        private static void WriteBlackAndWhite<TPixel>(Configuration configuration, Stream stream, ImageFrame<TPixel> image)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            int width = image.Width;
            int height = image.Height;
            MemoryAllocator allocator = configuration.MemoryAllocator;
            using IMemoryOwner<L8> row = allocator.Allocate<L8>(width);
            Span<L8> rowSpan = row.GetSpan();

            int previousValue = 0;
            int startBit = 0;
            for (int y = 0; y < height; y++)
            {
                Span<TPixel> pixelSpan = image.GetPixelRowSpan(y);

                PixelOperations<TPixel>.Instance.ToL8(
                    configuration,
                    pixelSpan,
                    rowSpan);

                for (int x = 0; x < width;)
                {
                    int value = previousValue;
                    for (int i = startBit; i < 8; i++)
                    {
                        if (rowSpan[x].PackedValue < 128)
                        {
                            value |= 0x80 >> i;
                        }

                        x++;
                        if (x == width)
                        {
                            previousValue = value;
                            startBit = (i + 1) & 7; // Round off to below 8.
                            break;
                        }
                    }

                    if (startBit == 0)
                    {
                        stream.WriteByte((byte)value);
                        previousValue = 0;
                    }
                }
            }
        }
    }
}
