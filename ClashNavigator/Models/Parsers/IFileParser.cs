﻿namespace ClashDetective.Models.Parsers;

public interface IFileParser<out T>
{
    T Parse(string filePath);
}