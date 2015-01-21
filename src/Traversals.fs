module Traversals

open AST
open Operators

//
// implement the distributive law via pattern matching
//   
let rec distrib ex =
    match ex with
    | BinOp (BTimes, e1, BinOp (o,e2,e3)) -> BinOp (o, e1 .* e2, e1 .* e3)
    | BinOp (o, e1, e2) -> BinOp (o, distrib e1, distrib e2)
    | UnOp (o, e) -> UnOp (o, distrib e)
    | _ -> ex

//
// implement me!
//
let rec elim_double_negatives ex =
  failwith "Undefined elim_double_negatives."
