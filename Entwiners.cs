using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entwined
{
    /// <summary>
    /// A class to entwine and detwine (serialize and deserialize) another class.
    /// </summary>
    public interface IEntwiner<T>
    {
        T Detwine(byte[] data);
        byte[] Entwine(T obj);
    }

    /// <summary>
    /// A class to entwine and detwine (serialize and deserialize) another class 
    /// using specified functions
    /// </summary>
    public class GenericEntwiner<T> : IEntwiner<T>
    {
        private readonly Func<T, byte[]> EntwinerFunc;
        private readonly Func<byte[], T> DetwinerFunc;
        public GenericEntwiner(Func<T, byte[]> entwiner, Func<byte[], T> detwiner)
        {
            EntwinerFunc = entwiner;
            DetwinerFunc = detwiner;
        }

        public byte[] Entwine(T obj) => EntwinerFunc(obj);
        public T Detwine(byte[] data) => DetwinerFunc(data);
    }
    /// <summary>
    /// An implementation of <see cref="IEntwiner"/> for strings
    /// </summary>
    public class StringEntwiner : IEntwiner<string>
    {
        public string Detwine(byte[] data) => Encoding.Unicode.GetString(data);

        public byte[] Entwine(string obj) => Encoding.Unicode.GetBytes(obj);
    }
    /// <summary>
    /// An implementation of <see cref="IEntwiner"/> for integers
    /// </summary>
    public class IntEntwiner : IEntwiner<int>
    {
        public int Detwine(byte[] data) => BitConverter.ToInt32(data, 0);

        public byte[] Entwine(int obj) => BitConverter.GetBytes(obj);
    }
    /// <summary>
    /// An implementation of <see cref="IEntwiner"/> for floats (singles)
    /// </summary>
    public class FloatEntwiner : IEntwiner<float>
    {
        public float Detwine(byte[] data) => BitConverter.ToSingle(data, 0);

        public byte[] Entwine(float obj) => BitConverter.GetBytes(obj);
    }
    /// <summary>
    /// An implementation of <see cref="IEntwiner"/> for doubles
    /// </summary>
    public class DoubleEntwiner : IEntwiner<double>
    {
        public double Detwine(byte[] data) => BitConverter.ToDouble(data, 0);

        public byte[] Entwine(double obj) => BitConverter.GetBytes(obj);
    }
    /// <summary>
    /// An implementation of <see cref="IEntwiner"/> for booleans
    /// </summary>
    public class BoolEntwiner : IEntwiner<bool>
    {
        public bool Detwine(byte[] data) => BitConverter.ToBoolean(data, 0);

        public byte[] Entwine(bool obj) => BitConverter.GetBytes(obj);
    }
}
