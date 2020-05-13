using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;

namespace ConsoleApp1
{
    class Program
    {

        [Serializable]
        class Bits
        {
            private uint size = 0;
            private ulong value = 0;

            public Bits() { }

            public Bits(Bits bits)
            {
                this.size = bits.size;
                this.value = bits.value;
            }

            public Bits(ulong value, uint size = 64)
            {
                this.size = size;
                this.value = value;
            }

            public bool addSize(int increment)
            {
                if (increment + (int)size > 64)
                    return false;
                else
                    size += (byte)increment;
                return true;
            }

            public bool addBit(bool bit)
            {
                if (addSize(1))
                {
                    return setBit((int)(size - 1), bit);
                }
                return false;
            }

            public bool setBit(int index, bool value)
            {
                if (index < size)
                {
                    if (value)
                    {
                        ulong tmp = 1ul;
                        tmp = tmp << index;
                        this.value = this.value | tmp;
                    }
                    else
                    {
                        ulong tmp = 0;
                        tmp = ~tmp;
                        tmp -= 1ul << index;
                        this.value = this.value & tmp;
                    }
                }
                return false;
            }

            public bool getBit(int index)
            {
                if (index < size)
                {
                    ulong tmp = 1;
                    tmp = tmp << index;
                    return (value & tmp) != 0;
                }
                return false;
            }

            public void clearZero()
            {
                while (!getBit((int)(size - 1)) && size > 1)
                    size--;
            }
            public uint getSize()
            {
                return size;
            }

            public ulong getValue()
            {
                return value;
            }

            public bool Equals(Bits bits)
            {
                return this.size == bits.size && this.value == bits.value;
            }

            public override String ToString()
            {
                StringBuilder result = new StringBuilder();
                for (int i = (int)size - 1; i > -1; i--)
                {
                    if (getBit(i))
                        result.Append(1);
                    else
                        result.Append(0);
                }

                return result.ToString();
            }
        }

        class BitsStream
        {
            private uint size = 0;
            List<ulong> value = new List<ulong>();

            public BitsStream(BinaryReader reader)
            {
                ulong tmp;
                while ((tmp = reader.ReadUInt64()) != 0)
                {
                    value.Add(tmp);
                    size += 64;
                }
            }

            public BitsStream(BinaryReader reader, uint size)
            {
                this.size = size;

                for (int i = 0; i < this.size; i += 64)
                {
                    value.Add(reader.ReadUInt64());
                }
            }

            public BitsStream() { }

            public void append(Bits bits)
            {
                if (size % 64 == 0)
                {
                    value.Add(bits.getValue());
                }
                else
                {
                    ulong tmp = bits.getValue();
                    tmp = tmp << (int)size % 64;
                    value[value.Count - 1] += tmp;
                    if ((64 - size % 64) < bits.getSize())
                    {
                        tmp = bits.getValue();
                        tmp = tmp >> (int)(64 - (size % 64));
                        value.Add(tmp);
                    }
                }

                size += bits.getSize();
            }

            public ulong[] toArray()
            {
                return value.ToArray();
            }

            public uint getSize()
            {
                return size;
            }

            public Bits getBits(int startIndex, int lastIndex)
            {
                if (startIndex < size && lastIndex <= size && lastIndex - startIndex > 0 && lastIndex - startIndex < 65)
                {
                    Bits res = new Bits();
                    for (int i = startIndex; i < lastIndex; i++)
                    {
                        res.addBit(getBit(i));
                    }
                    return res;
                }
                return null;
            }

            public BitsStream getSubStream(int startIndex, int lastIndex)
            {
                BitsStream result = new BitsStream();
                for (int i = startIndex; i < lastIndex && i < size; i += 64)
                {
                    int size = Math.Min(lastIndex - i, 64);
                    result.append(getBits(i, i + size));
                }

                return result;
            }

            public bool getBit(int index)
            {
                if (index < size)
                {
                    return new Bits(value[index / 64], 64).getBit(index % 64);
                }
                return false;
            }

            public bool Equals(BitsStream stream)
            {
                if (this.size == stream.size)
                {
                    for (int i = 0; i < value.Count; i++)
                    {
                        if (this.value[i] != stream.value[i]) return false;
                    }
                    return true;
                }
                return false;
            }
        }

        class DHF
        {
            BitsStream stream;
            Dictionary<char, Bits> masks;

            public DHF(BitsStream stream, Dictionary<char, Bits> masks)
            {
                this.stream = stream;
                this.masks = masks;
            }

            public void save(String name)
            {
                BinaryWriter writer = new BinaryWriter(new FileStream(name + ".dhf", FileMode.Create, FileAccess.Write));
                BinaryFormatter form = new BinaryFormatter();

                form.Serialize(writer.BaseStream, masks);


                writer.Write(stream.getSize());
                var array = stream.toArray();

                for (int i = 0; i < array.Length; i++)
                {
                    writer.Write(array[i]);
                }

                writer.Flush();
                writer.Close();
            }

