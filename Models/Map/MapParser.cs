using System;
using System.Collections.Generic;
using System.Linq;

namespace WpfGame.Models.Map;

public static class MapParser
{
    public static MapCell[][] Parse(IEnumerable<string> lines)
    {
        return lines
            .Select(x => x.Select(GetMapCell).ToArray())
            .ToArray();
    }

    private static MapCell GetMapCell(char ch) =>
        ch switch
        {
            'W' => MapCell.Wall,
            'F' => MapCell.Floor,
            'P' => MapCell.Player,
            'C' => MapCell.Crab,
            _ => throw new InvalidOperationException()
        };
}