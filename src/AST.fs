module AST

//
// first, establish the types to represent the AST that will be assembled
// and later traversed
//

// binary operators
type BOp = BPlus | BMinus | BTimes | BDivide

// unary operators
type UOp = UNeg

//
// discriminated union representing basic expressions.
// no precedence rules are considered - we just assume the children of
// the operators are parenthesized.
//
// e.g., (a+b)*(c+d) versus a+b*c+d.
//
type Expr = 
    | BinOp of BOp * Expr * Expr
    | UnOp of UOp * Expr
    | Variable of string
    | Constant of float