            public static DHF load(String name)
            {
                BinaryReader reader = new BinaryReader(new FileStream(name + ".dhf", FileMode.Open, FileAccess.Read));

                BinaryFormatter form = new BinaryFormatter();
                Dictionary<char, Bits> masks = (Dictionary<char, Bits>)form.Deserialize(reader.BaseStream);

                uint size = reader.ReadUInt32();
                return new DHF(new BitsStream(reader, size), masks);
            }

            public BitsStream getStream()
            {
                return stream;
            }

            public Dictionary<char, Bits> getMasks()
            {
                return masks;
            }
        }

        class TreeNode : IComparable<TreeNode>
        {
            public TreeNode parent { get; set; } = null;
            public TreeNode left { get; set; } = null;
            public TreeNode rigth { get; set; } = null;

            public char? value { get; set; } = null;
            public int count { get; set; } = 0;

            public TreeNode() { }

            public TreeNode(TreeNode parent, char? value, int count)
            {
                this.parent = parent;
                this.value = value;
                this.count = count;
            }

            public int CompareTo(TreeNode other)
            {
                if (value.HasValue && other.value.HasValue)
                    return value.Value.CompareTo(other.value.Value) | count.CompareTo(other.count);
                return count.CompareTo(other.count);
            }

            public Dictionary<char, Bits> getMasks(Bits bits = null)
            {
                Dictionary<char, Bits> res = new Dictionary<char, Bits>();

                if (left != null)
                {
                    var tmp0 = bits != null ? new Bits(bits) : new Bits();
                    tmp0.addBit(false);
                    var tmp = left.getMasks(tmp0);

                    foreach (var obj in tmp)
                    {
                        res.Add(obj.Key, obj.Value);
                    }
                }

                if (rigth != null)
                {
                    var tmp0 = bits != null ? new Bits(bits) : new Bits();
                    tmp0.addBit(true);
                    var tmp = rigth.getMasks(tmp0);

                    foreach (var obj in tmp)
                    {
                        res.Add(obj.Key, obj.Value);
                    }
                }

                if (value != null)
                {
                    res.Add(value.Value, bits != null ? bits : new Bits(0, 1));
                }

                return res;
            }
        }

        class TC : IComparer<TreeNode>
        {
            int IComparer<TreeNode>.Compare(TreeNode x, TreeNode y)
            {
                return -x.count.CompareTo(y.count);
            }
        }

        static void Main(string[] args)
        {
            StreamReader reader = new StreamReader("i.txt");

            String str = reader.ReadToEnd();

            reader.Close();

            Dictionary<char, int> count = new Dictionary<char, int>();

            for (int i = 0; i < str.Length; i++)
            {
                if (count.ContainsKey(str[i]))
                    count[str[i]] = count[str[i]] + 1;
                else
                    count.Add(str[i], 1);
            }

            count.OrderBy((obj) => {
                return -obj.Value;
            });

            var list = count.ToList().Aggregate(new List<TreeNode>(), (set0, obj) => {
                set0.Add(new TreeNode(null, obj.Key, obj.Value));
                return set0;
            });



            while (list.Count > 1)
            {
                list.Sort(new TC());

                var tmp = new TreeNode();

                tmp.rigth = list[list.Count - 1];
                tmp.left = list[list.Count - 2];

                list[list.Count - 1].parent = tmp;
                list[list.Count - 2].parent = tmp;

                tmp.count = tmp.rigth.count + tmp.left.count;

                list.Remove(list[list.Count - 1]);
                list.Remove(list[list.Count - 1]);
                list.Add(tmp);
            }

            var tree = list.First().getMasks();

            BitsStream stream = new BitsStream();

            for (int i = 0; i < str.Length; i++)
            {
                stream.append(tree[str[i]]);
            }

            new DHF(stream, tree).save("output");

            var dhf = DHF.load("output");

            var unstream = dhf.getStream();
            tree = dhf.getMasks();

            StringBuilder builder = new StringBuilder();

            int size = 0;
            bool boolean = false;

            var l0 = tree.ToList();

            var l = l0.OrderByDescending(obj => obj.Value.getValue());

            var dic = l.ToDictionary((obj) => {
                return obj.Key;
            });

            while (size < unstream.getSize())
            {
                foreach (var obj in l)
                {
                    var tmp = unstream.getBits(size, size + (int)obj.Value.getSize());
                    if (tmp != null && tmp.Equals(obj.Value))
                    {
                        builder.Append(obj.Key);
                        size += (int)obj.Value.getSize();
                        boolean = true;
                        break;
                    }
                }
                if (!boolean) size++;
                boolean = false;
            }

            StreamWriter outt = new StreamWriter("outputNew.txt");

            outt.Write(builder.ToString());

            outt.Flush();
            outt.Close();
        }
    }
}
