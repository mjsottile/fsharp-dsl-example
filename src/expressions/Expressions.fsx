//
// Simple EDSL for expressions in F#
//
#if INTERACTIVE
#load "AST.fs"
#load "AST_Utils.fs"
#load "Operators.fs"
#load "Traversals.fs"
#load "Interpreter.fs"
#endif

open AST
open AST_Utils
open Operators
open Traversals
open Interpreter

//
// some examples
//
let test1 = let x = v "x"
            let y = v "y"
            x .+ y

let test2 = let (x,y,a) = (v "x", v "y", v "a")
            a .* (x .+ y)

//
// do an example interpreter session with test2 example.  first,
// set up an environment that binds values to some names
//
let test2_env = Map.ofList [("a", fval 5.); ("x", fval 6.); ("y", fval 2.)]

// interpret test2
let result = interpret test2_env test2

// interpret test2 after the distributive law has been applied.  nice to
// see how easy it is to compose traversals and transformations.
let result2 = distrib test2 |> interpret test2_env

