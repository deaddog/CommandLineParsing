using System;

namespace ConsoleTools
{
    public struct Vector : IEquatable<Vector>
    {
        public static Vector Zero { get; } = default;

        public Vector(int width, int height)
        {
            Horizontal = width;
            Vertical = height;
        }

        public int Horizontal { get; }
        public int Vertical { get; }

        public static implicit operator Vector((int, int) coordinates) => new Vector(coordinates.Item1, coordinates.Item2);

        public void Deconstruct(out int horizontal, out int vertical)
        {
            horizontal = Horizontal;
            vertical = Vertical;
        }

        public static Vector operator +(Vector s1, Vector s2) => new Vector(s1.Horizontal + s2.Horizontal, s1.Vertical + s2.Vertical);
        public static Vector operator -(Vector s1, Vector s2) => new Vector(s1.Horizontal - s2.Horizontal, s1.Vertical - s2.Vertical);

        public static bool operator ==(Vector left, Vector right) => left.Equals(right);
        public static bool operator !=(Vector left, Vector right) => !left.Equals(right);

        public override bool Equals(object obj) => obj is Vector size && Equals(size);
        public bool Equals(Vector other) => Horizontal == other.Horizontal && Vertical == other.Vertical;

        public override int GetHashCode() => HashCode.Combine(Horizontal, Vertical);

        public override string ToString() => $"({Horizontal}, {Vertical})";
    }
}
