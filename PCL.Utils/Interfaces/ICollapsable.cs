using System;
namespace PCL.Utils
{
    public interface ICollapsable
    {
        bool IsCollapsed { get; }
        bool IsTemplate { get; }
    }
}
