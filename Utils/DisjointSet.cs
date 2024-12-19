namespace AdventOfCode2024.CSharp.Utils;

public class DisjointSet<T> where T : notnull
{

  private class Node(T Value, Node? Parent)
  {
    public readonly T Value = Value;
    public int Rank { get; set; } = 0;
    public Node? Parent = Parent;
  }

  private readonly Dictionary<T, Node> Forest = [];

  public void MakeSet(T x)
  {
    if (Forest.ContainsKey(x)) return;
    Forest[x] = new(x, null);
  }

  public bool SameUnion(T t1, T t2)
  {
    return FindNode(Forest[t1]) == FindNode(Forest[t2]);
  }

  public bool Contains(T x)
  {
    return Forest.ContainsKey(x);
  }

  public T Find(T x)
  {
    return FindNode(Forest[x]).Value;
  }

  private static Node FindNode(Node x)
  {
    if (x.Parent == null) return x;
    x.Parent = FindNode(x.Parent);
    return x.Parent;
  }

  public void Union(T x, T y)
  {
    MakeSet(x);
    MakeSet(y);
    var nodeX = FindNode(Forest[x]);
    var nodeY = FindNode(Forest[y]);

    if (nodeX == nodeY) return;

    if (nodeX.Rank < nodeY.Rank)
    {
      (nodeY, nodeX) = (nodeX, nodeY);
    }

    nodeY.Parent = nodeX;
    if (nodeX.Rank == nodeY.Rank)
    {
      nodeX.Rank += 1;
    }
  }
}