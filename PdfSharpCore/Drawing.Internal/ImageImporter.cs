#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Thomas Hövel
//
// Copyright (c) 2005-2016 empira Software GmbH, Cologne Area (Germany)
//
// http://www.PdfSharp.com
// http://sourceforge.net/projects/pdfsharp
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.IO;

namespace PdfSharpCore.Drawing.Internal
{
    /// <summary>
    /// The class that imports images of various formats.
    /// </summary>
    internal class ImageImporter
    {
        // TODO Make a singleton!
        /// <summary>
        /// Gets the image importer.
        /// </summary>
        public static ImageImporter GetImageImporter()
        {
            return new ImageImporter();
        }

        private ImageImporter()
        {
            _importers.Add(new ImageImporterJpeg());
            _importers.Add(new ImageImporterBmp());
            _importers.Add(new ImageImporterPng());
        }

        /// <summary>
        /// Imports the image.
        /// </summary>
        public ImportedImage ImportImage(Stream stream, PdfDocument document)
        {
            StreamReaderHelper helper = new StreamReaderHelper(stream);

            // Try all registered importers to see if any of them can handle the image.
            foreach (IImageImporter importer in _importers)
            {
                helper.Reset();
                ImportedImage image = importer.ImportImage(helper, document);
                if (image != null)
                    return image;
            }
            return null;
        }

        /// <summary>
        /// Imports the image.
        /// </summary>
        public ImportedImage ImportImage(Stream stream)
        {
            long length = -1;
            try
            {
                length = stream.Length;
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            { }

            if (length < -1 || length > Int32.MaxValue)
                throw new InvalidOperationException($"Image files with a size of {length} bytes are not supported. Use image files smaller than 2 GiB.");

            using (var helper = new StreamReaderHelper(stream, (int)length))
            {
                return TryImageImport(helper);
            }
        }

        /// <summary>
        /// Imports the image.
        /// </summary>
        public ImportedImage ImportImage(string filename)
        {
            var data = File.ReadAllBytes(filename);

            using (var helper = new StreamReaderHelper(data))
            {
                return TryImageImport(helper);
            }
        }

        ImportedImage TryImageImport(StreamReaderHelper helper)
        {
            // Try all registered importers to see if any of them can handle the image.
            foreach (var importer in _importers)
            {
                helper.Reset();
                var image = importer.ImportImage(helper);
                if (image != null)
                    return image;
            }
            return null;
        }

        private readonly List<IImageImporter> _importers = new List<IImageImporter>();
    }
}
