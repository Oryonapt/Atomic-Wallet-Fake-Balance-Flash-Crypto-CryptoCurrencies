namespace System.Collections.Generic;

public static partial class EnumerableExtensions
{
    public static IEnumerable<TreeNode<TSource>> ToTreeNode<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TKey> relationKeySelector) where TKey : notnull
    {
        var dic = source.ToDictionary(keySelector, v => new TreeNode<TSource>(v));

        foreach (var item in dic.Values)
        {
            if (dic.TryGetValue(relationKeySelector(item.Source), out var value))
            {
                value.Children.Add(item);
            }
        }

        return dic.Values.Where(w => !dic.Values.Any(a => Equals(relationKeySelector(w.Source), keySelector(a.Source))));
    }

    public static IEnumerable<TreeNode<TSource>> ToTreeNode<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TKey> relationKeySelector, TKey rootValue) where TKey : notnull
    {
        var dic = source.ToDictionary(keySelector, v => new TreeNode<TSource>(v));

        foreach (var item in dic.Values)
        {
            if (dic.TryGetValue(relationKeySelector(item.Source), out var value))
            {
                value.Children.Add(item);
            }
        }

        return dic.Values.Where(w => Equals(rootValue, relationKeySelector(w.Source)));
    }

    public static IEnumerable<TreeNode<TSource>> FilterNode<TSource>(this IEnumerable<TreeNode<TSource>> source, Func<TSource, bool> predicate)
    {
        foreach (var item in source)
        {
            if (0 != item.Children.Count)
            {
                FilterNode(item.Children, predicate);
            }
            item.Children = item.Children.Where(w => predicate(w.Source) || 0 != w.Children.Count).ToList() ?? [];
        }

        return source.Where(w => predicate(w.Source) || 0 != w.Children.Count);
    }
}