//
// experiment with syntactic sugar to clean things up
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

// some code to make assembling environments a little cleaner, hiding the
// data structure that is used to represent them.
let inline (<<-) v value =
  match v with
  | Variable name -> (name, fval value)
  | _ -> failwith "<<- cannot be used with non-variables"

// hide the fact that the environment is a Map by renaming the Map.ofList
let mkenv = Map.ofList 

//
// put it together
//
let tester =
    // declare a set of variables
    let (a,b,x,y) = (v "a", v "b", v "x", v "y" )

    // bind the variables to some values
    let env = mkenv [
                a <<- 2.7;
                b <<- 3.14;
                x <<- 6.;
                y <<- 7.; ]

    // define an expression using these variables
    let expr = (a .+ b) ./ (x .* y)

    // return everything: the expression, environment,
    // pretty string, and the interpreted expression result
    (expr, pretty expr, env, interpret env expr)
