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
        if (buffer == 0xFF)
        {
          this.stream.WriteByte(0x00);
        }

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

    public void writeInt(int data)
    {
      // data nach links shiften bis zur ersten 1 = n shifts
      // (maximale int bits - shifts)-oft ein bit schreiben

      const int maxIntegerBits = sizeof(int) * 8;
      int numberOfShifts = 0;
      for (int i = maxIntegerBits; i > 0; i--)
      {
        if (((data >> i) | 0) == 1)
        {
          numberOfShifts = i;
        }

      }

      // move needed bits to the most left
      int shiftedData = (data << numberOfShifts);

      // write needed bits
      writeBits(shiftedData, maxIntegerBits - numberOfShifts);
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

        // 
        if (data == 0xFF)
        {
          this.stream.WriteByte(0x00);
        }

        return;
      }

      for (int i = MAX_BITS - 1; i >= 0; i--)
      {
        // shift right to byte position i, then set every bit 0 except the last one with "& 1"
        int bit = ((data >> i) & 1);
        writeBit(bit);
      }
    }

    public void writeMarker(UInt16 marker)
    {
      this.stream.WriteByte((byte)(marker / 256));
      this.stream.WriteByte((byte)(marker % 256));
    }

    // Write two byte value
    public void writeWord(UInt16 hexValue)
    {
      writeByte((byte)(hexValue / 256));
      writeByte((byte)(hexValue % 256));
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


      for (int i = bufferLength - 1; i >= 0; i--)
      {
        // shift right to byte position i, then set every bit 0 except the last one with "& 1"
        int bit = ((this.buffer >> i) & 1);
        yield return bit;
      }
    }

    /*
      Print out the stream content and what is currently in the buffer
     */
    public void PrettyPrint()
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
      Console.WriteLine();

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
    public void Reset()
    {
      this.stream.Seek(0, SeekOrigin.Begin);
    }
  }
}
