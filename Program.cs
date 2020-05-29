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
                   this.value = this.value & ~(1ul << index) | (Convert.ToUInt64(value) << index); 
                }
                return false;
            }

            public bool getBit(int index)
            {
                if (index < size)
                {
                    return ((value >> index) & 1) == 1;
                }
                return false;
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
                return Convert.ToString((long) this.value, 2).PadLeft((int)size, '0');
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

            //public Bits getBits(int startIndex, int lastIndex)
            //{
            //    if (startIndex < size && lastIndex <= size && lastIndex - startIndex > 0 && lastIndex - startIndex < 65)
            //    {
            //        ulong rawResult = 0;
            //        if (startIndex % 64 < lastIndex % 64)
            //        {
            //            rawResult = value[startIndex / 64] >> (startIndex % 64);
            //            rawResult = rawResult & ((1ul << (lastIndex - startIndex)) - 1); 
            //        } 
            //        else 
            //        {
            //            ulong rawResultLeft  = value[startIndex / 64] >> (startIndex % 64);
            //            ulong rawResultRight = value[startIndex / 64 + 1] & ((1ul << (lastIndex % 64)) - 1);
            //            rawResult = rawResultLeft | (rawResultRight << (64 - startIndex % 64));
            //        }

            //        Bits res = new Bits(rawResult, (uint)(lastIndex - startIndex)); 

            //        return res;
            //    }
            //    return null;
            //} 

            public bool getBit(int index)
            {
                if (index < size)
                {
                    return new Bits(value[index / 64], 64).getBit(index % 64);
                }
                return false;
            }
        }

        class DHF
        {
            BitsStream stream;
            TreeNode tree;

            public DHF(BitsStream stream, TreeNode tree)
            {
                this.stream = stream; 
                this.tree = tree;
            }

            public void save(String name)
            {
                BinaryWriter writer = new BinaryWriter(new FileStream(name + ".dhf", FileMode.Create, FileAccess.Write));
                BinaryFormatter form = new BinaryFormatter();

                form.Serialize(writer.BaseStream, tree);

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
                TreeNode tree = (TreeNode) form.Deserialize(reader.BaseStream);
                
                uint size = reader.ReadUInt32();
                return new DHF(new BitsStream(reader, size), tree);
            }

            public BitsStream getStream()
            {
                return stream;
            }

            public TreeNode getTree()
            {
                return tree;
            }
        }

        [Serializable]
        class TreeNode : IComparable<TreeNode>
        {
            public TreeNode parent { get; set; } = null;
            public TreeNode left { get; set; } = null;
            public TreeNode right { get; set; } = null;

            public char? value { get; set; } = null;
            public int count { get; set; } = 0;

            public TreeNode() { }

            public TreeNode(TreeNode parent, char? value, int count)
            {
                this.parent = parent;
                this.value = value;
                this.count = count;
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

                if (right != null)
                {
                    var tmp0 = bits != null ? new Bits(bits) : new Bits();
                    tmp0.addBit(true);
                    var tmp = right.getMasks(tmp0);

                    foreach (var obj in tmp)
                    {
                        res.Add(obj.Key, obj.Value);
                    }
                }

                if (value != null && right == null && left == null)
                {
                    res.Add(value.Value, bits != null ? bits : new Bits(0, 1));
                }

                return res;
            }
            public int CompareTo(TreeNode other)
            {
                return count.CompareTo(other.count);
            }
        }

        //class TC : IComparer<TreeNode>
        //{
        //    int IComparer<TreeNode>.Compare(TreeNode x, TreeNode y)
        //    {
        //        return -x.count.CompareTo(y.count); 
        //    }
        //}

        class NodeCmp : IComparer<TreeNode>
        {
            int IComparer<TreeNode>.Compare(TreeNode x, TreeNode y)
            {
                int result = x.count.CompareTo(y.count);
                
                if (result == 0)
                {
                    if (x.value == y.value)
                    {
                        return 0;
                    }
                    else
                    {
                        return -1;
                    }
                }
                return result;
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

            var orderedList = new SortedDictionary<TreeNode, int>(new NodeCmp());

            foreach (var obj in count)
            {
                orderedList.Add(new TreeNode(null, obj.Key, obj.Value), 0);
            }

            while (orderedList.Count > 1)
            {
                KeyValuePair<TreeNode, int> min1 = orderedList.First();
                orderedList.Remove(min1.Key);
                
                
                KeyValuePair<TreeNode, int> min2 = orderedList.First();
                orderedList.Remove(min2.Key);
                
                var tmp = new TreeNode();

                tmp.right = min1.Key;
                tmp.left  = min2.Key;

                min1.Key.parent = tmp;
                min2.Key.parent = tmp;

                tmp.count = min1.Key.count + min2.Key.count; 
                tmp.value = min1.Key.value;

                orderedList.Add(tmp, 0);

            }

            var tree = orderedList.First().Key;
            var masks = orderedList.First().Key.getMasks();

            BitsStream stream = new BitsStream();

            for (int i = 0; i < str.Length; i++)
            {
                stream.append(masks[str[i]]);
            }

            new DHF(stream, tree).save("output");

            var dhf = DHF.load("output");

            var unstream = dhf.getStream();

            StringBuilder builder = new StringBuilder();

            int size = 0;
            
            TreeNode realTree = dhf.getTree();

            while (size < unstream.getSize())
            {
                TreeNode cur = realTree;
                while (cur.left != null || cur.right != null)
                {
                    var tmp = unstream.getBit(size);
                    if (tmp)
                    {
                        cur = cur.right;
                    }
                    else
                    {
                        cur = cur.left;
                    }
                    ++size;
                }

                builder.Append(cur.value);
            }

            StreamWriter outt = new StreamWriter("outputNew.txt");

            outt.Write(builder.ToString());

            outt.Flush();
            outt.Close();
        }
    }
}
