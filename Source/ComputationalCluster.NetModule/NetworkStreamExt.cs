using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.NetModule
{
    public static class NetworkStreamExt
    {
        private static int BUFFER_SIZE = 4096;
        public static int WriteBuffered(this NetworkStream stream, byte[] buffer, int offset, int size)
        {
            int sendedBytes = 0;
            int actualPosition = offset;
            while (actualPosition < offset + size)
            {
                int toWrite = size - actualPosition <= BUFFER_SIZE ? size - actualPosition : BUFFER_SIZE;
                stream.Write(buffer, actualPosition, toWrite);
                actualPosition += toWrite;
                sendedBytes += toWrite; 
            }
            return sendedBytes;
        }

        public static byte[] ReadBuffered(this NetworkStream stream, int offset)
        {
            List<byte> readData = new List<byte>();
            byte[] tempBuffer = new byte[BUFFER_SIZE];
            int actualPosition = offset;
            int length;
            do
            {
                length = stream.Read(tempBuffer, 0, BUFFER_SIZE);
                actualPosition += length;
                readData.AddRange(tempBuffer.Take(length));
            } while (stream.DataAvailable);
            return readData.ToArray();
        }
    }
}
