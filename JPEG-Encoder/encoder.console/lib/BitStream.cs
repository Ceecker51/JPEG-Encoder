using System;
using System.Collections.Generic;
using System.IO;

namespace encoder.lib
{
  public class BitStream
  {
    private const int MAX_BITS = 8;

    private Stream stream;
    private byte buffer;
    private int bufferLength;

    public BitStream()
    {
      this.stream = new MemoryStream();
      buffer = 0;
      bufferLength = 0;
    }

    /*
      Write a single bit to the BitStream
     */
    public void writeBit(int bit)
    {
      buffer = (byte)((buffer << 1) | bit);
      bufferLength++;

      if (bufferLength == MAX_BITS)
      {
        this.stream.WriteByte(buffer);
        buffer = 0;
        bufferLength = 0;
      }
    }

    /*
      Write multiple bits to the BitStream
     */
    public void writeBits(int data, int length)
    {
      for (int i = length - 1; i >= 0; i--)
      {
        int bit = ((data >> i) & 1);
        writeBit(bit);
      }
    }

    /*
      Write a single byte to the BitStream
     */
    public void writeByte(byte data)
    {
      // write directly to stream if no bits in buffer
      if (bufferLength == 0)
      {
        this.stream.WriteByte(data);
        return;
      }

      for (int i = MAX_BITS - 1; i >= 0; i--)
      {
        // shift right to byte position i, then set every bit 0 except the last one with "& 1"
        int bit = ((data >> i) & 1);
        writeBit(bit);
      }
    }
    
    // Write two byte value
    public void writeHex(UInt16 hexValue)
    { 
      writeByte((byte) (hexValue / 256));
      writeByte((byte) (hexValue % 256));
    }

    /*
     Read content from an external stream and write it to the BitStream
    */
    public void readFromStream(Stream inputStream)
    {
      if (inputStream == null) throw new NullReferenceException("No input stream provided");
      if (!inputStream.CanRead) throw new ArgumentException("Not able to read from stream");

      int readByte;
      while ((readByte = inputStream.ReadByte()) >= 0)
      {
        for (int i = MAX_BITS - 1; i >= 0; i--)
        {
          // shift right to byte position i, then set every bit 0 except the last one with "& 1"
          int bit = ((readByte >> i) & 1);
          writeBit(bit);
        }
      }
    }

        /*
          Write the content of BitStream to out to another stream
         */
        public void writeToStream(Stream outputStream)
        {
            if (outputStream == null) throw new NullReferenceException("No input stream provided");
            if (!outputStream.CanWrite) throw new ArgumentException("Not able to write to stream");

            // set stream position to beginning
            this.stream.Seek(0, SeekOrigin.Begin);

            int readByte;
            while ((readByte = this.stream.ReadByte()) >= 0)
            {
                outputStream.WriteByte((byte)readByte);
            }

            if (bufferLength > 0)
            {
                outputStream.WriteByte((byte)(this.buffer << (MAX_BITS - bufferLength)));

            }
        }

    /*
      Generate single bits that can be read
     */
    public IEnumerable<int> readBits()
    {
      if (!stream.CanRead) throw new ArgumentException("Not able to read from input");

      int readByte;
      while ((readByte = stream.ReadByte()) >= 0)
      {
        for (int i = MAX_BITS - 1; i >= 0; i--)
        {
          // shift right to byte position i, then set every bit 0 except the last one with "& 1"
          int bit = ((readByte >> i) & 1);
          yield return bit;
        }
      }
    }

    /*
      Print out the stream content and what is currently in the buffer
     */
    public void prettyPrint()
    {
      // set stream position to beginning
      this.stream.Seek(0, SeekOrigin.Begin);

      // print stream content
      Console.WriteLine("Current stream content: ");
      int bitCounter = 0;
      foreach (int bit in readBits())
      {
        Console.Write(bit);
        bitCounter++;

        if (bitCounter == 4) Console.Write(" ");

        if (bitCounter == MAX_BITS)
        {
          Console.WriteLine();
          bitCounter = 0;
        }

      }

      // print current buffer
      Console.Write("Current buffer: ");
      if (bufferLength == 0)
      {
        Console.Write("<empty>");
      }
      else
      {
        for (int i = bufferLength - 1; i >= 0; i--)
        {
          // shift right to byte position i, then set every bit 0 except the last one with "& 1"
          int bit = ((this.buffer >> i) & 1);
          Console.Write(bit);
        }

      }
      Console.WriteLine();
      
      // reset stream position to end
      this.stream.Seek(0, SeekOrigin.End); 
    }
        public void reset()
        {
            this.stream.Seek(0, SeekOrigin.Begin);
        }
    }
}
