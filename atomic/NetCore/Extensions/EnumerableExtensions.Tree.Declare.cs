
namespace System.Collections.Generic;

public class TreeNode<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] TSource> : Dictionary<string, object?>
{
    public TreeNode(TSource source)
    {
        Source = source;

        foreach (var item in typeof(TSource).GetProperties())
        {
            Add(item.Name, item.GetValue(source));
        }
        Add("Children", new List<TreeNode<TSource>>());
    }

    public TSource Source { get; }

    public List<TreeNode<TSource>> Children
    {
        get => (List<TreeNode<TSource>>)this["Children"]!;
        set => this["Children"] = value;
    }
}
