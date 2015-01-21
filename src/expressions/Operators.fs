module Operators

open AST

//
// an impotant part of embedding the DSL in F# is to come up with
// F# operators for assembling the AST without explicitly calling the
// consructors for the discriminated union                    
//
let inline (.+) e1 e2 = BinOp (BPlus,e1,e2)
let inline (.-) e1 e2 = BinOp (BMinus,e1,e2)
let inline (.*) e1 e2 = BinOp (BTimes,e1,e2)
let inline (./) e1 e2 = BinOp (BDivide,e1,e2)

//
// create a function called "neg" for negation
//
let neg e1 = UnOp (UNeg,e1)

//
// simple functiosn to lift constants into the AST as Constant nodes.
//
let fval (x : float) = Constant x

//
// similar function to lift strings into the AST for variable references
//
let v s = Variable s
