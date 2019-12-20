
// https://github.com/mono/ngit/tree/master/Sharpen/Sharpen
namespace ArsoftTestServer.JavaUtilities
{


    public class ByteBuffer
    {


        private byte[] buffer;
        private DataConverter c;
        private int capacity;
        private int index;
        private int limit;
        private int mark;
        private int offset;
        private ByteOrder order;

        public ByteBuffer()
        {
            this.c = DataConverter.BigEndian;
        }

        private ByteBuffer(byte[] buf, int start, int len)
        {
            this.buffer = buf;
            this.offset = 0;
            this.limit = start + len;
            this.index = start;
            this.mark = start;
            this.capacity = buf.Length;
            this.c = DataConverter.BigEndian;
        }

        public static ByteBuffer Allocate(int size)
        {
            return new ByteBuffer(new byte[size], 0, size);
        }

        public static ByteBuffer AllocateDirect(int size)
        {
            return Allocate(size);
        }

        public byte[] Array()
        {
            return buffer;
        }

        public int ArrayOffset()
        {
            return offset;
        }

        public int Capacity()
        {
            return capacity;
        }

        private void CheckGetLimit(int inc)
        {
            if ((index + inc) > limit)
            {
                throw new BufferUnderflowException();
            }
        }

        private void CheckPutLimit(int inc)
        {
            if ((index + inc) > limit)
            {
                throw new BufferUnderflowException();
            }
        }

        public void Clear()
        {
            index = offset;
            limit = offset + capacity;
        }

        public void Flip()
        {
            limit = index;
            index = offset;
        }


        // TODO: Correct ? 
        // Rewinds this buffer. The position is set to zero and the mark is discarded.
        public ByteBuffer Rewind()
        {
            this.Position(0);
            this.mark = 0;
            return this;
        }

        // Relative get method. Reads the byte at this buffer's current position, and then increments the position.
        public byte Get()
        {
            CheckGetLimit(1);
            return buffer[index++];
        }

        // Absolute get method. Reads the byte at the given index.
        public byte Get(int offset)
        {
            //TODO: Correct ?
            if (offset > buffer.Length)
                throw new System.ArgumentOutOfRangeException(nameof(offset));

            return buffer[offset];
        }

        public void Get(byte[] data)
        {
            Get(data, 0, data.Length);
        }

        public void Get(byte[] data, int start, int len)
        {
            CheckGetLimit(len);
            for (int i = 0; i < len; i++)
            {
                data[i + start] = buffer[index++];
            }
        }

        public int GetInt()
        {
            CheckGetLimit(4);
            int num = c.GetInt32(buffer, index);
            index += 4;
            return num;
        }

        public int GetInt(int index)
        {
            int originalPosition = this.Position();
            this.Position(index);
            int retValue = this.GetInt();
            this.Position(originalPosition);

            return retValue;
        }

        public long GetLong()
        {
            CheckGetLimit(8);
            long num = c.GetInt64(buffer, index);
            index += 8;
            return num;
        }

        public short GetShort()
        {
            CheckGetLimit(2);
            short num = c.GetInt16(buffer, index);
            index += 2;
            return num;
        }

        public short GetShort(int index)
        {
            int originalPosition = this.Position();
            this.Position(index);
            short retValue = this.GetShort();
            this.Position(originalPosition);

            return retValue;
        }

        public bool HasArray()
        {
            return true;
        }

        public int Limit()
        {
            return (limit - offset);
        }

        public void Limit(int newLimit)
        {
            limit = newLimit;
        }

        public void Mark()
        {
            mark = index;
        }

        public void Order(ByteOrder order)
        {
            this.order = order;
            if (order == ByteOrder.BIG_ENDIAN)
            {
                c = DataConverter.BigEndian;
            }
            else
            {
                c = DataConverter.LittleEndian;
            }
        }

        public int Position()
        {
            return (index - offset);
        }

        public void Position(int pos)
        {
            if ((pos < offset) || (pos > limit))
            {
                throw new BufferUnderflowException();
            }
            index = pos + offset;
        }

        public void Put(byte[] data)
        {
            Put(data, 0, data.Length);
        }

        public void Put(byte data)
        {
            CheckPutLimit(1);
            buffer[index++] = data;
        }

        public void Put(byte[] data, int start, int len)
        {
            CheckPutLimit(len);
            for (int i = 0; i < len; i++)
            {
                buffer[index++] = data[i + start];
            }
        }

        public void PutLong(long i)
        {
            Put(c.GetBytes(i));
        }

        public void PutInt(int i)
        {
            Put(c.GetBytes(i));
        }

        public void PutShort(short i)
        {
            Put(c.GetBytes(i));
        }

        public int Remaining()
        {
            return (limit - index);
        }

        public void Reset()
        {
            index = mark;
        }

        public ByteBuffer Slice()
        {
            ByteBuffer b = Wrap(buffer, index, buffer.Length - index);
            b.offset = index;
            b.limit = limit;
            b.order = order;
            b.c = c;
            b.capacity = limit - index;
            return b;
        }

        public static ByteBuffer Wrap(byte[] buf)
        {
            return new ByteBuffer(buf, 0, buf.Length);
        }

        public static ByteBuffer Wrap(byte[] buf, int start, int len)
        {
            return new ByteBuffer(buf, start, len);
        }
    }
}