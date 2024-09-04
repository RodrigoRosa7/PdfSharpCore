﻿// PDFsharp - A .NET library for processing PDF
// See the LICENSE file in the solution root for more information.

// ReSharper disable once CheckNamespace
namespace PdfSharpCore.BigGustave
{
    using System.IO;

    /// <summary>
    /// Enables execution of custom logic whenever a chunk is read.
    /// </summary>
    public interface IChunkVisitor
    {
        /// <summary>
        /// Called by the PNG reader after a chunk is read.
        /// </summary>
        void Visit(Stream stream, ImageHeader header, ChunkHeader chunkHeader, byte[] data, byte[] crc);
    }
}