using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace Tecnomapas.Blocos.Etx.ModuloArquivo.Business
{
	public class ArquivoZip
	{
		public MemoryStream Create(List<Arquivo.Arquivo> arquivos)
		{

			MemoryStream outputMemStream = new MemoryStream();
			ZipOutputStream zipStream = new ZipOutputStream(outputMemStream);

			zipStream.SetLevel(3); //0-9, 9 being the highest level of compression

			ZipEntry newEntry = null;

			try
			{
				foreach (var item in arquivos)
				{
					newEntry = new ZipEntry(String.Format("{0}.{1}", item.Nome, item.Extensao));
					newEntry.DateTime = DateTime.Now;

					zipStream.PutNextEntry(newEntry);

					if (!item.Buffer.CanRead && (item.Buffer is MemoryStream))
					{
						MemoryStream msTemp = new MemoryStream((item.Buffer as MemoryStream).ToArray());
						item.Buffer.Close();
						item.Buffer.Dispose();
						item.Buffer = msTemp;
					}

					StreamUtils.Copy(item.Buffer, zipStream, new byte[4096]);
					zipStream.CloseEntry();

					if (item != null)
					{
						item.Buffer.Close();
					}

					zipStream.IsStreamOwner = false;	// False stops the Close also Closing the underlying stream.
					// Must finish the ZipOutputStream before using outputMemStream. 
				}
			}
			finally
			{
				if (zipStream != null)
				{
					zipStream.Close();
				}
			}


			outputMemStream.Position = 0;

			return outputMemStream;

			/*
			// Alternative outputs:
			// ToArray is the cleaner and easiest to use correctly with the penalty of duplicating allocated memory.
			byte[] byteArrayOut = outputMemStream.ToArray();

			// GetBuffer returns a raw buffer raw and so you need to account for the true length yourself.
			byte[] byteArrayOut = outputMemStream.GetBuffer();
			long len = outputMemStream.Length;
			 */
		}

		public MemoryStream Create(string folderName)
		{
			MemoryStream msout = new MemoryStream();
			//FileStream fsOut = File.Create(outPathname);
			ZipOutputStream zipStream = new ZipOutputStream(msout);

			zipStream.SetLevel(3); //0-9, 9 being the highest level of compression

			//zipStream.Password = password;	// optional. Null is the same as not setting. Required if using AES.

			// This setting will strip the leading part of the folder path in the entries, to
			// make the entries relative to the starting folder.
			// To include the full path for each entry up to the drive root, assign folderOffset = 0.
			int folderOffset = 0;//folderName.Length + (folderName.EndsWith("\\") ? 0 : 1);

			CompressFolder(folderName, zipStream, folderOffset);

			//zipStream.IsStreamOwner = true;	// Makes the Close also Close the underlying stream
			zipStream.IsStreamOwner = false;	// Makes the Close also Close the underlying stream
			zipStream.Close();

			return msout;
		}

		// Recurses down the folder structure
		//
		private void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset)
		{

			string[] files = Directory.GetFiles(path);

			foreach (string filename in files)
			{

				FileInfo fi = new FileInfo(filename);

				string entryName = filename.Substring(folderOffset); // Makes the name in zip based on the folder
				entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
				ZipEntry newEntry = new ZipEntry(entryName);
				newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity

				// Specifying the AESKeySize triggers AES encryption. Allowable values are 0 (off), 128 or 256.
				// A password on the ZipOutputStream is required if using AES.
				//   newEntry.AESKeySize = 256;

				// To permit the zip to be unpacked by built-in extractor in WinXP and Server2003, WinZip 8, Java, and other older code,
				// you need to do one of the following: Specify UseZip64.Off, or set the Size.
				// If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, you do not need either,
				// but the zip will be in Zip64 format which not all utilities can understand.
				//   zipStream.UseZip64 = UseZip64.Off;
				newEntry.Size = fi.Length;

				zipStream.PutNextEntry(newEntry);

				// Zip the file in buffered chunks
				// the "using" will close the stream even if an exception occurs
				byte[] buffer = new byte[4096];
				using (FileStream streamReader = File.OpenRead(filename))
				{
					StreamUtils.Copy(streamReader, zipStream, buffer);
				}
				zipStream.CloseEntry();
			}
			string[] folders = Directory.GetDirectories(path);
			foreach (string folder in folders)
			{
				CompressFolder(folder, zipStream, folderOffset);
			}
		}
	}
}
