module AST_Utils

open AST

// given the AST, now we can write traversals of it.  the simplest and
// most useful for debugging is a pretty printer.

let pretty_bop o =
    match o with
    | BPlus -> "+"
    | BMinus -> "-"
    | BTimes -> "*"
    | BDivide -> "/"

let pretty_uop o =
    match o with
    | UNeg -> "-"

let rec pretty ex =
    match ex with
    | BinOp (op,l,r) -> "("+pretty l+pretty_bop op+pretty r+")"
    | UnOp (op,child) -> pretty_uop op+pretty child
    | Variable s -> s
    | Constant c -> string c

