using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosDataExtractor
{
    /// <summary>
    /// Many thanks to the awesome guys on epvp.
    /// I do not take ownership of the following code. 
    /// I for my self didnt revealed nor released the decryption methods.
    /// </summary>
    class NosExtractor
    {
        private readonly Action<string> _log;

        public NosExtractor(Action<string> log)
        {
            _log = log;
        }

        public bool Extract(string sourceFilename, DirectoryInfo storedirectory)
        {
            try
            {
                BinaryReader br = new BinaryReader(File.OpenRead(sourceFilename));

                int numFiles = br.ReadInt32();
                _log("## numFiles=" + numFiles);

                for (int fnum = 0; fnum < numFiles; fnum++)
                {
                    int fileCount = br.ReadInt32();
                    int fileNameSize = br.ReadInt32();
                    string fileName = Encoding.ASCII.GetString(br.ReadBytes(fileNameSize));
                    int fileCryptoMode = br.ReadInt32();
                    int fileSize = br.ReadInt32();
                    byte[] fileData = br.ReadBytes(fileSize);
                    if (fileCryptoMode == 1)
                    {
                        byte[] newdata = decryptnosfile(fileData);
                        //_log(string.Format("decrypted={1},{0}", Encoding.UTF7.GetString(newdata).Substring(0, 30), newdata.Length));
                        fileData = newdata;
                    }
                    else
                    {
                        //_log("does not need to be decrypted.");
                    }

                    _log("{" + $"{fileCount},{fileNameSize},{fileName},{fileCryptoMode},{fileSize}" + "}");

                    storedirectory.Create();
                    using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(Path.Combine(storedirectory.FullName, fileName))))
                    {
                        bw.Write(fileData);
                        bw.Flush();
                        bw.Close();
                    }
                }
            }
            catch (Exception)
            {
                _log("NOS File invalid.");
                return false;
            }
            return true;
        }

        private byte[] decryptnosfile(byte[] readInData)
        {
            var table = new byte[] { 0, 0, 45, 46, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 13 };
            int readlength = readInData.Length;
            List<byte> cOut = new List<byte>();

            int offset = 0;
            while (offset < readlength)
            {
                if (readInData[offset] == 255)
                {
                    offset++;
                    cOut.Add(13);
                    continue;
                }

                byte size = readInData[offset++];

                var i = 0;
                if ((size & 128) == 0)
                {
                    while ((i++ < size) && (offset < readlength))
                        cOut.Add((byte)(readInData[offset++] ^ 51));
                }
                else
                {
                    size &= 127;
                    while ((i < size) && (offset < readlength))
                    {
                        byte c = readInData[offset++];

                        cOut.Add(table[(c & 240) >> 4]);

                        if (table[c & 15] != 0)
                            cOut.Add(table[c & 15]);

                        i += 2;
                    }
                }
            }
            return cOut.ToArray();
        }
    }
}
