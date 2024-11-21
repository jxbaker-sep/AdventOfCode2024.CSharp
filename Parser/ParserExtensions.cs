using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Parser;

public static class ParserExtensions
{
  public static SelectParser<TIn, TOut> Select<TIn, TOut>(this Parser<TIn> parser, Func<TIn, TOut> fct) => new(parser, fct);
  public static WhereParser<T> Where<T>(this Parser<T> parser, Func<T, bool> fct) => new(parser, fct);
  public static RangeParser<T> Optional<T>(this Parser<T> parser) => new(parser, 0, 1);
  public static RangeParser<T> Range<T>(this Parser<T> parser, int min, int max) => new(parser, min, max);
  public static RangeParser<T> Star<T>(this Parser<T> parser) => new(parser);
  public static RangeParser<T> Plus<T>(this Parser<T> parser) => new(parser, 1);
  public static T ParseOrThrow<T>(this Parser<T> parser, string x) => 
    parser.Parse(x) is ParseSuccess<T> r ? r.Value : throw new ApplicationException();
  public static Parser<string> Join<T>(this Parser<List<T>> parser) => parser.Select(it => it.Join());

}
